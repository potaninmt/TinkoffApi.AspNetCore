using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;
using TinkoffApi.Data.Enums;
using TinkoffApi.Data.Models;
using static Tinkoff.Trading.OpenApi.Models.PortfolioCurrencies;

namespace TinkoffApi.BL.Infrastructure
{
    public class ContextApi
    {
        /// <summary>
        /// Событие, возникающее при получении сообщения от WebSocket-клиента.
        /// </summary>
        public event EventHandler<StreamingEventReceivedEventArgs> StreamingEventReceived;
        public int RetriesCount { get; set; } = 5;
        public int TimeDelay { get; set; } = 60000;
        private SandboxContext _sandboxContext { get; set; }
        private Context _bourseContext { get; set; }

        private Mode _mode { get; }

        private string _tokenSandbox { get; }
        private string _tokenBourse { get; }

        private bool IsStreaming { get; set; }


        public ContextApi(string tokenSandbox, string tokenBourse, Mode mode)
        {
            _tokenBourse = tokenBourse;
            _tokenSandbox = tokenSandbox;
            _mode = mode;
        }

        private void _sandboxContext_StreamingEventReceived(object sender, StreamingEventReceivedEventArgs e)
        {
            StreamingEventReceived(sender, e);
        }

        public void Connection()
        {
            switch (_mode)
            {
                case Mode.Sandbox:
                    var connectionSandbox = ConnectionFactory.GetSandboxConnection(_tokenSandbox);
                    _sandboxContext = connectionSandbox.Context;
                    break;
                case Mode.Bourse:
                    var connection = ConnectionFactory.GetConnection(_tokenBourse);
                    _bourseContext = connection.Context;
                    break;
            }
        }

        public void UseStreaming()
        {
            if (!IsStreaming)
            {
                switch (_mode)
                {
                    case Mode.Sandbox:
                        _sandboxContext.StreamingEventReceived += _sandboxContext_StreamingEventReceived;
                        break;
                    case Mode.Bourse:
                        _bourseContext.StreamingEventReceived += _sandboxContext_StreamingEventReceived;
                        break;
                }

                IsStreaming = true;
            }
        }

        public async Task<PlacedMarketOrder> PlaceMarketOrderAsync(string figi, int lots, OperationType operationType)
        {
            switch (_mode)
            {
                case Mode.Sandbox:
                    var orderSandbox = await _sandboxContext.PlaceMarketOrderAsync(new MarketOrder(figi, lots, operationType));
                    return orderSandbox;
                case Mode.Bourse:
                    var orderBourse = await _bourseContext.PlaceMarketOrderAsync(new MarketOrder(figi, lots, operationType));
                    return orderBourse;
                default: throw new NotImplementedException();
            }
        }

        public async Task<PortfolioCurrency> GetPortfolioCurrency(Currency currency)
        {
            int delay = TimeDelay / RetriesCount;
            int ost = TimeDelay % RetriesCount;
            for (int i = 0; i < RetriesCount; i++)
            {
                try
                {
                    switch (_mode)
                    {
                        case Mode.Sandbox:
                            var sandbox = await _sandboxContext.PortfolioCurrenciesAsync();
                            return sandbox.Currencies.Where(x => x.Currency == currency).FirstOrDefault();
                        case Mode.Bourse:
                            var bourse = await _bourseContext.PortfolioCurrenciesAsync();
                            return bourse.Currencies.Where(x => x.Currency == currency).FirstOrDefault();
                        default: throw new NotImplementedException();
                    }
                }
                catch
                {
                    if (i == RetriesCount - 1) delay += ost;
                    System.Threading.Thread.Sleep(delay);
                }
            }

            throw new Exception("Error send");
        }

