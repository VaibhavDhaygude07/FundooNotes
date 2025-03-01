using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Data.Entity

{
    public class Collaborator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       
        public int CollaboratorId { get; set; }
        public string Email { get; set; }
        [ForeignKey("CollaboratorNote")]
        public int NotesId { get; set; }
        [ForeignKey("CollaboratorUser")]
        public int UserId { get; set; }

    }
}
