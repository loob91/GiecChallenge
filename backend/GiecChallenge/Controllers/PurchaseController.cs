using Microsoft.AspNetCore.Mvc;
using GiecChallenge.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GiecChallenge.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;

namespace GiecChallenge.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class PurchaseController : ControllerBase
{
    private readonly ILogger<PurchaseController> _logger;
    private readonly IPurchaseRepository _purchaseRepository;
    private Guid _userId {get; set;}

    public PurchaseController(ILogger<PurchaseController> logger,
                             IPurchaseRepository purchaseRepository)
    {
        _logger = logger;
        _purchaseRepository = purchaseRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try {
            GetUserId();
            return Ok(await _purchaseRepository.GetAll(_userId));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        try {
            GetUserId();
            if (Guid.TryParse(id, out Guid purchaseIdGuid)) {
                return Ok(await _purchaseRepository.Get(_userId, purchaseIdGuid));
            }
            return StatusCode(500, new { Message = "Not a valid ID" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("{dateBegin}/{dateEnd}")]
    public async Task<IActionResult> GetBetweenDate(string dateBegin, string dateEnd)
    {
        try {
            GetUserId();
            return Ok(await _purchaseRepository.GetBetweenDate(_userId, DateTime.ParseExact(dateBegin, "dd-MM-yyyy", null), DateTime.ParseExact(dateEnd + " 23:59:59", "dd-MM-yyyy HH:mm:ss", null)));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("CO2/{dateBegin}/{dateEnd}")]
    public async Task<IActionResult> GetCO2BetweenDate(string dateBegin, string dateEnd)
    {
        try {
            GetUserId();
            return Ok(await _purchaseRepository.GetCO2BetweenDate(_userId, DateTime.ParseExact(dateBegin, "dd-MM-yyyy", null), DateTime.ParseExact(dateEnd + " 23:59:59", "dd-MM-yyyy HH:mm:ss", null)));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(PurchaseDto purchaseDto)
    {
        try {
            GetUserId();
            await _purchaseRepository.Create(_userId, purchaseDto);
            return Ok(new { message = "Purchase created" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PurchaseDto purchaseDto)
    {
        try {
            GetUserId();
            await _purchaseRepository.Update(_userId, purchaseDto);
            return Ok(new { message = "Purchase updated" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpDelete("{purchaseId}")]
    public async Task<IActionResult> Delete(string purchaseId)
    {
        try {
            GetUserId();
            if (Guid.TryParse(purchaseId, out Guid purchaseIdGuid)) {
                await _purchaseRepository.Delete(_userId, purchaseIdGuid);
                return Ok(new { message = "Purchase deleted" });
            }
            return StatusCode(500, new { Message = "Not a valid ID" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpDelete("line/{purchaseLineId}")]
    public async Task<IActionResult> DeleteLine(string purchaseLineId)
    {
        try {
            GetUserId();
            if (Guid.TryParse(purchaseLineId, out Guid purchaseLineIdGuid)) {
                await _purchaseRepository.DeleteLine(_userId, purchaseLineIdGuid);
                return Ok(new { message = "Purchase line deleted" });
            }
            return StatusCode(500, new { Message = "Not a valid ID" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost]
    [Route("laruche")]
    public async Task<IActionResult> PostLaRuche(PurchaseLaRucheDto command)
    {
        try {
            GetUserId();
            return Ok(await _purchaseRepository.ImportLaRuchePurchase(_userId, command));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    private void GetUserId() {
        if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            throw new Exception("Not authorized");
        this._userId = userId;
    }
}
