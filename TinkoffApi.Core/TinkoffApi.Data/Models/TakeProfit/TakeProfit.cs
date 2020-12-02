using System;
using TinkoffApi.Data.Enums;

namespace TinkoffApi.Data.Models.TakeProfit
{
    public class TakeProfit : TakeProfitData
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value">(0; +inf)</param>
        /// <param name="Procent">(0; +inf)</param>
        public TakeProfit(double Value = 0, double Procent = 0) : base()
        {
            if (Value != 0 && Procent != 0)
                throw new ArgumentException("only Value or Procent");

            if (Value != 0)
            {
                if (Value < 0)
                    throw new ArgumentException("Value must be positive value");

                formatValue = FormatValue.Value;
            }
            else
            {
                if (Procent < 0)
                    throw new ArgumentException("Procent must be positive value");

                formatValue = FormatValue.Procent;
            }

            this.Value = Value;
            this.Procent = Procent;

        }

        public FormatValue GetFormat()
        {
            return formatValue;
        }

        public TakeProfitData GetBase()
        {
            return this;
        }
    }
}
