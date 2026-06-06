using Microsoft.AspNetCore.Mvc;
using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Services;

namespace TechMoveLogisticSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyConversionService _currencyService;

        public CurrencyController(ICurrencyConversionService currencyService)
        {
            _currencyService = currencyService;
        }

        // GET: api/Currency/usd-to-zar-rate
        [HttpGet("usd-to-zar-rate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<decimal>> GetUsdToZarRate()
        {
            try
            {
                var rate = await _currencyService.GetUsdToZarRateAsync();

                return Ok(rate);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
        }

        // POST: api/Currency/convert-usd-to-zar
        [HttpPost("convert-usd-to-zar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<CurrencyConversionResultDto>> ConvertUsdToZar(CurrencyConversionRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Currency conversion request is required.");
            }

            try
            {
                var result = await _currencyService.ConvertUsdToZarAsync(request.UsdAmount);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, ex.Message);
            }
        }
    }
}