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
            var orderCardBackImage = new [] {imageCreator.CreateOrderCardBack()};

            var siteFrontImages = allSuits.SelectMany(suit => Enumerable.Range(0, 2).Select(index => imageCreator.CreateSiteFront(suit, index))).ToList();

            var dateStamp = DateTime.Now.ToString("yyyyMMddTHHmmss");
            Directory.CreateDirectory($"c:\\delete\\images\\{dateStamp}");

            var allImages = orderCardFrontImages
                .Concat(orderCardBackImage)
                .Concat(siteFrontImages)
                .ToList();

            if (useOverlay)
            {
                var overlay = new Bitmap("c:\\delete\\poker-card.png");
                overlay.SetResolution(300, 300);
                var landscapeOverlay = new Bitmap(overlay);
                landscapeOverlay.RotateFlip(RotateFlipType.Rotate90FlipNone);
                var matrix = new ColorMatrix { Matrix33 = .5f };
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                foreach (var image in allImages)
                {
                    var graphics = Graphics.FromImage(image.Bitmap);
                    if (image.Bitmap.Width < image.Bitmap.Height)
                    {
                        graphics.DrawImage(overlay, new Rectangle(0, 0, overlay.Width, overlay.Height), 0, 0, overlay.Width, overlay.Height, GraphicsUnit.Pixel, attributes);
                    }
                    else
                    {
                        graphics.DrawImage(landscapeOverlay, new Rectangle(0, 0, landscapeOverlay.Width, landscapeOverlay.Height), 0, 0, landscapeOverlay.Width, landscapeOverlay.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
            }

            foreach (var image in allImages)
                image.Bitmap.Save($"c:\\delete\\images\\{dateStamp}\\{image.Name}.png", ImageFormat.Png);
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
