using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace AutoUpdater
{
    public partial class MainForm : Form
    {
        private string _appDirectory = string.Empty;

        private string _targetUrl = string.Empty;

        private string _updatedAppFile = string.Empty;

        private string _zipPath = string.Empty;

        public MainForm()
        {
            InitializeComponent();

            Shown += new EventHandler(Start);
        }

        private void Start(object sender, EventArgs e)
        {
            try
            {
                var args = Environment.GetCommandLineArgs().Skip(1).ToArray();

                if (args.Length == 1)
                {
                    label1.Text = "Failed to update";

                    return;
                }

                _targetUrl = args[0];

                _appDirectory = args[1];

                _updatedAppFile = args[2];

                var directoryInfo = PrepareTempFolder();

                if (directoryInfo != null)
                {
                    DownloadFiles(_targetUrl, directoryInfo);
                }
            }
            catch (Exception)
            {
                SetLabel("Failed to update due to argument corruption.");
            }
            
        }

        private void ReRunApp(string appFile)
        {
            SetLabel("Starting updated application.");


            Process.Start(@appFile);

            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        private void DownloadFiles(string targetUrl, DirectoryInfo directory)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var targetDirectory = "";
                    _zipPath = targetDirectory = Path.Combine(directory.FullName, "win-x64.zip");

                    SetLabel("Downloading");
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;

                    client.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                    client.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                    client.DownloadFileAsync(new Uri(targetUrl), targetDirectory);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                SetLabel("Failed to download files.");
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled)
            {
                SetLabel("Extracting files.");

                var isExtracted = Extract(_zipPath);

                if (isExtracted)
                {
                    ReRunApp(_updatedAppFile);
                }
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private DirectoryInfo PrepareTempFolder()
        {
            var directory = Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "tempFolder"));

            foreach (var files in directory.GetFiles())
            {
                files.Delete();
            }

            return directory;
        }

        private bool Extract(string zipFile)
        {
            try
            {
                ZipFile.ExtractToDirectory(zipFile, Directory.GetParent(Directory.GetCurrentDirectory()).FullName, true);
                SetLabel("Application updated.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Debug.WriteLine(ex);
                progressBar1.Value = 0;
                SetLabel("Failed to extract files");
                return false;
            }
        }

        private void SetLabel(string label)
        {
            label1.Text = label;
        }
    }
}
