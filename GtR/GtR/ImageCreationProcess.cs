using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace GtR
{
    public class ImageCreationProcess
    {
        public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static readonly bool useOverlay = false;

        public byte[] Run(GtrConfig gtrConfig)
        {
            var images = CreateImages(gtrConfig);

            if (useOverlay)
            {
                var overlay = new Bitmap(@"Images\Misc\Poker Cards (2-5x3-5) 18 per sheet.png");
                overlay.SetResolution(300, 300);
                var landscapeOverlay = new Bitmap(overlay);
                landscapeOverlay.RotateFlip(RotateFlipType.Rotate90FlipNone);
                var matrix = new ColorMatrix { Matrix33 = .5f };
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                foreach (var image in images)
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

            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var image in images)
                    {
                        var fileName = Path.Combine(image.Subfolder, $"{image.Name}.png");

                        var entry = zipArchive.CreateEntry(fileName);

                        using (var singleFileStream = new MemoryStream())
                        {
                            image.Bitmap.Save(singleFileStream, ImageFormat.Png);
                            using (var zipStream = entry.Open())
                                zipStream.Write(singleFileStream.ToArray());
                        }
                    }
                }
                return memoryStream.ToArray();
            }
        }

        private static IEnumerable<ISaveableImage> CreateImages(GtrConfig gtrConfig)
        {
            switch(gtrConfig.SaveConfiguration)
            {
                case SaveConfiguration.Page:
                    return CreatePages(gtrConfig);
                case SaveConfiguration.SingleImage:
                    return CreateIndividualImages(gtrConfig);
                default:
                    throw new InvalidOperationException($"Invalid save configuration encountered: {gtrConfig.SaveConfiguration}.");
            }
        }

        private static IEnumerable<ISaveableImage> CreateIndividualImages(GtrConfig gtrConfig)
        {
            var allSuits = Enum.GetValues(typeof(CardSuit))
                .Cast<CardSuit>()
                .ToList();
            var imageCreator = new GloryToRomeImageCreator(gtrConfig);

            var orderCards = ReadOrderCards();
            var orderCardFrontImages = orderCards.SelectMany(orderCard => CreateCardsForOrderCard(imageCreator, orderCard)).ToList();
            var orderCardBackImage = imageCreator.CreateOrderCardBack();

            var siteFrontImages = allSuits.Select(suit => imageCreator.CreateSiteFront(suit)).ToList();
            var siteBackImages = allSuits.Select(suit => imageCreator.CreateSiteBack(suit)).ToList();

            var jackImageFront = imageCreator.CreateJackImageSword();
            var jackImageBack = imageCreator.CreateJackImageQuill();

            var merchantBonusCards = allSuits.Select(suit => imageCreator.CreateMerchantBonusImage(suit)).ToList();

            var leaderImage = imageCreator.CreateLeaderImage();

            return orderCardFrontImages
                .Concat(new[] { orderCardBackImage })
                .Concat(siteFrontImages)
                .Concat(siteBackImages)
                .Concat(new [] { jackImageFront, jackImageBack})
                .Concat(merchantBonusCards)
                .Concat(new [] { leaderImage })
                .ToList();
        }

        private static IEnumerable<ISaveableImage> CreatePages(GtrConfig gtrConfig)
        {
            var allSuits = Enum.GetValues(typeof(CardSuit))
                .Cast<CardSuit>()
                .ToList();
            var imageCreator = new GloryToRomeImageCreator(gtrConfig);

            var orderCards = ReadOrderCards();
            var orderCardFrontImages = orderCards.SelectMany(orderCard => CreateCardsForOrderCard(imageCreator, orderCard)).ToList();
            var orderCardBackImage = imageCreator.CreateOrderCardBack();

            var siteFrontImages = allSuits.SelectMany(suit => Enumerable.Range(0, 3).Select(index => imageCreator.CreateSiteFront(suit))).ToList();
            var siteBackImages = allSuits.SelectMany(suit => Enumerable.Range(0, 3).Select(index => imageCreator.CreateSiteBack(suit))).ToList();
            foreach (var siteBackImage in siteBackImages)
                siteBackImage.RotateBitmap(RotateFlipType.Rotate180FlipNone);

            var jackImageFront = imageCreator.CreateJackImageSword();
            var jackImageBack = imageCreator.CreateJackImageQuill();
            jackImageBack.RotateBitmap(RotateFlipType.Rotate180FlipNone);

            var merchantBonusFrontCards = allSuits.Select(suit => imageCreator.CreateMerchantBonusImage(suit)).ToList();
            var merchantBonusBackCards = allSuits.Select(suit => imageCreator.CreateMerchantBonusImage(suit)).ToList();
            foreach (var merchantBonusBackCard in merchantBonusBackCards)
                merchantBonusBackCard.RotateBitmap(RotateFlipType.Rotate180FlipNone);

            var leaderImageFront = imageCreator.CreateLeaderImage();
            var leaderImageBack = imageCreator.CreateLeaderImage();
            leaderImageBack.RotateBitmap(RotateFlipType.Rotate180FlipNone);

            var pages = new List<Page>();

            var remainingOrderCards = orderCardFrontImages.ToList();
            while (remainingOrderCards.Any())
            {
                var page = new Page($"OrderCards_{pages.Count}", "Pages");
                var cardsAdded = page.AddCardsToPage(remainingOrderCards);
                remainingOrderCards = remainingOrderCards.Skip(cardsAdded).ToList();
                pages.Add(page);
            }

            var pageOfOrderBackImages = Enumerable.Repeat(orderCardBackImage, Page.cardsPerColumn * Page.cardsPerRow).ToList();
            var orderBackPage = new Page("OrderCardBack", "Pages");
            orderBackPage.AddCardsToPage(pageOfOrderBackImages);
            pages.Add(orderBackPage);

            var siteFrontPage = new Page("SiteFront", "Pages");
            siteFrontPage.AddCardsToPage(siteFrontImages);
            pages.Add(siteFrontPage);

            var siteBackPage = new Page("SiteBack", "Pages");
            siteBackPage.AddCardsToPage(siteBackImages);
            pages.Add(siteBackPage);

            var miscImagesFront = Enumerable.Repeat(jackImageFront, 6)
                .Concat(merchantBonusFrontCards)
                .Concat(Enumerable.Repeat(leaderImageFront, 3))
                .ToList();

            var miscFrontPage = new Page("MiscFront", "Pages");
            miscFrontPage.AddCardsToPage(miscImagesFront);
            pages.Add(miscFrontPage);

            var miscImagesBack = Enumerable.Repeat(jackImageBack, 6)
                .Concat(merchantBonusBackCards.Take(3).Reverse())
                .Concat(merchantBonusBackCards.Skip(3).Take(3).Reverse())
                .Concat(Enumerable.Repeat(leaderImageBack, 3))
                .ToList();

            var miscBackPage = new Page("MiscBack", "Pages");
            miscBackPage.AddCardsToPage(miscImagesBack);
            pages.Add(miscBackPage);

            return pages;
        }

        private static IEnumerable<CardImage> CreateCardsForOrderCard(GloryToRomeImageCreator imageCreator, OrderCard orderCard)
        {
            return Enumerable.Range(0, orderCard.CardSuit.CardCountPerDeck())
                .Select(index => imageCreator.CreateOrderCardFront(orderCard, index))
                .ToList();
        }

        private static IList<OrderCard> ReadOrderCards()
        {
            var strings = File.ReadAllLines(Path.Combine(CurrentPath, "OrderCardData.txt"));
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
