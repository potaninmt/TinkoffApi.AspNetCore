using TinkoffApi.Data.Enums;

namespace TinkoffApi.Data.Models.StopLoss
{
    public class StopLossData
    {
        public double Value { get; set; }
        public double Procent { get; set; }

        public FormatValue formatValue;
    }
}
