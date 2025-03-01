using FundooNotes.Data.Entity;
using FundooNotes.Data.Models;
using FundooNotes.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;

namespace FundooNotes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaboratorController : ControllerBase
    {
        private readonly ICollaboratorRepository manager;
        public CollaboratorController(ICollaboratorRepository manager)
        {
            this.manager = manager;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCollab(CollaboratorModel model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { Success = false, Message = "Invalid input data" });

                // Validate UserId from claims
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    return Unauthorized(new { Success = false, Message = "User ID claim is missing" });
                }

                int UserId = int.Parse(userIdClaim.Value);
                var addCollaborator = await manager.CreateCollaboratorAsync(UserId, model);

                return Ok(new { Success = true, Message = "Add collaborator", Data = addCollaborator });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Validate UserId from claims
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    return Unauthorized(new { Success = false, Message = "User ID claim is missing" });
                }
                int UserId = int.Parse(userIdClaim.Value);
                var collaborators = await manager.GetCollaboratorsAsync(UserId);
                return Ok(new { Success = true, Message = "Get all collaborators", Data = collaborators });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteCollaborator(int CollaboratorId)
        {
           try
            {
                // Validate UserId from claims
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    return Unauthorized(new { Success = false, Message = "User ID claim is missing" });
                }
                int UserId = int.Parse(userIdClaim.Value);
                var deleteCollaborator = await manager.DeleteCollaboratorAsync(UserId, CollaboratorId);
                return Ok(new { Success = true, Message = "Delete collaborator", Data = deleteCollaborator });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }
}
