using Graduation_Project.BLL.DTOs.Product;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
 
namespace Graduation_Project.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductDescriptionService _descriptionService;
 
        public ProductsController(
            IProductService productService,
            IProductDescriptionService descriptionService)
        {
            _productService     = productService;
            _descriptionService = descriptionService;
        }
 
        // ✅ Public - Get all approved products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productService.GetAllApprovedProductsAsync();
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }
 
        // ✅ Public - Get product by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.Succeeded) return NotFound(result.Errors);
            return Ok(result.Data);
        }
 
        // ✅ Public - Get products by brand
        [HttpGet("brand/{brandId:int}")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            var result = await _productService.GetProductsByBrandAsync(brandId);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }
 
        // ✅ Public - Get printing techniques
        [HttpGet("printing-techniques")]
        public async Task<IActionResult> GetPrintingTechniques()
        {
            var result = await _productService.GetPrintingTechniquesAsync();
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }
 
        // ✅ Owner - Get my products
        [HttpGet("my-products")]
        [Authorize(Policy = "BrandOwnerOnly")]
        public async Task<IActionResult> GetMyProducts()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _productService.GetOwnerProductsAsync(userId);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }
 
        // ✅ Admin - Get pending products
        [HttpGet("pending")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _productService.GetPendingProductsAsync();
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(result.Data);
        }
 
        // ✅ Owner - Create product
        [HttpPost("brand/{brandId:int}")]
        [Authorize(Policy = "BrandOwnerOnly")]
        public async Task<IActionResult> Create(int brandId, [FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
 
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _productService.CreateProductAsync(userId, brandId, dto);
 
            if (result == null)
                return StatusCode(500, new { message = "Unexpected null result" });
 
            if (!result.Succeeded)
                return BadRequest(new { succeeded = false, errors = result.Errors, message = result.Message });
 
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.ProductId }, new
            {
                succeeded = true,
                message   = result.Message,
                data      = result.Data
            });
        }
 
        // ✅ Owner - Update product
        [HttpPut("{id:int}")]
        [Authorize(Policy = "BrandOwnerOnly")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
 
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _productService.UpdateProductAsync(id, userId, dto);
 
            if (!result.Succeeded)
                return BadRequest(new { succeeded = false, errors = result.Errors });
 
            return Ok(new { succeeded = true, data = result.Data });
        }
 
        // ✅ Owner - Delete product
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "BrandOwnerOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _productService.DeleteProductAsync(id, userId);
 
            if (!result.Succeeded)
                return BadRequest(new { succeeded = false, errors = result.Errors });
 
            return Ok(new { succeeded = true, message = result.Message });
        }
 
        // ✅ Owner - Generate description with AI
        [HttpPost("generate-description")]
        [Authorize(Policy = "BrandOwnerOnly")]
        public async Task<IActionResult> GenerateDescription([FromBody] GenerateDescriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
 
            var result = await _descriptionService.GenerateDescriptionAsync(dto);
 
            if (!result.Succeeded)
                return BadRequest(new { succeeded = false, errors = result.Errors });
 
            return Ok(new { succeeded = true, suggestion = result.Data });
        }
    }
}