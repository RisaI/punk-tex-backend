using System;
using System.ComponentModel.DataAnnotations;

namespace punk_tex_backend.Models
{
    public class ProjectToken
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}