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
    public class ProjectsController : ControllerBase
    {
        ProjectContext Database;
        public ProjectsController(ProjectContext database)
        {
            Database = database;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectToken token)
        {
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Test() {
            
            return Ok(User.Claims.ToList());
        }
    }
}