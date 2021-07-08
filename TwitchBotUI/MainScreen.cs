using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BotCore;

namespace TwitchBotUI
{
    public partial class MainScreen : Form
    {
        public bool Start = false;

        private static string _productVersion = "1.2";

        private static string _proxyListDirectory = "";

        private static bool _headless = false;

        CancellationTokenSource _tokenSource = new CancellationTokenSource();

        readonly CancellationToken _token = new CancellationToken();

        readonly Configuration _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public Core core = new Core();

        public MainScreen()
        {
            InitializeComponent();

            Text += " v" + _productVersion;

            LogInfo("Application started.");
            LoadFromAppSettings();
            var isAvailable = IsNewerVersionAvailable();

            if (isAvailable)
                UpdateBot();
        }

        private bool IsNewerVersionAvailable()
        {
            try
            {
                var webRequest = WebRequest.Create(@"https://mytwitchbot.com/Download/latestVersion.txt");
                webRequest.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webRequest.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    var latestVersion = reader.ReadToEnd();

                    return latestVersion != _productVersion;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void UpdateBot()
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to update?", "Newer version is available!", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var args = "https://mytwitchbot.com/Download/win-x64.zip" + " " + Directory.GetCurrentDirectory() + " " + Path.Combine(Directory.GetCurrentDirectory(), "TwitchBotUI.exe");
                try
                {
                    Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "AutoUpdaterOld"), true);
                }
                catch (Exception)
                {
                    //ignored
                }
                try
                {
                    var tempUpdaterPath = Path.Combine(Directory.GetCurrentDirectory(), "AutoUpdaterTemp");
                    var updaterPath = Path.Combine(Directory.GetCurrentDirectory(), "AutoUpdater");
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
                    MessageBox.Show("Sorry, updater failed.");
                    return;
                }

            }
        }

        public static DirectoryInfo GetExecutingDirectory()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory;
        }

        private void LoadFromAppSettings()
        {
            LogInfo("Reading configuration.");
            _proxyListDirectory = txtProxyList.Text = _configuration.AppSettings.Settings["proxyListDirectory"].Value;
            txtStreamUrl.Text = _configuration.AppSettings.Settings["streamUrl"].Value;
            _headless = checkHeadless.Checked = Convert.ToBoolean(_configuration.AppSettings.Settings["headless"].Value);
            _proxyListDirectory = txtProxyList.Text = _configuration.AppSettings.Settings["proxyListDirectory"].Value;
            numRefreshMinutes.Value = Convert.ToInt32(_configuration.AppSettings.Settings["refreshInterval"].Value);
            LogInfo("Configuration has been read.");
        }

        [Obsolete]
        private void startStopButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProxyList.Text) || string.IsNullOrEmpty(txtStreamUrl.Text))
            {
                LogInfo("Please choose a proxy directory and enter your stream URL.");
                return;
            }
            Start = !Start;

            if (Start)
            {
                startStopButton.BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory() + "\\Images\\button_stop.png");
                LogInfo("Initializing bot.");
                core.CanRun = true;
                TaskFactory factory = new TaskFactory(_token);
                _tokenSource = new CancellationTokenSource();

                Task.Run(() =>
                {
                    RunIt(null);
                }, _tokenSource.Token);

                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                startStopButton.BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory() + "\\Images\\button_stopping.png");
                startStopButton.Enabled = false;
                LogInfo("Terminating bot, please wait.");

                _tokenSource.Cancel();
                core.CanRun = false;

                try
                {
                    core.Stop();
                }
                catch (Exception)
                {
                    LogInfo("Termination error. (Ignored)");
                }
                
                startStopButton.BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory() + "\\Images\\button_start.png");
                startStopButton.Enabled = true;
            }
        }

        private void RunIt(object obj)
        {
            LogInfo("Saving the configuration.");

            _configuration.AppSettings.Settings["streamUrl"].Value = txtStreamUrl.Text;
            _configuration.AppSettings.Settings["headless"].Value = checkHeadless.Checked.ToString();
            _configuration.AppSettings.Settings["proxyListDirectory"].Value = txtProxyList.Text;
            _configuration.AppSettings.Settings["refreshInterval"].Value = numRefreshMinutes.Value.ToString();
            _configuration.Save(ConfigurationSaveMode.Modified);

            LogInfo("Configuration saved.");
            LogInfo("Bot is starting.");

            Int32.TryParse(txtBrowserLimit.Text, out var browserLimit);

            core.AllBrowsersTerminated += AllBrowsersTerminated;

            core.IntializationError += ErrorOccured;

            core.LogMessage += LogMessage;

            core.DidItsJob += DidItsJob;

            core.Start(_proxyListDirectory, txtStreamUrl.Text, _headless, browserLimit, Convert.ToInt32(numRefreshMinutes.Value));
        }

        private void ErrorOccured(string message)
        {
            core.Stop();

            LogError(message);

            core.AllBrowsersTerminated -= AllBrowsersTerminated;

            core.IntializationError -= ErrorOccured;

            core.LogMessage -= LogMessage;

            core.DidItsJob -= DidItsJob;
        }

        private void LogMessage(string message)
        {
            LogInfo(message);
        }

        private void AllBrowsersTerminated()
        {
            startStopButton.BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory() + "\\Images\\button_stop.png");

            LogInfo("Bot terminated.");
        }

        private void DidItsJob()
        {
            core.IntializationError -= ErrorOccured;

            core.LogMessage -= LogMessage;

            core.DidItsJob -= DidItsJob;

            LogInfo("Bot did it's job.");
        }

        private void LogInfo(string str)
        {
            if (logScreen.InvokeRequired)
            {
                logScreen.BeginInvoke(new Action(() =>
                {
                    logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + str + "\r\n";
                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.ScrollToCaret();
                }));
            }
            else
            {
                logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + str + "\r\n";
                logScreen.SelectionStart = logScreen.TextLength;
                logScreen.ScrollToCaret();
            }
        }

        private void LogError(string str)
        {
            if (logScreen.InvokeRequired)
            {
                logScreen.BeginInvoke(new Action(() =>
                {
                    logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + str + "\r\n";
                    logScreen.SelectionStart = logScreen.TextLength;
                    logScreen.ScrollToCaret();
                }));
            }
            else
            {
                logScreen.Text += DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss | ") + str + "\r\n";
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
                string sFileName = _proxyListDirectory = txtProxyList.Text = fileDialog.FileName;
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
            toolTip.SetToolTip(tipLimitInfo, "Rotates proxies with limited quantity of browser. Old ones dies, new ones born. 0 means, no limit.");
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
            toolTip.SetToolTip(tipRefreshBrowser, "Refreshes browser, just in case.");
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
            }

        }
    }
}
