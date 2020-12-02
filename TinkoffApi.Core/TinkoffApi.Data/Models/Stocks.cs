using System.Collections.Generic;
using System.Linq;
using TinkoffApi.Data.Enums;

namespace TinkoffApi.Data.Models
{
    public class Stocks
    {
        public List<Stock> stocks { get; }
        public Stocks()
        {
            stocks = new List<Stock>();
        }

        public string TryGetFigi(string ticker)
        {
            var stock = stocks.Where(x => x.ticker == ticker).FirstOrDefault();
            if (stock == null) return null;

            return stock.figi;
        }

        public bool TryAddStock(Stock stock)
        {
            if (IsUnique(stock))
            {
                stocks.Add(stock);

                return true;
            }

            return false;
        }

        public bool TryAddStock(string ticker, string figi, CurrencyEnum currency, string name, InstrumentTypeEnum type, int lot)
        {
            var stock = new Stock
            {
                figi = figi,
                ticker = ticker,
                currency = currency,
                type = type,
                name = name,
                lot = lot
            };

            return TryAddStock(stock);
        }

        public bool IsUnique(Stock stock)
        {
            if (TryGetFigi(stock.ticker) == null) return true;

            return false;
        }
    }
    public class Stock
    {
        public string figi { get; set; }
        public string ticker { get; set; }
        public CurrencyEnum currency { get; set; }
        public string name { get; set; }
        public InstrumentTypeEnum type { get; set; }
        public int lot { get; set; }
    }
}
