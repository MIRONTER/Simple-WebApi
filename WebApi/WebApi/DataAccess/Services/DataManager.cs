using Microsoft.EntityFrameworkCore;
using WebApi.DataAccess.Context;
using WebApi.DataAccess.Models;

namespace WebApi.DataAccess.Services
{
    public class DataManager
    {
        private readonly WebApiContext _context;

        public DataManager(WebApiContext context)
        {
            _context = context;
        }

        private static double Median(IEnumerable<double> items)
        {
            var data = items.OrderBy(n => n).ToArray();
            if (data.Length % 2 == 0)
                return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0;
            return data[data.Length / 2];
        }

        private static Value ParseValue(string filename, string[] values)
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

        public void UpdateResult(Result result, string filename)
        {
            result.AverageSeconds = _context.Values.Where(v => v.FileName == filename).Average(s => s.Seconds);
            result.AverageN = _context.Values.Where(v => v.FileName == filename).Average(n => n.N);
            result.MedianN = Median(_context.Values.Where(v => v.FileName == filename).Select(n => n.N));
            result.MaxN = _context.Values.Where(v => v.FileName == filename).Max(n => n.N);
            result.MinN = _context.Values.Where(v => v.FileName == filename).Min(n => n.N);
            result.Count = _context.Values.Where(v => v.FileName == filename).Count();
            result.MinDate = _context.Values.Where(v => v.FileName == filename).Min(d => d.Date);
            result.AllTime = (_context.Values.Where(v => v.FileName == filename).Max(d => d.Date) - result.MinDate).Ticks;
        }

        public bool AddValues(IFormFile file)
        {
            int lines = 0;
            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                string line;
                while ((line = reader.ReadLine()!) != null)
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

        public async Task<bool> CheckResult(Result result, string filename)
        {
            if (result == null)
            {
                return true;
            }
            else
            {
                List<Value> values = await _context.Values.Where(v => v.FileName == filename).ToListAsync();
                _context.Values.RemoveRange(values);
                return false;
            }
        }
    }
}
