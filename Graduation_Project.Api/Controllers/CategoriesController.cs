using Graduation_Project.BLL.DTOs.Category;
using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (!result.Succeeded)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }

        [HttpGet("{id}/stats")]
        public async Task<IActionResult> GetCategoryStats(int id)
        {
            var result = await _categoryService.GetCategoryStatsAsync(id);
            if (!result.Succeeded)
                return NotFound(result.Errors);

            return Ok(result.Data);
        }
[HttpPost]
[Authorize(Policy = "AdminOnly")]
public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _categoryService.CreateCategoryAsync(dto, adminId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetCategoryById), new { id = result.Data.CategoryId }, result.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _categoryService.UpdateCategoryAsync(id, dto, adminId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _categoryService.DeleteCategoryAsync(id, adminId);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = result.Message });
        }
        [Authorize]
[HttpGet("check")]
public IActionResult Check()
{
    return Ok("Authenticated");
}
    }
}