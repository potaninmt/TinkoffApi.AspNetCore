using Tinkoff.Trading.OpenApi.Models;
using TinkoffApi.BL.Infrastructure;

namespace TinkoffApi.BL.Hosted.Models
{
    public class CandleSubscribe
    {
        public ContextApi contextApi { get; set; }
        public string figi { get; set; }
        public CandleInterval candleInterval { get; set; }
    }
}
