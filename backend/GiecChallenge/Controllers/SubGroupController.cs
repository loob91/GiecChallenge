using Microsoft.AspNetCore.Mvc;
using GiecChallenge.Services;
using Microsoft.AspNetCore.Authorization;

namespace GiecChallenge.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class SubGroupController : ControllerBase
{
    private readonly ILogger<SubGroupController> _logger;
    private readonly ISubGroupRepository _subGroupRepository;

    public SubGroupController(ILogger<SubGroupController> logger,
                             ISubGroupRepository subgroupRepository)
    {
        _logger = logger;
        _subGroupRepository = subgroupRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try {
            return Ok(await _subGroupRepository.GetAllSubGroups());
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByCode(Guid id)
    {
        try {
            return Ok(await _subGroupRepository.GetSubGroup(id));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("name/{language}/{name}")]
    public async Task<IActionResult> GetByName(string language, string name)
    {
        try {
            return Ok(await _subGroupRepository.GetSubGroups(name.ToLower(), language));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(SubGroupDto subgroup)
    {
        try {
            await _subGroupRepository.Create(subgroup);
            return Ok(new { message = "SubGroup created" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(Guid id, SubGroupDto subgroup)
    {
        try {
            await _subGroupRepository.Update(id, subgroup);
            return Ok(new { message = "SubGroup updated" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}
