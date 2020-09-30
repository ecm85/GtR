using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace GtR
{
    class Program
    {
		private static bool useOverlay = false;

        static void Main(string[] args)
        {
            var allSuits = Enum.GetValues(typeof(CardSuit))
                .Cast<CardSuit>()
                .ToList();
            var imageCreator = new GloryToRomeImageCreator();


            var orderCards = ReadOrderCards();
            var orderCardFrontImages = orderCards.SelectMany(orderCard => CreateCardsForOrderCard(imageCreator, orderCard)).ToList();
            var orderCardBackImage = imageCreator.CreateOrderCardBack();

            var siteFrontImages = allSuits.SelectMany(suit => Enumerable.Range(0, 3).Select(index => imageCreator.CreateSiteFront(suit))).ToList();
            var siteBackImages = allSuits.SelectMany(suit => Enumerable.Range(0, 3).Select(index => imageCreator.CreateSiteBack(suit))).ToList();
            foreach (var siteBackImage in siteBackImages)
                siteBackImage.Bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);

            var dateStamp = DateTime.Now.ToString("yyyyMMddTHHmmss");
            Directory.CreateDirectory($"c:\\delete\\images\\{dateStamp}");

            var pages = new List<Page>();

            var remainingOrderCards = orderCardFrontImages.ToList();
            while(remainingOrderCards.Any())
            {
                var page = new Page($"OrderCards_{pages.Count}");
                var cardsAdded = page.AddCardsToPage(remainingOrderCards);
                remainingOrderCards = remainingOrderCards.Skip(cardsAdded).ToList();
                pages.Add(page);
            }

            var pageOfOrderBackImages = Enumerable.Repeat(orderCardBackImage, Page.cardsPerColumn * Page.cardsPerRow).ToList();
            var orderBackPage = new Page("OrderCardBack");
            orderBackPage.AddCardsToPage(pageOfOrderBackImages);
            pages.Add(orderBackPage);

            var siteFrontPage = new Page("SiteFront");
            siteFrontPage.AddCardsToPage(siteFrontImages);
            pages.Add(siteFrontPage);

            var siteBackPage = new Page("SiteBack");
            siteBackPage.AddCardsToPage(siteBackImages);
            pages.Add(siteBackPage);

            if (useOverlay)
            {
                var overlay = new Bitmap(@"Images\Misc\Poker Cards (2-5x3-5) 18 per sheet.png");
                overlay.SetResolution(300, 300);
                var landscapeOverlay = new Bitmap(overlay);
                landscapeOverlay.RotateFlip(RotateFlipType.Rotate90FlipNone);
                var matrix = new ColorMatrix { Matrix33 = .5f };
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                foreach (var page in pages)
                {
                    var graphics = Graphics.FromImage(page.Bitmap);
                    if (page.Bitmap.Width < page.Bitmap.Height)
                    {
                        graphics.DrawImage(overlay, new Rectangle(0, 0, overlay.Width, overlay.Height), 0, 0, overlay.Width, overlay.Height, GraphicsUnit.Pixel, attributes);
                    }
                    else
                    {
                        graphics.DrawImage(landscapeOverlay, new Rectangle(0, 0, landscapeOverlay.Width, landscapeOverlay.Height), 0, 0, landscapeOverlay.Width, landscapeOverlay.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
            }

            foreach (var page in pages)
                page.Bitmap.Save($"c:\\delete\\images\\{dateStamp}\\{page.Name}.png", ImageFormat.Png);
        }

        private static IEnumerable<CardImage> CreateCardsForOrderCard(GloryToRomeImageCreator imageCreator, OrderCard orderCard)
        {
            return Enumerable.Range(0, orderCard.CardSuit.CardCountPerDeck())
                .Select(index => imageCreator.CreateOrderCardFront(orderCard, index))
                .ToList();
        }

        private static IList<OrderCard> ReadOrderCards()
        {
            var strings = File.ReadAllLines("OrderCardData.txt");
            return strings
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split('\t'))
                .Select(tokens => new OrderCard
                {
                    CardName = tokens[0],
                    CardSuit = GetCardSuitFromMaterialText(tokens[1]),
                    CardText = tokens[2],
                    CardSet = GetCardSetFromText(tokens[3]),
                    ImageIsRoughlyCentered = bool.Parse(tokens[4])
                })
                .ToList();
        }

        public static CardSuit GetCardSuitFromMaterialText(string materialText)
        {
            switch(materialText)
            {
                case "Brick":
                    return CardSuit.RedLegionaryBrick;
                case "Concrete":
                    return CardSuit.GreyArchitectConcrete;
                case "Marble":
                    return CardSuit.PurplePatronMarble;
                case "Stone":
                    return CardSuit.BlueMerchantStone;
                case "Rubble":
                    return CardSuit.YellowLaborerRubble;
                case "Wood":
                    return CardSuit.GreenCraftsmanWood;
                default:
                    throw new InvalidOperationException($"Invalid material text encountered: {materialText}.");
            }
        }

        public static CardSet GetCardSetFromText(string text)
        {
            switch(text)
            {
                case "Standard":
                    return CardSet.Standard;
                case "Republic":
                    return CardSet.Republic;
                case "Imperium":
                    return CardSet.Imperium;
                default:
                    throw new InvalidOperationException($"Invalid card set encountered: {text}.");
            }
        }
    }
}
