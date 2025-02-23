using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FundooNotes.Data.Models;
using FundooNotes.Business.Interfaces;

namespace FundooNotes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        // Helper function to get UserId from JWT token
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? User.FindFirst("sub")?.Value;


            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID claim is missing from the token.");
            }

            return int.Parse(userIdClaim);
        }

        /// <summary>
        /// Get all notes for the logged-in user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            var notes = await _noteService.GetAllNotes(GetUserId());
            return Ok(notes);
        }

        /// <summary>
        /// Get a specific note by ID
        /// </summary>
        [HttpGet("{noteId}")]
        public async Task<IActionResult> GetNoteById(int noteId)
        {
            var note = await _noteService.GetNoteById(noteId, GetUserId());
            if (note == null) return NotFound("Note not found");
            return Ok(note);
        }

        /// <summary>
        /// Create a new note
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] Note note)
        {
            note.UserId = GetUserId();
            var result = await _noteService.CreateNote(note);
            return result ? Ok("Note created successfully!") : BadRequest("Failed to create note");
        }

        /// <summary>
        /// Update an existing note
        /// </summary>
        [HttpPut("{noteId}")]
        public async Task<IActionResult> UpdateNote(int noteId, [FromBody] Note note)
        {
            note.NoteId = noteId;
            note.UserId = GetUserId();
            var result = await _noteService.UpdateNote(note);
            return result ? Ok("Note updated successfully!") : BadRequest("Failed to update note");
        }

        /// <summary>
        /// Delete a note by ID
        /// </summary>
        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            var result = await _noteService.DeleteNote(noteId, GetUserId());
            return result ? Ok("Note deleted successfully!") : BadRequest("Failed to delete note");
        }
    }
}
