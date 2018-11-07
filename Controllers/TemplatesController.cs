using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using punk_tex_backend.Models;
using punk_tex_backend.Utils;

namespace punk_tex_backend
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplatesController : ControllerBase
    {
        ProjectContext Database;
        public TemplatesController(ProjectContext database)
        {
            Database = database;
        }

        [HttpGet]
        public IActionResult GetAll() {
            return Ok(Database.Templates.ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAll([FromRoute] Guid id) {
            Template template;
            try {
                template = Database.Templates.First(t => t.ID == id);
            } catch {
                return NotFound();
            }
            return Ok(template);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TemplateToken token) {
            Template template;
            try {
                template = Database.AddTemplate(token, null);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
            return Ok(template);
        }
    }
}