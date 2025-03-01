using FundooNotes.Data.Entity;
using FundooNotes.Data.Models;
using Login_API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundooNotes.Data.Repositories
{
    public class LabelRepository : ILabelRepository
    {
        private readonly UserDbContext _context;

        public LabelRepository(UserDbContext context)
        {
            _context = context;
        }

        // ✅ Prevents IDENTITY_INSERT error by NOT setting LabelId manually
        public async Task<Label> CreateLabelAsync(Label label)
        {
            label.LabelId = 0; // Ensures EF treats it as a new entry
            _context.Labels.Add(label);
            await _context.SaveChangesAsync();
            return label;
        }

        public async Task<Label?> GetLabelByIdAsync(int labelId)
        {
            return await _context.Labels.FirstOrDefaultAsync(l => l.LabelId == labelId);
        }

        public async Task UpdateLabelAsync(Label label)
        {
            _context.Labels.Update(label);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLabelAsync(Label label)
        {
            _context.Labels.Remove(label);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Label>> GetAllLabelsAsync(int userId)
        {
            return await _context.Labels.Where(l => l.UserId == userId).ToListAsync();
        }

       
    }
}
