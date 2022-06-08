using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BotCore;
using BotCore.Dto;
using BotCore.Log;
using Serilog;
using StreamViewerBot.Properties;
using StreamViewerBot.UI;

namespace StreamViewerBot
{
    public partial class MainScreen : Form
    {
        private static readonly string _productVersion = "2.9.2.2";

        private static string _proxyListDirectory = "";

        private static string _chatMessagesDirectory = "";

        private static string _ipCheckURL = "https://api.ipify.org/";

        private static bool _headless;

        private readonly Configuration _configuration =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private readonly Core _core = new Core();

        private readonly Dictionary<string, string> _dataSourceQuality = new Dictionary<string, string>();

        private string _quality = string.Empty;

        private readonly ConcurrentQueue<LoginDto> _lstLoginInfo = new ConcurrentQueue<LoginDto>();

        private readonly Dictionary<string, StreamService.Service> _serviceTypes =
            new Dictionary<string, StreamService.Service>();

        private StreamService.Service _serviceType;

        private bool _canStart;

        private Size _loginSize;

        private List<string> _nonPrivateProxies = new List<string>();

        private List<string> _chatMessages = new List<string>();

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private ValidatingForm _validatingForm = new ValidatingForm();

        private bool _withLoggedIn;

        public MainScreen()
        {
            InitializeComponent();

            var appId = _configuration.AppSettings.Settings["appId"].Value;

            if (string.IsNullOrEmpty(appId))
            {
                _configuration.AppSettings.Settings["appId"].Value = Guid.NewGuid().ToString();
                _configuration.Save(ConfigurationSaveMode.Modified);
            }

            Logger.CreateLogger(appId);

            Text += " v" + _productVersion;

            LogInfo(new Exception($"Application started. v{_productVersion}"));

            var isAvailable = IsNewerVersionAvailable();

            if (isAvailable)
            {
                var isForcedUpdate = IsForcedUpdate();
                
                if(isForcedUpdate)
                    UpdateBot(true);
                
                ShowChangelog();
                UpdateBot();
            }


            #region StreamQuality

            FillComboBoxes();

            void FillComboBoxes()
            {
                _dataSourceQuality.Add("Source", string.Empty);
                _dataSourceQuality.Add("480p", "{\"default\":\"480p30\"}");
                _dataSourceQuality.Add("360p", "{\"default\":\"360p30\"}");
                _dataSourceQuality.Add("160p", "{\"default\":\"160p30\"}");

                lstQuality.ValueMember = "Value";
                lstQuality.DisplayMember = "Key";
                lstQuality.DataSource = new BindingSource(_dataSourceQuality, null);
                lstQuality.SelectedIndex = _dataSourceQuality.Count - 1;

                _serviceTypes.Add("Twitch", StreamService.Service.Twitch);
                _serviceTypes.Add("YouTube", StreamService.Service.Youtube);
                _serviceTypes.Add("DLive", StreamService.Service.DLive);
                _serviceTypes.Add("Nimo Tv", StreamService.Service.NimoTv);
                _serviceTypes.Add("Twitter", StreamService.Service.Twitter);
                _serviceTypes.Add("Facebook", StreamService.Service.Facebook);
                _serviceTypes.Add("Trovo.live", StreamService.Service.TrovoLive);
                _serviceTypes.Add("Bigo Live", StreamService.Service.BigoLive);

                lstserviceType.ValueMember = "Value";
                lstserviceType.DisplayMember = "Key";
                lstserviceType.DataSource = new BindingSource(_serviceTypes, null);
                lstserviceType.SelectedIndex = 0;
            }

            #endregion
        }

        public sealed override Size MinimumSize
        {
            get => base.MinimumSize;
            set => base.MinimumSize = value;
        }

        public sealed override Size MaximumSize
        {
            get => base.MaximumSize;
            set => base.MaximumSize = value;
        }

