using Microsoft.AspNetCore.Mvc;
using GiecChallenge.Services;
using GiecChallenge.Models;
using Microsoft.AspNetCore.Authorization;

namespace GiecChallenge.Controllers;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;

    public UserController(ILogger<UserController> logger,
                             IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserDto userDto)
    {
        try {
            return Ok(await _userRepository.Login(userDto));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserDto userDto)
    {
        try {
            await _userRepository.Register(userDto);
            return Ok();
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}
