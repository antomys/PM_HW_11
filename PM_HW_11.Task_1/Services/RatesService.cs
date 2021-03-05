using System;
using System.Linq;
using System.Threading.Tasks;
using DepsWebApp.Clients;
using DepsWebApp.Models;
using DepsWebApp.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DepsWebApp.Services
{
    /// <summary>
    /// Service to get rates occasionally
    /// </summary>
    public class RatesService : IRatesService
    {
        private const string RatesCacheKey = "rates";
        private static readonly TimeSpan DefaultCacheLifeTime = TimeSpan.FromHours(1d);

        private readonly IRatesProviderClient _client;
        private readonly IMemoryCache _cache;

        private readonly string _baseCurrency;
        private readonly CacheOptions _cacheOptions;

        /// <summary>
        /// Constructor of RatesService class
        /// </summary>
        /// <param name="client">DI, IRatesProviderClient</param>
        /// <param name="cache">Cache from memory,m from DI</param>
        /// <param name="ratesOptions">rate options, from DI</param>
        /// <param name="cacheOptions">cache options, from DI</param>
        /// <exception cref="ArgumentOutOfRangeException">if client or cache is null</exception>
        public RatesService(
            IRatesProviderClient client, 
            IMemoryCache cache,
            IOptions<RatesOptions> ratesOptions, 
            IOptions<CacheOptions> cacheOptions)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));

            var ratesOptionsValue = ratesOptions?.Value;
            if (ratesOptionsValue == null || !ratesOptionsValue.IsValid)
                throw new ArgumentOutOfRangeException(nameof(ratesOptions), "Options are invalid");
            _baseCurrency = ratesOptionsValue.BaseCurrency.ToUpperInvariant();

            _cacheOptions = cacheOptions?.Value ??
                throw new ArgumentOutOfRangeException(nameof(cacheOptions), "Options are invalid");
        }


        /// <summary>
        /// Method to async exchange currencies
        /// </summary>
        /// <param name="srcCurrency">Currency to convert</param>
        /// <param name="destCurrency">Currency to be converted</param>
        /// <param name="amount">Amount to convert</param>
        /// <returns>Exchanged value</returns>
        /// <exception cref="InvalidOperationException">If currencies are either equal or invalid</exception>
        /// <exception cref="ArgumentOutOfRangeException">If amount is negative</exception>
        public async Task<ExchangeResult?> ExchangeAsync(string srcCurrency, string destCurrency, decimal amount)
        {
            var comparer = StringComparer.Ordinal;

            if (string.IsNullOrWhiteSpace(srcCurrency) || 
                string.IsNullOrWhiteSpace(destCurrency)) return null;

            srcCurrency = srcCurrency.ToUpperInvariant();
            destCurrency = destCurrency.ToUpperInvariant();
            
            //check if amount is null or negative
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            // check case with same src. and dest. currencies
            if (comparer.Equals(srcCurrency, destCurrency))
                return new ExchangeResult(decimal.One, amount, amount);
            
            var rates = await GetRatesAsync();
            if (rates == null) throw new InvalidOperationException("Currency rates are invalid");
            
            var srcRate = comparer.Equals(srcCurrency, _baseCurrency) 
                ? decimal.One
                : rates.FirstOrDefault(r => comparer.Equals(r.Currency, srcCurrency))?.Rate;

            var destRate = comparer.Equals(destCurrency, _baseCurrency)
                ? decimal.One
                : rates.FirstOrDefault(r => comparer.Equals(r.Currency, destCurrency))?.Rate;

            if (!srcRate.HasValue || !destRate.HasValue) return null;
            
            var rate = srcRate.Value / destRate.Value;
            return new ExchangeResult(rate, amount, rate * amount);
        }

        private async Task<CurrencyRate[]> GetRatesAsync()
        {
            return await _cache.GetOrCreateAsync(RatesCacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow =
                    _cacheOptions.RatesCacheLifeTime ?? DefaultCacheLifeTime;

                var rates = await _client.GetRatesAsync();
                return rates is CurrencyRate[] ratesArray 
                    ? ratesArray 
                    : rates?.ToArray();
            });
        }

        /// <summary>
        /// Method to refresh cache
        /// </summary>
        /// <returns></returns>
        public Task ActualizeRatesAsync()
        {
            _cache.Remove(RatesCacheKey);
            return Task.CompletedTask;
        }
    }
}
