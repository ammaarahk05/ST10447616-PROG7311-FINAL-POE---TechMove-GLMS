using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TechMoveLogisticSystem.Api.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;

        // API key from exchangerate-api.com
        private const string ApiKey = "4ebb3faf7e2103b174c311fe";

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Gets USD to ZAR exchange rate
        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                var url = $"https://v6.exchangerate-api.com/v6/{ApiKey}/latest/USD";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Failed to retrieve exchange rate from API.");
                }

                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);

                var rate = doc.RootElement
                    .GetProperty("conversion_rates")
                    .GetProperty("ZAR")
                    .GetDecimal();

                return rate;
            }
            catch
            {
                // alternative fallback if api key breaks 
                //return 18.00m;
                throw;
            }
        }

        // Converts USD to ZAR
        public decimal ConvertUsdToZar(decimal usdAmount, decimal rate)
        {
            return usdAmount * rate;
        }
    }
}