using FundooNotes.Data.Entity;
using FundooNotes.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Data.Repositories
{
    public interface ILabelRepository
    {
        Task<Label> CreateLabelAsync(Label label);
        Task<Label?> GetLabelByIdAsync(int labelId);
        Task UpdateLabelAsync(Label label);
        Task DeleteLabelAsync(Label label);
        Task<IEnumerable<Label>> GetAllLabelsAsync(int userId);



     
    }
}
