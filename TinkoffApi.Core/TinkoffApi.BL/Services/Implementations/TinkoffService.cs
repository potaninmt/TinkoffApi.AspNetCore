using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using TinkoffApi.BL.Hosted;
using TinkoffApi.BL.Infrastructure;
using TinkoffApi.DAL;
using TinkoffApi.Data;
using TinkoffApi.Data.Entities;
using TinkoffApi.Data.Enums;
using TinkoffApi.Data.Models;
using TinkoffApi.Data.Models.StopLoss;
using TinkoffApi.Data.Models.TakeProfit;

namespace AutoTrading.BL.Services.Implementations
{
    public class TinkoffService : ITinkoffService
    {
        private IConfiguration _configuration { get; }


        private ContextApi _contextApi { get; set; }

        private BackgroundService _backgroundService { get; }

        private TinkoffApiContext _context { get; }

        public TinkoffService(IConfiguration configuration, TinkoffApiContext context, Microsoft.Extensions.Hosting.IHostedService backgroundService)
        {
            _configuration = configuration;
            _backgroundService = (BackgroundService)backgroundService;
            _context = context;
            var mode = GetCurrentMode();
            _contextApi = new ContextApi(
                _configuration["Tinkoff:Sandbox:Token"], _configuration["Tinkoff:Bourse:Token"], mode
                );
            Connection();
        }

        private void Connection()
        {
            _contextApi.Connection();
            //_contextApi.UseStreaming();
        }


        ///// <summary>
        ///// возвращает описание акций: figi, Ticker, ...
        ///// </summary>
        ///// <returns></returns>
        //public async Task<MarketInstrumentList> GetBounds()
        //{
        //    switch (_mode)
        //    {
        //        case Mode.Sandbox:
        //            return await _sandboxContext.MarketBondsAsync();
        //        case Mode.Bourse:
        //            return await _bourseContext.MarketBondsAsync();
        //        default:
        //            throw new NotImplementedException();
        //    }
        //}

        ///// <summary>
        ///// возвращает описание акций: figi, Ticker, ...
        ///// </summary>
        ///// <returns></returns>
        //public async Task<MarketInstrumentList> GetStocks()
        //{
        //    switch (_mode)
        //    {
        //        case Mode.Sandbox:
        //            return await _sandboxContext.MarketStocksAsync();
        //        case Mode.Bourse:
        //            return await _bourseContext.MarketStocksAsync();
        //        default:
        //            throw new NotImplementedException();
        //    }
        //}

        /// <summary>
        /// Возвращает массив акций с описанием (ticker, figi)
        /// </summary>
        /// <returns></returns>
        public async Task<Stocks> GetStocksAsync()
        {
            return await _contextApi.GetStocksAsync();
        }

        public async Task<Stocks> GetMarketBondsAsync()
        {
            return await _contextApi.GetMarketBondsAsync();
        }

        public async Task<Stocks> GetMarketCurrenciesAsync()
        {
            return await _contextApi.GetMarketCurrenciesAsync();
        }

        public async Task<OperationHistory> PlaceMarketOrderAsync(string figi, int lots, OperationType operationType, StopLossData stopLoss, TakeProfitData takeProfit)
        {
            var result = await _contextApi.PlaceMarketOrderAsync(figi, lots, operationType);

            if (stopLoss == null)
                stopLoss = new StopLossData();
            if (takeProfit == null)
                takeProfit = new TakeProfitData();

            var operationHistory = new OperationHistory
            {
                orderType = (operationType == OperationType.Buy) ? TinkoffApi.Data.Enums.OrderType.Buy : TinkoffApi.Data.Enums.OrderType.Sell,
                lots = lots,
                figi = figi,
                orderMode = _contextApi.GetMode(),
                time = ServerTime.GetDate(),
                orderId = result.OrderId,
                operationStatus = TinkoffApi.Data.Enums.OperationStatus.Progress,
                Commission = new TinkoffApi.Data.Models.MoneyAmount(),
                Price = 0.0,
                StopLoss = stopLoss,
                TakeProfit = takeProfit
            };

            _context.OperationsHistory.Add(operationHistory);

            await SaveCurrentPortfolioBDAsync();

            await _context.SaveChangesAsync();

            _backgroundService.TaskFindOperation(_contextApi, figi, result.OrderId);

            return operationHistory;
        }

        /// <summary>
        /// Balance and info about this
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public async Task<PortfolioCurrencies.PortfolioCurrency> GetPortfolioCurrencyAsync(Currency currency)
        {
            return await _contextApi.GetPortfolioCurrency(currency);
        }

        //TODO переделать
        public void CandleSubscribe(string figi)
        {
            _backgroundService.CandleSubscribe(
             new TinkoffApi.BL.Hosted.Models.CandleSubscribe()
             {
                 contextApi = _contextApi,
                 candleInterval = CandleInterval.Minute,
                 figi = figi
             });
        }

        /// <summary>
        /// Сохранить текущее портфолио в БД
        /// </summary>
        /// <returns></returns>
        public async Task SaveCurrentPortfolioBDAsync()
        {
            var portfolio = await GetPortfolioCurrencyAsync(Tinkoff.Trading.OpenApi.Models.Currency.Rub);
            _context.PortfolioHistory.Add(new PortfolioHistory()
            {
                balance = (double)portfolio.Balance,
                currency = CurrencyEnum.Rub,
                time = ServerTime.GetDate()
            });

            await _context.SaveChangesAsync();
        }

        public async Task<Portfolio> GetPortfolioAsync()
        {
            return await _contextApi.GetPortfolio();
        }

        public async Task<List<Candle>> GetCandlesHistory(string figi, DateTime from, DateTime to, CandleInterval candleInterval)
        {
            if (from > to) throw new ArgumentException("{to} must be more than {from}");
            var result = new List<Candle>();

            var temp = new DateTime(from.Ticks);

            while (temp <= to)
            {
                var obj = await _contextApi.GetCandlesHistory(figi, temp, temp.AddDays(1), candleInterval);
                var candles = obj.Where(x => x.time >= from && x.time < to).ToList();

                result.AddRange(candles);
                switch (candleInterval)
                {
                    case CandleInterval.Minute:
                        temp = temp.AddDays(1);
                        break;
                }

                //System.Threading.Thread.Sleep(11);
            }
            return result;
        }

        public async Task<Operation> GetOperation(string figi, string operationId)
        {
            return await _contextApi.GetOperation(figi, operationId);
        }

        public OperationHistory GetOperationHistory(string figi, string operationId)
        {
            return _context.OperationsHistory.Where(x => x.figi == figi && x.orderId == operationId).FirstOrDefault();
        }

        public List<OperationHistory> GetOperationsHistory()
        {
            return _context.OperationsHistory.ToList();
        }

        private Mode GetCurrentMode()
        {
            switch (_configuration["Mode"])
            {
                case "Sandbox":
                    return Mode.Sandbox;
                case "Bourse":
                    return Mode.Bourse;
                default: throw new NotImplementedException();
            }
        }



    }
}
