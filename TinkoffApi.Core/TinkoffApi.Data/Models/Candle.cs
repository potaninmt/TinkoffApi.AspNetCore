using System;
using TinkoffApi.Data.Enums;

namespace TinkoffApi.Data.Models
{
    [Serializable]
    public class Candle
    {
        public double open { get; set; }
        public double close { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double volume { get; set; }
        public DateTime time { get; set; }
        public CandleIntervalEnum interval { get; set; }
        public string figi { get; set; }
    }
}
