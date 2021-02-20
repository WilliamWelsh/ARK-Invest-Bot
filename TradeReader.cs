using System;
using System.Collections.Generic;

namespace ARK_Invest_Bot
{
    /// <summary>
    /// This will convert the ARK email into a list of trade objects
    /// </summary>
    public static class TradeReader
    {
        public static List<Trade> ReadTrades(string email)
        {
            // Scrape the email
            var trades = new List<Trade>();

            var date = DateTime.Now.ToString("MM/dd");

            email = email.CutBeforeAndAfter("</tr>", "</table>");

            do
            {
                email = email.CutBefore("<td>").CutBefore("<td>");

                var fundName = email.CutAfter("</td>");

                email = email.CutBefore("<td>").CutBefore("<td>");
                var direction = email.CutAfter("</td>");

                email = email.CutBefore("<td>");
                var ticker = email.CutAfter("</td>");

                email = email.CutBefore("<td>").CutBefore("<td>").CutBefore("align='right'>");
                var shares = email.CutAfter("</td>");

                email = email.CutBefore("align='right'>");
                var percentOfEtf = email.CutAfter("</td>");

                trades.Add(new Trade
                {
                    Fund = fundName,
                    Date = date,
                    Direction = direction,
                    Ticker = ticker,
                    Shares = shares,
                    PercentOfEtf = percentOfEtf
                });
            } while (email.Contains("<td>"));

            return trades;
        }
    }
}