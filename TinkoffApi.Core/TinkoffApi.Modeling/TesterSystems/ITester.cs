using System;
using TinkoffApi.Data.Enums;
using TinkoffApi.Data.Models;

namespace TinkoffApi.Modeling.TesterSystems
{
    public interface ITester
    {
        Reward Reward { get; }
        Statistic Statistic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="candles">все свечи из датасета</param>
        /// <param name="curInd">индекс текущей свечи</param>
        /// <returns></returns>
        OrderType ModelingStep(Candle[] candles, int curInd, int historyLen, TimeSpan predictTime);
        Statistic Modeling(Candle[] candles, int historyLen, TimeSpan predictTime, int step, int shift);
    }
}
