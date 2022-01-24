using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace BotCore.Log
{
    public class Logger
    {

        public static void CreateLogger(string appId)
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.NewRelicLogs(
                    endpointUrl: "https://log-api.eu.newrelic.com/log/v1",
                    applicationName: "ViewerBot",
                    insertKey: "NRII-3nbcrMjHHs0RrT0GhRNqpd16YVMFHdcI")
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("appId",appId)
                .CreateLogger();
        }
    }
}
