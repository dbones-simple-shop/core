namespace Core.Infrastructure.MassTransit
{
    using System;
    using System.Threading.Tasks;
    using global::MassTransit;
    using Microsoft.Extensions.Logging;

    public class LoggingReceiveObserver : IReceiveObserver
    {
        private readonly ILogger<LoggingReceiveObserver> _logger;

        public LoggingReceiveObserver(ILogger<LoggingReceiveObserver> logger)
        {
            _logger = logger;
        }

        public Task PreReceive(ReceiveContext context)
        {
            _logger.LogInformation($"BUS Request Started: {context.ContentType}, {context.InputAddress}");
            return Task.CompletedTask;
        }

        public Task PostReceive(ReceiveContext context)
        {
            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            _logger.LogInformation($"BUS Request Completed. CorrelationId: {context.CorrelationId}, MessageId: {context.MessageId}, MessageType: {context?.Message?.GetType()}, Consumer: {consumerType}, Duration: {duration.ToString("g")}");

            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            _logger.LogError(new EventId(1337), exception, $"Request Failed. CorrelationId: {context.CorrelationId}, MessageId: {context.MessageId}, MessageType: {context?.Message?.GetType()}, Consumer: {consumerType}");
            return Task.CompletedTask;
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            _logger.LogError(new EventId(1338), exception, $"ReceiveFault: {context.ContentType} {context.InputAddress}");
            return Task.CompletedTask;
        }
    }

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