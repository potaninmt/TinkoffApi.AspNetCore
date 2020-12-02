using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TinkoffApi.Data.Enums;
using TinkoffApi.Data.Models;
using TinkoffApi.Data.Models.StopLoss;
using TinkoffApi.Data.Models.TakeProfit;

namespace TinkoffApi.Data.Entities
{
    public class OperationHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string orderId { get; set; }
        public OrderType orderType { get; set; }
        public Mode orderMode { get; set; }
        public int lots { get; set; }
        public double Price { get; set; }
        public DateTime time { get; set; }
        public string figi { get; set; }
        public OperationStatus operationStatus { get; set; }
        public MoneyAmount Commission { get; set; }
        public Status Status { get; set; }
        public StopLossData StopLoss { get; set; }
        public TakeProfitData TakeProfit { get; set; }
        public TimeSpan Expire { get; set; }
    }
}
