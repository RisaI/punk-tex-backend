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
    public class ProjectController : ControllerBase
    {
        ProjectContext Database;
        public ProjectController(ProjectContext database)
        {
            Database = database;
        }

        [HttpPost]
        public ActionResult Post([FromBody] ProjectToken token)
        {
            // Database.AddProject(token, )
            return null;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Test() {
            
            return Ok(User.Claims.ToList());
        }
    }
}