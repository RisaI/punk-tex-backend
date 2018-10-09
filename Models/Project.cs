using System;
using System.ComponentModel.DataAnnotations;

namespace punk_tex_backend.Models
{
    public class Project
    {
        public Guid? Identifier { get; set; }
        public Guid OP { get; set; }

        [Required]
        public string Name;
        public string Description;
        [Timestamp]
        public DateTime CreatedAt;

        
    }
}