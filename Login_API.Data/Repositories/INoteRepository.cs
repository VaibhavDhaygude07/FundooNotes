using FundooNotes.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FundooNotes.Data.Repositories
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllNotes(int userId);
        Task<Note> GetNoteById(int noteId, int userId);
        Task<bool> CreateNote(Note note);
        Task<bool> UpdateNote(Note note);
        Task<bool> DeleteNote(int noteId, int userId);
    }
}
