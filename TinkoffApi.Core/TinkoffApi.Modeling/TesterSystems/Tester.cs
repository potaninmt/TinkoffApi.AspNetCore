using System;
using System.Collections.Generic;
using System.Linq;
using TinkoffApi.Data.Enums;
using TinkoffApi.Data.Models;

namespace TinkoffApi.Modeling.TesterSystems
{
    public class Tester : ITester
    {
        public double TakeProfit { get; }

        public double StopLoss { get; }

        public Reward Reward { get; }

        public Statistic Statistic { get; set; }

        /// <summary>
        /// double[] - candles array, int - current position
        /// </summary>
        public Func<Candle[], OrderType> Predictor { get; set; }

        /// <summary>
        /// Candle[] - свечи
        /// double - цена покупки
        /// double - цена продажи
        /// </summary>
        public Func<Candle[], double, double> AlgorithmSell { get; set; }

        public event Action<double> OrderClose;


        public Tester(Func<Candle[], OrderType> predictor, Func<Candle[], double, double> algorithmSell)
        {
            Reward = new Reward();
            Statistic = new Statistic();
            Predictor = predictor;
            AlgorithmSell = algorithmSell;
            OrderClose += OrderCloseBase;
        }

        /// <summary>
        /// Моделирование алгоритма по выборке
        /// </summary>
        /// <param name="candles">генеральная совокупность</param>
        /// <param name="step">шаг моделирования(по умолчанию: 1)</param>
        /// <param name="shift">смещение слева(по умолчанию: 0)</param>
        /// <returns></returns>
        public Statistic Modeling(Candle[] candles, int historyLen, TimeSpan predictTime, int step = 1, int shift = 0)
        {
            var l = candles.Length;
            for (int i = shift + historyLen; i < l; i += step)
                ModelingStep(candles, i, historyLen, predictTime);

            return Statistic;
        }

        /// <summary>
        /// Моделирование одного ордера
        /// </summary>
        /// <param name="candles">генеральная совокупность</param>
        /// <param name="curInd">текущая позиция моделирования</param>
        /// <param name="predictLen">время моделирования</param>
        /// <returns></returns>
        public OrderType ModelingStep(Candle[] candles, int curInd, int historyLen, TimeSpan predictTime)
        {
            //Arrange
            var curCandle = candles[curInd];
            var historyCandles = GetHistoryCandles(candles, curInd, historyLen);
            var futureCandles = GetFutureCloses(candles, curCandle.time, predictTime);
            if (futureCandles.Length == 0)
                return OrderType.None;

            var valBuy = curCandle.close;

            //Act
            var action = Predictor(historyCandles);
            double reward = 0.0;
            switch (action)
            {
                case OrderType.Buy:
                    var valSell = AlgorithmSell(futureCandles, valBuy);
                    reward = Reward.GetReward(valBuy, valSell);
                    OrderClose(reward);
                    break;
                case OrderType.Sell:

                    break;
            }

            Statistic.Total++;
            return action;
        }

        /// <summary>
        /// Возвращает историю закрытия свечей
        /// </summary>
        /// <param name="candles"></param>
        /// <param name="curInd"></param>
        /// <param name="historyLen"></param>
        /// <returns></returns>
        private Candle[] GetHistoryCandles(Candle[] candles, int curInd, int historyLen)
        {
            if (curInd - historyLen + 1 < 0) return null;

            Candle[] array = new Candle[historyLen];
            Array.Copy(candles, curInd - historyLen + 1, array, 0, historyLen);

            return array;
        }

        public Candle[] GetFutureCloses(Candle[] candles, DateTime curTime, TimeSpan predictTime)
        {
            return candles.Where(x => x.time > curTime && x.time < curTime.Add(predictTime)).ToArray();

            //Array.Copy(candles, curInd + 1, array, 0, predictLen);
        }

        public void ResetStatistic()
        {
            Statistic = new Statistic();
        }

        //events
        public void OrderCloseBase(double reward)
        {
            Statistic.Reward += reward;

            if (reward > 0)
                Statistic.Pos++;

            Statistic.N++;
        }
    }
}
