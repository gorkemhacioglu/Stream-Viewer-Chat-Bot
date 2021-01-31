using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BotCore;

namespace TwitchBotUI
{
    public partial class MainScreen : Form
    {
        public bool start = false;
        public static string proxyListDirectory = "";
        public static string streamUrl = "";
        public static bool headless = false;
        public Thread mainThread = null;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token = new CancellationToken();
        Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public Core a = new Core();
        public MainScreen()
        {
            InitializeComponent();
            LogInfo("Application started.");
            LoadFromAppSettings();
        }

        private void LoadFromAppSettings()
        {
            LogInfo("Reading configuration.");
            proxyListDirectory = txtProxyList.Text = configuration.AppSettings.Settings["proxyListDirectory"].Value;
            streamUrl = txtStreamUrl.Text = configuration.AppSettings.Settings["streamUrl"].Value;
            headless = checkHeadless.Checked = Convert.ToBoolean(configuration.AppSettings.Settings["headless"].Value);
            LogInfo("Configuration has been read.");
            //config.AppSettings.Settings["test"].Value = "blah";
            //config.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection("appSettings");
        }

        [Obsolete]
        private void startStopButton_Click(object sender, EventArgs e)
        {
            start = !start;

            if (start)
            {
                startStopButton.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\Images\\button_stop.png");
                LogInfo("Initializing bot.");
                a.canRun = true;
                TaskFactory factory = new TaskFactory(token);
                tokenSource = new CancellationTokenSource();
                var task = Task.Run(() =>
                {
                    RunIt(null);
                }, tokenSource.Token);
                //mainThread = new Thread(RunIt);
                //mainThread.Start();
                ConfigurationManager.RefreshSection("appSettings");
                LogInfo("Bot is running.");
            }
            else
            {
                startStopButton.Image = Image.FromFile(Directory.GetCurrentDirectory() + "\\Images\\button_start.png");
                LogInfo("Terminating bot, please wait.");

                tokenSource.Cancel();
                a.canRun = false;

                try
                {
                    a.Stop();
                }
                catch (Exception)
                {
                    LogInfo("Termination error. (Ignored)");
                }
                LogInfo("Bot terminated.");
            }
        }

        private void RunIt(object obj)
        {
            LogInfo("Saving the configuration.");
            configuration.AppSettings.Settings["streamUrl"].Value = txtStreamUrl.Text;
            configuration.AppSettings.Settings["headless"].Value = checkHeadless.Checked.ToString();
            configuration.AppSettings.Settings["proxyListDirectory"].Value = txtProxyList.Text;
            configuration.Save(ConfigurationSaveMode.Modified);            
            LogInfo("Configuration saved.");
            a.Start(proxyListDirectory, txtStreamUrl.Text, headless);
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
            
            //log.Info(str);
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
            //log.Error(str);
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
    }
}
