using Microsoft.AspNetCore.Mvc;
using GiecChallenge.Services;
using Microsoft.AspNetCore.Authorization;

namespace GiecChallenge.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class AlimentController : ControllerBase
{
    private readonly ILogger<AlimentController> _logger;
    private readonly IAlimentRepository _alimentRepository;

    public AlimentController(ILogger<AlimentController> logger,
                             IAlimentRepository alimentRepository)
    {
        _logger = logger;
        _alimentRepository = alimentRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try {
            return Ok(await _alimentRepository.GetAllAliments());
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByCode(string id)
    {
        try {
            return Ok(await _alimentRepository.GetAliment(id));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        try {
            return Ok(await _alimentRepository.GetAliments(name.ToLower()));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(AlimentDto aliment)
    {
        try {
            await _alimentRepository.Create(aliment);
            return Ok(new { message = "Aliment created" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost("full")]
    public async Task<IActionResult> FullPost(List<AlimentDto> aliments)
    {
        try {
            foreach (AlimentDto aliment in aliments)
                await _alimentRepository.Create(aliment);
            return Ok(new { message = "Aliments created" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(AlimentDto aliment)
    {
        try {
            await _alimentRepository.Update(aliment);
            return Ok(new { message = "Aliment updated" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut("full")]
    public async Task<IActionResult> UpdateFull(List<AlimentDto> aliments)
    {
        try {
            foreach (AlimentDto aliment in aliments) {
                try {
                    await _alimentRepository.Update(aliment);
                }
                catch {
                    await _alimentRepository.Create(aliment);
                }
            }
            return Ok(new { message = "Aliments updated" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}
