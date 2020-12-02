using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TinkoffApi.Data.Enums;

namespace TinkoffApi.Data.Entities
{
    public class PortfolioHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public double balance { get; set; }
        public CurrencyEnum currency { get; set; }
        public DateTime time { get; set; }
    }
}
