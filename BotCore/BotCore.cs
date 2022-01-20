using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using BotCore.Dto;
using Newtonsoft.Json;

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

        private bool _firstPage = true;

        private bool _useLowCpuRam;

        private static readonly ConcurrentQueue<ChromeDriverService> DriverServices = new ConcurrentQueue<ChromeDriverService>();

        private StreamReader _file;

        public Action AllBrowsersTerminated;

        public Action<Exception> InitializationError;

        public Action<Exception> LogMessage;

        public Action DidItsJob;

        public Action IncreaseViewer;

        public Action DecreaseViewer;

        public Action<string> LiveViewer;

        private readonly object _lockObject = new object();

        private readonly string _loginCookiesPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "loginCookies.json");

        private List<Process> _initialChromeProcesses = new List<Process>();

        readonly JsonSerializerSettings _isoDateFormatSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
        };

        public void Start(string proxyListDirectory, string stream, StreamService.Service service, bool headless, int browserLimit, int refreshInterval, string preferredQuality, ConcurrentQueue<LoginDto> loginInfos, bool useLowCpuRam)
        {
            BrowserLimit = browserLimit;
            CanRun = true;
            _firstPage = true;
            _useLowCpuRam = useLowCpuRam;
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

            _initialChromeProcesses = Process.GetProcessesByName("chrome").ToList();

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

                        var array = line.Split(':');

                        string text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory\\backgroundTemplate.js");
                        text = text.Replace("{ip}", array[0]).Replace("{port}", array[1]).Replace("{username}", array[2]).Replace("{password}", array[3]);
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory\\background.js", text);

                        ZipFile.CreateFromDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory", AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\background" + i + ".zip");

                        Thread thr = new Thread(Request) { Priority = ThreadPriority.AboveNormal };
                        Random r = new Random();
                        int rInt = r.Next(5000, 8000);

                        while (BrowserLimit > 0 && DriverServices.Count >= BrowserLimit)
                        {
                            Thread.Sleep(1000);
                        }

                        if (!CanRun)
                            continue;

                        loginInfos.TryDequeue(out var loginInfo);

                        thr.Start(new SessionConfigurationDto { Url = line, Count = i, PreferredQuality = preferredQuality, LoginInfo = loginInfo, Service = service });

                        i++;

                        Thread.Sleep(BrowserLimit == 0 ? rInt : 1000);
                    }

                    _file.Close();
                }
                catch (Exception e)
                {
                    InitializationError?.Invoke(e is IndexOutOfRangeException
                        ? new Exception("Please select a valid proxy file.")
                        : e);
                }

                if (!CanRun)
                    break;

            } while (browserLimit > 0);

            DidItsJob?.Invoke();
        }

        private void StoreCookie(Tuple<string, List<Cookie>> cookie)
        {

            var myCookie = new List<MyCookie>();

            foreach (var item in cookie.Item2)
            {
                if (item.Expiry != null)
                    myCookie.Add(new MyCookie()
                    {
                        Domain = item.Domain,
                        Expiry = item.Expiry.Value.Ticks,
                        HttpOnly = item.IsHttpOnly,
                        Name = item.Name,
                        Path = item.Path,
                        Value = item.Value,
                        Secure = item.Secure
                    });
                else
                {
                    myCookie.Add(new MyCookie()
                    {
                        Domain = item.Domain,
                        Expiry = DateTime.MaxValue.Ticks,
                        HttpOnly = item.IsHttpOnly,
                        Name = item.Name,
                        Path = item.Path,
                        Value = item.Value,
                        Secure = item.Secure
                    });
                }
            }
            lock (_lockObject)
            {
                if (!File.Exists(_loginCookiesPath))
                {
                    var item = new Dictionary<string, List<MyCookie>> { { cookie.Item1, myCookie } };
                    File.WriteAllText(_loginCookiesPath, Newtonsoft.Json.JsonConvert.SerializeObject(item), Encoding.UTF8);
                    return;
                }

                string readCookiesJson = File.ReadAllText(_loginCookiesPath);
                var readCookies = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<MyCookie>>>(readCookiesJson);

                readCookies.TryGetValue(cookie.Item1, out var value);

                if (value?.Count > 0)
                {
                    readCookies[cookie.Item1] = myCookie;
                }
                else
                    readCookies.Add(cookie.Item1, myCookie);


                File.WriteAllText(_loginCookiesPath, Newtonsoft.Json.JsonConvert.SerializeObject(readCookies), Encoding.UTF8);
            }
        }

        private List<MyCookie> GetCookie(string username)
        {
            lock (_lockObject)
            {
                if (!File.Exists(_loginCookiesPath))
                {
                    return new List<MyCookie>();
                }

                string readCookiesJson = File.ReadAllText(_loginCookiesPath);
                var readCookies = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<MyCookie>>>(readCookiesJson);

                return readCookies.FirstOrDefault(x => x.Key == username).Value;
            }
        }

        private void KillAllProcesses()
        {
            var allChromeProcesses = Process.GetProcessesByName("chrome");

            foreach (var process in allChromeProcesses)
            {
                if (!(_initialChromeProcesses.Any(x => x.Id == process.Id)))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        CreateNoWindow = true,
                        FileName = "CMD.exe"
                    };
                    string strCmd = $"/C taskkill /F /PID {process.Id}";
                    startInfo.Arguments = strCmd;
                    Process processTemp = new Process();
                    processTemp.StartInfo = startInfo;
                    processTemp.Start();
                }
            }

            _initialChromeProcesses.Clear();

            var allChromeDriverProcesses = Process.GetProcessesByName("chromedriver");

            foreach (var chromeDriverService in allChromeDriverProcesses)
            {
                try
                {
                    chromeDriverService.Kill();
                }
                catch (Exception)
                {
                    //ignored
                }
            }
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
                    if (DriverServices.Count >= BrowserLimit)
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

                Proxy proxy = new Proxy
                {
                    Kind = ProxyKind.Manual
                };
                string proxyUrl = array[0] + ":" + array[1];
                proxy.SslProxy = proxyUrl;
                proxy.HttpProxy = proxyUrl;

                var chromeOptions = new ChromeOptions { Proxy = proxy, AcceptInsecureCertificates = true };

                var localChrome = AppDomain.CurrentDomain.BaseDirectory + "\\Extensions\\LocalChrome\\chrome.exe";
                if (File.Exists(localChrome))
                {
                    chromeOptions.BinaryLocation = localChrome;
                }


                if (_useLowCpuRam)
                {
                    chromeOptions.AddExtension(AppDomain.CurrentDomain.BaseDirectory +
                                               "\\Extensions\\TwitchAlternative.crx");
                }

                if (Headless)
                    chromeOptions.AddArgument("headless");
                else
                    chromeOptions.AddExtension(ZipDirectory + itm.Count + ".zip");

                string[] resolutions =
                    { "960,720", "1080,720", "1280,800", "1280,720", "960,600", "1024,768", "800,600" };

                chromeOptions.AddArgument("window-size=" + resolutions[r.Next(0, resolutions.Length - 1)]);
                chromeOptions.AddArgument(
                    "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36");
                chromeOptions.AddArgument("mute-audio");

                chromeOptions.AddExcludedArgument("enable-automation");

                chromeOptions.AddAdditionalOption("useAutomationExtension", false);

                chromeOptions.PageLoadStrategy = PageLoadStrategy.Default;

                var driver = new ChromeDriver(driverService, chromeOptions)
                { Url = StreamUrl };

                if (BrowserLimit > 0)
                {
                    Thread.Sleep(1000);

                    return;
                }

                DriverServices.Enqueue(driverService);

                IncreaseViewer?.Invoke();

                bool firstPage = false;

                var startDate = DateTime.Now;

                if (itm.Service == StreamService.Service.Twitch)
                {
                    if (!Headless && !_useLowCpuRam)
                    {
                        IJavaScriptExecutor js = driver;
                        js.ExecuteScript("window.localStorage.setItem('video-quality', '" + itm.PreferredQuality + "');");
                        driver.Navigate().Refresh();
                    }

                    bool matureClicked = false;
                    int matureCheckCount = 0;
                    bool cacheClicked = false;
                    int cacheCheckCount = 0;

                    if (itm.LoginInfo != null)
                    {
                        Thread.Sleep(1000);

                        var allCookies = GetCookie(itm.LoginInfo.Username);

                        if (allCookies != null)
                        {
                            foreach (var cookie in allCookies)
                            {
                                driver.Manage().Cookies.AddCookie(new Cookie(cookie.Name, cookie.Value, cookie.Domain,
                                    cookie.Path, new DateTime(ticks: cookie.Expiry)));
                            }
                        }

                        try
                        {
                            var loginButton = driver.FindElement(By.XPath(
                                "/html/body/div[1]/div/div[2]/nav/div/div[3]/div[3]/div/div[1]/div[1]/button/div/div"));

                            if (loginButton != null)
                            {
                                loginButton.Click();

                                Thread.Sleep(1000);

                                var usernameBox = driver.FindElement(By.XPath(
                                    "/html/body/div[3]/div/div/div/div/div/div[1]/div/div/div[3]/form/div/div[1]/div/div[2]/input"));

                                if (usernameBox != null)
                                {
                                    usernameBox.Click();

                                    Thread.Sleep(1000);

                                    usernameBox.SendKeys(itm.LoginInfo.Username);

                                    var passwordBox = driver.FindElement(By.XPath(
                                        "/html/body/div[3]/div/div/div/div/div/div[1]/div/div/div[3]/form/div/div[2]/div/div[1]/div[2]/div[1]/input"));

                                    if (passwordBox != null)
                                    {
                                        passwordBox.Click();

                                        Thread.Sleep(1000);

                                        passwordBox.SendKeys(itm.LoginInfo.Password);

                                        Thread.Sleep(1000);

                                        var login = driver.FindElement(By.XPath(
                                            "/html/body/div[3]/div/div/div/div/div/div[1]/div/div/div[3]/form/div/div[3]/button/div/div"));


                                        Thread.Sleep(1000);

                                        login?.Click();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMessage?.Invoke(new Exception($"Login failed: {ex.Message}"));
                        }

                        while (true)
                        {
                            Thread.Sleep(1000);

                            var cookie = driver.Manage().Cookies.GetCookieNamed("auth-token");

                            if (!string.IsNullOrEmpty(cookie?.Value))
                            {
                                StoreCookie(new Tuple<string, List<Cookie>>(itm.LoginInfo.Username,
                                    new List<Cookie>(driver.Manage().Cookies.AllCookies)));

                                break;
                            }
                        }
                    }

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                if (_firstPage)
                                {
                                    firstPage = true;
                                    _firstPage = false;
                                }

                                if (firstPage)
                                {
                                    var liveViewers = driver.FindElement(By.XPath(
                                        "/html/body/div[1]/div/div[2]/div[1]/main/div[2]/div[3]/div/div/div[1]/div[1]/div[2]/div/div[1]/div/div/div/div[2]/div[2]/div[2]/div/div/div[1]/div[1]/div/p/span"));

                                    if (liveViewers != null)
                                    {
                                        LiveViewer.Invoke(liveViewers.Text);
                                        Thread.Sleep(5000);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                LiveViewer.Invoke("N/A");
                            }

                            Thread.Sleep(1000);

                            try
                            {
                                if (!matureClicked && matureCheckCount < 5)
                                {
                                    try
                                    {
                                        var mature = driver.FindElement(By.XPath(
                                            "/html/body/div[1]/div/div[2]/div[1]/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div/div[7]/div/div[3]/button/div/div"));

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
                                        var cache = driver.FindElement(By.XPath(
                                            "/html/body/div[1]/div/div[2]/div[1]/div/div/div[3]/button/div/div/div"));

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
                                var connectionError = driver.FindElement(By.XPath(
                                    "//*[@id=\"root\"]/div/div[2]/div/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div[5]/div/div[3]/button/div/div[2]"));

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
                    catch (Exception)
                    {
                        //ignored
                    }
                }
                else if (itm.Service == StreamService.Service.Youtube)
                {
                    Thread.Sleep(3000);

                    try
                    {
                        var play = driver.FindElement(By.XPath(
                            "/html/body/ytd-app/div/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[1]/div/div/div/ytd-player/div/div/div[5]/button"));

                        play?.Click();
                    }
                    catch (Exception)
                    {
                        //ignored
                    }


                    #region Quality
                    //Thread.Sleep(5500);

                    //var skip = driver.FindElements(By.ClassName("ytp-ad-skip-button-container"));
                    //skip.FirstOrDefault()?.Click();

                    //Thread.Sleep(1000);

                    //var settings = driver.FindElements(By.ClassName("ytp-settings-button"));
                    //settings.FirstOrDefault()?.Click();

                    //Thread.Sleep(2000);

                    //var qualityButton = driver.FindElement(By.XPath(
                    //    "/html/body/ytd-app/div/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[1]/div/div/div/ytd-player/div/div/div[27]/div/div/div[3]/div[3]"));

                    //if (qualityButton != null)
                    //{
                    //    qualityButton.Click();
                    //}

                    //Thread.Sleep(1000);

                    //var lowestQuality = driver.FindElement(By.XPath(
                    //    "/html/body/ytd-app/div/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[1]/div/div/div/ytd-player/div/div/div[27]/div/div/div[3]/div[3]"));

                    //lowestQuality?.Click();
                    #endregion

                    //if (itm.LoginInfo != null)
                    //{
                    //    #region Login

                    //    var signIn = driver.FindElement(By.XPath(
                    //        "/html/body/ytd-app/div/div/ytd-masthead/div[3]/div[3]/div[2]/ytd-button-renderer/a/tp-yt-paper-button"));

                    //    signIn?.Click();

                    //    Thread.Sleep(3000);

                    //    var usernameBox = driver.FindElement(By.XPath(
                    //        "/html/body/div[1]/div[1]/div[2]/div/div[2]/div/div/div[2]/div/div[1]/div/form/span/section/div/div/div[1]/div/div[1]/div/div[1]/input"));

                    //    usernameBox.SendKeys(itm.LoginInfo.Username);

                    //    Thread.Sleep(1000);

                    //    var next = driver.FindElement(By.XPath(
                    //        "/html/body/div[1]/div[1]/div[2]/div/div[2]/div/div/div[2]/div/div[2]/div/div[1]/div/div/button/span"));

                    //    next?.Click();

                    //    Thread.Sleep(3000);

                    //    var passwordBox = driver.FindElement(By.XPath(
                    //        "/html/body/div[1]/div[1]/div[2]/div/div[2]/div/div/div[2]/div/div[1]/div/form/span/section/div/div/div[1]/div[1]/div/div/div/div/div[1]/div/div[1]/input"));

                    //    passwordBox.SendKeys(itm.LoginInfo.Password);

                    //    Thread.Sleep(1000);

                    //    next = driver.FindElement(By.XPath(
                    //        "/html/body/div[1]/div[1]/div[2]/div/div[2]/div/div/div[2]/div/div[2]/div/div[1]/div/div/button/span"));

                    //    next?.Click();

                    //    #endregion
                    //}

                    while (true)
                    {
                        try
                        {
                            if (_firstPage)
                            {
                                firstPage = true;
                                _firstPage = false;
                            }

                            if (firstPage)
                            {
                                var liveViewers = driver.FindElement(By.XPath(
                                    "/html/body/ytd-app/div/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[6]/div[2]/ytd-video-primary-info-renderer/div/div/div[1]/div[1]/ytd-video-view-count-renderer/span[1]"));

                                if (liveViewers != null)
                                {
                                    LiveViewer.Invoke(liveViewers.Text.Split(' ')[0]);
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            LiveViewer.Invoke("N/A");
                        }


                        if (_refreshInterval != 0 && DateTime.Now - startDate > TimeSpan.FromMinutes(_refreshInterval))
                        {
                            driver.Navigate().Refresh();

                            startDate = DateTime.Now;
                        }
                    }
                }
                else if (itm.Service == StreamService.Service.DLive)
                {
                    Thread.Sleep(3000);

                    var isPlaying = false;

                    while (true)
                    {
                        try
                        {
                            if (_firstPage)
                            {
                                firstPage = true;
                                _firstPage = false;
                            }

                            if (firstPage)
                            {
                                try
                                {
                                    var liveViewers = driver.FindElement(By.XPath(
                                        "/html/body/div/div[1]/div[20]/div[2]/div/div[2]/div/div/div/div[1]/div/div[1]/div[3]/div/div[1]/div/div[2]/div[2]"));

                                    if (liveViewers != null)
                                    {
                                        LiveViewer.Invoke(liveViewers.Text.Split(" ")[0]);
                                        Thread.Sleep(5000);
                                    }
                                }
                                catch (Exception e)
                                {
                                    //ignored
                                }

                                try
                                {
                                    var liveViewers = driver.FindElement(By.XPath(
                                        "/html/body/div/div[1]/div[18]/div[2]/div/div/div/div/div/div/div/div/div[3]/div/div[3]/div/div/div[1]/div/div[1]/div[2]/div/div[1]/span"));

                                    if (liveViewers != null)
                                    {
                                        LiveViewer.Invoke(liveViewers.Text);
                                        Thread.Sleep(5000);
                                    }
                                }
                                catch (Exception)
                                {
                                    //ignored
                                }
                            }

                            if ((!isPlaying))
                            {
                                var play = driver.FindElement(By.XPath(
                                    "/html/body/div/div[1]/div[14]/div[2]/div/div[2]/div/div/div/div/div/div/div[1]/div/div/div/div/div[4]/div[2]/button/svg"));

                                if (play != null)
                                {
                                    play?.Click();
                                    isPlaying = true;
                                }
                            }

                            Thread.Sleep(1000);
                        }
                        catch (Exception)
                        {
                            //ignored
                        }
                    }
                }
                else if (itm.Service == StreamService.Service.NimoTv)
                {
                    Thread.Sleep(3000);

                    var isPlaying = false;

                    while (true)
                    {
                        try
                        {
                            if (_firstPage)
                            {
                                firstPage = true;
                                _firstPage = false;
                            }

                            if (firstPage)
                            {
                                var liveViewers = driver.FindElement(By.XPath(
                                    "/html/body/div[2]/div[2]/div[2]/div[1]/div/div/div[2]/div[2]/div[1]/div[1]/div/div[2]/div[3]/span"));

                                if (liveViewers != null)
                                {
                                    LiveViewer.Invoke(liveViewers.Text);
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            LiveViewer.Invoke("N/A");
                        }

                        try
                        {
                            if ((!isPlaying))
                            {
                                var play = driver.FindElement(By.XPath(
                                    "/html/body/div[2]/div[2]/div[2]/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div[1]/div[2]/div/span"));

                                if (play != null)
                                {
                                    play?.Click();
                                    isPlaying = true;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            //ignored
                        }

                        Thread.Sleep(1000);
                    }
                }
                else if (itm.Service == StreamService.Service.Twitter)
                {
                    Thread.Sleep(3000);

                    while (true)
                    {
                        try
                        {
                            if (_firstPage)
                            {
                                firstPage = true;
                                _firstPage = false;
                            }

                            if (firstPage)
                            {
                                var liveViewers = driver.FindElement(By.XPath(
                                    "/html/body/div[2]/div[2]/div[2]/div[1]/div/div/div[2]/div[2]/div[1]/div[1]/div/div[2]/div[3]/span"));

                                if (liveViewers != null)
                                {
                                    LiveViewer.Invoke(liveViewers.Text);
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            LiveViewer.Invoke("N/A");
                        }
                        Thread.Sleep(1000);
                    }
                }
                else if (itm.Service == StreamService.Service.Facebook)
                {
                    Thread.Sleep(3000);


                    while (true)
                    {
                        try
                        {
                            if (_firstPage)
                            {
                                firstPage = true;
                                _firstPage = false;
                            }

                            if (firstPage)
                            {
                                var liveViewers = driver.FindElement(By.XPath(
                                    "/html/body/div[1]/div/div[1]/div/div[3]/div/div/div[1]/div[2]/div[1]/div/div/div/div[1]/div[1]/div/div/div/div[2]/div/div[5]/div[2]/span[2]"));

                                if (liveViewers != null)
                                {
                                    LiveViewer.Invoke(liveViewers.Text);
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            //ignored
                        }

                    }
                }

                try
                {
                    driver.Quit();
                }
                catch (Exception)
                {
                    //ignored
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("only supports Chrome version"))
                {
                    CanRun = false;
                    InitializationError?.Invoke(new Exception($"Please update your Google Chrome!"));
                }
            }
            catch (Exception ex)
            {
                InitializationError?.Invoke(ex);
            }
        }

        private class MyCookie
        {
            public bool Secure { get; set; }
            public bool HttpOnly { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public string Domain { get; set; }
            public string Path { get; set; }
            public long Expiry { get; set; }
        }
    }
}
