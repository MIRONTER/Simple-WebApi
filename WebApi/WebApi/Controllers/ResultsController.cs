using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Tools;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly InfotecsContext _context;

        public ResultsController(InfotecsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Result>>> GetResults(Filter filter)
        {
            List<Result> results = await _context.Results.ToListAsync();
            if (filter.FileName != null) { results = results.Where(v => v.FileName == filter.FileName).ToList(); }

            if (filter.MinDate != null) { results = results.Where(v => v.MinDate >= filter.MinDate).ToList(); }
            if (filter.MaxDate != null) { results = results.Where(v => v.MinDate <= filter.MaxDate).ToList(); }

            if (filter.MinSeconds != null) { results = results.Where(v => v.AverageSeconds >= filter.MinSeconds).ToList(); }
            if (filter.MaxSeconds != null) { results = results.Where(v => v.AverageSeconds <= filter.MaxSeconds).ToList(); }

            if (filter.MinN != null) { results = results.Where(v => v.AverageN >= filter.MinN).ToList(); }
            if (filter.MaxN != null) { results = results.Where(v => v.AverageN <= filter.MaxN).ToList(); }
            return results;
        }

    }
}
