using FundooNotes.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundooNotes.Data.Models;
using FundooNotes.Data.Repositories;

namespace FundooNotes.Business.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _notesRepository;

        public NoteService(INoteRepository notesRepository)
        {
            _notesRepository = notesRepository;
        }
        public async Task<IEnumerable<Note>> GetAllActiveNotes(int userId)
        {
            return await _notesRepository.GetAllActiveNotes(userId);
        }



        public async Task<IEnumerable<Note>> GetAllNotes(int userId) => await _notesRepository.GetAllNotes(userId);

        public async Task<Note> GetNoteById(int noteId, int userId) => await _notesRepository.GetNoteById(noteId, userId);

        public async Task<bool> CreateNote(Note note) => await _notesRepository.CreateNote(note);

        public async Task<bool> UpdateNote(Note note) => await _notesRepository.UpdateNote(note);

        public async Task<bool> DeleteNote(int noteId, int userId) => await _notesRepository.DeleteNote(noteId, userId);

        public async Task<bool>ToggleNoteTrash(int noteId, int userId) => await _notesRepository.ToggleNoteTrash(noteId, userId);

        public async Task<bool> ToggleArchive(int noteId, int userId) => await _notesRepository.ToggleArchive(noteId, userId);
        
    }
}
