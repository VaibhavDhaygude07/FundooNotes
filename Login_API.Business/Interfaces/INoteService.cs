using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundooNotes.Data.Models;

namespace FundooNotes.Business.Interfaces
{
    public interface INoteService
    {

        Task<IEnumerable<Note>> GetAllNotes(int userId);
        Task<Note> GetNoteById(int noteId, int userId);
        Task<bool> CreateNote(Note note);
        Task<bool> UpdateNote(Note note);
        Task<bool> DeleteNote(int noteId, int userId);
    }
}

