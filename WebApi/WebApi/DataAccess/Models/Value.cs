using System;
using System.Collections.Generic;

namespace WebApi.DataAccess.Models;

public partial class Value
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public DateTime Date { get; set; }

    public int Seconds { get; set; }

    public double N { get; set; }

    public virtual Result FileNameNavigation { get; set; } = null!;
}
