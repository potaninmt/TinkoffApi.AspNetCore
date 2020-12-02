using TinkoffApi.Data.Models.StopLoss;
using TinkoffApi.Data.Models.TakeProfit;

namespace TinkoffApi.Data.Models.Body
{
    public class PlaceMarkerBody
    {
        public StopLossData stopLoss;
        public TakeProfitData takeProfit;
    }
}
