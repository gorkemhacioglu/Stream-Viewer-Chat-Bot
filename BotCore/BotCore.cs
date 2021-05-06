using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Threading.Timer;

namespace BotCore
{
    public class Core
    {
        public static string zipDirectory = "";

        public static string proxyListDirectory = "";

        public static string streamUrl = "";

        public static bool headless = false;

        public static int browserLimit = 0;

        public bool canRun = true;

        bool _muteClicked = false;

        private static readonly ConcurrentDictionary<int, Thread> _threads = new ConcurrentDictionary<int, Thread>();

        private static readonly ConcurrentQueue<int> _threadIds = new ConcurrentQueue<int>();

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private System.IO.StreamReader _file = null;

        public void Start(string proxyListDirectory, string stream, bool headless, int browserLimit)
        {
            Core.browserLimit = browserLimit;
            canRun = true;
            Core.headless = headless;
            int i = 0;
            streamUrl = stream;
            System.IO.DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\");
            foreach (FileInfo res in di.GetFiles())
            {
                res.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            if (Core.browserLimit > 0)
            {
                Thread thr = new Thread(LoopWithLimit);
                thr.Start();
            }

            zipDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\background";

            do
            {
                try
                {
                    Debug.WriteLine("New Loop");

                    _file = new System.IO.StreamReader(proxyListDirectory);

                    string line;

                    while (canRun && (line = _file.ReadLine()) != null)
                    {
                        var array = line.ToString().Split(':');
                        string text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory\\backgroundTemplate.js");
                        text = text.Replace("{ip}", array[0]).Replace("{port}", array[1]).Replace("{username}", array[2]).Replace("{password}", array[3]);
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory\\background.js", text);

                        ZipFile.CreateFromDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory", AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\background" + i + ".zip");

                        Thread thr = new Thread(Request) { Priority = ThreadPriority.Normal };
                        Random r = new Random();
                        int rInt = r.Next(3000, 6000);

                        while (browserLimit > 0 && _threads.Count >= Core.browserLimit)
                        {
                            Thread.Sleep(500);
                        }

                        _lock.EnterWriteLock();
                        thr.Start(new Item { Url = line, Count = i });
                        _threadIds.Enqueue(thr.ManagedThreadId);
                        _threads.TryAdd(thr.ManagedThreadId, thr);
                        _lock.ExitWriteLock();
                        i++;

                        Thread.Sleep(browserLimit == 0 ? rInt : 50);
                    }

                    _file.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Loop Error:" + e);
                }

            } while (browserLimit > 0);


        }

        public void Stop()
        {
            canRun = false;

            _file.Close();

            _lock.EnterReadLock();

            while (_threads.Count > 0)
            {
                _threadIds.TryDequeue(out var threadId);
                _threads.TryRemove(threadId, out var tempThread);

                if (tempThread != null) tempThread.Priority = ThreadPriority.Highest;
            }

            _threads.Clear();

            _lock.ExitReadLock();
        }

        private void LoopWithLimit()
        {
            while (canRun)
            {
                try
                {
                    if (_threads.Count >= browserLimit)
                    {
                        Thread tempThread = null;

                        _lock.EnterWriteLock();

                        _threadIds.TryDequeue(out var threadId);

                        _threads.TryGetValue(threadId, out tempThread);

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

                if (headless)
                    chromeOptions.AddArgument("headless");

                string[] resolutions = { "960,720", "1080,720", "1280,800", "1280,720", "960,600", "1024,768", "800,600" };
                chromeOptions.AddArgument("window-size=" + resolutions[r.Next(0, resolutions.Length - 1)]);
                chromeOptions.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36");
                chromeOptions.AddExcludedArgument("enable-automation");
                chromeOptions.AddAdditionalCapability("useAutomationExtension", false);
                chromeOptions.AddExtension(zipDirectory + itm.Count + ".zip");
                chromeOptions.PageLoadStrategy = PageLoadStrategy.Default;

                var driver = new ChromeDriver(chromeOptions) { Url = streamUrl };

                driver.Navigate();

                bool matureClicked = false;
                int matureCheckCount = 0;

                bool cacheClicked = false;
                int cacheCheckCount = 0;

                if (browserLimit > 0)
                {
                    Thread.Sleep(1000);

                    Thread.CurrentThread.Priority = ThreadPriority.Highest;
                }


                var startDate = DateTime.Now;

                while (Thread.CurrentThread.Priority != ThreadPriority.Highest)
                {
                    try
                    {
                        if (!_muteClicked)
                        {
                            _muteClicked = true;
                            new Actions(driver).SendKeys("m").Perform();
                        }

                        if (!matureClicked && matureCheckCount < 5)
                        {
                            var mature = driver.FindElementsByClassName("tw-flex-grow-0");

                            foreach (var btn in mature)
                            {
                                try
                                {
                                    if (btn.Text != null && btn.Text == "Start Watching")
                                    {
                                        btn.Click();
                                        matureClicked = true;
                                    }
                                }
                                catch
                                {
                                    //ignored
                                }
                            }

                            matureCheckCount++;
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
                                //ignored because there is no cache button
                            }
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

                        if (DateTime.Now - startDate > TimeSpan.FromMinutes(5))
                        {
                            driver.Navigate().Refresh();

                            startDate = DateTime.Now;
                        }

                        #region StreamQuality
                        //try
                        //{
                        //    var player = driver.FindElementByXPath("/html/body/div[1]/div/div[2]/div/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div[2]");

                        //    if (player != null)
                        //    {
                        //        Actions actions = new Actions(driver);

                        //        actions.ContextClick(player).Perform();

                        //        Thread.Sleep(1500);

                        //        var settings = driver.FindElementByXPath("/html/body/div[1]/div/div[2]/div/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div[7]/div/div[2]/div[2]/div[1]/div[2]/div/button/span/div/div/div/svg/g/path[1]");

                        //        if (settings != null)
                        //        {
                        //            actions.Click(settings).Perform();

                        //            var quality = driver.FindElementByXPath("/html/body/div[1]/div/div[2]/div/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div[7]/div/div[2]/div[2]/div[1]/div[1]/div/div/div/div/div/div/div[3]/button/div");

                        //            if (quality != null)
                        //            {
                        //                actions.Click(quality).Perform();

                        //                var onesixty = driver.FindElementByXPath("/html/body/div[1]/div/div[2]/div/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div[7]/div/div[2]/div[2]/div[1]/div[1]/div/div/div/div/div/div/div[8]/div/div/div/label/div");

                        //                if (onesixty != null)
                        //                    actions.Click(onesixty).Perform();
                        //            }
                        //        }
                        //    }


                        //}
                        //catch (Exception)
                        //{
                        //    //ignored
                        //}
                        #endregion
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                }

                driver.Quit();

                _threads.TryRemove(Thread.CurrentThread.ManagedThreadId, out _);
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
    }
}
