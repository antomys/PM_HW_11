using System.Text.Json.Serialization;

namespace PM_HW_11.Task_2.Models
{
    public class CurrencyConverterModel
    {
        [JsonPropertyName("srcCurrency")]
        public string SrcCurrency { get; set; }
        
        [JsonPropertyName("dstCurrency")]
        public string DstCurrency { get; set; }
        [JsonPropertyName("amount")]
        public int? Amount { get; set; }

        public CurrencyConverterModel(string srcCurrency, string dstCurrency, int? amount)
        {
            SrcCurrency = srcCurrency;
            DstCurrency = dstCurrency;
            Amount = amount;
        }
    }
}