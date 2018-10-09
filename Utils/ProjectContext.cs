using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using punk_tex_backend.Models;

namespace punk_tex_backend.Utils
{
    public class ProjectContext : DbContext
    {
        public DbSet<User>    Users    { get; set; }
        public DbSet<Project> Projects { get; set; }


        public User Register(RegisterToken user) {
            if (user.Email == null || user.Password == null) {
                throw new Exception("Email and password are required.");
            }
            if (Users.Any(u => u.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase))) {
                throw new Exception("User with this email already exists.");
            }

            var output = new User(user);
            output.Identifier = Guid.NewGuid();
            output.Password = Crypto.HashPassword(output.Password);
            Users.Add(output);
            return output.Clone();
        }

        public Project AddProject(ProjectToken token, Guid user) {
            var output = new Project();
            output.Name = token.Name;
            output.Description = token.Description;
            output.CreatedAt = DateTime.Now;
            output.Identifier = Guid.NewGuid();
            output.OP = user;
            
            Projects.Add(output);
            return output;
        }

        public User GetUser(Guid index) {
            return GetUser(u => u.Identifier == index);
        }

        public User GetUser(Predicate<User> p) {
            var user = Users.Find(p);
            if (user == null) {
                throw new ArgumentOutOfRangeException("No such user.");
            }
            return user.Clone();
        }
        
        public User GetUser(string email) {
            return GetUser(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
        }

        public User GetUser(LoginToken token) {
            var user = GetUser(token.Email);
            if (!Crypto.CheckAgainstHash(token.Password, user.Password)) {
                throw new Exception("Invalid password.");
            }
            return user;
        }
    }
}