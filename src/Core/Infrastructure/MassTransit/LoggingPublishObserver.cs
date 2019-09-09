namespace Core.Infrastructure.MassTransit
{
    using System;
    using System.Threading.Tasks;
    using global::MassTransit;
    using Microsoft.Extensions.Logging;

    public class LoggingPublishObserver : IPublishObserver
    {
        private readonly ILogger<LoggingPublishObserver> _logger;

        public LoggingPublishObserver(ILogger<LoggingPublishObserver> logger)
        {
            _logger = logger;
        }

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            _logger.LogInformation($"BUS publishing {typeof(T).FullName}, correlationId: {context.CorrelationId}");
            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            _logger.LogInformation($"BUS published {typeof(T).FullName}, correlationId: {context.CorrelationId}");
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            _logger.LogError(exception, $"BUS cannot publish message {typeof(T).FullName}");
            return Task.CompletedTask;
        }
    }
}