using SixLabors.Fonts;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ARK_Invest_Bot
{
    /// <summary>
    /// Convert a list of transactions into an image.
    /// </summary>
    public static class ImageGenerator
    {
        // Custom red color
        private static readonly Color Red = new(new Rgb24(231, 76, 60));

        // Arial Font
        private static readonly Font Font = SystemFonts.CreateFont("Arial", 20, FontStyle.Regular);

        // Renderer Options for the Font
        private static RendererOptions rendererOptions = new RendererOptions(Font);

        // Measure string size
        private static FontRectangle Measure(string text) => TextMeasurer.Measure(text, rendererOptions);

        // Draw text onto the result
        private static void DrawData(this Image<Rgba32> image, string text, float x, float row)
        {
            var size = Measure(text);
            image.Mutate(i => i.DrawText(text, Font, Color.White, new PointF(x - size.Width / 2, 10 + row * 25 - size.Height / 2)));
        }

        // Convert a list of trade objects into a pretty image
        public static void MakeImage(List<Trade> trades)
        {
            // FUND | DATE | TRADE DIRECTION | TICKER | # SHARES | % OF ETF
            var columns = 6;

            // Add 1 to trades.count for the top header row
            using (var image = new Image<Rgba32>(80 * columns, (trades.Count + 1) * 25, new Rgba32(10, 10, 35)))
            {
                // Headers
                image.DrawData("Fund", 40, 0);
                image.DrawData("Date", 120, 0);
                image.DrawData("Side", 200, 0);
                image.DrawData("Ticker", 280, 0);
                image.DrawData("Shares", 360, 0);
                image.DrawData("% ETF", 440, 0);

                for (int i = 0; i < trades.Count; i++)
                {
                    // Get the current trade to draw
                    var trade = trades[i];

                    // Determine background color based on trade direction
                    image.Mutate(x => x.Fill(trade.Direction == "Buy" ? Color.Green : Red, new RectangleF(160, (i + 1) * 25, 80, 30)));

                    // Fund Name
                    image.DrawData(trade.Fund, 40, i + 1);

                    // Date
                    image.DrawData(trade.Date, 120, i + 1);

                    // Trade Direction
                    image.DrawData(trade.Direction, 200, i + 1);

                    // Ticker
                    image.DrawData(trade.Ticker, 280, i + 1);

                    // # of Shares
                    image.DrawData(trade.Shares, 360, i + 1);

                    // % of ETF
                    image.DrawData(trade.PercentOfEtf, 440, i + 1);
                }

                // Save the image
                image.Save("ark.png");
            }
        }
    }
}