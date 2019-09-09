namespace Core.Infrastructure.Tracing
{
    using System;
    using System.Reflection;
    using Jaeger;
    using Jaeger.Samplers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using OpenTracing;
    using OpenTracing.Contrib.NetCore.CoreFx;
    using OpenTracing.Util;
    using Microsoft.Extensions.Configuration;
    using OpenTracing.Propagation;

    public static class JaegerServiceCollectionExtensions
    {
        public static IServiceCollection AddJaeger(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                var tracingConfig = new TracingConfiguration();
                configuration.GetSection("Tracing").Bind(tracingConfig);

                var jaegerUri = $"http://{tracingConfig.JaegerUrl}:14268/api/traces";

                string serviceName = Assembly.GetEntryAssembly().GetName().Name;
                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                ITracer tracer;

                if (!jaegerUri.ToString().Contains("localhost"))
                {
                    Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", serviceName);
                    Environment.SetEnvironmentVariable("JAEGER_ENDPOINT", jaegerUri);
                    Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "const");
                    var config = Jaeger.Configuration.FromEnv(loggerFactory);
                    tracer = config.GetTracer();
                }
                else
                {
                    ISampler sampler = new ConstSampler(sample: true);

                    tracer = new Tracer.Builder(serviceName)
                        .WithLoggerFactory(loggerFactory)
                        .WithSampler(sampler)
                        .Build();
                }

                Datadog.Trace.OpenTracing.OpenTracingTracerFactory.CreateTracer();
                var settings = Datadog.Trace.Configuration.TracerSettings.FromDefaultSources();
                var trace = new Datadog.Trace.Tracer(settings);



                GlobalTracer.Register(tracer);

                return tracer;
            });

            // Prevent endless loops when OpenTracing is tracking HTTP requests to Jaeger.
            services.Configure<HttpHandlerDiagnosticOptions>(options =>
            {
                options.IgnorePatterns.Add(request => request.RequestUri.ToString().ToLower().Contains("/api/traces"));
                options.IgnorePatterns.Add(request => request.RequestUri.ToString().ToLower().EndsWith("hc"));
            });

            return services;
        }
    }
}