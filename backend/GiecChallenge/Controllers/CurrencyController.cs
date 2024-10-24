using Microsoft.AspNetCore.Mvc;
using GiecChallenge.Services;
using Microsoft.AspNetCore.Authorization;

namespace GiecChallenge.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ILogger<CurrencyController> _logger;
    private readonly ICurrencyRepository _currencyRepository;

    public CurrencyController(ILogger<CurrencyController> logger,
                             ICurrencyRepository currencyRepository)
    {
        _logger = logger;
        _currencyRepository = currencyRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try {
            return Ok(await _currencyRepository.GetAllCurrencies());
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("iso/{id}")]
    public async Task<IActionResult> GetByIso(string ISOCode)
    {
        try {
            return Ok(await _currencyRepository.GetCurrencyByISO(ISOCode));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByCode(Guid id)
    {
        try {
            return Ok(await _currencyRepository.GetCurrency(id));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("name/{language}/{name}")]
    public async Task<IActionResult> GetByName(string name, string language)
    {
        try {
            return Ok(await _currencyRepository.GetCurrencies(name.ToLower(), language));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(CurrencyDto currency)
    {
        try {
            await _currencyRepository.Create(currency);
            return Ok(new { message = "Currency created" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(Guid id, CurrencyDto currency)
    {
        try {
            await _currencyRepository.Update(id, currency);
            return Ok(new { message = "Currency updated" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}
