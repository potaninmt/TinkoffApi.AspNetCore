namespace TinkoffApi.Modeling.TesterSystems
{
    public class SLTP
    {
        public double tProb;
        public double sProb;

        public SLTP(double tProcent, double sProcent)
        {
            this.tProb = tProcent / 100.0;
            this.sProb = sProcent / 100.0;
        }

        public double GetValue(double[] x, double valOrder, int ind, int len)
        {
            var tP = valOrder * (1.0 + tProb);
            var sL = valOrder * (1.0 - sProb);
            var value = 0.0;
            for (int i = ind; i < ind + len; i++)
            {
                var v = x[i];
                if (v >= tP)
                {
                    value = v;

                    break;
                }
                else if (v <= sL)
                {
                    value = v;

                    break;
                }
            }

            if (value == 0.0) value = x[ind + len - 1];

            return value;
        }
    }
}
