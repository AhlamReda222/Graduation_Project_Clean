using Graduation_Project.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_Project.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "BrandOwnerOnly")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        // الأونر يشوف العقد
        [HttpGet]
        public IActionResult GetContract()
        {
            var result = _contractService.GetContract();
            return Ok(result.Data);
        }

        // الأونر يشوف هو وافق ولا لأ
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _contractService.GetContractStatusAsync(userId);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        // الأونر يوافق على العقد
        [HttpPost("accept")]
        public async Task<IActionResult> Accept()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _contractService.AcceptContractAsync(userId);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = result.Message });
        }
    }
}