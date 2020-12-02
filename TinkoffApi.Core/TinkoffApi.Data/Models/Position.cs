namespace TinkoffApi.Data.Models
{
    public class Position
    {
        public string name { get; set; }
        public string figi { get; set; }
        public string ticker { get; set; }
        public string isin { get; set; }
        public int instrumentType { get; set; }
        public double balance { get; set; }
        public double blocked { get; set; }
        public ExpectedYield expectedYield { get; set; }
        public int lots { get; set; }
        public AveragePositionPrice averagePositionPrice { get; set; }
        public object averagePositionPriceNoNkd { get; set; }
    }
}
