using Microsoft.AspNetCore.Mvc;
using GiecChallenge.Services;
using Microsoft.AspNetCore.Authorization;

namespace GiecChallenge.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class LanguageController : ControllerBase
{
    private readonly ILogger<LanguageController> _logger;
    private readonly ILanguageRepository _languageRepository;

    public LanguageController(ILogger<LanguageController> logger,
                             ILanguageRepository languageRepository)
    {
        _logger = logger;
        _languageRepository = languageRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try {
            return Ok(await _languageRepository.GetAllLanguages());
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByCode(Guid id)
    {
        try {
            return Ok(await _languageRepository.GetLanguage(id));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name, string language)
    {
        try {
            return Ok(await _languageRepository.GetLanguages(name.ToLower(), language));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(LanguageDto language)
    {
        try {
            await _languageRepository.Create(language);
            return Ok(new { message = "Language created" });
        }
        catch (Exception ex) {
            return Ok(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, LanguageDto language)
    {
        try {
            if (Guid.TryParse(id, out Guid result)) {
                await _languageRepository.Update(result, language);
                return Ok(new { message = "Language updated" });
            }
            return Ok(new { message = "Id is not in good format" });
        }
        catch (Exception ex) {
            return Ok(new { message = ex.Message });
        }
    }
}
