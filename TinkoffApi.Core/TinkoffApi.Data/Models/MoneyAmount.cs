using TinkoffApi.Data.Enums;

namespace TinkoffApi.Data.Models
{
    public class MoneyAmount
    {
        public CurrencyEnum Currency { get; set; }
        public double Value { get; set; }
    }
}