        public async Task<Stocks> GetStocksAsync()
        {
            int delay = TimeDelay / RetriesCount;
            int ost = TimeDelay % RetriesCount;
            for (int i = 0; i < RetriesCount; i++)
            {
                try
                {
                    MarketInstrumentList marketInstrumentList;
                    switch (_mode)
                    {
                        case Mode.Sandbox:
                            marketInstrumentList = await _sandboxContext.MarketStocksAsync();
                            break;
                        case Mode.Bourse:
                            marketInstrumentList = await _bourseContext.MarketStocksAsync();
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    var stocks = new Stocks();
                    foreach (var instrument in marketInstrumentList.Instruments)
                    {
                        stocks.TryAddStock(instrument.Ticker, instrument.Figi, (CurrencyEnum)instrument.Currency, instrument.Name, (InstrumentTypeEnum)instrument.Type, instrument.Lot);
                    }

                    return stocks;
                }
                catch
                {
                    if (i == RetriesCount - 1) delay += ost;
                    System.Threading.Thread.Sleep(delay);
                }
            }

            throw new Exception("Error send");
        }

        public async Task<Stocks> GetMarketBondsAsync()
        {
            int delay = TimeDelay / RetriesCount;
            int ost = TimeDelay % RetriesCount;
            for (int i = 0; i < RetriesCount; i++)
            {
                try
                {
                    var stocks = new Stocks();
                    MarketInstrumentList marketInstrumentList;
                    switch (_mode)
                    {
                        case Mode.Sandbox:
                            marketInstrumentList = await _sandboxContext.MarketBondsAsync();
                            break;
                        case Mode.Bourse:
                            marketInstrumentList = await _bourseContext.MarketStocksAsync();
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    foreach (var instrument in marketInstrumentList.Instruments)
                    {
                        stocks.TryAddStock(instrument.Ticker, instrument.Figi, (CurrencyEnum)instrument.Currency, instrument.Name, (InstrumentTypeEnum)instrument.Type, instrument.Lot);
                    }

                    return stocks;
                }
                catch
                {
                    if (i == RetriesCount - 1) delay += ost;
                    System.Threading.Thread.Sleep(delay);
                }
            }

            throw new Exception("Error send");
        }

        public async Task<Stocks> GetMarketCurrenciesAsync()
        {
            int delay = TimeDelay / RetriesCount;
            int ost = TimeDelay % RetriesCount;
            for (int i = 0; i < RetriesCount; i++)
            {
                try
                {
                    var stocks = new Stocks();
                    MarketInstrumentList marketInstrumentList;
                    switch (_mode)
                    {
                        case Mode.Sandbox:
                            marketInstrumentList = await _sandboxContext.MarketCurrenciesAsync();
                            break;
                        case Mode.Bourse:
                            marketInstrumentList = await _bourseContext.MarketCurrenciesAsync();
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    foreach (var instrument in marketInstrumentList.Instruments)
                    {
                        stocks.TryAddStock(instrument.Ticker, instrument.Figi, (CurrencyEnum)instrument.Currency, instrument.Name, (InstrumentTypeEnum)instrument.Type, instrument.Lot);
                    }

                    return stocks;

                }
                catch
                {
                    if (i == RetriesCount - 1) delay += ost;
                    System.Threading.Thread.Sleep(delay);
                }
            }

            throw new Exception("Error send");
        }

        public async Task SendStreamingRequestAsync<T>(T request) where T : StreamingRequest
        {
            switch (_mode)
            {
                case Mode.Sandbox:
                    await _sandboxContext.SendStreamingRequestAsync(request);
                    break;
                case Mode.Bourse:
                    await _bourseContext.SendStreamingRequestAsync(request);
                    break;
                default: throw new NotImplementedException();
            }
        }

        public async Task<Portfolio> GetPortfolio()
        {
            int delay = TimeDelay / RetriesCount;
            int ost = TimeDelay % RetriesCount;
            for (int i = 0; i < RetriesCount; i++)
            {
                try
                {
                    switch (_mode)
                    {
                        case Mode.Sandbox:
                            return await _sandboxContext.PortfolioAsync();
                        case Mode.Bourse:
                            return await _bourseContext.PortfolioAsync();
                        default: throw new NotImplementedException();
                    }
                }
                catch
                {
                    if (i == RetriesCount - 1) delay += ost;
                    System.Threading.Thread.Sleep(delay);
                }
            }

            throw new Exception("Error send");
        }

        public async Task<List<Candle>> GetCandlesHistory(string figi, DateTime from, DateTime to, CandleInterval candleInterval)
        {
            int delay = TimeDelay / RetriesCount;
            int ost = TimeDelay % RetriesCount;
            for (int i = 0; i < RetriesCount; i++)
            {
                try
                {
                    switch (_mode)
                    {
                        case Mode.Sandbox:
                            var listS = await _sandboxContext.MarketCandlesAsync(figi, from, to, candleInterval);
                            var candles = listS.Candles.Select(x => new Candle
                            {
                                close = (double)x.Close,
                                figi = x.Figi,
                                high = (double)x.High,
                                interval = (CandleIntervalEnum)x.Interval,
                                low = (double)x.Low,
                                open = (double)x.Open,
                                time = x.Time,
                                volume = (double)x.Volume

                            }).ToList();

                            return candles;

                        case Mode.Bourse:
                            var listB = await _bourseContext.MarketCandlesAsync(figi, from, to, candleInterval);
                            var candlesB = listB.Candles.Select(x => new Candle
                            {
                                close = (double)x.Close,
                                figi = x.Figi,
                                high = (double)x.High,
                                interval = (CandleIntervalEnum)x.Interval,
                                low = (double)x.Low,
                                open = (double)x.Open,
                                time = x.Time,
                                volume = (double)x.Volume

                            }).ToList();

                            return candlesB;

                        default: throw new NotImplementedException();
                    }
                }
                catch
                {
                    if (i == RetriesCount - 1) delay += ost;
                    System.Threading.Thread.Sleep(delay);
                }
            }

            throw new Exception("Error send");
        }

        public async Task<Operation> GetOperation(string figi, string operationId)
        {
            int delay = TimeDelay / RetriesCount;
            int ost = TimeDelay % RetriesCount;
            for (int i = 0; i < RetriesCount; i++)
            {
                try
                {
                    switch (_mode)
                    {
                        case Mode.Sandbox:
                            var operationsSandbox = await _sandboxContext.OperationsAsync(DateTime.UtcNow.AddDays(-3), Interval.Week, figi);
                            return operationsSandbox
                                    .Where(x => x.Id == operationId)
                                    .FirstOrDefault();
                        case Mode.Bourse:
                            return (await _bourseContext.OperationsAsync(DateTime.UtcNow.AddDays(-3), Interval.Week, figi))
                                .Where(x => x.Id == operationId)
                                .FirstOrDefault();
                        default: throw new NotImplementedException();
                    }
                }
                catch
                {
                    if (i == RetriesCount - 1) delay += ost;
                    System.Threading.Thread.Sleep(delay);
                }
            }

            throw new Exception("Error send");

        }


        public Mode GetMode()
        {
            return _mode;
        }

    }
}
