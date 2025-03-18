using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Data.Models
{
    public class NoteModel
    {

        [Required]
        [MaxLength(200)]
        public string title { get; set; }

        [Required]
        public string content { get; set; }

        public string Color { get; set; }



    }
}
