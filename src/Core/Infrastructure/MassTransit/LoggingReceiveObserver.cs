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
            _logger.LogInformation($"Request Started: {context.ContentType}, {context.InputAddress}");
            return Task.CompletedTask;
        }

        public Task PostReceive(ReceiveContext context)
        {
            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            _logger.LogInformation($"Request Completed. CorrelationId: {context.CorrelationId}, MessageId: {context.MessageId}, MessageType: {context?.Message?.GetType()}, Consumer: {consumerType}, Duration: {duration.ToString("g")}");

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
}