using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace punk_tex_backend.Models
{
    public class LoginToken
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}