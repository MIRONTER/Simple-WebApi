using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Result
{
    public DateTime AllTime { get; set; }

    public DateTime MinDate { get; set; }

    public double AverageSeconds { get; set; }

    public double AverageN { get; set; }

    public double MedianN { get; set; }

    public int MaxN { get; set; }

    public int MinN { get; set; }

    public int Count { get; set; }
}
