using Login_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundooNotes.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FundooNotes.Data.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly UserDbContext _context;

        public NoteRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetAllNotes(int userId)
        {
            return await _context.Notes.Where(n => n.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Note>> GetAllActiveNotes(int userId)
        {
            return await _context.Notes
                .Where(n => n.UserId == userId && !n.isArchive && !n.IsTrashed)
                .ToListAsync();
        }

        public async Task<Note> GetNoteById(int noteId, int userId)
        {
            return await _context.Notes.FirstOrDefaultAsync(n => n.NoteId == noteId && n.UserId == userId);
        }

        public async Task<bool> CreateNote(Note note)
        {
            _context.Notes.Add(note);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateNote(Note note)
        {
            _context.Notes.Update(note);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteNote(int noteId, int userId)
        {
            var note = await GetNoteById(noteId, userId);
            if (note != null)
            {
                _context.Notes.Remove(note);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }





        public async Task<bool> ToggleArchive(int noteId, int userId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note != null && note.UserId == userId)
            {
                note.isArchive = !note.isArchive;
                return await _context.SaveChangesAsync() > 0;  // Return boolean
            }
            return false;
        }

        public async Task<bool> ToggleNoteTrash(int noteId, int userId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note != null && note.UserId == userId)
            {
                note.IsTrashed = !note.IsTrashed;
                return await _context.SaveChangesAsync() > 0;  // Return boolean
            }
            return false;
        }
    }

}
