using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebAppP2P
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            var port = config.GetValue<string>("port");
            var hostBuilder = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>();

            if (!string.IsNullOrEmpty(port))
            {
                hostBuilder.UseUrls("http://0.0.0.0:" + port);
            }
#if DEBUG
            hostBuilder.UseSetting("detailedErrors", "true");
#endif
            hostBuilder.UseIISIntegration();
#if DEBUG
            hostBuilder.CaptureStartupErrors(true);
#endif
            var host = hostBuilder.Build();
            return host;
        }
    }
}
