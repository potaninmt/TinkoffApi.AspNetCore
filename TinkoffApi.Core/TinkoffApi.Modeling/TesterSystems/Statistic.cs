namespace TinkoffApi.Modeling.TesterSystems
{
    public class Statistic
    {
        public int Pos { get; set; }
        public int N { get; set; }
        public double Total { get; set; }
        public double Reward { get; set; }
        public double SL { get; set; }
        public double TP { get; set; }

        public int W { get; set; }
        public int HistLen { get; set; }

        public double Accuracy()
        {
            return (double)Pos / N;
        }
    }
}
