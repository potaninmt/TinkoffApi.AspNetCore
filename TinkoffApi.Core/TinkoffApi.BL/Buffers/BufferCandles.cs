using System;
using System.Collections.Generic;

namespace TinkoffApi.BL.Buffers
{
    public class BufferFiggiesCandles
    {
        public Dictionary<string, Candles> figiesCandles { get; set; }

        public BufferFiggiesCandles()
        {
            figiesCandles = new Dictionary<string, Candles>();
        }

        public Candles GetCandles(string figi, string period)
        {
            Candles candles;
            figiesCandles.TryGetValue(Key(figi, period), out candles);

            return candles;
        }

        public void AddCandle(string figi, string period, Candle candle)
        {
            var key = Key(figi, period);
            if (!figiesCandles.ContainsKey(key))
            {
                figiesCandles.Add(key, new Candles());
            }

            figiesCandles[key].Add(candle);
        }

        private string Key(string figi, string period)
        {
            return figi + "_" + period;
        }
    }

    public class Candles
    {
        public List<Candle> candles { get; set; }
        public Candles()
        {
            candles = new List<Candle>();
        }

        public void Add(Candle candle)
        {
            candles.Add(candle);
        }
        public void Add(DateTime time, double value)
        {
            Add(new Candle
            {
                time = time,
                value = value
            });
        }
    }

    public class Candle
    {
        public DateTime time { get; set; }
        public double value { get; set; }
    }
}
