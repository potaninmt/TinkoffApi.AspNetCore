using System;
using TinkoffApi.Data.Enums;

namespace TinkoffApi.Data.Models.StopLoss
{
    public class StopLoss : StopLossData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value">(0; +inf)</param>
        /// <param name="Procent">(0; 100)</param>
        public StopLoss(double Value = 0, double Procent = 0) : base()
        {
            if (Value != 0 && Procent != 0)
                throw new ArgumentException("only Value or Procent");

            if (Value != 0)
            {
                if (Value < 0)
                    throw new ArgumentException("Value must be positive value");

                this.formatValue = FormatValue.Value;
            }
            else
            {
                if (Procent < 0 || Procent > 100)
                    throw new ArgumentException("Procent must be around (0;100)");

                this.formatValue = FormatValue.Procent;
            }

            this.Value = Value;
            this.Procent = Procent;

        }

        public FormatValue GetFormat()
        {
            return formatValue;
        }

        public StopLoss GetBase()
        {
            return this;
        }
    }
}
