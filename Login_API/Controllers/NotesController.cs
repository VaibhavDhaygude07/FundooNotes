using FundooNotes.API.Controllers;
using FundooNotes.Business.Interfaces;
using FundooNotes.Data.Models;
using Login_API.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FundooNotes.Data.Entity;
using StackExchange.Redis;
using System.Text.Json;

namespace FundooNotes.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")] // Explicitly set JWT authentication
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly IDatabase _redisDb;

        public NotesController(INoteService noteService, IConnectionMultiplexer redis)
        {
            _noteService = noteService;
            _redisDb = redis.GetDatabase();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            string cacheKey = $"notes:user:{UserId}";

            var cachedNotes = await _redisDb.StringGetAsync(cacheKey);
            if (!cachedNotes.IsNullOrEmpty)
            {
                return Ok(JsonSerializer.Deserialize<IEnumerable<Note>>(cachedNotes));
            }

            var notes = await _noteService.GetAllActiveNotes(UserId);
            if (notes.Any())
            {
                await _redisDb.StringSetAsync(cacheKey, JsonSerializer.Serialize(notes), TimeSpan.FromMinutes(10));
            }

            return Ok(notes);
        }

        [HttpGet("{noteId}")]
        public async Task<IActionResult> GetNoteById(int noteId)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            string cacheKey = $"note:{noteId}:user:{UserId}";

            var cachedNote = await _redisDb.StringGetAsync(cacheKey);
            if (!cachedNote.IsNullOrEmpty)
            {
                return Ok(JsonSerializer.Deserialize<Note>(cachedNote));
            }

            var note = await _noteService.GetNoteById(noteId, UserId);
            if (note == null) return NotFound("Note not found");

            await _redisDb.StringSetAsync(cacheKey, JsonSerializer.Serialize(note), TimeSpan.FromMinutes(10));

            return Ok(note);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] NoteModel model)
        {
            Note note = new Note
            {
                Title = model.Title,
                Content = model.Content,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            };

            var result = await _noteService.CreateNote(note);
            if (result)
            {
                await _redisDb.KeyDeleteAsync($"notes:user:{note.UserId}");
                return Ok("Note created successfully!");
            }

            return BadRequest("Failed to create note");
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
            if (result)
            {
                await _redisDb.KeyDeleteAsync($"note:{noteId}:user:{UserId}");
                await _redisDb.KeyDeleteAsync($"notes:user:{UserId}");
                return Ok("Note updated successfully!");
            }

            return BadRequest("Failed to update note");
        }

        [HttpDelete("{noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _noteService.DeleteNote(noteId, UserId);

            if (result)
            {
                await _redisDb.KeyDeleteAsync($"note:{noteId}:user:{UserId}");
                await _redisDb.KeyDeleteAsync($"notes:user:{UserId}");
                return Ok("Note deleted successfully!");
            }

            return BadRequest("Failed to delete note");
        }

        [HttpPut("{noteId}/Archive")]
        public async Task<IActionResult> ToggleArchive(int noteId)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var note = await _noteService.GetNoteById(noteId, UserId);

            if (note == null) return NotFound("Note not found");
            if (note.IsDeleted) return BadRequest("Cannot archive: Note is in Trash");

            var result = await _noteService.ToggleArchive(noteId, UserId);
            if (result)
            {
                await _redisDb.KeyDeleteAsync($"note:{noteId}:user:{UserId}");
                await _redisDb.KeyDeleteAsync($"notes:user:{UserId}");
                note = await _noteService.GetNoteById(noteId, UserId);
                return Ok(new { success = true, Message = "Note Archive Toggled Successfully", Data = $"Note Archived Status: {note.isArchive}" });
            }

            return BadRequest("Failed to toggle archive status");
        }

        [HttpPut("{noteId}/Trash")]
        public async Task<IActionResult> ToggleNoteTrash(int noteId)
        {
            var UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var note = await _noteService.GetNoteById(noteId, UserId);

            if (note == null) return NotFound("Note not found");

            var result = await _noteService.ToggleNoteTrash(noteId, UserId);
            if (result)
            {
                await _redisDb.KeyDeleteAsync($"note:{noteId}:user:{UserId}");
                await _redisDb.KeyDeleteAsync($"notes:user:{UserId}");
                note = await _noteService.GetNoteById(noteId, UserId);
                return Ok(new { success = true, Message = "Note Trash Toggled Successfully", Data = $"Note Trash Status: {note.IsTrashed}" });
            }

            return BadRequest("Failed to toggle trash status");
        }
    }
}
