using TechMoveLogisticSystem.Api.DTOs;

namespace TechMoveLogisticSystem.Api.Services
{
    public interface ICurrencyConversionService
    {
        // Gets the current USD to ZAR exchange rate from the external API
        Task<decimal> GetUsdToZarRateAsync();

        // Converts a USD amount into ZAR using the latest rate
        Task<CurrencyConversionResultDto> ConvertUsdToZarAsync(decimal usdAmount);
    }
}