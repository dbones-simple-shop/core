using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Core.Infrastructure.Tracing
{

    public static class SetupTracing
    {
        public static IWebHostBuilder ConfigureTracing(this IWebHostBuilder webHostBuilder, IConfiguration configuration)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                var tracingConfig = new TracingConfiguration();
                configuration.GetSection("Tracing").Bind(tracingConfig);
                

                if (tracingConfig.EnableOpenTracing)
                {
                    services.AddJaeger(configuration);
                    services.AddOpenTracing();   
                }
            });
            
            return webHostBuilder;
        }
    }
}