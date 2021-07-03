using BotCore;
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
    class Program
    {
        public static string proxyListDirectory = "";
        public static string streamUrl = "";
        public static bool headless = false;

        [Obsolete]
        static void Main(string[] args)
        {
            proxyListDirectory = System.Configuration.ConfigurationSettings.AppSettings["proxyListDirectory"];
            streamUrl = System.Configuration.ConfigurationSettings.AppSettings["streamUrl"];
            headless = Convert.ToBoolean(System.Configuration.ConfigurationSettings.AppSettings["headless"]);

            Core a = new Core();

            a.Start(proxyListDirectory, streamUrl, headless, 0, 0);

        }
        
    }
}
