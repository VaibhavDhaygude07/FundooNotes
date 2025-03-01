using FundooNotes.Data.Entity;
using FundooNotes.Data.Models;
using Login_API.Data;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundooNotes.Data.Repositories
{
    public class CollaboratorRepository : ICollaboratorRepository
    {
        private readonly UserDbContext _context;

        public CollaboratorRepository(UserDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Add a new collaborator.
        /// </summary>
        public async Task<Collaborator> CreateCollaboratorAsync(int userId, CollaboratorModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var collaborator = new Collaborator
            {
                UserId = userId,
                NotesId = model.NotesId,
                Email = model.Email
            };

            await _context.Collaborators.AddAsync(collaborator);
            await _context.SaveChangesAsync();
            return collaborator;
        }

        /// <summary>
        /// Get all collaborators for a specific user.
        /// </summary>
        public async Task<List<Collaborator>> GetCollaboratorsAsync(int userId)
        {
            return await _context.Collaborators
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        /// <summary>
        /// Remove a collaborator.
        /// </summary>
        public async Task<bool> DeleteCollaboratorAsync(int userId, int collaboratorId)
        {
            var collaborator = await _context.Collaborators
                .FirstOrDefaultAsync(c => c.UserId == userId && c.UserId == collaboratorId);

            if (collaborator == null) return false;

            _context.Collaborators.Remove(collaborator);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
