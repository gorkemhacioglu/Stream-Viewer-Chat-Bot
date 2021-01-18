using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Opera;
using System;
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
        [Obsolete]
        static void Main(string[] args)
        {
            int i = 0;

            //string[] proxyList = { "162.144.48.236:3838", "162.144.50.197:3838", "69.241.4.90:80", "59.29.245.151:3128" };
            //while (i < proxyList.Length)
            //{
            //    Thread thr = new Thread(req);
            //    thr.Start(proxyList[i]);
            //    i++;

            //};

            string line = string.Empty;
            System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Source\TwitchBot\TwitchBot\bin\Debug\netcoreapp3.1\proxylist.txt");
            while ((line = file.ReadLine()) != null)
            {
                Thread thr = new Thread(req);
                Random r = new Random();
                int rInt = r.Next(0, 500);
                Thread.Sleep(rInt);
                thr.Start(new Item { url = line, count = i});
                i++;
            }

            file.Close();
            Console.WriteLine("Waiting for end");
            Console.ReadLine();

        }

        [Obsolete]
        private static void req(object obj)
        {
            try
            {
                Item itm = (Item)obj;
                var array = itm.url.ToString().Split(':');
                var proxy = new Proxy();
                proxy.HttpProxy = array[0]+':'+array[1];
                proxy.SslProxy = array[0] + ':' + array[1];
                //var opera_options = new OperaOptions();
                //opera_options.Proxy = proxy;
                //opera_options.AddArgument("--enable-blink-features=ShadowDOMV0");
                //var driver = new OperaDriver(opera_options);
                var chrome_options = new ChromeOptions();
                chrome_options.Proxy = proxy;
                chrome_options.AcceptInsecureCertificates = true;
                //chrome_options.AddArgument("headless");
                chrome_options.AddExtension("C:\\Source\\TwitchBot\\TwitchBot\\bin\\Debug\\netcoreapp3.1\\background"+ itm.count +".zip");
                //chrome_options.AddArgument("--proxy-server=" + proxyList[i]);
                var driver = new ChromeDriver(chrome_options);

                driver.Url = "https://whatismyipaddress.com/";
                //driver.Url = "https://twitch.tv/deneyyapiyoruz";
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
                        Random r = new Random();
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
