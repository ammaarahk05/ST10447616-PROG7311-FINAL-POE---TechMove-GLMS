using System.Net.Http.Json;
using TechMoveLogisticSystem.Api.DTOs;

namespace TechMoveLogisticSystem.Api.Services
{
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CurrencyConversionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            // the API URL is kept in appsettings so it is not hardcoded in the service
            var apiUrl = _configuration["CurrencyApi:Url"];

            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                throw new InvalidOperationException("Currency API URL is missing from configuration.");
            }

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Currency API is currently unavailable.");
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ExchangeRateApiResponse>();

            if (apiResponse == null || apiResponse.Rates == null || !apiResponse.Rates.ContainsKey("ZAR"))
            {
                throw new InvalidOperationException("USD to ZAR rate could not be found in the API response.");
            }

            var rate = apiResponse.Rates["ZAR"];

            if (rate <= 0)
            {
                throw new InvalidOperationException("Invalid USD to ZAR exchange rate returned by API.");
            }

            return Math.Round(rate, 4);
        }

        public async Task<CurrencyConversionResultDto> ConvertUsdToZarAsync(decimal usdAmount)
        {
            if (usdAmount < 0)
            {
                throw new ArgumentException("USD amount cannot be negative.");
            }

            var rate = await GetUsdToZarRateAsync();

            return new CurrencyConversionResultDto
            {
                UsdAmount = usdAmount,
                ExchangeRate = rate,
                ZarAmount = Math.Round(usdAmount * rate, 2),
                ConvertedAt = DateTime.UtcNow
            };
        }

        private class ExchangeRateApiResponse
        {
            public Dictionary<string, decimal>? Rates { get; set; }
        }
    }
}