namespace ARK_Invest_Bot
{
    /// <summary>
    /// A transaction from an ARK fund
    /// </summary>
    public class Trade
    {
        public string Fund { get; set; }
        public string Date { get; set; }
        public string Direction { get; set; }
        public string Ticker { get; set; }
        public string Shares { get; set; }
        public string PercentOfEtf { get; set; }
    }
}