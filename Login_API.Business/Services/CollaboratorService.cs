using FundooNotes.Data.Entity;
using FundooNotes.Data.Repositories;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Business.Services
{
    public class CollaboratorService
    {
        private readonly ICollaboratorRepository repository;
        public CollaboratorService(ICollaboratorRepository repository)
        {
            this.repository = repository;
        }
        public async Task<Collaborator> CreateCollaboratorAsync(int UserId, CollaboratorModel model)
        {
            return await repository.CreateCollaboratorAsync(UserId, model);
        }
        public async Task<List<Collaborator>> GetCollaboratorsAsync(int UserId)
        {
            return await repository.GetCollaboratorsAsync(UserId);
        }
        public async Task<bool> DeleteCollaboratorAsync(int UserId, int CollaboratorId)
        {
            return await repository.DeleteCollaboratorAsync(UserId, CollaboratorId);
        }
    }
}
