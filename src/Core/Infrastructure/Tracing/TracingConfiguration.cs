namespace Core.Infrastructure.Tracing
{
    public class TracingConfiguration
    {
        public bool EnableOpenTracing { get; set; } = true;
        public string Tracer { get; set; } = "jaeger";
        public string JaegerUrl { get; set; } = "localhost";
        public string DataDogUrl { get; set; } = "localhost";

    }
}