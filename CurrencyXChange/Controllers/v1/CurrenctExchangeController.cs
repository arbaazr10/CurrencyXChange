using CurrencyXchange.Business.Services.Exchange;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CurrencyXChange.Contracts.v1.EndPoints;

namespace CurrencyXChange.Controllers.v1;

[ApiController]
[Route("/api/v1/[controller]")]
public class CurrenctExchangeController : ControllerBase
{


    private readonly ILogger<CurrenctExchangeController> _logger;
    private readonly IExchangeServices _exchangeRateService;

    public CurrenctExchangeController(ILogger<CurrenctExchangeController> logger, IExchangeServices exchangeRateService)
    {
        _logger = logger;
        _exchangeRateService = exchangeRateService;
    }

    [HttpGet(CurrencyExchange.Latest)]
    public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
    {
        try
        {
            if (string.IsNullOrEmpty(baseCurrency))
                return BadRequest("Base currency is required.");

            var rates = await _exchangeRateService.GetLatestRatesAsync(baseCurrency,"frankfurter");
            return Ok(rates);
        }
        catch (Exception ex)
        { 
            _logger.LogError(ex, "Error fetching latest rates");
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet(CurrencyExchange.Convert)]
    public async Task<IActionResult> ConvertCurrency([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount)
    {
        try
        {
            var excludedCurrencies = new[] { "TRY", "PLN", "THB", "MXN" };
            if (excludedCurrencies.Contains(from) || excludedCurrencies.Contains(to))
                return BadRequest("Currency conversion for TRY, PLN, THB, and MXN is not allowed.");

            var result = await _exchangeRateService.ConvertCurrencyAsync(from, to, amount,"frankfurter");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting currency");
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet(CurrencyExchange.Historical)]
    public async Task<IActionResult> GetHistoricalRates(
        [FromQuery] string baseCurrency,
        [FromQuery] string startDate,
        [FromQuery] string endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var history = await _exchangeRateService.GetHistoricalRatesAsync(baseCurrency, startDate, endDate, page, pageSize, "frankfurter");
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching historical rates");    
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}
