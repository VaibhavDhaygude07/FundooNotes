using FundooNotes.Data.Entity;
using FundooNotes.Data.Models;
using Login_API.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FundooNotes.Data.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly UserDbContext _context;
        private readonly IDatabase _redisDb;

        public NoteRepository(UserDbContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redisDb = redis.GetDatabase();
        }

        public async Task<IEnumerable<Note>> GetAllNotes(int userId)
        {
            string cacheKey = $"notes:user:{userId}";
            var cachedNotes = await _redisDb.StringGetAsync(cacheKey);

            if (!cachedNotes.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<IEnumerable<Note>>(cachedNotes);
            }

            var notes = await _context.Notes.Where(n => n.UserId == userId).ToListAsync();

            if (notes.Any())
            {
                await _redisDb.StringSetAsync(cacheKey, JsonSerializer.Serialize(notes), TimeSpan.FromMinutes(10));
            }

            return notes;
        }

        public async Task<IEnumerable<Note>> GetAllActiveNotes(int userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId && !n.isArchive && !n.IsTrashed)
                .ToListAsync();
        }

        public async Task<Note> GetNoteById(int noteId, int userId)
        {
            string cacheKey = $"note:{noteId}:user:{userId}";
            var cachedNote = await _redisDb.StringGetAsync(cacheKey);

            if (!cachedNote.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<Note>(cachedNote);
            }

            var note = await _context.Notes.FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);

            if (note != null)
            {
                await _redisDb.StringSetAsync(cacheKey, JsonSerializer.Serialize(note), TimeSpan.FromMinutes(10));
            }

            return note;
        }

        public async Task<bool> CreateNote(Note note)
        {
            _context.Notes.Add(note);
            bool isSaved = await _context.SaveChangesAsync() > 0;

            if (isSaved)
            {
                await _redisDb.KeyDeleteAsync($"notes:user:{note.UserId}");
            }

            return isSaved;
        }

        public async Task<bool> UpdateNote(Note note)
        {
            _context.Notes.Update(note);
            bool isUpdated = await _context.SaveChangesAsync() > 0;

            if (isUpdated)
            {
                await _redisDb.KeyDeleteAsync($"note:{note.NoteId}:user:{note.UserId}");
                await _redisDb.KeyDeleteAsync($"notes:user:{note.UserId}");
            }

            return isUpdated;
        }

        public async Task<bool> DeleteNote(int noteId, int userId)
        {
            var note = await GetNoteById(noteId, userId);
            if (note != null)
            {
                _context.Notes.Remove(note);
                bool isDeleted = await _context.SaveChangesAsync() > 0;

                if (isDeleted)
                {
                    await _redisDb.KeyDeleteAsync($"note:{noteId}:user:{userId}");
                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                }

                return isDeleted;
            }
            return false;
        }

        public async Task<bool> ToggleArchive(int noteId, int userId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note != null && note.UserId == userId)
            {
                note.isArchive = !note.isArchive;
                bool isUpdated = await _context.SaveChangesAsync() > 0;

                if (isUpdated)
                {
                    await _redisDb.KeyDeleteAsync($"note:{noteId}:user:{userId}");
                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                }

                return isUpdated;
            }
            return false;
        }

        public async Task<bool> ToggleNoteTrash(int noteId, int userId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note != null && note.UserId == userId)
            {
                note.IsTrashed = !note.IsTrashed;
                bool isUpdated = await _context.SaveChangesAsync() > 0;

                if (isUpdated)
                {
                    await _redisDb.KeyDeleteAsync($"note:{noteId}:user:{userId}");
                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                }

                return isUpdated;
            }
            return false;
        }
    }
}
