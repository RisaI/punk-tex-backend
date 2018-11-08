using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using punk_tex_backend.Models;

namespace punk_tex_backend.Utils
{
    public class ProjectContext : DbContext
    {
        public DbSet<User>     Users     { get; set; }
        public DbSet<Project>  Projects  { get; set; }
        public DbSet<Template> Templates { get; set; }

        public ProjectContext(DbContextOptions<ProjectContext> config) : base(config) {
            this.Database.EnsureCreated();
        }

        public User Register(RegisterToken user) {
            if (user.Email == null || user.Password == null) {
                throw new Exception("Email and password are required.");
            }
            if (Users.Any(u => u.Email == user.Email.ToLowerInvariant())) {
                throw new Exception("User with this email already exists.");
            }
            
            var output = User.Create(user);
            output.ID = Guid.NewGuid();
            output.Password = Crypto.HashPassword(output.Password);
            System.Diagnostics.Debug.WriteLine(output.Email);
            
            Users.Add(output);
            SaveChanges();
            
            return output;
        }

        public Project AddProject(ProjectToken token, Guid user) {
            var output = new Project();
            output.Name = token.Name;
            output.Description = token.Description;
            output.CreatedAt = DateTime.Now;
            output.ID = Guid.NewGuid();
            output.OP = user;
            
            Projects.Add(output);
            SaveChanges();

            return output;
        }

        public User GetUser(Guid index) {
            return GetUser(u => u.ID == index);
        }

        public User GetUser(Func<User, bool> p) {
            var user = Users.Single(p);
            if (user == null) {
                throw new ArgumentOutOfRangeException("No such user.");
            }
            return user;
        }
        
        public User GetUser(string email) {
            return GetUser(u => u.Email == email.ToLowerInvariant());
        }

        public User GetUser(LoginToken token) {
            var user = GetUser(token.Email);
            if (!Crypto.CheckAgainstHash(token.Password, user.Password)) {
                throw new Exception("Invalid password.");
            }
            return user;
        }

        public Template AddTemplate(TemplateToken token, Guid? author) {
            var output = new Template() {
                ID = Guid.NewGuid(),
                Author = author,

                Title = token.Title,
                Description = token.Description,
                
                Added = DateTime.Now,
            };

            Templates.Add(output);
            SaveChanges();

            return output;
        }
    }
}