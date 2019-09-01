using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting.Elasticsearch;

namespace Core.Infrastructure.Logging
{
    public static class SetupLogging 
    {
        public static IWebHostBuilder ConfigureLogging(this IWebHostBuilder builder)
        {
            
            return builder.UseSerilog((builderContext, config) =>
            {
                var logger = builderContext.Configuration.GetSection("Logging").Get<LoggingConfiguration>().Logger;

                config
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext();

                if (logger == "fluentd")
                {
                    config.WriteTo.Console(new ElasticsearchJsonFormatter());
                }    
                    
                else
                {
                    config.WriteTo.Console();    
                }
                
            });
        }
    }
}