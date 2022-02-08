using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BotCore;
using BotCore.Dto;
using Newtonsoft.Json;
using StreamViewerBot.Properties;
using StreamViewerBot.UI;

namespace StreamViewerBot
{
    public partial class MainScreen : Form
    {
        public bool Start;

        private static string _productVersion = "2.7.3";

        private static string _proxyListDirectory = "";

        private static bool _headless;

        private string _appId = "";

        private bool _withLoggedIn = false;

        CancellationTokenSource _tokenSource = new CancellationTokenSource();

        readonly Configuration _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        readonly Dictionary<string, string> _dataSourceQuality = new Dictionary<string, string>();

        readonly Dictionary<string, StreamService.Service> _serviceTypes = new Dictionary<string, StreamService.Service>();

        private readonly ConcurrentQueue<LoginDto> _lstLoginInfo = new ConcurrentQueue<LoginDto>();

        private Size _loginSize = new Size();

        public Core Core = new Core();

        private ValidatingForm _validatingForm = new ValidatingForm();


        public MainScreen()
        {
            InitializeComponent();

            _appId = _configuration.AppSettings.Settings["appId"].Value;

            if (string.IsNullOrEmpty(_appId))
            {
                _configuration.AppSettings.Settings["appId"].Value = Guid.NewGuid().ToString();
                _configuration.Save(ConfigurationSaveMode.Modified);
            }

            BotCore.Log.Logger.CreateLogger(_appId);

            Text += " v" + _productVersion;

            LogInfo(new Exception($"Application started. v{_productVersion}"));

            var isAvailable = IsNewerVersionAvailable();

            if (isAvailable)
                UpdateBot();

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

        private bool IsNewerVersionAvailable()
        {
            try
            {
                var webRequest = WebRequest.Create(@"https://streamviewerbot.com/Download/latestVersion.txt");
                webRequest.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webRequest.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
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

        private void CheckProxiesArePrivateOrNot()
        {
            try
            {
                var error = false;

                var nonPrivateProxies = new List<string>();

                var _file = new StreamReader(_proxyListDirectory);

                string line;

                while ((line = _file.ReadLine()) != null)
                {
                    try
                    {
                        var webRequest = WebRequest.Create(@"https://ipgeolocation.abstractapi.com/v1/?api_key=29f41b2e9e914f14af81ca80a66d6980");
                        webRequest.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                        webRequest.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

                        var lineArr = line.Split(':');

                        if (lineArr.Length != 4)
                        {
                            MessageBox.Show(new Form() { TopMost = true }, "Proxy format must be in this format;\r\nIPADDRESS:PORT:USERNAME:PASSWORD\r\nFix and try again.");
                            error = true;
                            break;
                        }

                        IWebProxy proxy = new WebProxy(lineArr[0], Convert.ToInt32(lineArr[1]));
                        string proxyUsername = lineArr[2];
                        string proxyPassword = lineArr[3];
                        proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                        webRequest.Proxy = proxy;

                        webRequest.Timeout = 2000;
                        using var response = webRequest.GetResponse();
                        using var content = response.GetResponseStream();
                        if (content != null)
                        {
                            using var reader = new StreamReader(content);
                            var json = reader.ReadToEnd();
                            var data = JsonConvert.DeserializeObject<ProxyResponseDto.Root>(json);
                            if (GetIPAddress() == data.ip_address)
                            {
                                nonPrivateProxies.Add(line);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        nonPrivateProxies.Add(line);
                    }

                }

                if (_validatingForm.InvokeRequired)
                {
                    _validatingForm.BeginInvoke(new Action(() =>
                    {
                        _validatingForm.Close();

                    }));
                }
                else
                {
                    _validatingForm.Close();
                }

                _file.Dispose();

                if (nonPrivateProxies.Count > 0)
                {
                    var proxyDisplayer = new ProxyDisplayer(nonPrivateProxies);
                    proxyDisplayer.Show();
                    error = true;
                }

                if(!error)
                    MessageBox.Show("Proxies are working OK!");

                string GetIPAddress()
                {
                    String address = "";
                    WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
                    using (WebResponse response = request.GetResponse())
                    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                    {
                        address = stream.ReadToEnd();
                    }

                    int first = address.IndexOf("Address: ") + 9;
                    int last = address.LastIndexOf("</body>");
                    address = address.Substring(first, last - first);

                    return address;
                }
            }
            catch (Exception exception)
            {
                try
                {
                    Serilog.Log.Logger.Error(exception.ToString());
                }
                catch (Exception)
                {
                    //ignored
                }
            }
        }



        private void UpdateBot()
        {
            DialogResult dialogResult = MessageBox.Show(GetFromResource("MainScreen_UpdateBot_Do_you_want_to_update_"), GetFromResource("MainScreen_UpdateBot_Newer_version_is_available_"), MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
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
                        string destFile = Path.Combine(tempUpdaterPath, Path.GetFileName(file));
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
            txtStreamUrl.Text = _configuration.AppSettings.Settings["streamUrl"].Value;
            _headless = checkHeadless.Checked = Convert.ToBoolean(_configuration.AppSettings.Settings["headless"].Value);
            numRefreshMinutes.Value = Convert.ToInt32(_configuration.AppSettings.Settings["refreshInterval"].Value);
            _withLoggedIn = Convert.ToBoolean(_configuration.AppSettings.Settings["withLoggedIn"].Value);
            txtLoginInfos.Text = _configuration.AppSettings.Settings["loginInfos"].Value;
            checkLowCpuRam.Checked = Convert.ToBoolean(_configuration.AppSettings.Settings["uselowcpuram"].Value);

            ShowLoggedInPart(_withLoggedIn);
        }

        private void ShowLoggedInPart(bool visibility)
        {
            ClientSize = visibility ? _loginSize : new Size(_loginSize.Width - txtLoginInfos.Width - (tipLiveViewer.Width / 2), _loginSize.Height);
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

                    _lstLoginInfo.Enqueue(new LoginDto() { Username = parts[0], Password = parts[1] });
                }
            }

            Start = !Start;

            if (Start)
            {
                startStopButton.BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\button_stop.png");
                LogInfo(new Exception("Initializing bot."));
                Core.CanRun = true;
                _tokenSource = new CancellationTokenSource();

                Task.Run(() =>
                {
                    RunIt(null);
                }, _tokenSource.Token);

                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                startStopButton.BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\button_stopping.png");
                startStopButton.Enabled = false;
                LogInfo(new Exception("Terminating bot, please wait."));

                _tokenSource.Cancel();
                Core.CanRun = false;

                try
                {
                    Core.Stop();
                }
                catch (Exception)
                {
                    LogInfo(new Exception("Termination error. (Ignored)"));
                }

                Core.InitializationError -= ErrorOccurred;

                Core.LogMessage -= LogMessage;

                Core.DidItsJob -= DidItsJob;

                Core.IncreaseViewer -= IncreaseViewer;

                Core.DecreaseViewer -= DecreaseViewer;

                startStopButton.BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\button_start.png");
                startStopButton.Enabled = true;
            }
        }

        private void RunIt(object obj)
        {
            LogInfo(new Exception("Saving the configuration."));

            _configuration.AppSettings.Settings["streamUrl"].Value = txtStreamUrl.Text;
            _configuration.AppSettings.Settings["headless"].Value = checkHeadless.Checked.ToString();
            _configuration.AppSettings.Settings["proxyListDirectory"].Value = txtProxyList.Text;
            _configuration.AppSettings.Settings["refreshInterval"].Value = numRefreshMinutes.Value.ToString();
            _configuration.AppSettings.Settings["withLoggedIn"].Value = _withLoggedIn.ToString();
            _configuration.AppSettings.Settings["loginInfos"].Value = txtLoginInfos.Text;
            _configuration.AppSettings.Settings["uselowcpuram"].Value = checkLowCpuRam.Checked.ToString();

            _configuration.Save(ConfigurationSaveMode.Modified);

            LogInfo(new Exception("Configuration saved."));
            LogInfo(new Exception("Bot is starting."));

            Int32.TryParse(txtBrowserLimit.Text, out var browserLimit);

            _headless = checkHeadless.Checked;

            Core.AllBrowsersTerminated += AllBrowsersTerminated;

            Core.InitializationError += ErrorOccurred;

            Core.LogMessage += LogMessage;

            Core.DidItsJob += DidItsJob;

            Core.IncreaseViewer += IncreaseViewer;

            Core.DecreaseViewer += DecreaseViewer;

            Core.LiveViewer += SetLiveViewer;

            var quality = string.Empty;

            if (lstQuality.InvokeRequired)
            {
                lstQuality.BeginInvoke(new Action(() =>
                {
                    quality = lstQuality.SelectedValue.ToString();
                }));
            }
            else
            {
                quality = lstQuality.SelectedValue.ToString();
            }

            var serviceType = StreamService.Service.Twitch;

            if (lstserviceType.InvokeRequired)
            {
                lstQuality.BeginInvoke(new Action(() =>
                {
                    serviceType = (StreamService.Service)lstserviceType.SelectedValue;
                }));
            }
            else
            {
                serviceType = (StreamService.Service)lstserviceType.SelectedValue;
            }

            Core.Start(_proxyListDirectory, txtStreamUrl.Text, serviceType, _headless, browserLimit, Convert.ToInt32(numRefreshMinutes.Value), quality, _lstLoginInfo, checkLowCpuRam.Checked);
        }


        private void SetBotViewer(string count)
        {
            if (lblViewer.InvokeRequired)
            {
                lblViewer.BeginInvoke(new Action(() =>
                {
                    lblViewer.Text = count;
                }));
            }
            else
            {
                lblViewer.Text = count;
            }
        }

        private void SetLiveViewer(string count)
        {
            if (lblLiveViewer.InvokeRequired)
            {
                lblLiveViewer.BeginInvoke(new Action(() =>
                {
                    lblLiveViewer.Text = count;
                }));
            }
            else
            {
                lblLiveViewer.Text = count;
            }
        }

        private void DecreaseViewer()
        {
            if (lblViewer.InvokeRequired)
            {
                lblViewer.BeginInvoke(new Action(() =>
                {
                    lblViewer.Text = (Convert.ToInt32(lblViewer.Text) - 1).ToString();
                }));
            }
            else
            {
                lblViewer.Text = (Convert.ToInt32(lblViewer.Text) - 1).ToString();
            }
        }

        private void IncreaseViewer()
        {
            if (lblViewer.InvokeRequired)
            {
                lblViewer.BeginInvoke(new Action(() =>
                {
                    lblViewer.Text = (Convert.ToInt32(lblViewer.Text) + 1).ToString();
                }));
            }
            else
            {
                lblViewer.Text = (Convert.ToInt32(lblViewer.Text) + 1).ToString();
            }
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
            startStopButton.BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\Images\\button_stop.png");

            SetBotViewer("0");
            SetLiveViewer("0");

            LogInfo(new Exception("Bot terminated."));

            Core.AllBrowsersTerminated -= AllBrowsersTerminated;
        }

        private void DidItsJob()
        {
            LogInfo(new Exception("Bot did it's job, wait for viewers."));
        }

        private void LogInfo(Exception exception)
        {
            try
            {
                Serilog.Log.Logger.Information(exception.Message.ToString());
            }
            catch (Exception)
            {
                //ignored
            }

            if (logScreen.InvokeRequired)
            {
                logScreen.BeginInvoke(new Action(() =>
                {
                    logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + exception.Message + "\r\n";
                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.ScrollToCaret();
                }));
            }
            else
            {
                logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + exception.Message + "\r\n";
                logScreen.SelectionStart = logScreen.TextLength;
                logScreen.ScrollToCaret();
            }
        }

        private void LogError(Exception exception)
        {
            try
            {
                Serilog.Log.Logger.Error(exception.ToString());
            }
            catch (Exception)
            {
                //ignored
            }

            if (logScreen.InvokeRequired)
            {
                logScreen.BeginInvoke(new Action(() =>
                {
                    logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + exception.Message + "\r\n";
                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.ScrollToCaret();
                }));
            }
            else
            {
                logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + exception.Message + "\r\n";
                logScreen.SelectionStart = logScreen.TextLength;
                logScreen.ScrollToCaret();
            }
        }

        private void browseProxyList_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.Filter = "txt files (*.txt)|*.txt";
            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = false;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _proxyListDirectory = txtProxyList.Text = fileDialog.FileName;
                _validatingForm = new ValidatingForm();
                _validatingForm.Show();
                _validatingForm.Location = new Point(this.Location.X + this.Width / 2 - _validatingForm.Width / 2, this.Location.Y + this.Height / 2 - _validatingForm.Height / 2);

                var t = Task.Run(() => CheckProxiesArePrivateOrNot());
            }
        }

        private void MainScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {
                string strCmdLine = "/C explorer \"https://www.vultr.com/?ref=8827163\"";
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
            toolTip.SetToolTip(tipLimitInfo, "Feature disabled temporarily.");//"Rotates proxies with limited quantity of browser. Old ones dies, new ones born. 0 means, no limit.");
        }

        private void lblProxyList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string strCmdLine = "/C explorer \"https://github.com/gorkemhacioglu/TwitchViewerBot/wiki/Configuration";
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
                lblRefreshMin.Enabled = lblRefreshMin2.Enabled = lblRefreshMin3.Enabled = numRefreshMinutes.Enabled = tipRefreshBrowser.Enabled = true;
            }
            else
            {
                lblRefreshMin.Enabled = lblRefreshMin2.Enabled = lblRefreshMin3.Enabled = numRefreshMinutes.Enabled = tipRefreshBrowser.Enabled = false;
                numRefreshMinutes.Value = 0;
                _withLoggedIn = false;
                ShowLoggedInPart(false);
            }
        }

        private void lstQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQuality.InvokeRequired)
            {
                lstQuality.BeginInvoke(new Action(() =>
                {
                    Core.PreferredQuality = lstQuality.SelectedValue.ToString();
                }));
            }
            else
            {
                Core.PreferredQuality = lstQuality.SelectedValue.ToString();
            }
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
                MessageBox.Show(GetFromResource("MainScreen_checkHeadless_CheckedChanged_Enable_IP_authorization_to_use_your_proxies_in_headless_mode"), GetFromResource("MainScreen_checkHeadless_CheckedChanged_Warning"), MessageBoxButtons.OK);

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
                string strCmdLine = "/C explorer \"https://www.webshare.io/?referral_code=ceuygyx4sir2";
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

        private void lstserviceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            switch (comboBox.SelectedValue)
            {
                case StreamService.Service.Facebook:
                    MessageBox.Show(
                        GetFromResource("MainScreen_LoginRequiredForFacebook"));
                    break;
            }
        }
    }
}
