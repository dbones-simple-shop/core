using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.MassTransit.HealthCheck
{
    using System;
    using System.Threading.Tasks;
    using global::MassTransit;

    /// <summary>
    /// observe for when masstransit is connected to the bus.
    /// </summary>
    public class HealthCheckBusObserver : IBusObserver
    {
        private readonly MassTransitHealthCheckState _state;
        private readonly ILogger<HealthCheckBusObserver> _logger;

        public HealthCheckBusObserver(
            MassTransitHealthCheckState state, 
            ILogger<HealthCheckBusObserver> logger)
        {
            _state = state;
            _logger = logger;
        }

        public Task PostCreate(IBus bus)
        {
            //not sure the state.
            //_state.IsRunning = ;
            return Task.CompletedTask;
        }

        public Task CreateFaulted(Exception exception)
        {
            _state.IsRunning = false;
            return Task.CompletedTask;
        }

        public Task PreStart(IBus bus)
        {
            _state.IsRunning = false;
            return Task.CompletedTask;
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            busReady.ContinueWith(_ =>
            { 
                _state.IsRunning = true;
                _logger.LogInformation("connected to the bus");
            });

            return Task.CompletedTask;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            _logger.LogWarning("StartFaulted", exception);
            _state.IsRunning = false;
            return Task.CompletedTask;
        }

        public Task PreStop(IBus bus)
        {
            _state.IsRunning = false;
            return Task.CompletedTask;
        }

        public Task PostStop(IBus bus)
        {
            _logger.LogInformation("Stopped");
            _state.IsRunning = false;
            return Task.CompletedTask;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            _logger.LogWarning("StopFaulted", exception);
            _state.IsRunning = false;
            return Task.CompletedTask;
        }
    }
}