using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using BotCore.Dto;

namespace BotCore
{
    public class Core
    {
        public static string ZipDirectory = string.Empty;

        public static string StreamUrl = string.Empty;

        public static bool Headless;

        public static int BrowserLimit;

        public string PreferredQuality;

        private int _refreshInterval;

        public bool CanRun = true;

        private bool _error;

        private bool _firstPage = true;

        private static readonly ConcurrentQueue<ChromeDriverService> DriverServices = new ConcurrentQueue<ChromeDriverService>();

        private StreamReader _file;

        public Action AllBrowsersTerminated;

        public Action<string> InitializationError;

        public Action<string> LogMessage;

        public Action DidItsJob;

        public Action IncreaseViewer;

        public Action DecreaseViewer;

        public Action<string> LiveViewer;

        public void Start(string proxyListDirectory, string stream, bool headless, int browserLimit, int refreshInterval, string preferredQuality, ConcurrentQueue<LoginDto> loginInfos)
        {
            BrowserLimit = browserLimit;
            CanRun = true;
            Headless = headless;
            PreferredQuality = preferredQuality;
            _refreshInterval = refreshInterval;
            int i = 0;
            StreamUrl = stream;
            DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\");
            foreach (FileInfo res in di.GetFiles())
            {
                res.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            if (BrowserLimit > 0)
            {
                Thread thr = new Thread(LoopWithLimit);
                thr.Start();
            }

            ZipDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\background";

            do
            {
                try
                {
                    _file = new StreamReader(proxyListDirectory);

                    string line;

                    while (CanRun && (line = _file.ReadLine()) != null)
                    {
                        line = line.Replace(" ", "");

                        if (string.IsNullOrEmpty(line))
                            continue;

                        var array = line.ToString().Split(':');

                        string text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory\\backgroundTemplate.js");
                        text = text.Replace("{ip}", array[0]).Replace("{port}", array[1]).Replace("{username}", array[2]).Replace("{password}", array[3]);
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory\\background.js", text);

                        ZipFile.CreateFromDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory", AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\background" + i + ".zip");

                        Thread thr = new Thread(Request) { Priority = ThreadPriority.Normal };
                        Random r = new Random();
                        int rInt = r.Next(3000, 6000);

                        while (BrowserLimit > 0 && DriverServices.Count >= BrowserLimit)
                        {
                            Thread.Sleep(1000);
                        }

                        if (!CanRun)
                            continue;

                        loginInfos.TryDequeue(out var loginInfo);

                        thr.Start(new SessionConfigurationDto { Url = line, Count = i, PreferredQuality = preferredQuality, LoginInfo = loginInfo });

                        if (loginInfo != null)
                            loginInfos.Enqueue(loginInfo);

                        i++;

                        Thread.Sleep(BrowserLimit == 0 ? rInt : 1000);
                    }

                    _file.Close();
                }
                catch (Exception e)
                {
                    InitializationError.Invoke(e is IndexOutOfRangeException
                        ? "Please select a valid proxy file."
                        : $"Uppss! {e.Message}");
                    _error = true;
                }

                if (!CanRun)
                    break;

            } while (browserLimit > 0);

            if (!_error)
                DidItsJob?.Invoke();
        }

        private void KillAllProcesses() 
        {
            string strCmd = string.Empty;

            strCmd = "/C taskkill /IM " + "chrome.exe" + " /F";
            Process.Start("CMD.exe", strCmd);

            //foreach (var driverService in DriverServices)
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            if (!driverService.IsRunning)
            //                break;

            //            strCmdText = "/C taskkill /F /PID " + Process.GetProcessById(driverService.ProcessId);
            //            Process.Start("CMD.exe", strCmdText);

            //            break;
            //        }
            //        catch (Exception ex)
            //        {
            //            //ignored
            //        }
            //    }
            //}

            strCmd = "/C taskkill /IM " + "chromedriver.exe" + " /F";
            Process.Start("CMD.exe", strCmd);
        }

        public void Stop()
        {
            CanRun = false;

            _file.Close();

            KillAllProcesses();

            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\zipSource");

            foreach (var file in files)
            {
                File.Delete(file);
            }

            DriverServices.Clear();

            AllBrowsersTerminated?.Invoke();
        }

        private void LoopWithLimit()
        {
            while (CanRun)
            {
                try
                {
                    if(DriverServices.Count >= BrowserLimit) 
                    { 
                        KillAllProcesses();
                        DriverServices.Clear();
                    }
                    Thread.Sleep(500);
                }
                catch (Exception)
                {
                    //ignored
                }
            }
        }

