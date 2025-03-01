using FundooNotes.Data.Entity;
using FundooNotes.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Business.Interfaces
{
    public interface ILabelService
    {
        Task<Label> CreateLabelAsync(int userId, string name);
        Task<Label?> GetLabelByIdAsync(int labelId);
        Task UpdateLabelAsync(Label label);
        Task DeleteLabelAsync(Label label);
        Task<IEnumerable<Label>> GetAllLabelsAsync(int userId);
      

    }
}
