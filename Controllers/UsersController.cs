using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using punk_tex_backend.Models;
using punk_tex_backend.Utils;

namespace punk_tex_backend
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        ProjectContext Database;
        public UsersController(ProjectContext database)
        {
            Database = database;
        }

        [HttpGet, Authorize]
        public IActionResult Get()
        {
            var user = Database.GetUser(Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "uid").Value));
            return Ok(user);
        }
    }
}