namespace TinkoffApi.Modeling.TesterSystems
{
    public class Reward
    {
        private double q;
        private double qq;
        private double comission;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comission">Procent</param>
        public Reward(double comission = 0.05)
        {
            this.comission = comission;
            var p = comission * 0.01 /*procent to probability*/;
            q = 1.0 - p;
            qq = 1.0 + p;
        }
        public double GetReward(double valueBuy, double valueSell, int n = 1)
        {
            var reward = q * valueSell - qq * valueBuy;

            return n * reward;
        }
    }
}
