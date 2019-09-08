namespace Core.Infrastructure.Tracing
{
    public class TracingConfiguration
    {
        public bool EnableOpenTracing { get; set; } = true;
        public string JaegerUrl { get; set; } = "jaeger-collector.istio-system";

    }
}