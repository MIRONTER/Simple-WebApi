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

        // GET: api/Values
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Value>>> GetValues()
        //{
        //    return await _context.Values.ToListAsync();
        //}

        // GET: api/Values/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Value>> GetValue(int id)
        //{
        //    var value = await _context.Values.FindAsync(id);

        //    if (value == null)
        //    {
        //        return NotFound();
        //    }

        //    return value;
        //}



        // PUT: api/Values/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutValue(int id, Value value)
        //{
        //    if (id != value.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(value).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ValueExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Values
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Value>> PostValue(Value value)
        //{
        //    _context.Values.Add(value);
        //    await _context.SaveChangesAsync();

        //    //return CreatedAtAction("GetValue", new { id = value.Id }, value);
        //    return CreatedAtAction(nameof(GetValue), new { id = value.Id }, value);
        //}



        [HttpPost]
        public async Task<IActionResult> PostValue(IFormFile file)
        {
            ToolsForSQL tools = new ToolsForSQL(_context);

            var result = await _context.Results.FindAsync(file.FileName);
            var needNewResult = await tools.CheckResult(result, file.FileName);

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
                tools.UpdateResult(result, file.FileName);
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

        // DELETE: api/Values/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteValue(int id)
        //{
        //    var value = await _context.Values.FindAsync(id);
        //    if (value == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Values.Remove(value);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool ValueExists(int id)
        //{
        //    return _context.Values.Any(e => e.Id == id);
        //}
    }
}
