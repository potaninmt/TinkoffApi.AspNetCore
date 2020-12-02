using System;
using TinkoffApi.Data.Models;

namespace TinkoffApi.BL
{
    public static class StocksExtensions
    {
        public static void Transform(this Stocks stocks, Func<Stock, Stock> func)
        {
            for (int i = 0; i < stocks.stocks.Count; i++)
            {
                var stock = stocks.stocks[i];
                var newStock = func(stock);
                if (newStock == null)
                {
                    stocks.stocks.RemoveAt(i--);
                }
                else
                {
                    stocks.stocks[i] = stock;
                }
            }
        }
    }
}
