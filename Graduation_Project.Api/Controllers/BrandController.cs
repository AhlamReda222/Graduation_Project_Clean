using Graduation_Project.BLL.DTOs.Brand;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // الكل يشوف الـ Brands
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _brandService.GetAllBrandsAsync();
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _brandService.GetBrandByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        // Owner يشوف Brands بتاعته
        [HttpGet("my-brands")]
        [Authorize(Policy = "BrandOwnerOnly")]
        public async Task<IActionResult> GetMyBrands()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _brandService.GetBrandsByOwnerAsync(userId);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }


        // Owner يعدل Brand بتاعته
        [HttpPut("{id}")]
        [Authorize(Policy = "BrandOwnerOnly")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _brandService.UpdateBrandAsync(id, userId, dto);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        // Owner يحذف Brand بتاعته
        [HttpDelete("{id}")]
        [Authorize(Policy = "BrandOwnerOnly")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _brandService.DeleteBrandAsync(id, userId);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = result.Message });
        }

        // Admin فقط - يفعل أو يوقف Brand
        [HttpPatch("{id}/toggle-status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _brandService.ToggleBrandStatusAsync(id, adminId);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = result.Message });
        }
    }
}