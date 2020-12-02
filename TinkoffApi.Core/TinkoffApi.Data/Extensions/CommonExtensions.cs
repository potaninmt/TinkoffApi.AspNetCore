using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TinkoffApi.Data.Models;
using TinkoffApi.Data.Models.StopLoss;
using TinkoffApi.Data.Models.TakeProfit;

namespace TinkoffApi.Data.Extensions
{
    public static class CommonExtensions
    {
        public static StopLossData SetValue(this StopLossData stopLossData, double close)
        {
            if (stopLossData.Value == 0.0)
            {
                stopLossData.Value = close * (1.0 - stopLossData.Procent / 100.0);
            }

            return stopLossData;
        }

        public static TakeProfitData SetValue(this TakeProfitData takeProfitData, double close)
        {
            if (takeProfitData.Value == 0.0)
            {
                takeProfitData.Value = close * (1.0 + takeProfitData.Procent / 100.0);
            }

            return takeProfitData;
        }

        public static void Save(this IEnumerable<Candle> candles, string path)
        {
            using(var stream = new FileStream(path, FileMode.Create))
            {
                var binary = new BinaryFormatter();
                binary.Serialize(stream, candles);
            }
        }
    }
}
