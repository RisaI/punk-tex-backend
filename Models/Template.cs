using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace punk_tex_backend.Models
{
    public class Template
    {
        [Key]
        public Guid? ID { get; set; }
        public Guid? Author { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [Timestamp]
        public DateTime Added { get; set; }
    }

    public class TemplateToken
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}