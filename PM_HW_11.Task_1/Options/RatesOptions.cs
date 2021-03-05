namespace DepsWebApp.Options
{
    /// <summary>
    /// Options of currency rate
    /// </summary>
    public class RatesOptions
    {
        /// <summary>
        /// Base currency
        /// </summary>
        public string BaseCurrency { get; set; }
        /// <summary>
        /// Check if currency is null or whitespace
        /// </summary>
        public bool IsValid => !string.IsNullOrWhiteSpace(BaseCurrency);
    }
}
