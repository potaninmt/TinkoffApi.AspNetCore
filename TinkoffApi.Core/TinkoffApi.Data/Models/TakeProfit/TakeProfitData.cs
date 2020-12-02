using TinkoffApi.Data.Enums;

namespace TinkoffApi.Data.Models.TakeProfit
{
    public class TakeProfitData
    {
        public double Value { get; set; }
        public double Procent { get; set; }

        public FormatValue formatValue;
    }
}
