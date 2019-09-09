namespace Core.Infrastructure.Logging
{
    public class LoggingConfiguration
    {
        public string Logger { get; set; }
        public string DataDogApiKey { get; set; }
        public bool FilterAspNetCore { get; set; } = true;
    }
}