using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TestApp
{
    using Core.Infrastructure.Application;
    using Core.Infrastructure.HealthChecks;
    using Core.Infrastructure.Logging;
    using Core.Infrastructure.MassTransit;
    using Core.Infrastructure.Redis;
    using Core.Infrastructure.Serializing;
    using Core.Infrastructure.Swagger;
    using Core.Infrastructure.Tracing;

    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile(Path.Combine("config", "stagesettings.json"), true)
                .Build();

            var port = configuration.GetSection("Application").Get<ApplicationConfiguration>().PortNumber;

            var host = WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(configuration)
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddJsonFile(Path.Combine("config", "stagesettings.json"), true);
                })
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                })
                .ConfigureApplication()
                .ConfigureLogging()
                //.ConfigureMartin()
                .ConfigureSwagger()
                .ConfigureSerializer()
                .ConfigureHeathChecks()
                //.ConfigureRedis()
                .ConfigureMassTransit()
                .ConfigureTracing(configuration)
                //.ConfigureWebApi()
                .UseUrls($"http://*:{port}")
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }


    }
}
