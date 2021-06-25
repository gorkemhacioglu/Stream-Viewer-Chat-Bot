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
        public bool start = false;

        private static string _productVersion = "1.1";

        public static string proxyListDirectory = "";

        public static string streamUrl = "";

        public static bool headless = false;

        public Thread mainThread = null;

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
            DialogResult dialogResult = MessageBox.Show("Do you want to update?", "Newer version is available", MessageBoxButtons.YesNo);
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
            proxyListDirectory = txtProxyList.Text = _configuration.AppSettings.Settings["proxyListDirectory"].Value;
            streamUrl = txtStreamUrl.Text = _configuration.AppSettings.Settings["streamUrl"].Value;
            headless = checkHeadless.Checked = Convert.ToBoolean(_configuration.AppSettings.Settings["headless"].Value);
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
            start = !start;

            if (start)
            {
                startStopButton.BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory() + "\\Images\\button_stop.png");
                LogInfo("Initializing bot.");
                core.canRun = true;
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
                core.canRun = false;

                try
                {
                    core.Stop();
                }
                catch (Exception)
                {
                    LogInfo("Termination error. (Ignored)");
                }
                LogInfo("Bot terminated.");

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
            _configuration.Save(ConfigurationSaveMode.Modified);

            LogInfo("Configuration saved.");
            LogInfo("Bot is starting.");

            Int32.TryParse(txtBrowserLimit.Text, out var browserLimit);

            core.Start(proxyListDirectory, txtStreamUrl.Text, headless, browserLimit);

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
                string sFileName = proxyListDirectory = txtProxyList.Text = fileDialog.FileName;
            }
        }

        private void emergency_Click(object sender, EventArgs e)
        {

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
                var browserProcess = System.Diagnostics.Process.Start("CMD.exe", strCmdLine);
                browserProcess?.Close();
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }
}
