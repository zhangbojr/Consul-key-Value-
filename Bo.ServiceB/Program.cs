using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Winton.Extensions.Configuration.Consul;

namespace Bo.ServiceB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    hostingContext.Configuration = config.Build();
                    string consul_url = hostingContext.Configuration["Consul_Url"];
                    config.AddConsul(
                                $"{env.ApplicationName}/appsettings.{env.EnvironmentName}.json",
                                cancellationTokenSource.Token,
                                options =>
                                {
                                    options.Optional = true;
                                    options.ReloadOnChange = true;
                                    options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; };
                                    options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); };
                                }
                                ).AddEnvironmentVariables();

                    hostingContext.Configuration = config.Build();
                }).UseUrls("http://localhost:5012")
                 .UseStartup<Startup>();
        }
    }
}
