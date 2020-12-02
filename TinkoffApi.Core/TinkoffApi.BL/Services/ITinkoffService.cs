using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using TinkoffApi.Data.Entities;
using TinkoffApi.Data.Models;
using TinkoffApi.Data.Models.StopLoss;
using TinkoffApi.Data.Models.TakeProfit;
using static Tinkoff.Trading.OpenApi.Models.PortfolioCurrencies;

namespace AutoTrading.BL.Services
{
    /// <summary>
    /// Сервис для реализации работы с Tinkoff OpenApi
    /// </summary>
    public interface ITinkoffService
    {
        Task SaveCurrentPortfolioBDAsync();
        Task<Stocks> GetStocksAsync();
        Task<Stocks> GetMarketBondsAsync();
        Task<Stocks> GetMarketCurrenciesAsync();
        Task<Portfolio> GetPortfolioAsync();
        Task<OperationHistory> PlaceMarketOrderAsync(string figi, int lots, OperationType operationType, StopLossData stopLoss, TakeProfitData takeProfit);
        Task<PortfolioCurrency> GetPortfolioCurrencyAsync(Currency currency);

        Task<List<Candle>> GetCandlesHistory(string figi, DateTime from, DateTime to, CandleInterval candleInterval);

        Task<Operation> GetOperation(string figi, string operationId);

        OperationHistory GetOperationHistory(string figi, string operationId);
        List<OperationHistory> GetOperationsHistory();

        void CandleSubscribe(string figi);
    }
}
