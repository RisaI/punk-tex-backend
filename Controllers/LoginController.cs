using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using punk_tex_backend.Models;
using punk_tex_backend.Utils;

namespace punk_tex_backend.Routes
{
    [ApiController]
    [Route("api")]
    public class LoginController : ControllerBase
    {
        ProjectContext Database;
        public LoginController(ProjectContext database) {
            Database = database;
        }

        [HttpPost("login")]
        public User Login([FromBody] LoginToken login) {
            var user = Database.GetUser(login);
            return user;
        }

        [HttpPost("register")]
        public User Register([FromBody] RegisterToken user) {
            return Database.Register(user);
        }
    }
}