        private void ShowChangelog()
        {
            try
            {
                var webRequest = WebRequest.Create(@"https://streamviewerbot.com/Download/changelog.txt");
                webRequest.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webRequest.Headers.Add(
                    "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                webRequest.Timeout = 5000;
                using var response = webRequest.GetResponse();
                using var content = response.GetResponseStream();
                if (content != null)
                {
                    using var reader = new StreamReader(content);
                    var changeLog = reader.ReadToEnd();

                    var changeLogViewer = new Changelog();
                    changeLogViewer.SetChangelog(changeLog);
                    changeLogViewer.ShowDialog();
                }
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private bool IsNewerVersionAvailable()
        {
            try
            {
                var webRequest = WebRequest.Create(@"https://streamviewerbot.com/Download/latestVersion.txt");
                webRequest.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webRequest.Headers.Add(
                    "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                webRequest.Timeout = 5000;
                using var response = webRequest.GetResponse();
                using var content = response.GetResponseStream();
                if (content != null)
                {
                    using var reader = new StreamReader(content);
                    var latestVersion = reader.ReadToEnd();

                    return latestVersion != _productVersion;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
        
        private bool IsForcedUpdate()
        {
            try
            {
                var webRequest = WebRequest.Create(@"https://streamviewerbot.com/Download/isForcedUpdate.txt");
                webRequest.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webRequest.Headers.Add(
                    "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                webRequest.Timeout = 5000;
                using var response = webRequest.GetResponse();
                using var content = response.GetResponseStream();
                if (content != null)
                {
                    using var reader = new StreamReader(content);
                    var value = Convert.ToBoolean(reader.ReadToEnd());

                    return value;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private void CheckProxiesArePrivateOrNot()
        {
            _nonPrivateProxies = new List<string>();

            try
            {
                var proxies = File.ReadAllLines(_proxyListDirectory);

                var error = ExecuteProxyTest(proxies);

                if (!error) LogToScreen("Proxies are working OK!");
            }
            catch (Exception exception)
            {
                try
                {
                    Log.Logger.Error(exception.ToString());
                }
                catch (Exception)
                {
                    //ignored
                }
            }
        }

        private bool CheckChatMessages()
        {
            _chatMessages = new List<string>();

            if (File.Exists(_chatMessagesDirectory))
            {
                try
                {
                    var messages = File.ReadAllText(_chatMessagesDirectory);

                    _chatMessages = messages.Split(';').ToList();

                    return true;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Please make sure that your messages are valid. Split all messages with ;\r\nFor example=> Hello;Hi everyone;It's awesome;Cool!;Great to see you...");
                    try
                    {
                        Log.Logger.Error(exception.ToString());
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                    return false;
                }
            }

            return true;
        }

        private void Retest(string[] proxies)
        {
            var error = ExecuteProxyTest(proxies);

            if (!error) LogToScreen("Proxies are working OK!");
        }

        private string GetIPAddress()
        {
            var address = "";
            var request = WebRequest.Create(_ipCheckURL);
            request.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
            request.Headers.Add(
                "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            request.Timeout = 5000;
            using (var response = request.GetResponse())
            using (var stream = new StreamReader(response.GetResponseStream()!))
            {
                address = stream.ReadToEnd();
            }

            request.Abort();
            return address;
        }

        public bool ExecuteProxyTest(string[] proxies)
        {
            var error = false;
            _nonPrivateProxies.Clear();

            if (_validatingForm.IsDisposed)
                _validatingForm = new ValidatingForm();

            if (_validatingForm.InvokeRequired)
                _validatingForm.BeginInvoke(new Action(() => { _validatingForm.Show(); }));
            else
                _validatingForm.Show();

            if (_validatingForm.ProgressBar.InvokeRequired)
            {
                lstQuality.BeginInvoke(new Action(() =>
                {
                    _validatingForm.ProgressBar.Value = 0;
                    _validatingForm.ProgressBar.Maximum = proxies.Length;
                }));
            }
            else
            {
                _validatingForm.ProgressBar.Value = 0;
                _validatingForm.ProgressBar.Maximum = proxies.Length;
            }

            foreach (var line in proxies)
            {
                if (_validatingForm.ProgressBar.InvokeRequired)
                    lstQuality.BeginInvoke(new Action(() => { _validatingForm.ProgressBar.Value++; }));
                else
                    _validatingForm.ProgressBar.Value++;

                try
                {
                    var webRequest = WebRequest.Create(_ipCheckURL);
                    webRequest.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                    webRequest.Headers.Add(
                        "User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

                    var lineArr = line.Split(':');

                    if (lineArr.Length != 4)
                    {
                        MessageBox.Show(new Form { TopMost = true },
                            "Proxy format must be in this format;\r\nIPADDRESS:PORT:USERNAME:PASSWORD\r\nFix and try again.");
                        error = true;
                        break;
                    }

                    IWebProxy proxy = new WebProxy(lineArr[0], Convert.ToInt32(lineArr[1]));
                    var proxyUsername = lineArr[2];
                    var proxyPassword = lineArr[3];
                    proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                    webRequest.Proxy = proxy;

                    webRequest.Timeout = 5000;
                    using var response = webRequest.GetResponse();
                    using var content = response.GetResponseStream();
                    if (content != null)
                    {
                        using var reader = new StreamReader(content);
                        var requestAddress = reader.ReadToEnd();
                        var address = GetIPAddress();

                        if (string.IsNullOrEmpty(requestAddress) || string.IsNullOrEmpty(address))
                        {
                            _nonPrivateProxies.Add(line);
                            continue;
                        }

                        if (address == requestAddress) _nonPrivateProxies.Add(line);
                    }

                    webRequest.Abort();
                }
                catch (Exception)
                {
                    _nonPrivateProxies.Add(line);
                }
            }

            if (_validatingForm.InvokeRequired)
                _validatingForm.BeginInvoke(new Action(() => { _validatingForm.Close(); }));
            else
                _validatingForm.Close();

            if (_nonPrivateProxies.Count > 0)
            {
                var uiThread = new Thread(() =>
                {
                    var proxyDisplayer = new ProxyDisplayer(_nonPrivateProxies);
                    proxyDisplayer.Retest += Retest;
                    Application.Run(proxyDisplayer);
                });
                uiThread.SetApartmentState(ApartmentState.STA);
                uiThread.Start();
                error = true;
            }

            return error;
        }

        private void LogToScreen(string log)
        {
            if (logScreen.InvokeRequired)
            {
                logScreen.BeginInvoke(new Action(() =>
                {
                    logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + log + Environment.NewLine;
                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.ScrollToCaret();
                }));
            }
            else
            {
                logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + log + Environment.NewLine;
                logScreen.SelectionStart = logScreen.TextLength;
                logScreen.ScrollToCaret();
            }
        }

        private void UpdateBot(bool isForcedUpdate = false)
        {
            if (isForcedUpdate)
            {
                ExecuteUpdate();
                return;
            }

            var dialogResult = MessageBox.Show(GetFromResource("MainScreen_UpdateBot_Do_you_want_to_update_"),
                GetFromResource("MainScreen_UpdateBot_Newer_version_is_available_"), MessageBoxButtons.YesNo);
            
            if (dialogResult == DialogResult.Yes)
            {
                ExecuteUpdate();
            }

            void ExecuteUpdate()
            {
                var args = "https://streamviewerbot.com/Download/win-x64.zip" + "*" +
                           AppDomain.CurrentDomain.BaseDirectory.Replace(' ', '?') + "*" +
                           Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace(' ', '?'), "StreamViewerBot.exe");
                try
                {
                    Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoUpdaterOld"), true);
                }
                catch (Exception)
                {
                    //ignored
                }

                try
                {
                    var tempUpdaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoUpdaterTemp");
                    var updaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoUpdater");
                    Directory.CreateDirectory(tempUpdaterPath);
                    foreach (var file in Directory.GetFiles(updaterPath))
                    {
                        var destFile = Path.Combine(tempUpdaterPath, Path.GetFileName(file));
                        File.Move(file, destFile, true);
                    }

                    var filename = Path.Combine(tempUpdaterPath, "AutoUpdater.exe");
                    Process.Start(filename, args);
                    Environment.Exit(0);
                }
                catch (Exception)
                {
                    MessageBox.Show(GetFromResource("MainScreen_UpdateBot_Sorry__updater_failed_"));
                }
            }
        }

        private string GetFromResource(string key)
        {
            return Resources.ResourceManager.GetString(key);
        }

        private void LoadFromAppSettings()
        {
            LogInfo(new Exception("Reading configuration."));
            _proxyListDirectory = txtProxyList.Text = _configuration.AppSettings.Settings["proxyListDirectory"].Value;
            _chatMessagesDirectory = txtChatMessages.Text = _configuration.AppSettings.Settings["chatMessageDirectory"].Value;
            txtStreamUrl.Text = _configuration.AppSettings.Settings["streamUrl"].Value;
            _headless = checkHeadless.Checked =
                Convert.ToBoolean(_configuration.AppSettings.Settings["headless"].Value);
            numRefreshMinutes.Value = Convert.ToInt32(_configuration.AppSettings.Settings["refreshInterval"].Value);
            _withLoggedIn = Convert.ToBoolean(_configuration.AppSettings.Settings["withLoggedIn"].Value);
            txtLoginInfos.Text = _configuration.AppSettings.Settings["loginInfos"].Value;
            checkLowCpuRam.Checked = Convert.ToBoolean(_configuration.AppSettings.Settings["uselowcpuram"].Value);

            ShowLoggedInPart(_withLoggedIn);
        }

        private void ShowLoggedInPart(bool visibility)
        {
            ClientSize = visibility
                ? _loginSize
                : new Size(_loginSize.Width - txtLoginInfos.Width - tipLiveViewer.Width / 2, _loginSize.Height);
        }

        private void startStopButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProxyList.Text) || string.IsNullOrEmpty(txtStreamUrl.Text))
            {
                LogInfo(new Exception("Please choose a proxy directory and enter your stream URL."));
                return;
            }

            _lstLoginInfo.Clear();

            if (_withLoggedIn)
            {
                foreach (var line in txtLoginInfos.Text.Split("\r\n"))
                {
                    var parts = line.Split(' ');

                    if (parts.Length != 2)
                    {
                        LogInfo(new Exception("Please correct the format of your login credentials"));

                        return;
                    }

                    _lstLoginInfo.Enqueue(new LoginDto { Username = parts[0], Password = parts[1] });
                }

                if (!CheckChatMessages())
                    return;
            }

            _serviceType = StreamService.Service.Twitch;

            if (lstserviceType.InvokeRequired)
            {
                lstserviceType.BeginInvoke(new Action(() =>
                {
                    _serviceType = (StreamService.Service)lstserviceType.SelectedValue;
                }));
            }
            else
            {
                _serviceType = (StreamService.Service)lstserviceType.SelectedValue;
            }

            if (lstQuality.InvokeRequired)
                lstQuality.BeginInvoke(new Action(() => { _quality = lstQuality.SelectedValue.ToString(); }));
            else
                _quality = lstQuality.SelectedValue.ToString();

            _canStart = !_canStart;

            if (_canStart)
            {
                startStopButton.BackgroundImage =
                    Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\button_stop.png");
                LogInfo(new Exception("Initializing bot."));
                _core.CanRun = true;
                _tokenSource = new CancellationTokenSource();

                Task.Run(() => { RunIt(null); }, _tokenSource.Token);

                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                startStopButton.BackgroundImage =
                    Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\button_stopping.png");
                startStopButton.Enabled = false;
                LogInfo(new Exception("Terminating bot, please wait."));

                _tokenSource.Cancel();
                _core.CanRun = false;

                try
                {
                    _core.Stop();
                }
                catch (Exception)
                {
                    LogInfo(new Exception("Termination error. (Ignored)"));
                }

                _core.InitializationError -= ErrorOccurred;

                _core.LogMessage -= LogMessage;

                _core.DidItsJob -= DidItsJob;

                _core.IncreaseViewer -= IncreaseViewer;

                _core.DecreaseViewer -= DecreaseViewer;

                startStopButton.BackgroundImage =
                    Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\button_start.png");
                startStopButton.Enabled = true;
            }
        }

        private void RunIt(object obj)
        {
            LogInfo(new Exception("Saving the configuration."));

            _configuration.AppSettings.Settings["streamUrl"].Value = txtStreamUrl.Text;
            _configuration.AppSettings.Settings["headless"].Value = checkHeadless.Checked.ToString();
            _configuration.AppSettings.Settings["proxyListDirectory"].Value = txtProxyList.Text;
            _configuration.AppSettings.Settings["chatMessageDirectory"].Value = txtChatMessages.Text;
            _configuration.AppSettings.Settings["refreshInterval"].Value = numRefreshMinutes.Value.ToString();
            _configuration.AppSettings.Settings["withLoggedIn"].Value = _withLoggedIn.ToString();
            _configuration.AppSettings.Settings["loginInfos"].Value = txtLoginInfos.Text;
            _configuration.AppSettings.Settings["uselowcpuram"].Value = checkLowCpuRam.Checked.ToString();

            _configuration.Save(ConfigurationSaveMode.Modified);

            LogInfo(new Exception("Bot is starting."));

            int.TryParse(txtBrowserLimit.Text, out var browserLimit);

            _headless = checkHeadless.Checked;

            _core.AllBrowsersTerminated += AllBrowsersTerminated;

            _core.InitializationError += ErrorOccurred;

            _core.LogMessage += LogMessage;

            _core.DidItsJob += DidItsJob;

            _core.IncreaseViewer += IncreaseViewer;

            _core.DecreaseViewer += DecreaseViewer;

            _core.LiveViewer += SetLiveViewer;

            var needs = new ExecuteNeedsDto()
            {
                Headless = _headless,
                Service = _serviceType,
                Stream = txtStreamUrl.Text,
                BrowserLimit = browserLimit,
                ChatMessages = _chatMessages,
                LoginInfos = _lstLoginInfo,
                PreferredQuality = _quality,
                RefreshInterval = Convert.ToInt32(numRefreshMinutes.Value),
                ProxyListDirectory = _proxyListDirectory,
                UseLowCpuRam = false //TEMPORARY DISABLED => checkLowCpuRam.Checked
            };
            _core.Start(needs);
        }


        private void SetBotViewer(string count)
        {
            if (lblViewer.InvokeRequired)
                lblViewer.BeginInvoke(new Action(() => { lblViewer.Text = count; }));
            else
                lblViewer.Text = count;
        }

        private void SetLiveViewer(string count)
        {
            if (lblLiveViewer.InvokeRequired)
                lblLiveViewer.BeginInvoke(new Action(() => { lblLiveViewer.Text = count; }));
            else
                lblLiveViewer.Text = count;
        }

        private void DecreaseViewer()
        {
            if (lblViewer.InvokeRequired)
                lblViewer.BeginInvoke(new Action(() =>
                {
                    lblViewer.Text = (Convert.ToInt32(lblViewer.Text) - 1).ToString();
                }));
            else
                lblViewer.Text = (Convert.ToInt32(lblViewer.Text) - 1).ToString();
        }

        private void IncreaseViewer()
        {
            if (lblViewer.InvokeRequired)
                lblViewer.BeginInvoke(new Action(() =>
                {
                    lblViewer.Text = (Convert.ToInt32(lblViewer.Text) + 1).ToString();
                }));
            else
                lblViewer.Text = (Convert.ToInt32(lblViewer.Text) + 1).ToString();
        }

        private void ErrorOccurred(Exception exception)
        {
            LogError(exception);
        }

        private void LogMessage(Exception exception)
        {
            LogInfo(exception);
        }

        private void AllBrowsersTerminated()
        {
            startStopButton.BackgroundImage =
                Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\button_stop.png");

            SetBotViewer("0");
            SetLiveViewer("0");

            LogInfo(new Exception("Bot terminated."));

            _core.AllBrowsersTerminated -= AllBrowsersTerminated;
        }

        private void DidItsJob()
        {
            LogInfo(new Exception("Bot did it's job, wait at least 3-5 minutes to see on the stream platform."));
        }

        private void LogInfo(Exception exception)
        {
            try
            {
                Log.Logger.Information(exception.Message);
            }
            catch (Exception)
            {
                //ignored
            }

            if (logScreen.InvokeRequired)
            {
                logScreen.BeginInvoke(new Action(() =>
                {
                    logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + exception.Message +
                                      Environment.NewLine;
                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.ScrollToCaret();
                }));
            }
            else
            {
                logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + exception.Message +
                                  Environment.NewLine;
                logScreen.SelectionStart = logScreen.TextLength;
                logScreen.ScrollToCaret();
            }
        }

        private void LogError(Exception exception)
        {
            try
            {
                if (exception.Message.Contains("Timeout") && exception.Message.Contains("exceeded") || exception.Message.Contains("ERR_TIMED_OUT"))
                {
                    exception = new Exception(
                        "Load Timeout: Low system resources may cause this. Close unused applications.");
                }

                Log.Logger.Error(exception.ToString());
            }
            catch (Exception)
            {
                //ignored
            }

            if (logScreen.InvokeRequired)
            {
                logScreen.BeginInvoke(new Action(() =>
                {
                    logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + exception.Message +
                                      Environment.NewLine;
                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.ScrollToCaret();
                }));
            }
            else
            {
                logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + exception.Message +
                                  Environment.NewLine;
                logScreen.SelectionStart = logScreen.TextLength;
                logScreen.ScrollToCaret();
            }
        }

        private void browseProxyList_Click(object sender, EventArgs e)
        {
            MessageBox.Show("You have to use private proxies, free proxies will not work!");
            
            var fileDialog = new OpenFileDialog();

            fileDialog.Filter = "txt files (*.txt)|*.txt";
            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = false;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _proxyListDirectory = txtProxyList.Text = fileDialog.FileName;

                _validatingForm = new ValidatingForm();
                _validatingForm.Show();
                _validatingForm.Location = new Point(Location.X + Width / 2 - _validatingForm.Width / 2,
                    Location.Y + Height / 2 - _validatingForm.Height / 2);

                _ = Task.Run(CheckProxiesArePrivateOrNot);
            }
        }

        private void chatMessagesList_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Only Twitch, NimoTV and Trovo Live supported for now!\r\n\r\nCreate a .txt file with ; separated messages.\r\nBot will consume your messages and won't send a sent message again, so make your list as long as possible.\r\n\r\nPlease don't forget to disable Followers-only mode and Subscriber-only chat on Twitch Moderation Settings", "Warning");
            var fileDialog = new OpenFileDialog();

            fileDialog.Filter = "txt files (*.txt)|*.txt";
            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = false;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _chatMessagesDirectory = txtChatMessages.Text = fileDialog.FileName;

                _ = Task.Run(CheckChatMessages);
            }
        }

        private void MainScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!_canStart)
                    _core.Stop();
            }
            catch (Exception)
            {
                LogInfo(new Exception("Termination error. (Ignored)"));
            }

            Application.Exit();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {
                var strCmdLine = "/C explorer \"https://www.vultr.com/?ref=8827163\"";
                var browserProcess = Process.Start("CMD.exe", strCmdLine);
                browserProcess?.Close();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void picLimitInfo_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(tipLimitInfo,
                "Feature disabled temporarily."); //"Rotates proxies with limited quantity of browser. Old ones dies, new ones born. 0 means, no limit.");
        }

        private void lblProxyList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var strCmdLine =
                    "/C explorer \"https://github.com/gorkemhacioglu/Stream-Viewer-Bot/wiki/Configuration#:~:text=Your%20proxy%20list.%20You%20have%20to%20buy%20private%20proxies.";
                var browserProcess = Process.Start("CMD.exe", strCmdLine);
                browserProcess?.Close();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void refreshInterval_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(tipRefreshBrowser, "Refreshes browser, just in case. Example: Connection loss");
        }

        private void txtBrowserLimit_TextChanged(object sender, EventArgs e)
        {
            var value = txtBrowserLimit.Text;

            if (value == "0" || value == string.Empty)
            {
                lblRefreshMin.Enabled = lblRefreshMin2.Enabled =
                    lblRefreshMin3.Enabled = numRefreshMinutes.Enabled = tipRefreshBrowser.Enabled = true;
            }
            else
            {
                lblRefreshMin.Enabled = lblRefreshMin2.Enabled = lblRefreshMin3.Enabled =
                    numRefreshMinutes.Enabled = tipRefreshBrowser.Enabled = false;
                numRefreshMinutes.Value = 0;
                _withLoggedIn = false;
                ShowLoggedInPart(false);
            }
        }

        private void lstQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQuality.InvokeRequired)
                lstQuality.BeginInvoke(new Action(() =>
                {
                    _core.PreferredQuality = lstQuality.SelectedValue.ToString();
                }));
            else
                _core.PreferredQuality = lstQuality.SelectedValue.ToString();
        }

        private void streamQuality_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(tipQuality, "Stream quality.");
        }

        private void btnWithLoggedIn_Click(object sender, EventArgs e)
        {
            _withLoggedIn = !_withLoggedIn;

            if (_withLoggedIn)
            {
                checkLowCpuRam.Checked = false;
                checkLowCpuRam.Enabled = false;
            }
            else if (!checkHeadless.Checked)
            {
                checkLowCpuRam.Enabled = true;
            }

            var serviceType = StreamService.Service.Twitch;

            if (lstserviceType.InvokeRequired)
                lstserviceType.BeginInvoke(new Action(() =>
                {
                    serviceType = (StreamService.Service)lstserviceType.SelectedValue;
                }));
            else
                serviceType = (StreamService.Service)lstserviceType.SelectedValue;

            if (serviceType == StreamService.Service.NimoTv)
                MessageBox.Show(
                    GetFromResource("MainScreen_NimoTVTypeWithCountryCode"), "Information");

            ShowLoggedInPart(_withLoggedIn);
        }

        private void tipLiveViewer_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(tipLiveViewer, "Your bots will be here soon.");
        }

