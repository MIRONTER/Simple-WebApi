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
using WebApi.DataAccess.Context;
using WebApi.DataAccess.Models;
using WebApi.DataAccess.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly WebApiContext _context;

        public ValuesController(WebApiContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostValue(IFormFile file)
        {
            DataManager tools = new DataManager(_context);

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
