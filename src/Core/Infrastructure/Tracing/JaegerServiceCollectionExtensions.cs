using System;
using System.Reflection;
using Jaeger;
using Jaeger.Samplers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Contrib.NetCore.CoreFx;
using OpenTracing.Util;

namespace Core.Infrastructure.Tracing
{
    using Microsoft.Extensions.Configuration;

    public static class JaegerServiceCollectionExtensions
    {
        public static IServiceCollection AddJaeger(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var tracingConfig = new TracingConfiguration();
            configuration.GetSection("Tracing").Bind(tracingConfig);
            var jaegerUri = new Uri($"http://{tracingConfig.JaegerUrl}:14268/api/traces");

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                string serviceName = Assembly.GetEntryAssembly().GetName().Name;

                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                ISampler sampler = new ConstSampler(sample: true);

                ITracer tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(sampler)
                    .Build();

                GlobalTracer.Register(tracer);

                return tracer;
            });

            // Prevent endless loops when OpenTracing is tracking HTTP requests to Jaeger.
            services.Configure<HttpHandlerDiagnosticOptions>(options =>
            {
                options.IgnorePatterns.Add(request =>  request.RequestUri.ToString().Contains("/api/traces") 
                                                       ||  jaegerUri.IsBaseOf(request.RequestUri));
            });

            return services;
        }
    }
}