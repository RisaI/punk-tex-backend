using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using punk_tex_backend.Utils;

namespace punk_tex_backend.Models
{
    public class User
    {
        [Key]
        public Guid? ID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        [Required]
        public string Email { get; set; }
        [Required,IgnoreDataMember]
        public string Password { get; set; }

        public static User Create(RegisterToken token) {
            var user = new User();
            
            user.FirstName = token.FirstName;
            user.LastName = token.LastName;
            user.Email = token.Email.ToLowerInvariant();
            user.Password = token.Password;

            return user;
        }
    }
}