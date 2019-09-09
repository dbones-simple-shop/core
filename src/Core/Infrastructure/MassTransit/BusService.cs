using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Core.Infrastructure.MassTransit
{
    using Microsoft.Extensions.Logging;

    public class BusService :
        IHostedService
    {
        private readonly IBusControl _busControl;
        private readonly ILogger<BusService> _logger;
        private readonly ILogger<LoggingReceiveObserver> _messageReceiveLogger;
        private readonly ILogger<LoggingPublishObserver> _publishingObserverLogger;

        public BusService(IBusControl busControl, 
            ILogger<BusService> logger, 
            ILogger<LoggingReceiveObserver> messageReceiveLogger,
            ILogger<LoggingPublishObserver> publishingObserverLogger)
        {
            _busControl = busControl;
            _logger = logger;
            _messageReceiveLogger = messageReceiveLogger;
            _publishingObserverLogger = publishingObserverLogger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("starting bus");

            _busControl.ConnectReceiveObserver(new LoggingReceiveObserver(_messageReceiveLogger));
            _busControl.ConnectPublishObserver(new LoggingPublishObserver(_publishingObserverLogger));
            return _busControl.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _busControl.StopAsync(cancellationToken);
        }
    }
}