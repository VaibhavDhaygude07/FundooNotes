using Login_API.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FundooNotes.Data.Entity

{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key Relationship with User
        [ForeignKey("UserId")]
        public User User { get; set; }

        public bool IsDeleted { get; set; } = false;
    
        public bool isArchive { get; set; }
        public bool IsTrashed { get; internal set; }
    }
}