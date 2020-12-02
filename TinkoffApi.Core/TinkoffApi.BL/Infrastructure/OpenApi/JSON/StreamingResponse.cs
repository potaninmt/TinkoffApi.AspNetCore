namespace TinkoffApi.BL.Infrastructure.OpenApi.JSON
{
    public class StreamingResponse
    {
        public string Figi { get; set; }
        public string Interval { get; set; }
        public string Time { get; set; }
        public double Open { get; set; }
        public int Close { get; set; }
        public double High { get; set; }
        public int Low { get; set; }
        public int Volume { get; set; }
    }
}
