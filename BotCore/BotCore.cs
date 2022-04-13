using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BotCore.Dto;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace BotCore;

public class Core
{
    private static readonly ConcurrentQueue<IBrowser> Browsers = new();

    private static readonly Random _random = new();

    public static string ZipDirectory = string.Empty;

    public static string StreamUrl = string.Empty;

    public static bool Headless;

    public static int BrowserLimit;

    private readonly JsonSerializerSettings _isoDateFormatSettings = new()
    {
        DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
        DateParseHandling = DateParseHandling.DateTime
    };

    private readonly object _lockObject = new();

    private readonly string _loginCookiesPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "loginCookies.json");

    private readonly ConcurrentBag<string> _chatMessages = new();

    private StreamReader _file;

    private bool _firstPage = true;

    private List<Process> _initialChromeProcesses = new();

    private IPlaywright _playwright;

    private int _refreshInterval;

    private bool _useLowCpuRam;

    public Action AllBrowsersTerminated;

    public bool CanRun = true;

    public Action DecreaseViewer;

    public Action DidItsJob;

    public Action IncreaseViewer;

    public Action<Exception> InitializationError;

    public Action<string> LiveViewer;

    public Action<Exception> LogMessage;

    public string PreferredQuality;

    public void Start(string proxyListDirectory, List<string> chatMessages, string stream,
        StreamService.Service service, bool headless,
        int browserLimit, int refreshInterval, string preferredQuality, ConcurrentQueue<LoginDto> loginInfos,
        bool useLowCpuRam)
    {
        if (useLowCpuRam)
            refreshInterval = 1;

        if (_playwright != null)
            _playwright.Dispose();

        _playwright = Playwright.CreateAsync().GetAwaiter().GetResult();

        BrowserLimit = browserLimit;
        CanRun = true;
        _firstPage = true;
        _useLowCpuRam = useLowCpuRam;
        Headless = headless;
        PreferredQuality = preferredQuality;
        _refreshInterval = refreshInterval;
        var i = 0;
        StreamUrl = stream;
        var di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\");
        foreach (var res in di.GetFiles()) res.Delete();
        foreach (var dir in di.GetDirectories()) dir.Delete(true);

        Shuffle(ref chatMessages);
        foreach (var item in chatMessages) _chatMessages.Add(item);

        if (BrowserLimit > 0)
        {
            var thr = new Thread(LoopWithLimit);
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

                    var proxy = new Proxy
                    {
                        Server = "http://" + array[0] + ":" + array[1],
                        Username = array[2],
                        Password = array[3]
                    };

                    // var text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory +
                    //                             "\\zipDirectory\\backgroundTemplate.js");
                    //
                    //
                    // text = text.Replace("{ip}", array[0]).Replace("{port}", array[1])
                    //     .Replace("{username}", array[2]).Replace("{password}", array[3]);
                    // File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory\\background.js",
                    //     text);
                    //
                    // ZipFile.CreateFromDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory",
                    //     AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\background" + i + ".zip");

                    var thr = new Thread(Request) {Priority = ThreadPriority.AboveNormal};
                    var r = new Random();
                    var rInt = r.Next(5000, 8000);

                    while (BrowserLimit > 0 && Browsers.Count >= BrowserLimit) Thread.Sleep(1000);

                    if (!CanRun)
                        continue;

                    loginInfos.TryDequeue(out var loginInfo);

                    thr.Start(new SessionConfigurationDto
                    {
                        Url = line,
                        Count = i,
                        PreferredQuality = preferredQuality,
                        LoginInfo = loginInfo,
                        Service = service,
                        Proxy = proxy
                    });

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

    private void Shuffle(ref List<string> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = _random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    private void StoreCookie(Tuple<string, List<BrowserContextCookiesResult>> cookie)
    {
        var myCookie = new List<MyCookie>();

        foreach (var item in cookie.Item2)
            myCookie.Add(new MyCookie
            {
                Domain = item.Domain,
                Expiry = item.Expires,
                HttpOnly = Convert.ToBoolean(item.HttpOnly),
                Name = item.Name,
                Path = item.Path,
                Value = item.Value,
                Secure = Convert.ToBoolean(item.Secure)
            });

        lock (_lockObject)
        {
            if (!File.Exists(_loginCookiesPath))
            {
                var item = new Dictionary<string, List<MyCookie>> {{cookie.Item1, myCookie}};
                File.WriteAllText(_loginCookiesPath, JsonConvert.SerializeObject(item), Encoding.UTF8);
                return;
            }

            var readCookiesJson = File.ReadAllText(_loginCookiesPath);
            var readCookies = JsonConvert.DeserializeObject<Dictionary<string, List<MyCookie>>>(readCookiesJson);

            readCookies.TryGetValue(cookie.Item1, out var value);

            if (value?.Count > 0)
                readCookies[cookie.Item1] = myCookie;
            else
                readCookies.Add(cookie.Item1, myCookie);


            File.WriteAllText(_loginCookiesPath, JsonConvert.SerializeObject(readCookies), Encoding.UTF8);
        }
    }

    private List<MyCookie> GetCookie(string username)
    {
        lock (_lockObject)
        {
            if (!File.Exists(_loginCookiesPath)) return new List<MyCookie>();

            var readCookiesJson = File.ReadAllText(_loginCookiesPath);
            var readCookies = JsonConvert.DeserializeObject<Dictionary<string, List<MyCookie>>>(readCookiesJson);

            return readCookies.FirstOrDefault(x => x.Key == username).Value;
        }
    }

    private void KillAllProcesses()
    {
        var allChromeProcesses = Process.GetProcessesByName("chrome");

        foreach (var process in allChromeProcesses)
            if (!_initialChromeProcesses.Any(x => x.Id == process.Id))
            {
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = "CMD.exe"
                };
                var strCmd = $"/C taskkill /F /PID {process.Id}";
                startInfo.Arguments = strCmd;
                var processTemp = new Process();
                processTemp.StartInfo = startInfo;
                processTemp.Start();
            }

        _initialChromeProcesses.Clear();

        var allChromeDriverProcesses = Process.GetProcessesByName("chromedriver");

        foreach (var chromeDriverService in allChromeDriverProcesses)
            try
            {
                chromeDriverService.Kill();
            }
            catch (Exception)
            {
                //ignored
            }
    }

    public void Stop()
    {
        CanRun = false;

        _file?.Close();

        KillAllProcesses();

        var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\zipSource");

        foreach (var file in files) File.Delete(file);

        Browsers.Clear();

        AllBrowsersTerminated?.Invoke();
    }

    private void LoopWithLimit()
    {
        while (CanRun)
            try
            {
                if (Browsers.Count >= BrowserLimit)
                {
                    KillAllProcesses();
                    Browsers.Clear();
                }

                Thread.Sleep(500);
            }
            catch (Exception)
            {
                //ignored
            }
    }

    private void Request(object obj)
    {
        try
        {
            var r = new Random();
            var itm = (SessionConfigurationDto) obj;
            var array = itm.Url.Split(':');

            var args = new List<string>();

            string[] resolutions =
                {"1480,900", "1550,790", "1600,900", "1920,1080", "1480,768", "1780,940"};

            var browserLaunchOptions = new BrowserTypeLaunchOptions()
            {
                Headless = false,
                Channel = "chrome",
                Timeout = 120000,
                Proxy = itm.Proxy
            };

            if (_useLowCpuRam)
            {
                args.Add("--disable-extensions-except=" + AppDomain.CurrentDomain.BaseDirectory +
                         "\\Extensions\\TwitchAlternative.crx");
                args.Add("--load-extension=" + AppDomain.CurrentDomain.BaseDirectory +
                         "\\Extensions\\TwitchAlternative.crx");
            }

            if (Headless) browserLaunchOptions.Headless = true;

            var localChrome = AppDomain.CurrentDomain.BaseDirectory + "\\Extensions\\LocalChrome\\chrome.exe";
            if (File.Exists(localChrome)) browserLaunchOptions.ExecutablePath = localChrome;

            args.Add("--mute-audio");

            args.Add("--enable-automation");

            args.Add("--useAutomationExtension=false");

            browserLaunchOptions.Args = args;

            var browser = _playwright.Chromium.LaunchAsync(browserLaunchOptions).GetAwaiter().GetResult();

            var page = browser.NewPageAsync(new BrowserNewPageOptions()
            {
                ViewportSize = new ViewportSize
                {
                    Width = Convert.ToInt32(resolutions[r.Next(0, resolutions.Length - 1)].Split(',')[0]),
                    Height = Convert.ToInt32(resolutions[r.Next(0, resolutions.Length - 1)].Split(',')[1])
                },
                UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36",
                Geolocation = new Geolocation() {Latitude = r.Next(-90, 90), Longitude = r.Next(-180, 180)},
            }).GetAwaiter().GetResult();

            page.GotoAsync(StreamUrl, new PageGotoOptions() {Timeout = 120000, WaitUntil = WaitUntilState.Load})
                .GetAwaiter().GetResult();

            if (BrowserLimit > 0)
            {
                Thread.Sleep(1000);

                return;
            }

            Browsers.Enqueue(browser);

            IncreaseViewer?.Invoke();

            var firstPage = false;

            var startDate = DateTime.Now;

            var messageStartDate = DateTime.Now;

            var messageInterval = _random.Next(1, 10);

            if (itm.Service == StreamService.Service.Twitch)
            {
                if (!Headless && !_useLowCpuRam)
                    try
                    {
                        page.EvaluateAsync("window.localStorage.setItem('video-quality', '" + itm.PreferredQuality +
                                           "');");
                        page.ReloadAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                var matureClicked = false;
                var matureCheckCount = 0;
                var cacheClicked = false;
                var cacheCheckCount = 0;
                if (itm.LoginInfo != null)
                {
                    Thread.Sleep(1000);

                    var allCookies = GetCookie(itm.LoginInfo.Username);

                    if (allCookies != null)
                        foreach (var cookie in allCookies)
                        {
                            Cookie[] cookies =
                            {
                                new()
                                {
                                    Domain = cookie.Domain, Expires = cookie.Expiry, Name = cookie.Name,
                                    Path = cookie.Path, Secure = cookie.Secure, Url = cookie.Path,
                                    HttpOnly = cookie.HttpOnly, Value = cookie.Value
                                }
                            };

                            page.Context.AddCookiesAsync(cookies);
                        }

                    try
                    {
                        var loginButton =
                            page.Locator(
                                "xpath=/html/body/div[1]/div/div[2]/nav/div/div[3]/div[3]/div/div[1]/div[1]/button/div/div");

                        if (loginButton.CountAsync().GetAwaiter().GetResult() > 0)
                        {
                            Click(ref loginButton);

                            Thread.Sleep(1000);

                            var usernameBox =
                                page.Locator(
                                    "xpath=/html/body/div[3]/div/div/div/div/div/div[1]/div/div/div[3]/form/div/div[1]/div/div[2]/input");

                            if (usernameBox.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                Click(ref usernameBox);

                                Thread.Sleep(1000);

                                usernameBox.TypeAsync(itm.LoginInfo.Username).GetAwaiter().GetResult();

                                var passwordBox =
                                    page.Locator(
                                        "xpath=/html/body/div[3]/div/div/div/div/div/div[1]/div/div/div[3]/form/div/div[2]/div/div[1]/div[2]/div[1]/input");

                                if (passwordBox.CountAsync().GetAwaiter().GetResult() > 0)
                                {
                                    Click(ref passwordBox);

                                    Thread.Sleep(1000);

                                    passwordBox.TypeAsync(itm.LoginInfo.Password).GetAwaiter().GetResult();

                                    Thread.Sleep(1000);

                                    var login = page.Locator(
                                        "xpath=/html/body/div[3]/div/div/div/div/div/div[1]/div/div/div[3]/form/div/div[3]/button/div/div");

                                    Thread.Sleep(1000);

                                    if (login.CountAsync().GetAwaiter().GetResult() > 0)
                                        Click(ref login);
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

                        var cookie = page.Context.CookiesAsync().GetAwaiter().GetResult()
                            .Any(x => x.Name == "auth-token");

                        if (cookie)
                        {
                            StoreCookie(new Tuple<string, List<BrowserContextCookiesResult>>(itm.LoginInfo.Username,
                                new List<BrowserContextCookiesResult>(page.Context.CookiesAsync().GetAwaiter()
                                    .GetResult().ToArray())));

                            break;
                        }
                    }
                }

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
                            var liveViewers =
                                page.Locator(
                                    "xpath=/html/body/div[1]/div/div[2]/div[1]/main/div[2]/div[3]/div/div/div[1]/div[1]/div[2]/div/div[1]/div/div/div/div[2]/div[2]/div[2]/div/div/div[1]/div[1]/div/p/span");

                            if (liveViewers.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                LiveViewer.Invoke(liveViewers.InnerTextAsync().GetAwaiter().GetResult());
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
                            try
                            {
                                var mature =
                                    page.Locator(
                                        "xpath=/html/body/div[1]/div/div[2]/div[1]/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div/div[5]/div/div[3]/button/div/div");

                                if (mature.CountAsync().GetAwaiter().GetResult() > 0)
                                {
                                    Click(ref mature);
                                    matureClicked = true;
                                    matureCheckCount++;
                                }
                            }
                            catch
                            {
                                //ignored because there is no mature button
                            }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    try
                    {
                        if (!cacheClicked && cacheCheckCount < 5)
                            try
                            {
                                var cache = page.Locator(
                                    "xpath=/html/body/div[1]/div/div[2]/div[1]/div/div/div[3]/button/div/div/div");

                                if (cache.CountAsync().GetAwaiter().GetResult() > 0)
                                {
                                    Click(ref cache);
                                    cacheClicked = true;
                                }

                                cacheCheckCount++;
                            }
                            catch (Exception)
                            {
                                //ignored because there is no cache button
                            }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    try
                    {
                        var connectionError =
                            page.Locator(
                                "xpath=/html/body/div[1]/div/div[2]/div[1]/main/div[2]/div[3]/div/div/div[2]/div/div[2]/div/div/div/div/div[7]/div/div[3]/button/div/div[2]");

                        if (connectionError.CountAsync().GetAwaiter().GetResult() > 0)
                            connectionError.ClickAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    try
                    {
                        if (_refreshInterval != 0 &&
                            DateTime.Now - startDate > TimeSpan.FromMinutes(_refreshInterval))
                        {
                            page.ReloadAsync().GetAwaiter().GetResult();

                            startDate = DateTime.Now;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    try
                    {
                        if (messageInterval != 0 &&
                            DateTime.Now - messageStartDate > TimeSpan.FromMinutes(messageInterval) &&
                            itm.LoginInfo != null)
                        {
                            SendMessage();

                            messageStartDate = DateTime.Now;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }

                void SendMessage()
                {
                    try
                    {
                        var chatBox = page.WaitForSelectorAsync(".chat-wysiwyg-input__editor").GetAwaiter()
                            .GetResult();

                        if (_chatMessages.TryTake(out var message))
                        {
                            chatBox?.TypeAsync(message).GetAwaiter().GetResult();
                            page.Keyboard.PressAsync("Enter").GetAwaiter().GetResult();
                        }
                    }
                    catch (Exception)
                    {
                        //ignored  
                    }
                }
            }

            if (itm.Service == StreamService.Service.Youtube)
            {
                Thread.Sleep(3000);

                try
                {
                    var play = page.Locator(
                        "xpath=/html/body/ytd-app/div/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[1]/div/div/div/ytd-player/div/div/div[5]/button");

                    play?.ClickAsync().GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                    //ignored
                }

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
                            var liveViewers = page.Locator(
                                "xpath=/html/body/ytd-app/div/ytd-page-manager/ytd-watch-flexy/div[5]/div[1]/div/div[6]/div[2]/ytd-video-primary-info-renderer/div/div/div[1]/div[1]/ytd-video-view-count-renderer/span[1]");

                            if (liveViewers.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                LiveViewer.Invoke(
                                    liveViewers.InnerTextAsync().GetAwaiter().GetResult().Split(' ')[0]);
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
                        if (_refreshInterval != 0 &&
                            DateTime.Now - startDate > TimeSpan.FromMinutes(_refreshInterval))
                        {
                            page.ReloadAsync().GetAwaiter().GetResult();

                            startDate = DateTime.Now;
                        }
                    }
                    catch
                    {
                        //ignored
                    }
                }
            }

            if (itm.Service == StreamService.Service.DLive)
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
                                var liveViewers =
                                    page.Locator(
                                        "xpath=/html/body/div/div[1]/div[20]/div[2]/div/div[2]/div/div/div/div[1]/div/div[1]/div[3]/div/div[1]/div/div[2]/div[2]");

                                if (liveViewers.CountAsync().GetAwaiter().GetResult() > 0)
                                {
                                    LiveViewer.Invoke(liveViewers.InnerTextAsync().GetAwaiter().GetResult()
                                        .Split(" ")[0]);
                                    Thread.Sleep(5000);
                                }
                            }
                            catch (Exception)
                            {
                                //ignored
                            }

                            try
                            {
                                var liveViewers =
                                    page.Locator(
                                        "xpath=/html/body/div/div[1]/div[18]/div[2]/div/div/div/div/div/div/div/div/div[3]/div/div[3]/div/div/div[1]/div/div[1]/div[2]/div/div[1]/span");

                                if (liveViewers.CountAsync().GetAwaiter().GetResult() > 0)
                                {
                                    LiveViewer.Invoke(liveViewers.InnerTextAsync().GetAwaiter().GetResult());
                                    Thread.Sleep(5000);
                                }
                            }
                            catch (Exception)
                            {
                                //ignored
                            }
                        }

                        if (!isPlaying)
                        {
                            var play = page.Locator(
                                "xpath=/html/body/div/div[1]/div[14]/div[2]/div/div[2]/div/div/div/div/div/div/div[1]/div/div/div/div/div[4]/div[2]/button/svg");

                            if (play.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                Click(ref play);
                                isPlaying = true;
                            }
                        }

                        Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    try
                    {
                        if (_refreshInterval != 0 &&
                            DateTime.Now - startDate > TimeSpan.FromMinutes(_refreshInterval))
                        {
                            page.ReloadAsync().GetAwaiter().GetResult();
                            isPlaying = false;
                            startDate = DateTime.Now;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }
            }

            if (itm.Service == StreamService.Service.NimoTv)
            {
                Thread.Sleep(3000);

                var isPlaying = false;

                if (itm.LoginInfo != null)
                {
                    Thread.Sleep(1000);

                    var allCookies = GetCookie(itm.LoginInfo.Username);

                    if (allCookies != null)
                        foreach (var cookie in allCookies)
                        {
                            Cookie[] cookies =
                            {
                                new()
                                {
                                    Domain = cookie.Domain, Expires = cookie.Expiry, Name = cookie.Name,
                                    Path = cookie.Path, Secure = cookie.Secure, Url = cookie.Path,
                                    HttpOnly = cookie.HttpOnly, Value = cookie.Value
                                }
                            };

                            page.Context.AddCookiesAsync(cookies);
                        }

                    try
                    {
                        var loginButton =
                            page.Locator("xpath=/html/body/div[2]/div[1]/div/div[2]/div/div[2]/button");

                        if (loginButton.CountAsync().GetAwaiter().GetResult() > 0)
                        {
                            Click(ref loginButton);

                            Thread.Sleep(1000);

                            var usernameBox =
                                page.Locator(
                                    "xpath=/html/body/div[6]/div/div[2]/div/div[2]/div/div/div[3]/div[1]/div[2]/input");

                            if (usernameBox.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                Click(ref usernameBox);

                                Thread.Sleep(1000);

                                usernameBox.TypeAsync(itm.LoginInfo.Username.Split('/')[1]).GetAwaiter()
                                    .GetResult();

                                var countryCodeArrow =
                                    page.Locator(
                                        "xpath=/html/body/div[6]/div/div[2]/div/div[2]/div/div/div[3]/div[1]/div[2]/div[1]");

                                if (countryCodeArrow.CountAsync().GetAwaiter().GetResult() > 0)
                                {
                                    Click(ref countryCodeArrow);

                                    Thread.Sleep(1000);

                                    var searchCountryCode =
                                        page.Locator(
                                            "xpath=/html/body/div[6]/div/div[2]/div/div[4]/div/div/div/div[1]/input");

                                    if (searchCountryCode.CountAsync().GetAwaiter().GetResult() > 0)
                                    {
                                        searchCountryCode.TypeAsync(itm.LoginInfo.Username.Split('/')[0]
                                            .Replace("+", string.Empty)).GetAwaiter().GetResult();

                                        Thread.Sleep(1000);

                                        var firstElement =
                                            page.Locator(
                                                "xpath=/html/body/div[6]/div/div[2]/div/div[4]/div/div/div/div[2]/div[1]/div[2]");

                                        if (firstElement.CountAsync().GetAwaiter().GetResult() > 0)
                                            Click(ref firstElement);
                                    }
                                }

                                var passwordBox =
                                    page.Locator(
                                        "xpath=/html/body/div[6]/div/div[2]/div/div[2]/div/div/div[3]/div[1]/div[3]/input");

                                if (passwordBox.CountAsync().GetAwaiter().GetResult() > 0)
                                {
                                    Click(ref passwordBox);

                                    Thread.Sleep(1000);

                                    passwordBox.TypeAsync(itm.LoginInfo.Password).GetAwaiter().GetResult();

                                    Thread.Sleep(1000);

                                    var login = page.Locator(
                                        "xpath=/html/body/div[6]/div/div[2]/div/div[2]/div/div/div[3]/div[1]/button");


                                    Thread.Sleep(1000);

                                    if (login.CountAsync().GetAwaiter().GetResult() > 0)
                                        Click(ref login);
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

                        var cookie = page.Context.CookiesAsync().GetAwaiter().GetResult()
                            .Any(x => x.Name == "userName");

                        if (cookie)
                        {
                            StoreCookie(new Tuple<string, List<BrowserContextCookiesResult>>(itm.LoginInfo.Username,
                                new List<BrowserContextCookiesResult>(page.Context.CookiesAsync().GetAwaiter()
                                    .GetResult().ToArray())));

                            break;
                        }
                    }
                }

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
                            var liveViewers =
                                page.Locator(
                                    "xpath=/html/body/div[2]/div[2]/div[2]/div[1]/div/div/div[2]/div[2]/div[1]/div[1]/div/div[2]/div[3]/span");

                            if (liveViewers.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                LiveViewer.Invoke(liveViewers.InnerTextAsync().GetAwaiter().GetResult());
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
                        if (!isPlaying)
                        {
                            var play = page.Locator(
                                "xpath=/html/body/div[2]/div[2]/div[2]/div[1]/div/div/div[2]/div[2]/div[1]/div[2]/div[1]/div[2]/div/span");

                            if (play.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                Click(ref play);
                                isPlaying = true;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    try
                    {
                        if (_refreshInterval != 0 &&
                            DateTime.Now - startDate > TimeSpan.FromMinutes(_refreshInterval))
                        {
                            page.ReloadAsync().GetAwaiter().GetResult();
                            isPlaying = false;
                            startDate = DateTime.Now;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    try
                    {
                        if (messageInterval != 0 &&
                            DateTime.Now - messageStartDate > TimeSpan.FromMinutes(messageInterval) &&
                            itm.LoginInfo != null)
                        {
                            SendMessage();

                            messageStartDate = DateTime.Now;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    void SendMessage()
                    {
                        try
                        {
                            var chatBox = page.WaitForSelectorAsync(".nimo-room__chatroom__chat-box__input")
                                .GetAwaiter().GetResult();

                            if (chatBox != null && _chatMessages.TryTake(out var message))
                            {
                                chatBox.TypeAsync(message).GetAwaiter().GetResult();
                                page.Keyboard.PressAsync("Enter");
                            }
                        }
                        catch (Exception)
                        {
                            //ignored  
                        }
                    }

                    Thread.Sleep(1000);
                }
            }

            if (itm.Service == StreamService.Service.Twitter)
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
                            var liveViewers =
                                page.Locator(
                                    "xpath=/html/body/div[2]/div[2]/div[2]/div[1]/div/div/div[2]/div[2]/div[1]/div[1]/div/div[2]/div[3]/span");

                            if (liveViewers.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                LiveViewer.Invoke(liveViewers.InnerTextAsync().GetAwaiter().GetResult());
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
                        if (_refreshInterval != 0 &&
                            DateTime.Now - startDate > TimeSpan.FromMinutes(_refreshInterval))
                        {
                            page.ReloadAsync().GetAwaiter().GetResult();
                            startDate = DateTime.Now;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    Thread.Sleep(1000);
                }
            }

            if (itm.Service == StreamService.Service.Facebook)
            {
                Thread.Sleep(3000);

                if (itm.LoginInfo != null)
                {
                    Thread.Sleep(1000);

                    var allCookies = GetCookie(itm.LoginInfo.Username);

                    if (allCookies != null)
                        foreach (var cookie in allCookies)
                        {
                            Cookie[] cookies =
                            {
                                new()
                                {
                                    Domain = cookie.Domain, Expires = cookie.Expiry, Name = cookie.Name,
                                    Path = cookie.Path, Secure = cookie.Secure, Url = cookie.Path,
                                    HttpOnly = cookie.HttpOnly, Value = cookie.Value
                                }
                            };

                            page.Context.AddCookiesAsync(cookies);
                        }

                    try
                    {
                        var usernameBox =
                            page.Locator(
                                "xpath=/html/body/div[1]/div/div[1]/div/div[2]/div[2]/div[2]/div/form/div[2]/div[1]/label/input");

                        if (usernameBox.CountAsync().GetAwaiter().GetResult() > 0)
                        {
                            Click(ref usernameBox);

                            Thread.Sleep(1000);

                            usernameBox.TypeAsync(itm.LoginInfo.Username).GetAwaiter().GetResult();

                            var passwordBox =
                                page.Locator(
                                    "xpath=/html/body/div[1]/div/div[1]/div/div[2]/div[2]/div[2]/div/form/div[2]/div[2]/label/input");

                            if (passwordBox.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                Click(ref passwordBox);

                                Thread.Sleep(1000);

                                passwordBox.TypeAsync(itm.LoginInfo.Password).GetAwaiter().GetResult();

                                Thread.Sleep(1000);

                                var login = page.Locator(
                                    "xpath=/html/body/div[1]/div/div[1]/div/div[2]/div[2]/div[2]/div/form/div[2]/div[3]/div/div/div[1]/div/span/span");

                                Thread.Sleep(1000);

                                if (login.CountAsync().GetAwaiter().GetResult() > 0)
                                    Click(ref login);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage?.Invoke(new Exception($"Login failed: {ex.Message}"));
                    }

                    Thread.Sleep(3000);
                    page.ReloadAsync().GetAwaiter().GetResult();

                    while (true)
                    {
                        Thread.Sleep(1000);

                        var cookie = page.Context.CookiesAsync().GetAwaiter().GetResult()
                            .Any(x => x.Name == "c_user");

                        if (cookie)
                        {
                            StoreCookie(new Tuple<string, List<BrowserContextCookiesResult>>(itm.LoginInfo.Username,
                                new List<BrowserContextCookiesResult>(page.Context.CookiesAsync().GetAwaiter()
                                    .GetResult().ToArray())));

                            break;
                        }
                    }
                }

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
                            var liveViewers =
                                page.Locator(
                                    "xpath=/html/body/div[1]/div/div[1]/div/div[3]/div/div/div[1]/div[2]/div[1]/div/div/div/div[1]/div[1]/div/div/div/div[2]/div/div[5]/div[2]/span[2]");

                            if (liveViewers.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                LiveViewer.Invoke(liveViewers.InnerTextAsync().GetAwaiter().GetResult());
                                Thread.Sleep(5000);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    try
                    {
                        if (_refreshInterval != 0 &&
                            DateTime.Now - startDate > TimeSpan.FromMinutes(_refreshInterval))
                        {
                            page.ReloadAsync().GetAwaiter().GetResult();
                            startDate = DateTime.Now;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }
                }
            }

            if (itm.Service == StreamService.Service.TrovoLive)
            {
                Thread.Sleep(5000);

                if (!Headless && !_useLowCpuRam)
                    try
                    {
                        page.EvaluateAsync("window.localStorage.setItem('live/userClarityLevel', '" +
                                           itm.PreferredQuality + "');");
                        page.ReloadAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                if (itm.LoginInfo != null)
                {
                    Thread.Sleep(1000);

                    var allCookies = GetCookie(itm.LoginInfo.Username);

                    if (allCookies != null)
                        foreach (var cookie in allCookies)
                        {
                            Cookie[] cookies =
                            {
                                new()
                                {
                                    Domain = cookie.Domain, Expires = cookie.Expiry, Name = cookie.Name,
                                    Path = cookie.Path, Secure = cookie.Secure, Url = cookie.Path,
                                    HttpOnly = cookie.HttpOnly, Value = cookie.Value
                                }
                            };

                            page.Context.AddCookiesAsync(cookies);
                        }

                    try
                    {
                        var loginSignUpButton =
                            page.Locator("xpath=/html/body/div[1]/div/div/nav/div[3]/div[3]/button");

                        if (loginSignUpButton.CountAsync().GetAwaiter().GetResult() > 0)
                        {
                            Click(ref loginSignUpButton);

                            Thread.Sleep(4000);

                            ILocator usernameBox;

                            try
                            {
                                usernameBox =
                                    page.Locator(
                                        "xpath=/html/body/div[4]/div/div[2]/div[3]/div[1]/div[1]/div/input");
                            }
                            catch
                            {
                                usernameBox =
                                    page.Locator(
                                        "xpath=/html/body/div[4]/div/div[2]/div[3]/div[1]/div[1]/div[1]/input");
                            }

                            if (usernameBox.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                Click(ref usernameBox);

                                Thread.Sleep(1000);

                                usernameBox.TypeAsync(itm.LoginInfo.Username).GetAwaiter().GetResult();

                                Thread.Sleep(1000);

                                var passwordBox =
                                    page.Locator(
                                        "xpath=/html/body/div[4]/div/div[2]/div[3]/div[1]/div[3]/div/input");

                                if (passwordBox.CountAsync().GetAwaiter().GetResult() > 0)
                                {
                                    passwordBox.TypeAsync(itm.LoginInfo.Password).GetAwaiter().GetResult();

                                    var login = page.Locator(
                                        "xpath=/html/body/div[4]/div/div[2]/div[3]/div[1]/button");

                                    Thread.Sleep(1000);

                                    if (login.CountAsync().GetAwaiter().GetResult() > 0)
                                        Click(ref login);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage?.Invoke(new Exception($"Login failed: {ex.Message}"));
                    }

                    Thread.Sleep(3000);
                    page.ReloadAsync().GetAwaiter().GetResult();

                    while (true)
                    {
                        Thread.Sleep(1000);

                        var cookie = page.Context.CookiesAsync().GetAwaiter().GetResult().Any(x => x.Name == "uid");

                        if (cookie)
                        {
                            StoreCookie(new Tuple<string, List<BrowserContextCookiesResult>>(itm.LoginInfo.Username,
                                new List<BrowserContextCookiesResult>(page.Context.CookiesAsync().GetAwaiter()
                                    .GetResult().ToArray())));

                            break;
                        }
                    }
                }

                var matureClicked = false;

                var chatRulesClicked = false;

                var matureCheckCount = 0;

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
                            var liveViewers =
                                page.Locator(
                                    "xpath=/html/body/div[1]/div/div[1]/div/div[3]/div/div/div[1]/div[2]/div[1]/div/div/div/div[1]/div[1]/div/div/div/div[2]/div/div[5]/div[2]/span[2]");

                            if (liveViewers.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                LiveViewer.Invoke(liveViewers.InnerTextAsync().GetAwaiter().GetResult());
                                Thread.Sleep(5000);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    try
                    {
                        if (_refreshInterval != 0 &&
                            DateTime.Now - startDate > TimeSpan.FromMinutes(_refreshInterval))
                        {
                            page.ReloadAsync().GetAwaiter().GetResult();
                            startDate = DateTime.Now;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    try
                    {
                        if (!matureClicked && matureCheckCount < 5)
                            try
                            {
                                ILocator mature = null;

                                try
                                {
                                    mature = page.Locator(
                                        "xpath=/html/body/div[1]/div/div/div/div[2]/div/div/div[1]/div[1]/div[1]/div/div[4]/div[3]/section/div/button[2]");
                                }
                                catch
                                {
                                    //ignored
                                }

                                if (mature.CountAsync().GetAwaiter().GetResult() == 0)
                                    mature = page.Locator(
                                        "xpath=/html/body/div[1]/div/div/div/div[2]/div/div/div[1]/div[1]/div[1]/div/div[2]/div[3]/section/div/button[2]");

                                if (mature.CountAsync().GetAwaiter().GetResult() > 0)
                                {
                                    Click(ref mature);
                                    matureClicked = true;
                                }

                                matureCheckCount++;
                            }
                            catch
                            {
                                //ignored because there is no mature button
                            }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    if (!chatRulesClicked)
                        try
                        {
                            var chatRules = page.Locator(
                                "xpath=/html/body/div[1]/div/div/div/div[2]/div/section/div[3]/div/section/section/div/button");

                            if (chatRules.CountAsync().GetAwaiter().GetResult() > 0)
                            {
                                chatRules.ClickAsync().GetAwaiter().GetResult();
                                chatRulesClicked = true;
                            }
                        }
                        catch (Exception)
                        {
                            //ignored
                        }

                    try
                    {
                        if (messageInterval != 0 &&
                            DateTime.Now - messageStartDate > TimeSpan.FromMinutes(messageInterval) &&
                            itm.LoginInfo != null)
                        {
                            SendMessage();

                            messageStartDate = DateTime.Now;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    void SendMessage()
                    {
                        try
                        {
                            var chatBox =
                                page.Locator(
                                    "xpath=/html/body/div[1]/div/div/div/div[2]/div/section/div[3]/div/section/div[1]/div[1]/div[1]");

                            if (chatBox.CountAsync().GetAwaiter().GetResult() > 0 &&
                                _chatMessages.TryTake(out var message))
                            {
                                chatBox.TypeAsync(message).GetAwaiter().GetResult();
                                page.Keyboard.PressAsync("Enter");
                            }
                        }
                        catch (Exception)
                        {
                            //ignored  
                        }
                    }

                    Thread.Sleep(1000);
                }
            }

            if (itm.Service == StreamService.Service.BigoLive)
            {
                Thread.Sleep(2000);

                page.ReloadAsync().GetAwaiter().GetResult();

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
                            var liveViewers = page.WaitForSelectorAsync(".info-view-nums").GetAwaiter().GetResult();

                            if (liveViewers != null)
                            {
                                LiveViewer.Invoke(liveViewers.InnerTextAsync().GetAwaiter().GetResult());
                                Thread.Sleep(5000);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    try
                    {
                        if (_refreshInterval != 0 &&
                            DateTime.Now - startDate > TimeSpan.FromMinutes(_refreshInterval))
                        {
                            page.ReloadAsync().GetAwaiter().GetResult();
                            startDate = DateTime.Now;
                        }
                    }
                    catch (Exception)
                    {
                        //ignored
                    }

                    Thread.Sleep(1000);
                }
            }

            try
            {
                page.CloseAsync().GetAwaiter().GetResult();
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
                InitializationError?.Invoke(new Exception("Please update your Google Chrome!"));
            }
        }
        catch (Exception ex)
        {
            InitializationError?.Invoke(ex);
        }
    }

    private void Click(ref ILocator locator)
    {
        locator.ClickAsync().GetAwaiter().GetResult();
    }

    private class MyCookie
    {
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public float Expiry { get; set; }
    }
}