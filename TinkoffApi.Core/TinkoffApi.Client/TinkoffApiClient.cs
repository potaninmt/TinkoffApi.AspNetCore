using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
//using System.Text.Json;
using System.Threading.Tasks;
using TinkoffApi.Client.Enums;
using TinkoffApi.Data.Entities;
using TinkoffApi.Data.Models;
using TinkoffApi.Data.Models.Body;
using TinkoffApi.Data.Models.StopLoss;
using TinkoffApi.Data.Models.TakeProfit;

namespace TinkoffApi.Client
{
    public class TinkoffApiClient : IDisposable
    {
        private readonly string _host;
        private readonly bool _reconnection;
        private readonly HttpClient _client;

        private HubConnection hubConnection;
        private Dictionary<string, List<Candle>> candlesHistory;
        private DateTime LastCandleEvent;

        public event Func<Exception, Task> Closed;
        public event Func<string, Task> Reconnected;
        public event Func<Candle, bool, Task> CandleEvent;

        // constants
        private readonly string hub = "tinkoffHub";

        /// <summary>
        ///
        /// </summary>
        /// <param name="host"></param>
        /// <param name="hub"></param>
        /// <param name="reconnection"></param>
        public TinkoffApiClient(string host, bool reconnection = true)
        {
            _host = host.TrimEnd('/');
            _reconnection = reconnection;

            var url = $"{_host}/{hub}";
            if (reconnection)
            {
                hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();
            }
            else
            {
                hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();
            }

            hubConnection.Closed += HubConnection_Closed;
            hubConnection.Reconnected += HubConnection_Reconnected;
            hubConnection.On<string>("Send", OnSend);

            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMinutes(30);
            candlesHistory = new Dictionary<string, List<Candle>>();
        }

        public async Task StartAsync()
        {
            await hubConnection.StartAsync();
        }

        public void StartWebsocket()
        {
            Task.Run(async () => await hubConnection.StartAsync())
                .Wait();
        }

        public List<Candle> GetCandles(string figi)
        {
            List<Candle> result;

            candlesHistory.TryGetValue(figi, out result);

            return result;
        }


        #region Rest methods
        #region async
        public async Task<OperationHistory> CreatePlaceMarketOrderAsync(string figi, OperationType operationType, int lots, StopLossData stopLoss = null, TakeProfitData takeProfit = null)
        {
            var url = $"{_host}/api/Tinkoff/placeMarketOrder?operationType={(int)operationType}&lots={lots}&figi={figi}";
            var body = new PlaceMarkerBody()
            {
                stopLoss = stopLoss,
                takeProfit = takeProfit
            };
            var content = JsonConvert.SerializeObject(body);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(httpRequestMessage);

            if (!response.IsSuccessStatusCode)
                return null;

            var str = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<OperationHistory>(str);
        } // stocks/rus
        
        public async Task<List<OperationHistory>> GetOperationsHistoryAsync()
        {
            var url = $"{_host}/api/Tinkoff/get/operationshistory";

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var str = await response.Content.ReadAsStringAsync();

            var operationsHistory = JsonConvert.DeserializeObject<List<OperationHistory>>(str);

            return operationsHistory;
        }
        public async Task<Positions> GetPortfolioAsync()
        {
            var url = $"{_host}/api/Tinkoff/get/portfolio";

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var str = await response.Content.ReadAsStringAsync();

            var positions = JsonConvert.DeserializeObject<Positions>(str);

            return positions;
        }

        public async Task<Stocks> GetStocksRusAsync()
        {
            var url = $"{_host}/api/Tinkoff/stocks/rus";

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var str = await response.Content.ReadAsStringAsync();

            var stocks = JsonConvert.DeserializeObject<Stocks>(str);

            return stocks;
        }

        public async Task<Stocks> GetStocksAsync()
        {
            var url = $"{_host}/api/Tinkoff/stocks";

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var str = await response.Content.ReadAsStringAsync();

            var stocks = JsonConvert.DeserializeObject<Stocks>(str);

            return stocks;
        }

        public async Task<List<Candle>> GetCandlesHistoryAsync(string figi, DateTime from, DateTime to)
        {
            var url = $"{_host}/api/Tinkoff/get/candles?figi={figi}&from={from.ToString("O")}&to={to.ToString("O")}";

            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var str = await response.Content.ReadAsStringAsync();

            var candles = JsonConvert.DeserializeObject<List<Candle>>(str);

            return candles;
        }
        #endregion
        #region sync
        public OperationHistory CreatePlaceMarketOrder(string figi, OperationType operationType, int lots, StopLossData stopLoss = null, TakeProfitData takeProfit = null)
        {
            OperationHistory operation = null;
            Task.Run(async () => operation = await CreatePlaceMarketOrderAsync(figi, operationType, lots, stopLoss, takeProfit))
                .Wait();

            return operation;

        }
        
        public List<OperationHistory> GetOperationsHistory()
        {
            List<OperationHistory> operations = null;
            Task.Run(async () => operations = await GetOperationsHistoryAsync())
                .Wait();

            return operations;
        }
        
        public Positions GetPortfolio()
        {
            Positions portfolio = null;
            Task.Run(async () => portfolio = await GetPortfolioAsync())
                .Wait();

            return portfolio;
        }
        
        public Stocks GetStocksRus()
        {
            Stocks stocks = null;
            Task.Run(async () => stocks = await GetStocksRusAsync())
                .Wait();

            return stocks;
        }

        public Stocks GetStocks()
        {
            Stocks stocks = null;
            Task.Run(async () => stocks = await GetStocksAsync())
                .Wait();

            return stocks;
        }

        public List<Candle> GetCandlesHistory(string figi, DateTime from, DateTime to)
        {
            List<Candle> candles = null;
            Task.Run(async () => candles = await GetCandlesHistoryAsync(figi, from, to))
                .Wait();

            return candles;
        }
        #endregion
        #endregion


        // private
        private Task HubConnection_Reconnected(string arg)
        {
            Reconnected?.Invoke(arg);

            return Task.CompletedTask;
        }

        private Task HubConnection_Closed(Exception arg)
        {
            Closed?.Invoke(arg);

            return Task.CompletedTask;
        }

        private void OnSend(string arg)
        {
            var candle = JsonConvert.DeserializeObject<Candle>(arg);
            var IsNew = (candle.time != LastCandleEvent) ? true : false;

            if (IsNew)
            {
                if (!candlesHistory.ContainsKey(candle.figi))
                {
                    candlesHistory.Add(candle.figi, new List<Candle>());
                }

                candlesHistory[candle.figi].Add(candle);
            }
            else
            {
                if (!candlesHistory.ContainsKey(candle.figi))
                {
                    candlesHistory.Add(candle.figi, new List<Candle>());
                }

                var item = candlesHistory[candle.figi];
                item[item.Count - 1] = candle;
            }

            CandleEvent?.Invoke(candle, IsNew);


            LastCandleEvent = candle.time;
        }

        public async Task DisposeAsync()
        {
            await hubConnection.StopAsync();
            await hubConnection.DisposeAsync();
            _client.Dispose();
        }

        public void Dispose()
        {
            Task.Run(async () => await DisposeAsync())
                .Wait();
        }
    }
}
