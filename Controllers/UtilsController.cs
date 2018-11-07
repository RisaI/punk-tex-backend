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
    public class UtilsController : ControllerBase
    {
        [HttpPost("latex")]
        public async Task<IActionResult> MdToLatex()
        {
            try {
                using (var reader = new System.IO.StreamReader(Request.Body))
                using (var writer = new System.IO.StringWriter()) {
                    var document = await reader.ReadToEndAsync();
                    Markdig.Markdown.Parse(document).ToLatex(writer);
                    return Ok(writer.ToString());
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("compile/{id?}")]
        public async Task<IActionResult> Compile([FromRoute] Guid? id)
        {
            try {
                using (var reader = new System.IO.StreamReader(Request.Body)) {
                    var mem = await Latex.Compile(await reader.ReadToEndAsync());
                    return File(mem, "application/pdf");
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("mdtopdf")]
        public async Task<IActionResult> MdToPdf() {
            try {
                using (var reader = new System.IO.StreamReader(Request.Body))
                using (var writer = new System.IO.StringWriter()) {
                    Markdig.Markdown.Parse(await reader.ReadToEndAsync()).ToLatex(writer);
                    return File(await Latex.Compile(writer.ToString()), "application/pdf");
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}