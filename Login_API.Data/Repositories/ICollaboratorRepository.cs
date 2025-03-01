using System.Collections.Generic;
using System.Threading.Tasks;
using FundooNotes.Data.Entity;
using ModelLayer.Models;

namespace FundooNotes.Data.Repositories
{
    public interface ICollaboratorRepository
    {
        Task<Collaborator> CreateCollaboratorAsync(int userId, CollaboratorModel model);
        Task<List<Collaborator>> GetCollaboratorsAsync(int userId);
        Task<bool> DeleteCollaboratorAsync(int userId, int collaboratorId);
    }
}
