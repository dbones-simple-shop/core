using System.Collections.Generic;
using System.Text;

namespace Core.Infrastructure.MassTransit.HealthCheck
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public class MassTransitHealthCheck : IHealthCheck
    {
        private readonly MassTransitHealthCheckState _state;

        public MassTransitHealthCheck(MassTransitHealthCheckState state)
        {
            _state = state;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            HealthCheckResult result;
            result = _state.IsRunning 
                ? new HealthCheckResult(HealthStatus.Healthy, "bus is connected") 
                : new HealthCheckResult(HealthStatus.Unhealthy, "Bus is not connected");

            return Task.FromResult(result);
        }
    }
}
