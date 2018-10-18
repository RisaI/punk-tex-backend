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
            
            var claims = new Claim[] {
                new Claim("uid", user.ID.ToString())
            };
            
            var token = new JwtSecurityToken(
                null, null, // ISSUER and AUDIENCE not needed yet
                claims, 
                null, DateTime.Now.AddHours(2),
                new SigningCredentials(new SymmetricSecurityKey(Config.JWTSecret), SecurityAlgorithms.HmacSha256)
            );
            
            return Ok(new {
                user = user,
                token = (new JwtSecurityTokenHandler()).WriteToken(token),
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