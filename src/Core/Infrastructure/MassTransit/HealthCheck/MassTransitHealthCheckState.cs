namespace Core.Infrastructure.MassTransit.HealthCheck
{
    public class MassTransitHealthCheckState
    {
        private volatile bool _isRunning = false;
        public bool IsRunning
        {
            get => _isRunning;
            set => _isRunning = value;
        }
    }
}