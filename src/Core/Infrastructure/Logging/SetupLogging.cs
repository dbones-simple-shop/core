using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Elasticsearch;

namespace Core.Infrastructure.Logging
{
    using System.Reflection;

    public static class SetupLogging 
    {
        public static IWebHostBuilder ConfigureLogging(this IWebHostBuilder builder)
        {
            
            return builder.UseSerilog((builderContext, config) =>
            {
                var loggerConfig = builderContext.Configuration.GetSection("Logging").Get<LoggingConfiguration>();

                config
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext();

                if (loggerConfig.Logger == "fluentd")
                {
                    config.WriteTo.Console(new ElasticsearchJsonFormatter());
                }
                else if (loggerConfig.Logger == "datadog")
                {
                    string serviceName = Assembly.GetEntryAssembly().GetName().Name;
                    config.WriteTo.DatadogLogs(loggerConfig.DataDogApiKey, service: serviceName);
                }

                else
                {
                    config.WriteTo.Console();    
                }
                
            });
        }
    }
}