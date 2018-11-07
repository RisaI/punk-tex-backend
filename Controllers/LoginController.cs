using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        public IActionResult Login([FromBody] LoginToken login) {
            User user;
            try {
                user = Database.GetUser(login);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
            
            return Ok(new {
                user = user,
                token = JWT.Create(user).Serialize()
            });
        }

        [HttpGet("login"), Authorize]
        public IActionResult Login() {
            var user = Database.GetUser(Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "uid").Value));
            return Ok(new {
                user = user,
                token = JWT.Create(user).Serialize(),
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterToken user) {
            try {
                return Ok(Database.Register(user));
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }
    }
}