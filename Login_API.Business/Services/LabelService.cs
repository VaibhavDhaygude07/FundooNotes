using FundooNotes.Business.Interfaces;
using FundooNotes.Data.Entity;
using FundooNotes.Data.Models;
using FundooNotes.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Business.Services
{
    public class LabelService : ILabelService
    {
        private readonly ILabelRepository _labelRepository;

        public LabelService(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public async Task<Label> CreateLabelAsync(int userId, string name)
        {
            var label = new Label { UserId = userId, Name = name };
            return await _labelRepository.CreateLabelAsync(label);
        }

        public async Task<Label?> GetLabelByIdAsync(int labelId)
        {
            return await _labelRepository.GetLabelByIdAsync(labelId);
        }

        public async Task UpdateLabelAsync(Label label)
        {
            await _labelRepository.UpdateLabelAsync(label);
        }

        public async Task DeleteLabelAsync(Label label)
        {
            await _labelRepository.DeleteLabelAsync(label);
        }

        public async Task<IEnumerable<Label>> GetAllLabelsAsync(int userId)
        {
            return await _labelRepository.GetAllLabelsAsync(userId);
        }

       
    }
}
