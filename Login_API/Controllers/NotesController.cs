using FundooNotes.API.Controllers;
using FundooNotes.Business.Interfaces;
using FundooNotes.Data.Models;
using Login_API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace FundooNotes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")] // Explicitly set JWT authentication
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var notes = await _noteService.GetAllActiveNotes(UserId);//GetAllNotes(UserId);
            return Ok(notes);
        }

        [HttpGet("{noteId}")]
        public async Task<IActionResult> GetNoteById(int noteId)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var note = await _noteService.GetNoteById(noteId, UserId);
            if (note == null) return NotFound("Note not found");
            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] NoteModel model)
        {
            Note note = new Note();
            note.Title = model.Title;
            note.Content = model.Content;
            note.UpdatedAt = DateTime.UtcNow;
            note.CreatedAt = DateTime.UtcNow;
            note.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _noteService.CreateNote(note);
            return result ? Ok("Note created successfully!") : BadRequest("Failed to create note");
        }

        [HttpPut("{noteId}")]
        public async Task<IActionResult> UpdateNote(int noteId, [FromBody] NoteModel model)
        {
           
            int UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            Note existingNote = await _noteService.GetNoteById(noteId, UserId);
            if (existingNote == null) return NotFound("Note not found");
            existingNote.Title = model.Title;
            existingNote.Content = model.Content;
            existingNote.UpdatedAt = DateTime.UtcNow;
            var result = await _noteService.UpdateNote(existingNote);
            return result ? Ok("Note updated successfully!") : BadRequest("Failed to update note");
        }

        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _noteService.DeleteNote(noteId, UserId);
            return result ? Ok("Note deleted successfully!") : BadRequest("Failed to delete note");
        }

        [HttpPut("Archive/{noteId}")]
        public async Task<IActionResult> ToggleArchive(int noteId)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var note = await _noteService.GetNoteById(noteId, UserId);
            if (note == null) return NotFound("Note not found");

            if (note.IsDeleted)
            {
                return BadRequest(new { success = false, Message = "Note cannot be Archived: Note is in Trash" });
            }

            var result = await _noteService.ToggleArchive(noteId, UserId);
            if (!result) return BadRequest("Failed to toggle archive status");

            note = await _noteService.GetNoteById(noteId, UserId);
            return Ok(new { success = true, Message = "Note Archive Toggled Successfully", Data = $"Note Archived Status: {note.isArchive}" });
        }

        [HttpPut("Trash/{noteId}")]
       public async Task<IActionResult> ToggleNoteTrash(int noteId)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var note = await _noteService.GetNoteById(noteId, UserId);
            if (note == null) return NotFound("Note not found");
            var result = await _noteService.ToggleNoteTrash(noteId, UserId);
            if (!result) return BadRequest("Failed to toggle trash status");
            note = await _noteService.GetNoteById(noteId, UserId);
            return Ok(new { success = true, Message = "Note Trash Toggled Successfully", Data = $"Note Trash Status: {note.IsTrashed}" });
        }




    }
   
}

