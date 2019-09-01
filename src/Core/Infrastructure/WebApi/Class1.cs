using System;
using System.Linq;
using System.Reflection;
using Jaeger.Thrift.Agent.Zipkin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.WebApi
{
    public static class SetupWebApi
    {
        
        public static IWebHostBuilder ConfigureWebApi(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                var controllerTypes = Assembly
                    .GetEntryAssembly()
                    .ExportedTypes.Where(x => x.FullName.EndsWith("Controller") && !x.IsAbstract);

                foreach (var controllerType in controllerTypes)
                {
                    Console.WriteLine($"controller {controllerType}");
                    services.AddScoped(controllerType);
                }
            });
            
            return webHostBuilder;
        }

        public static IApplicationBuilder ConfigureApplicationWebApi(this IApplicationBuilder app)
        {
           
            return app;
        }
    }
}