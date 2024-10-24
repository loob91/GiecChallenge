using Microsoft.AspNetCore.Mvc;
using GiecChallenge.Services;
using Microsoft.AspNetCore.Authorization;

namespace GiecChallenge.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class GroupController : ControllerBase
{
    private readonly ILogger<GroupController> _logger;
    private readonly IGroupRepository _groupRepository;

    public GroupController(ILogger<GroupController> logger,
                             IGroupRepository userRepository)
    {
        _logger = logger;
        _groupRepository = userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try {
            return Ok(await _groupRepository.GetAllGroups());
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByCode(Guid id)
    {
        try {
            return Ok(await _groupRepository.GetGroup(id));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("name/{language}/{name}")]
    public async Task<IActionResult> GetByName(string name, string language)
    {
        try {
            return Ok(await _groupRepository.GetGroups(name.ToLower(), language));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(GroupDto product)
    {
        try {
            await _groupRepository.Create(product);
            return Ok(new { message = "Group created" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(Guid id, GroupDto product)
    {
        try {
            await _groupRepository.Update(id, product);
            return Ok(new { message = "Group updated" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}
