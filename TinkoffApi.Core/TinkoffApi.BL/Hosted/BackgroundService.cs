using AutoTrading.BL.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using TinkoffApi.BL.Buffers;
using TinkoffApi.BL.Cache;
using TinkoffApi.BL.Hosted.Models;
using TinkoffApi.BL.Hubs;
using TinkoffApi.BL.Infrastructure;
using TinkoffApi.DAL;
using TinkoffApi.Data.Extensions;

namespace TinkoffApi.BL.Hosted
{
    public class BackgroundService : IHostedService, IDisposable
    {
        private IServiceScopeFactory _scopeFactory { get; }
        private List<Timer> _timers { get; set; }

        private List<Task> _tasks { get; set; }

        private BufferFiggiesCandles bufferFiggiesCandles { get; set; }

        private bool IsStop { get; set; }

        private readonly IHubContext<TinkoffHub> _hubContext;

        private readonly IServiceScope _scope;

        private TinkoffApiContext _context;
        private ITinkoffService _service;

        public BackgroundService(IServiceScopeFactory scopeFactory, IHubContext<TinkoffHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _scope = _scopeFactory.CreateScope();
            bufferFiggiesCandles = new BufferFiggiesCandles();
            _hubContext = hubContext;
        }
        private async void Update(object state)
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timers = new List<Timer>();
            _tasks = new List<Task>();

            bufferFiggiesCandles = new BufferFiggiesCandles();
            _context = _scope.ServiceProvider.GetRequiredService<TinkoffApiContext>();
            _service = _scope.ServiceProvider.GetRequiredService<ITinkoffService>();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            IsStop = true;
            foreach (var _timer in _timers)
            {
                _timer?.Change(Timeout.Infinite, 0);
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _scope.Dispose();
            _context.Dispose();
            foreach (var _timer in _timers)
            {
                _timer?.Dispose();
            }
            foreach (var _task in _tasks)
            {
                _task.Dispose();
            }
            _tasks.Clear();
            _timers.Clear();
            _timers = null;
            _tasks = null;
            bufferFiggiesCandles = null;
        }

        //TODO переделать
        public void CandleSubscribe(CandleSubscribe candleSubscribe)
        {
            var contextApi = candleSubscribe.contextApi;
            contextApi.UseStreaming();
            contextApi.StreamingEventReceived += ContextApi_StreamingEventReceived;

            //int period = 7500; //ms
            //var timer = new Timer(TimerCandleSubscribe, candleSubscribe, 0, period);

            // _timers.Add(timer);

            Task.Run(async () => await contextApi.SendStreamingRequestAsync(StreamingRequest.SubscribeCandle(candleSubscribe.figi, candleSubscribe.candleInterval)));
        }

        public void TaskFindOperation(ContextApi contextApi, string figi, string operationId)
        {
            var task = Task.Run(async () =>
            {
                while (!IsStop)
                {
                    var operation = await contextApi.GetOperation(figi, operationId);
                    if (operation != null)
                    {
                        using (var contextDb = _scope.ServiceProvider.GetRequiredService<TinkoffApiContext>())
                        {
                            var operationDb = contextDb.OperationsHistory.Where(x => x.orderId == operationId).FirstOrDefault();
                            if (operation.Status == OperationStatus.Decline)
                            {
                                contextDb.Remove(operationDb);
                                return;
                            }

                            // get values
                            var orderType1 = operationDb.orderType;
                            var orderType2 = (orderType1 == Data.Enums.OrderType.Buy) ? Data.Enums.OrderType.Sell : Data.Enums.OrderType.Buy;
                            var price = (double)operation.Price;

                            var operationLastDb = contextDb.OperationsHistory.Where(x =>
                                                                                    x.figi == operationDb.figi &&
                                                                                    x.lots == operationDb.lots &&
                                                                                    x.operationStatus == Data.Enums.OperationStatus.Done &&
                                                                                    x.orderType == orderType2
                                                                                    ).FirstOrDefault();

                            //operationDb
                            if (operationLastDb != null)
                            {
                                operationDb.operationStatus = (Data.Enums.OperationStatus)operation.Status;
                                operationDb.Commission = new Data.Models.MoneyAmount()
                                {
                                    Currency = 0,// (Data.Enums.CurrencyEnum)operation.Commission.Currency,
                                    Value = 0//(double)operation.Commission.Value
                                };
                                operationDb.Price = price;
                                operationDb.Status = Data.Enums.Status.Open;

                                operationDb.TakeProfit = operationDb.TakeProfit.SetValue(price);
                                operationDb.StopLoss = operationDb.StopLoss.SetValue(price);

                                OperationsHistoryCache.AddOrUpdate(operationDb);
                            }

                            //operationLastDb
                            if (operationLastDb != null)
                            {
                                operationLastDb.Status = Data.Enums.Status.Close;
                            }

                            await contextDb.SaveChangesAsync();
                            return;
                        }
                    }
                    Thread.Sleep(5000);
                }
            });
            _tasks.Add(task);
        }

        private async void ContextApi_StreamingEventReceived(object sender, Tinkoff.Trading.OpenApi.Network.StreamingEventReceivedEventArgs e)
        {
            var response = (CandleResponse)e.Response;
            var payload = response.Payload;
            var close = (double)payload.Close;
            var serverTime = payload.Time;

            await _hubContext.Clients.All.SendAsync("Send", JsonConvert.SerializeObject(payload));

            var orders = OperationsHistoryCache.operations.Where(x =>
                x.operationStatus == Data.Enums.OperationStatus.Done &&
                x.orderType == Data.Enums.OrderType.Buy &&
                x.Status == Data.Enums.Status.Open
            ).ToArray();


            foreach (var order in orders)
            {
                var stopLoss = order.StopLoss.Value;
                var takeProfit = order.TakeProfit.Value;

                var time = order.time;
                var expire = order.Expire;
                time = time.AddSeconds(expire.TotalSeconds);

                if (close <= stopLoss || close >= takeProfit || serverTime >= time)
                {
                    await _service.PlaceMarketOrderAsync(order.figi, order.lots, OperationType.Sell, null, null);
                }
            }
        }

        private async void TimerCandleSubscribe(object obj)
        {
            var candleSubscribe = (CandleSubscribe)obj;
            var contextApi = candleSubscribe.contextApi;
            var figi = candleSubscribe.figi;
            var candleInterval = candleSubscribe.candleInterval;
            await contextApi.SendStreamingRequestAsync(StreamingRequest.SubscribeCandle(figi, candleInterval));
        }
    }
}
