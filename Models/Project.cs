using System;
using System.ComponentModel.DataAnnotations;

namespace punk_tex_backend.Models
{
    public class Project
    {
        [Key]
        public Guid? ID { get; set; }
        public Guid OP { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Timestamp]
        public DateTime CreatedAt { get; set; }

        
    }
}