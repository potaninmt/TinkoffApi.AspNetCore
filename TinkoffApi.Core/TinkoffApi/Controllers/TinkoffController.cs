using AutoTrading.BL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using TinkoffApi.BL;
using TinkoffApi.Data.Enums;
using TinkoffApi.Data.Models.Body;

namespace TinkoffApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [SwaggerTag("Контроллер для работы с tinkoff open api")]
    public class TinkoffController : Controller
    {
        //const string figiDefault = "BBG006L8G4H1"; // BBG006L8G4H1
        private readonly ITinkoffService _tinkoffService;
        public TinkoffController(ITinkoffService tinkoffService)
        {
            _tinkoffService = tinkoffService;

        }

        /// <summary>
        /// Возвращает список всех стоков с описанием
        /// </summary>
        /// <returns></returns>
        [Route("stocks")]
        [HttpGet]
        public async Task<IActionResult> GetStocksAsync()
        {
            var result = await _tinkoffService.GetStocksAsync();

            return this.Ok(result);
        }

        /// <summary>
        /// Возвращает список российских стоков с описанием
        /// </summary>
        /// <returns></returns>
        [Route("stocks/rus")]
        [HttpGet]
        public async Task<IActionResult> GetStocksRusAsync()
        {
            var result = await _tinkoffService.GetStocksAsync();
            result.Transform(stock =>
            {
                if (stock.currency == CurrencyEnum.Rub) return stock;
                return null;
            });

            return this.Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("bonds")]
        [HttpGet]
        public async Task<IActionResult> GetMarketBondsAsync()
        {
            var result = await _tinkoffService.GetMarketBondsAsync();

            return this.Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("currencies")]
        [HttpGet]
        public async Task<IActionResult> GetMarketCurrenciesAsync()
        {
            var result = await _tinkoffService.GetMarketCurrenciesAsync();

            return this.Ok(result);
        }


        /// <summary>
        /// Новый ордер
        /// </summary>
        /// <returns></returns>
        [Route("placeMarketOrder")]
        [HttpPost]
        public async Task<IActionResult> PlaceMarketOrderAsync([FromBody] PlaceMarkerBody placeMarkerBody, string figi, int lots, int operationType)
        {
            try
            {
                var stopLoss = placeMarkerBody.stopLoss;
                var takeProfit = placeMarkerBody.takeProfit;

                var result = await _tinkoffService.PlaceMarketOrderAsync(figi, lots, (OperationType)operationType, stopLoss, takeProfit);

                return this.Ok(result);
            }
            catch (Exception e)
            {
                return this.StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }

        }

        /// <summary>
        /// Возвращает портфолио по заданной валюте
        /// </summary>
        /// <returns></returns>
        [Route("portfolioCurrency")]
        [HttpGet]
        public async Task<IActionResult> GetPortfolioCurrency(int currency)
        {
            var result = await _tinkoffService.GetPortfolioCurrencyAsync((Currency)currency);

            return this.Ok(result);
        }

        /// <summary>
        /// Подписывается на обновления свечей
        /// </summary>
        /// <returns></returns>
        [Route("candleSubscribe")]
        [HttpGet]
        public async Task<IActionResult> CandleSubscribe(string figi)
        {
            _tinkoffService.CandleSubscribe(figi);

            return this.Ok();
        }

        /// <summary>
        /// Возвращает список активных акций(ордеров)
        /// </summary>
        /// <returns></returns>
        [Route("get/portfolio")]
        [HttpGet]
        public async Task<IActionResult> GetPortfolio()
        {
            var result = await _tinkoffService.GetPortfolioAsync();
            return Ok(result);
        }

        /// <summary>
        /// Возвращает историю свечей по figi
        /// </summary>
        /// <param name="figi"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="candleInterval"></param>
        /// <returns></returns>
        [Route("get/candles")]
        [HttpGet]
        public async Task<IActionResult> GetCandlesHistory(string figi, string from, string to, CandleInterval candleInterval = CandleInterval.Minute)
        {
            var type = "O";
            var result = await _tinkoffService.GetCandlesHistory(figi, DateTime.ParseExact(from, type, CultureInfo.InvariantCulture), DateTime.ParseExact(to, type, CultureInfo.InvariantCulture), candleInterval);
            return Ok(result);
        }


        //[Route("get/operation")]
        //[HttpGet]
        //public async Task<IActionResult> GetOperation(string figi, string operationId)
        //{
        //    var result = await _tinkoffService.GetOperation(figi, operationId);
        //    return Ok(result);
        //}

        [Route("get/operationhistory")]
        [HttpGet]
        public IActionResult GetOperationHistory(string figi, string operationId)
        {
            var result = _tinkoffService.GetOperationHistory(figi, operationId);
            return Ok(result);
        }

        [Route("get/operationshistory")]
        [HttpGet]
        public IActionResult GetOperationsHistory()
        {
            var result = _tinkoffService.GetOperationsHistory();

            return Ok(result);
        }

    }
}
