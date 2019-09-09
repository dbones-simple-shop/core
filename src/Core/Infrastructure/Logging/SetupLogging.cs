using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Elasticsearch;

namespace Core.Infrastructure.Logging
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public static class SetupLogging 
    {
        public static IWebHostBuilder ConfigureLogging(this IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IHostedService, GlobalExceptionHandlerHostedService>();
                });
            
            return builder.UseSerilog((builderContext, config) =>
            {
                var loggerConfig = builderContext.Configuration.GetSection("Logging").Get<LoggingConfiguration>();
                string serviceName = Assembly.GetEntryAssembly().GetName().Name;

                config
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext();

                if (loggerConfig.Logger == "fluentd")
                {
                    config.Enrich.WithProperty("Service", serviceName);
                    config.WriteTo.Console(new ElasticsearchJsonFormatter());
                }
                else if (loggerConfig.Logger == "datadog")
                {
                    config.WriteTo.DatadogLogs(loggerConfig.DataDogApiKey, service: serviceName);
                }
                else
                { 
                    config.WriteTo.Console();    
                }
                
            });
        }

        public class GlobalExceptionHandlerHostedService : IHostedService
        {
            private readonly ILogger<GlobalExceptionHandlerHostedService> _logger;

            public GlobalExceptionHandlerHostedService(ILogger<GlobalExceptionHandlerHostedService> logger)
            {
                _logger = logger;
            }

            public Task StartAsync(CancellationToken cancellationToken)
            {
                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    var ex = args.ExceptionObject as Exception;
                    var senderName = sender?.GetType().FullName ?? "unknown";
                    _logger.LogError($"Unhandled exception from: {senderName}", ex);
                };

                _logger.LogDebug("setup a global exception handler.");
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}