        private void Request(object obj)
        {
            try
            {
                Random r = new Random();
                SessionConfigurationDto itm = (SessionConfigurationDto)obj;
                var array = itm.Url.Split(':');

                var driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;

                Proxy proxy = new Proxy();
                proxy.Kind = ProxyKind.Manual;
                string proxyUrl = array[0] + ":" + array[1];
                proxy.SslProxy = proxyUrl;
                proxy.HttpProxy = proxyUrl;

                var chromeOptions = new ChromeOptions { Proxy = proxy, AcceptInsecureCertificates = true };

                if (Headless)
                    chromeOptions.AddArgument("headless");
                else
                    chromeOptions.AddExtension(ZipDirectory + itm.Count + ".zip");

                string[] resolutions = { "960,720", "1080,720", "1280,800", "1280,720", "960,600", "1024,768", "800,600" };

                chromeOptions.AddArgument("window-size=" + resolutions[r.Next(0, resolutions.Length - 1)]);
                chromeOptions.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36");
                chromeOptions.AddArgument("mute-audio");

                chromeOptions.AddExcludedArgument("enable-automation");

                chromeOptions.AddAdditionalCapability("useAutomationExtension", false);

                chromeOptions.PageLoadStrategy = PageLoadStrategy.Default;

                var driver = new ChromeDriver(driverService, chromeOptions) { Url = StreamUrl };//"https://www.twitch.tv/"+ Guid.NewGuid() };

                if (!Headless)
                {
                    IJavaScriptExecutor js = driver;
                    js.ExecuteScript("window.localStorage.setItem('video-quality', '" + itm.PreferredQuality + "');");
                    driver.Navigate().Refresh();
                }

                if (BrowserLimit > 0)
                {
                    Thread.Sleep(1000);

                    return;
                 }

                DriverServices.Enqueue(driverService);

                bool matureClicked = false;
                int matureCheckCount = 0;
                bool cacheClicked = false;
                int cacheCheckCount = 0;

                if (itm.LoginInfo != null)
                {
                    try
                    {
                        var loginButton = driver.FindElementByXPath(
                        "/html/body/div[1]/div/div[2]/nav/div/div[3]/div[3]/div/div[1]/div[1]/button/div/div");

                        if (loginButton != null)
                        {
                            loginButton.Click();

                            Thread.Sleep(1000);

                            var usernameBox = driver.FindElementByXPath(
                                "/html/body/div[3]/div/div/div/div/div/div[1]/div/div/div[3]/form/div/div[1]/div/div[2]/input");

                            if (usernameBox != null)
                            {
                                usernameBox.Click();

                                Thread.Sleep(1000);

                                usernameBox.SendKeys(itm.LoginInfo.Username);

                                var passwordBox = driver.FindElementByXPath(
                                    "/html/body/div[3]/div/div/div/div/div/div[1]/div/div/div[3]/form/div/div[2]/div/div[1]/div[2]/div[1]/input");

                                if (passwordBox != null)
                                {
                                    passwordBox.Click();

                                    Thread.Sleep(1000);

                                    passwordBox.SendKeys(itm.LoginInfo.Password);

                                    Thread.Sleep(1000);

                                    var login = driver.FindElementByXPath(
                                        "/html/body/div[3]/div/div/div/div/div/div[1]/div/div/div[3]/form/div/div[3]/button/div/div");

                                    Thread.Sleep(1000);

                                    login?.Click();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        LogMessage.Invoke("Login failed.");
                    }
                }

                var startDate = DateTime.Now;

                IncreaseViewer?.Invoke();

                try
                {
                    var shoudIRun = true;

                    bool firstPage = false;

                    while (shoudIRun)
                    {
                        try
                        {
                            shoudIRun = !Process.GetProcessById(driverService.ProcessId).HasExited;
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        try
                        {

                            if (_firstPage) 
                            {
                                firstPage = _firstPage;
                            }

                            if (firstPage)
                            {
                                var liveViewers = driver.FindElementByXPath(
                                    "/html/body/div[1]/div/div[2]/div[1]/main/div[2]/div[3]/div/div/div[1]/div[1]/div[2]/div/div[1]/div/div/div/div[2]/div[2]/div[2]/div/div/div[1]/div[1]/div/p/span");

                                if (liveViewers != null)
                                {
                                    LiveViewer.Invoke(liveViewers.Text);

                                    _firstPage = false;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        Thread.Sleep(1000);

                        try
                        {
                            if (!matureClicked && matureCheckCount < 5)
                            {
                                try
                                {
                                    var mature = driver.FindElementByXPath(
                                        "/html/body/div[1]/div/div[2]/div[1]/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div/div[5]/div/div[3]/button/div/div");

                                    mature?.Click();
                                    matureClicked = true;
                                    matureCheckCount++;
                                }
                                catch
                                {
                                    //ignored because there is no mature button
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        try
                        {
                            if (!cacheClicked && cacheCheckCount < 5)
                            {
                                try
                                {
                                    var cache = driver.FindElementByXPath("/html/body/div[1]/div/div[2]/div[1]/div/div/div[3]/button/div/div/div");

                                    if (cache != null)
                                    {
                                        cache.Click();
                                        cacheClicked = true;
                                    }

                                    cacheCheckCount++;
                                }
                                catch (Exception)
                                {
                                    //ignored because there is no cache button
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }


                        try
                        {
                            var connectionError = driver.FindElementByXPath("//*[@id=\"root\"]/div/div[2]/div/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div[5]/div/div[3]/button/div/div[2]");

                            if (connectionError != null)
                            {
                                Actions actions = new Actions(driver);

                                actions.Click(connectionError).Perform();
                            }
                        }
                        catch (Exception)
                        {
                            //ignored
                        }

                        if (_refreshInterval != 0 && DateTime.Now - startDate > TimeSpan.FromMinutes(_refreshInterval))
                        {
                            driver.Navigate().Refresh();

                            startDate = DateTime.Now;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //ignored
                }

                DecreaseViewer?.Invoke();

                try
                {
                    driver.Quit();
                }
                catch (Exception)
                {
                    //ignored
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex);
            }
        }
    }
}
