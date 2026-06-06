namespace TechMoveLogisticSystem.Api.DTOs
{
    public class CurrencyConversionResultDto
    {
        public decimal UsdAmount { get; set; }

        public decimal ExchangeRate { get; set; }

        public decimal ZarAmount { get; set; }

        public string SourceCurrency { get; set; } = "USD";

        public string TargetCurrency { get; set; } = "ZAR";

        public DateTime ConvertedAt { get; set; }
    }
}