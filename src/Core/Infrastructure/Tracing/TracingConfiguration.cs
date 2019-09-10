namespace Core.Infrastructure.Tracing
{
    public class TracingConfiguration
    {
        public bool EnableOpenTracing { get; set; } = true;

        /// <summary>
        /// jaeger = collector
        /// jeagerAgent = agent
        /// datadog = datadog tracing
        /// </summary>
        public string Tracer { get; set; } = "jaeger";
        public string JaegerUrl { get; set; } = "localhost";
        public string JaegerAgent { get; set; } = "localhost";

        public string DataDogUrl { get; set; } = "localhost";

    }
}