namespace WebApi.Tools
{
    public class Filter
    {
        public string? FileName { get; set; }

        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public int? MinSeconds { get; set; }
        public int? MaxSeconds { get; set; }

        public double? MinN { get; set; }
        public double? MaxN { get; set; }
    }
}
