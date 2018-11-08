using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public IActionResult GetAll([FromRoute] Guid id) {
            Template template;
            try {
                template = Database.Templates.First(t => t.ID == id);
            } catch {
                return NotFound();
            }
            return Ok(template);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] TemplateToken token) {
            using (Stream stream = token.File.OpenReadStream())
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read)) {
                if (archive.GetEntry("main.tex") == null)
                    return BadRequest("main.tex file missing");

                Template template;
                try {
                    template = Database.AddTemplate(token, null);
                } catch (Exception ex) {
                    return BadRequest(ex.Message);
                }

                try {
                    var path = Templates.CreateDirectory(template);
                    foreach (var entry in archive.Entries) {
                        entry.ExtractToFile(Path.Combine(path, entry.FullName));
                    }
                } catch (Exception ex){
                    Templates.RemoveTemplate(template);
                    Database.Templates.Remove(template);
                    return BadRequest(ex.Message);
                }
                return Ok(template);
            }
        }
    }
}