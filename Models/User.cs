using System;
using System.ComponentModel.DataAnnotations;
using punk_tex_backend.Utils;

namespace punk_tex_backend.Models
{
    public class User
    {
        public Guid? Identifier {
            get;
            set;
        }

        public string FirstName, LastName;
        public string Email;
        internal string Password;

        public User(RegisterToken token) {
            FirstName = token.FirstName;
            LastName = token.LastName;
            Email = token.Email;
            Password = token.Password;
        }

        public User Clone() {
            return this.MemberwiseClone() as User;
        }
    }
}