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
        private readonly ToolsForSQL _toolsForSQL = new ToolsForSQL();

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

        private Value ParseValue(string filename, string[] values)
        {
            Value value = new Value
            {
                FileName = filename,
                Date = DateTime.ParseExact(values[0], "yyyy-MM-dd_HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture),
                Seconds = Convert.ToInt32(values[1]),
                N = Convert.ToDouble(values[2])
            };
            return value;
        }

        private void UpdateResult(Result result, string filename)
        {
            result.AverageSeconds = _context.Values.Where(v => v.FileName == filename).Average(s => s.Seconds);
            result.AverageN = _context.Values.Where(v => v.FileName == filename).Average(n => n.N);
            result.MedianN = _toolsForSQL.Median(_context.Values.Where(v => v.FileName == filename).Select(n => n.N));
            result.MaxN = _context.Values.Where(v => v.FileName == filename).Max(n => n.N);
            result.MinN = _context.Values.Where(v => v.FileName == filename).Min(n => n.N);
            result.Count = _context.Values.Where(v => v.FileName == filename).Count();
            result.MinDate = _context.Values.Where(v => v.FileName == filename).Min(d => d.Date);
            result.AllTime = (_context.Values.Where(v => v.FileName == filename).Max(d => d.Date) - result.MinDate).Ticks;
        }

        private bool AddValues(IFormFile file)
        {
            int lines = 0;
            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines++;
                    string[] values = line.Split(';');
                    if (values.Length > 0)
                    {
                        Value value = ParseValue(file.FileName, values);
                        if (value.Date < DateTime.Now && value.Date > Convert.ToDateTime("01.01.2000") && value.N >= 0)
                        {
                            _context.Values.Add(value);
                        }
                    }
                }
            }
            if (lines > 0 && lines <= 10000) { return true; }
            else { return false; }
        }

        private async Task<bool> CheckResult(Result result, string filename)
        {
            if (result == null)
            {
                result = new Result
                {
                    FileName = filename,
                    MinDate = DateTime.Now
                };
                return true;
            }
            else
            {
                List<Value> values = await _context.Values.Where(v => v.FileName == filename).ToListAsync();
                _context.Values.RemoveRange(values);
                return false;
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostValue(IFormFile file)
        {
            var result = await _context.Results.FindAsync(file.FileName);
            var newResult = await CheckResult(result, file.FileName);

            if (AddValues(file))
            {
                if (newResult) { _context.Results.Add(result); }
                await _context.SaveChangesAsync();
                UpdateResult(result, file.FileName);
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
