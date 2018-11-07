using System;
using System.ComponentModel.DataAnnotations;

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

        [Required]
        public string Body { get; set; }
    }

    public class TemplateToken
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        [Required]
        public string Body { get; set; }
    }
}