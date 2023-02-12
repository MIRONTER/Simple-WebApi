namespace WebApi.Tools
{
    public class ToolsForSQL
    {
        public double Median(IEnumerable<double> items)
        {
            var data = items.OrderBy(n => n).ToArray();
            if (data.Length % 2 == 0)
                return (data[data.Length / 2 - 1] + data[data.Length / 2]) / 2.0;
            return data[data.Length / 2];
        }
    }
}
