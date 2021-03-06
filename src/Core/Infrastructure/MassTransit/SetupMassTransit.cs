using System;
using System.Linq;
using System.Reflection;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using MassTransit.OpenTracing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.MassTransit
{
    using System.Diagnostics;
    using HealthCheck;

    public static class SetupMassTransit
    {
       
        public static IWebHostBuilder ConfigureMassTransit(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureServices(services =>
            {
                //healthcheck part 1
                var busConnectionState = new MassTransitHealthCheckState();
                services.AddSingleton(busConnectionState);


                var consumerTypes = Assembly
		            .GetEntryAssembly()
		            .ExportedTypes
		            .Where(x => !x.IsAbstract && typeof(IConsumer).IsAssignableFrom(x))
		            .ToList();

	            services.AddSingleton<IHostedService, BusService>();
	            

	            services.AddMassTransit(config =>
	            {
		            foreach (var consumerType in consumerTypes)
		            {
			            config.AddConsumer(consumerType);
		            }
		            
		            config.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
		            {
			            var busConfig = new BusConfiguration();
			            var configuration = provider.GetService<IConfiguration>();
			            configuration.GetSection("Bus").Bind(busConfig);

			            busConfig.Password = string.IsNullOrEmpty(busConfig.Password) ? "guest" : busConfig.Password;
			            busConfig.Username = string.IsNullOrEmpty(busConfig.Username) ? "guest" : busConfig.Username;
			            busConfig.Host = string.IsNullOrEmpty(busConfig.Host) ? "localhost" : busConfig.Host;

			            var host = cfg.Host(new Uri($"rabbitmq://{busConfig.Host}"), h =>
			            {
				            h.Username(busConfig.Username);
				            h.Password(busConfig.Password);
			            });

                        //opentracing
                        if (busConfig.Tracing == "DiagnosticListener")
                        {
                            var source = new DiagnosticListener("Masstrasit.Rabbit");
                            cfg.UseDiagnosticsActivity(source);
                        }
                        else
                        {
                            //works with Jaeger
                            cfg.PropagateOpenTracingContext();
                        }
                        

                        cfg.ReceiveEndpoint(host, busConfig.ReceiveEndpoint, e =>
			            {
				            e.UseMessageRetry(x => x.Interval(10, new TimeSpan(0,0,0,0,500)));    
				            e.ConfigureConsumer(provider, consumerTypes.ToArray());
			            });

                        //healthcheck part 2
                        ILogger<HealthCheckBusObserver> healthCheckLogger = provider.GetService<ILogger<HealthCheckBusObserver>>();
                        cfg.ConnectBusObserver(new HealthCheckBusObserver(busConnectionState, healthCheckLogger));

                    }));
		            
	            });  
	            
            });

            return webHostBuilder;
        }
    }
}