using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DepsWebApp.Services
{
    /// <summary>
    /// Cache hosted service
    /// </summary>
    public class CacheHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _cacheTimer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scopeFactory"></param>
        public CacheHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        /// <summary>
        /// Starts timer to actualize data
        /// </summary>
        /// <param name="cancellationToken"> cancellation token</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var period = TimeSpan.FromDays(1d);
            var nextDate = now.Add(period).Date;
            var dueTime = nextDate - now;
            
            // reset cache every midnight (UTC)
            _cacheTimer = new Timer(ActualizeCache, _scopeFactory, dueTime, period);
            
            return Task.CompletedTask;
        }

        private void ActualizeCache(object state)
        {
            var factory = (IServiceScopeFactory) state;
            using var scope = factory.CreateScope();
            var rates = scope.ServiceProvider.GetRequiredService<IRatesService>();
            rates.ActualizeRatesAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Stops actualizing data
        /// </summary>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cacheTimer?.Dispose();
            return Task.CompletedTask;
        }
    }
}
