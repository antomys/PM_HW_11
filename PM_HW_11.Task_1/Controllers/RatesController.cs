using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DepsWebApp.Middlewares;
using DepsWebApp.Services;

namespace DepsWebApp.Controllers
{
    /// <summary>
    /// Change currencies
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class RatesController : ControllerBase
    {
        private readonly ILogger<RatesController> _logger;
        private readonly IRatesService _rates;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rates">list of rates</param>
        /// <param name="logger">logger</param>
        public RatesController(
            IRatesService rates,
            ILogger<RatesController> logger)
        {
            _rates = rates;
            _logger = logger;
        }

        /// <summary>
        /// Changes amount of one currency into another
        /// </summary>
        /// <param name="srcCurrency">Source currency</param>
        /// <param name="dstCurrency">Destination currency</param>
        /// <param name="amount">Amount to convert</param>
        /// <returns>Converted currency</returns>
        [HttpGet("{srcCurrency}/{dstCurrency}")]
        [TypeFilter(typeof(CustomExceptionFilter))]
        [ProducesResponseType(typeof(int), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult<decimal>> Get(string srcCurrency, string dstCurrency, decimal? amount)
        {
            var exchange =  await _rates.ExchangeAsync(srcCurrency, dstCurrency, amount ?? decimal.One);
            if (exchange.HasValue) return exchange.Value.DestinationAmount;
            _logger.LogDebug($"Can't exchange from '{srcCurrency}' to '{dstCurrency}'");
            return BadRequest("Invalid currency code");
        }
    }
}
