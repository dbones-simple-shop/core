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
        public static IServiceCollection AddJaeger(this IServiceCollection services, IConfiguration configuration, bool useAgent = true)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                var tracingConfig = new TracingConfiguration();
                configuration.GetSection("Tracing").Bind(tracingConfig);

                string serviceName = tracingConfig.ServiceName ?? Assembly.GetEntryAssembly().GetName().Name;
                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                ITracer tracer;

                //HACK TIME
                var jaegerUri = $"http://{tracingConfig.JaegerUrl}:14268/api/traces";
                var useLocalSettigns = jaegerUri.ToString().Contains("localhost");
                
                if (false)
                {
                    //use the collector
                    Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", serviceName);
                    Environment.SetEnvironmentVariable("JAEGER_ENDPOINT", jaegerUri);
                    Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "const");
                    var config = Jaeger.Configuration.FromEnv(loggerFactory);
                    tracer = config.GetTracer();
                }

                if (true)
                {
                    //use the agent
                    Environment.SetEnvironmentVariable("JAEGER_SERVICE_NAME", serviceName);
                    Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", tracingConfig.JaegerAgent);
                    Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", "6831");
                    Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "const");
                    var config = Jaeger.Configuration.FromEnv(loggerFactory);
                    tracer = config.GetTracer();
                }
                else
                {
                    //use lcoalhost defaults.
                    ISampler sampler = new ConstSampler(sample: true);

                    tracer = new Tracer.Builder(serviceName)
                        .WithLoggerFactory(loggerFactory)
                        .WithSampler(sampler)
                        .Build();
                }



                GlobalTracer.Register(tracer);

                return tracer;
            });

            // Prevent endless loops when OpenTracing is tracking HTTP requests to Jaeger.
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