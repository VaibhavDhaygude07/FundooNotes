using FundooNotes.Data.Entity;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Business.Interfaces
{
    public interface ICollaboratorService
    {
        public Collaborator CreateCollaborator(int UserId, CollaboratorModel model);
        public List<Collaborator> GetCollaborators(int UserId);
        public bool DeleteCollaborator(int UserId, int CollaboratorId);
    }
}
