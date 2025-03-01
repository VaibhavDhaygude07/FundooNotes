using FundooNotes.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FundooNotes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LabelController : ControllerBase
    {
        private readonly ILabelService _labelService;

        public LabelController(ILabelService labelService)
        {
            _labelService = labelService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLabel([FromBody] string name)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            var label = await _labelService.CreateLabelAsync(userId, name);

            return CreatedAtAction(nameof(GetLabelById), new { labelId = label.LabelId }, label);
        }

        [HttpGet("{labelId}")]
        public async Task<IActionResult> GetLabelById(int labelId)
        {
            var label = await _labelService.GetLabelByIdAsync(labelId);
            if (label == null) return NotFound();

            return Ok(label);
        }

        [HttpPut("{labelId}")]
        public async Task<IActionResult> UpdateLabel(int labelId, [FromBody] string name)
        {
            var label = await _labelService.GetLabelByIdAsync(labelId);
            if (label == null) return NotFound();
            label.Name = name;
            await _labelService.UpdateLabelAsync(label);
            return NoContent();
        }

        [HttpDelete("{labelId}")]
        public async Task<IActionResult> DeleteLabel(int labelId)
        {
            var label = await _labelService.GetLabelByIdAsync(labelId);
            if (label == null) return NotFound();
            await _labelService.DeleteLabelAsync(label);
            return NoContent();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllLabels()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);
            var labels = await _labelService.GetAllLabelsAsync(userId);

            return Ok(labels);
        }

       






    }
}
