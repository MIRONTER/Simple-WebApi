using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using WebApi.Models;
using WebApi.Tools;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly InfotecsContext _context;

        public ValuesController(InfotecsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostValue(IFormFile file)
        {
            ToolsForSQL tools = new ToolsForSQL(_context);

            Result? result = await _context.Results.FindAsync(file.FileName);
            bool needNewResult = await tools.CheckResult(result!, file.FileName);

            if (tools.AddValues(file))
            {
                if (needNewResult)
                {
                    result = new Result
                    {
                        FileName = file.FileName,
                        MinDate = DateTime.Now
                    };
                    _context.Results.Add(result);
                }
                await _context.SaveChangesAsync();
                tools.UpdateResult(result!, file.FileName);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else { return BadRequest(); }
        }

        [HttpGet("{fileName}")]
        public async Task<ActionResult<IEnumerable<Value>>> GetValues(string fileName)
        {
            return await _context.Values.Where(v => v.FileName == fileName).ToListAsync();
        }
    }
}
