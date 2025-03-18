using FundooNotes.Business.Services;
using FundooNotes.Data.Entity;
using FundooNotes.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Business.Interfaces
{
    public interface INoteService
    {

       
        Task<IEnumerable<Note>> GetAllNotes(int userId);
      
        Task<Note> GetNoteById(int noteId, int userId);
        Task<bool> CreateNote(Note note);
        Task<bool> UpdateNote(Note note);
        Task<bool> DeleteNote(int noteId, int userId);
        Task<bool> ToggleNoteTrash(int noteId, int userId);

        Task<bool> ToggleArchive(int noteId, int userId);
    }
}

