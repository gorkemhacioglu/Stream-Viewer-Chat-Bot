using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Opera;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchBot
{
    public class Item { 
    
        public string url { get; set; }

        public int count { get; set; }
    }

    

    class Program
    {
        public static string zipDirectory = "";
        public static string proxyListDirectory = "";
        public static string streamUrl = "";
        public static bool headless = false;
        [Obsolete]
        static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile(AppDomain.CurrentDomain.BaseDirectory+"appsettings.json", true, true)
                .AddEnvironmentVariables();

            var config = builder.Build();

            proxyListDirectory = config["proxyListDirectory"];

            streamUrl = config["streamUrl"];

            headless = Convert.ToBoolean(config["headless"]);

            int i = 0;
            System.IO.DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\");
            foreach (FileInfo res in di.GetFiles())
            {
                res.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            zipDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\zipSource\\background";
            string line = string.Empty;

            System.IO.StreamReader file = new System.IO.StreamReader(proxyListDirectory);
            while ((line = file.ReadLine()) != null)
            {
                var array = line.ToString().Split(':');
                string text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory\\backgroundTemplate.js");
                text = text.Replace("{ip}", array[0]).Replace("{port}", array[1]).Replace("{username}", array[2]).Replace("{password}", array[3]);
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory\\background.js", text);

                ZipFile.CreateFromDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\zipDirectory", AppDomain.CurrentDomain.BaseDirectory +"\\zipSource\\background"+i+".zip");

                Thread thr = new Thread(req);
                Random r = new Random();
                int rInt = r.Next(0, 10000);
                Thread.Sleep(rInt);
                thr.Start(new Item { url = line, count = i});
                i++;
            }

            file.Close();
            Console.ReadKey();

        }

        [Obsolete]
        private static void req(object obj)
        {
            try
            {
                Random r = new Random();
                Item itm = (Item)obj;
                var array = itm.url.ToString().Split(':');
                var proxy = new Proxy();
                proxy.HttpProxy = array[0] + ':' + array[1];
                proxy.SslProxy = array[0] + ':' + array[1];
                var chrome_options = new ChromeOptions();
                chrome_options.Proxy = proxy;
                chrome_options.AcceptInsecureCertificates = true;
                
                if(headless)
                    chrome_options.AddArgument("headless");

                string[] resolutions = {"1152,864", "1080,720", "1400,1050", "1280,800","1280,720","1024,600","1024,768","800,600"};
                chrome_options.AddArgument("window-size=" + resolutions[r.Next(0, resolutions.Length-1)]);
                chrome_options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36");
                chrome_options.AddExcludedArgument("enable-automation");
                chrome_options.AddAdditionalCapability("useAutomationExtension", false);
                chrome_options.AddExtension(zipDirectory + itm.count +".zip");
                
                var driver = new ChromeDriver(chrome_options);
                
                //driver.Url = "https://15d7a43b4075c3068ed719ff0b3a5937.m.pipedream.net";
                //driver.Url = "https://whatismyipaddress.com/";
                driver.Url = streamUrl;
                driver.Navigate();

                while (true)
                {
                    var matureButton = driver.FindElementByClassName("simplebar-scrollbar");

                    if (matureButton != null)
                    { 
                    
                    }
                    var item = driver.FindElementByClassName("simplebar-scrollbar");

                    if (item != null)
                    {
                        
                        int rInt = r.Next(0, 5000);
                        item.SendKeys(Keys.Down);
                        Thread.Sleep(rInt);
                        item.SendKeys(Keys.Up);
                        Thread.Sleep(rInt);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex);
            }
            return;
        }
    }
}
