using System;

namespace DepsWebApp.Options
{
    /// <summary>
    /// Options of nbu client
    /// </summary>
    public class NbuClientOptions
    {
        /// <summary>
        /// Base address of nbu client
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Check if base address is valid
        /// </summary>
        public bool IsValid => !string.IsNullOrWhiteSpace(BaseAddress) &&
                               Uri.TryCreate(BaseAddress, UriKind.Absolute, out _);
    }
}
