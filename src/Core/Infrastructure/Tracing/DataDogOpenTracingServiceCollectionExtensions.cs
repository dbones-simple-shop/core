namespace Core.Infrastructure.Tracing
{
    using System;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using OpenTracing;
    using OpenTracing.Contrib.NetCore.CoreFx;
    using OpenTracing.Util;

    public static class DataDogOpenTracingServiceCollectionExtensions
    {
        public static IServiceCollection AddDataDog(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                var tracingConfig = new TracingConfiguration();
                configuration.GetSection("Tracing").Bind(tracingConfig);

                var datadogUrl = $"http://{tracingConfig.DataDogUrl}:8126";
                var serviceName = Assembly.GetEntryAssembly().GetName().Name;

                var tracer = Datadog.Trace.OpenTracing.OpenTracingTracerFactory.CreateTracer(new Uri(datadogUrl), serviceName);
                GlobalTracer.Register(tracer);

                return tracer;
            });

            // Prevent endless loops when OpenTracing is tracking HTTP requests to opentracing.
            services.Configure<HttpHandlerDiagnosticOptions>(options =>
            {
                options.IgnorePatterns.Add(request => request.RequestUri.ToString().ToLower().Contains("/api/traces"));
                options.IgnorePatterns.Add(request => request.RequestUri.ToString().ToLower().EndsWith("hc"));
                options.IgnorePatterns.Add(request => request.RequestUri.ToString().ToLower().Contains("datadoghq"));
            });

            return services;
        }
    }
}