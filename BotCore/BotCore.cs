using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Threading;

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

        private static readonly ConcurrentDictionary<int, Thread> Threads = new ConcurrentDictionary<int, Thread>();

        private static readonly ConcurrentQueue<int> ThreadIds = new ConcurrentQueue<int>();

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private StreamReader _file;

        public Action AllBrowsersTerminated;

        public Action<string> InitializationError;

        public Action<string> LogMessage;

        public Action DidItsJob;

        public void Start(string proxyListDirectory, string stream, bool headless, int browserLimit, int refreshInterval, string preferredQuality)
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

            if (Core.BrowserLimit > 0)
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
                        line = line.Replace(" ","");

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

                        while (browserLimit > 0 && Threads.Count >= Core.BrowserLimit)
                        {
                            Thread.Sleep(500);
                        }

                        if (!CanRun)
                            continue;

                        _lock.EnterWriteLock();
                        thr.Start(new Item { Url = line, Count = i, PreferredQuality = preferredQuality});
                        ThreadIds.Enqueue(thr.ManagedThreadId);
                        Threads.TryAdd(thr.ManagedThreadId, thr);
                        _lock.ExitWriteLock();
                        i++;

                        Thread.Sleep(browserLimit == 0 ? rInt : 50);
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
                DidItsJob.Invoke();
        }

        public void Stop()
        {
            CanRun = false;

            _file.Close();

            _lock.EnterWriteLock();

            while (Threads.Count > 0)
            {
                ThreadIds.TryDequeue(out var threadId);
                Threads.TryGetValue(threadId, out var tempThread);

                if (tempThread != null) tempThread.Priority = ThreadPriority.Highest;
            }

            _lock.ExitWriteLock();

            AllBrowsersTerminated?.Invoke();
        }

        private void LoopWithLimit()
        {
            while (CanRun)
            {
                try
                {
                    if (Threads.Count >= BrowserLimit)
                    {
                        Thread tempThread = null;

                        _lock.EnterWriteLock();

                        ThreadIds.TryDequeue(out var threadId);

                        Threads.TryGetValue(threadId, out tempThread);

                        _lock.ExitWriteLock();

                        if (tempThread != null)
                        {
                            tempThread.Priority = ThreadPriority.Highest;

                            while (tempThread.IsAlive)
                            {
                                Thread.Sleep(500);
                            }
                        }
                    }
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
                Item itm = (Item)obj;
                var array = itm.Url.ToString().Split(':');
                var proxy = new Proxy { HttpProxy = array[0] + ':' + array[1], SslProxy = array[0] + ':' + array[1] };
                var chromeOptions = new ChromeOptions { Proxy = proxy, AcceptInsecureCertificates = true };

                if (Headless)
                    chromeOptions.AddArgument("headless");

                string[] resolutions = { "960,720", "1080,720", "1280,800", "1280,720", "960,600", "1024,768", "800,600" };
                chromeOptions.AddArgument("window-size=" + resolutions[r.Next(0, resolutions.Length - 1)]);
                chromeOptions.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36");
                chromeOptions.AddExcludedArgument("enable-automation");
                chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
                chromeOptions.AddExtension(ZipDirectory + itm.Count + ".zip");
                chromeOptions.PageLoadStrategy = PageLoadStrategy.Default;

                var driver = new ChromeDriver(chromeOptions) { Url = StreamUrl };

                IJavaScriptExecutor js = driver;
                js.ExecuteScript("window.localStorage.setItem('video-quality', '" + itm.PreferredQuality + "'); window.localStorage.setItem('volume', '0');");
                
                driver.Navigate().Refresh();
                
                bool matureClicked = false;
                int matureCheckCount = 0;
                bool cacheClicked = false;
                int cacheCheckCount = 0;

                if (BrowserLimit > 0)
                {
                    Thread.Sleep(1000);

                    Thread.CurrentThread.Priority = ThreadPriority.Highest;
                }


                var startDate = DateTime.Now;

                while (Thread.CurrentThread.Priority != ThreadPriority.Highest)
                {
                    try
                    {
                        if (!matureClicked && matureCheckCount < 5)
                        {
                            try
                            {
                                var mature = driver.FindElementByXPath(
                                    "/html/body/div[1]/div/div[2]/div/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div[5]/div/div[3]/button/div/div");

                                if (mature != null)
                                {
                                    mature.Click();
                                }

                                matureCheckCount++;
                            }
                            catch
                            {
                                //ignored because there is no mature button
                            }
                        }

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
                                //ignored because themre is no cache button
                            }
                            Thread.Sleep(250);
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
                    catch (Exception)
                    {
                        //ignored
                    }
                }

                driver.Quit();

                Threads.TryRemove(Thread.CurrentThread.ManagedThreadId, out _);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex);
            }
        }
    }
    public class Item
    {
        public string Url { get; set; }

        public int Count { get; set; }

        public string PreferredQuality { get; set; }
    }
}
