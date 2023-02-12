using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Result
{
    public long? AllTime { get; set; }

    public DateTime MinDate { get; set; }

    public double? AverageSeconds { get; set; }

    public double? AverageN { get; set; }

    public double? MedianN { get; set; }

    public double? MaxN { get; set; }

    public double? MinN { get; set; }

    public int? Count { get; set; }

    public string FileName { get; set; } = null!;

    public virtual ICollection<Value> Values { get; } = new List<Value>();
}
