using TinkoffApi.Data.Models;

namespace TinkoffApi.Modeling.TesterSystems
{
    public static class MethodExtensions
    {
        public static double[] GetSeq(this double[] x, int ind, int len, int scale = 1)
        {
            var result = new double[len / scale];
            int c = 0;
            for (int i = 0; i < len; i += scale)
            {
                result[c++] = x[ind + i];
            }

            return result;
        }

        /// <summary>
        /// Возвращает подпоследовательность значений закрытия свечи [ind; ind + len)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="ind"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static double[] GetSeqClose(this Candle[] x, int ind, int len)
        {
            var result = new double[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = x[ind + i].close;
            }

            return result;
        }
    }
}
