using Microsoft.AspNetCore.Mvc;
using GiecChallenge.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GiecChallenge.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductRepository _productRepository;
    private Guid _userId {get; set;}

    public ProductController(ILogger<ProductController> logger,
                             IProductRepository productRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try {
            return Ok(await _productRepository.GetAllProducts());
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByCode(Guid id)
    {
        try {
            return Ok(await _productRepository.GetProduct(id));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("name/{language}/{name}")]
    public async Task<IActionResult> GetByName(string name, string language)
    {
        try {
            var result = await _productRepository.GetProducts(name.ToLower(), language);
            return Ok(result);
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetByGroup(string groupId)
    {
        try {
            return Ok(await _productRepository.GetProductsByGroup(groupId));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpGet("subgroup/{subGroupId}")]
    public async Task<IActionResult> GetBySubGroup(string subGroupId)
    {
        try {
            return Ok(await _productRepository.GetProductsBySubGroup(subGroupId));
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(ProductDto product)
    {
        try {
            await _productRepository.Create(product);
            return Ok(new { message = "Product created" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(Guid id, ProductDto product)
    {
        try {
            await _productRepository.Update(id, product);
            return Ok(new { message = "Product updated" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPost("translation")]
    public async Task<IActionResult> Post(ProductUserTranslationDTO translationDTO)
    {
        try {
            await _productRepository.CreateTranslation(translationDTO);
            return Ok(new { message = "Translation created" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpPut("translation")]
    public async Task<IActionResult> UpdateTranslation(ProductUserTranslationDTO translationDTO)
    {
        try {
            await _productRepository.UpdateTranslation(translationDTO, _userId);
            return Ok(new { message = "Translation deleted" });
        }
        catch (Exception ex) {
            return StatusCode(500, new { Message = ex.Message });
        }
    }

    [HttpDelete("translation")]
    public async Task<IActionResult> DeleteTranslation(ProductUserTranslationDTO translationDTO)
    {
        try {
            await _productRepository.DeleteTranslation(translationDTO, _userId);
            return Ok(new { message = "Translation deleted" });
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