        private void MainScreen_Shown(object sender, EventArgs e)
        {
            _loginSize = ClientSize;

            LoadFromAppSettings();
        }

        private void checkHeadless_CheckedChanged(object sender, EventArgs e)
        {
            if (checkHeadless.Checked)
            {
                MessageBox.Show(
                    GetFromResource(
                        "MainScreen_checkHeadless_CheckedChanged_Enable_IP_authorization_to_use_your_proxies_in_headless_mode"),
                    GetFromResource("MainScreen_checkHeadless_CheckedChanged_Warning"), MessageBoxButtons.OK);

                checkLowCpuRam.Checked = false;

                checkLowCpuRam.Enabled = false;
            }
            else if (!_withLoggedIn)
            {
                checkLowCpuRam.Enabled = true;
            }
        }

        private void picWebshare_Click(object sender, EventArgs e)
        {
            try
            {
                var strCmdLine = "/C explorer \"https://www.webshare.io/?referral_code=ceuygyx4sir2";
                var browserProcess = Process.Start("CMD.exe", strCmdLine);
                browserProcess?.Close();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void picWebshare_MouseEnter(object sender, EventArgs e)
        {
            picWebshare.BackColor = Color.LightGray;
        }

        private void picWebshare_MouseLeave(object sender, EventArgs e)
        {
            picWebshare.BackColor = Color.Transparent;
        }

        private void picVulture_MouseEnter(object sender, EventArgs e)
        {
            picVulture.BackColor = Color.LightGray;
        }

        private void picVulture_MouseLeave(object sender, EventArgs e)
        {
            picVulture.BackColor = Color.Transparent;
        }

        private void lstServiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnWithLoggedIn.Visible = checkLowCpuRam.Enabled = lstQuality.Enabled = true;

            var comboBox = (ComboBox)sender;

            switch (comboBox.SelectedValue)
            {
                case StreamService.Service.Twitch:
                    _dataSourceQuality.Clear();
                    _dataSourceQuality.Add("Source", string.Empty);
                    _dataSourceQuality.Add("480p", "{\"default\":\"480p30\"}");
                    _dataSourceQuality.Add("360p", "{\"default\":\"360p30\"}");
                    _dataSourceQuality.Add("160p", "{\"default\":\"160p30\"}");
                    break;
                case StreamService.Service.Facebook:
                    MessageBox.Show(
                        GetFromResource("MainScreen_LoginRequiredForFacebook"), "Information");
                    break;
                case StreamService.Service.Twitter:
                    if (_withLoggedIn)
                    {
                        btnWithLoggedIn_Click(null, null);
                        btnWithLoggedIn.Visible = false;
                    }
                    break;
                case StreamService.Service.DLive:
                    if (_withLoggedIn)
                    {
                        btnWithLoggedIn_Click(null, null);
                        btnWithLoggedIn.Visible = false;
                    }
                    break;
                case StreamService.Service.TrovoLive:
                    _dataSourceQuality.Clear();
                    _dataSourceQuality.Add("Source", "");
                    _dataSourceQuality.Add("1080p", "4|5000");
                    _dataSourceQuality.Add("720p", "3|2500");
                    _dataSourceQuality.Add("480p", "2|1500");
                    _dataSourceQuality.Add("360p", "1|600");
                    _dataSourceQuality.Add("144p", "0|280");
                    break;
                case StreamService.Service.BigoLive:
                    MessageBox.Show("Login is required for Bigo Live.\r\nPlease login manually after browsers deployed. Automated login is not available yet :(", "Information");
                    checkLowCpuRam.Enabled = lstQuality.Enabled = false;
                    break;
                default:
                    checkLowCpuRam.Enabled = lstQuality.Enabled = false;
                    break;
            }
            
            if (lstQuality.InvokeRequired)
                lstQuality.BeginInvoke(new Action(() =>
                {
                    lstQuality.DataSource = new BindingSource(_dataSourceQuality, null);
                    lstQuality.SelectedIndex = _dataSourceQuality.Count - 1;
                }));
            else
            {
                lstQuality.DataSource = new BindingSource(_dataSourceQuality, null);
                lstQuality.SelectedIndex = _dataSourceQuality.Count - 1;
            }
        }
    }
}