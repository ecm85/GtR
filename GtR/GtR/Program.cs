using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GtR
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentProcessId();
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        private static readonly bool useOverlay = false;

        private static readonly IDictionary<OSPlatform, string> RootDirectoryByPlatform = new Dictionary<OSPlatform, string>
        {
            [OSPlatform.Windows] = $"c:{Path.DirectorySeparatorChar}",
            [OSPlatform.OSX] = $@"{Path.DirectorySeparatorChar}tmp"
        };

        public static void Main(string[] args)
        {
            try
            {
                var allPlatforms = new[] { OSPlatform.Windows, OSPlatform.Linux, OSPlatform.OSX, OSPlatform.FreeBSD };
                var currentPlatforms = allPlatforms
                    .Where(platform => RuntimeInformation.IsOSPlatform(platform))
                    .ToList();
                if (currentPlatforms.Count != 1)
                    throw new InvalidOperationException("Cannot determine OS.");
                var currentPlatform = currentPlatforms.Single();
                var supportedPlatforms = RootDirectoryByPlatform.Keys;
                if (!RootDirectoryByPlatform.Keys.Contains(currentPlatform))
                    throw new InvalidOperationException("Current OS is not supported.");

                var saveConfiguration = GtrConfig.Current.SaveConfiguration;
                Console.WriteLine($"Cards will be {CardImage.cardShortSideInInches} inches x {CardImage.cardLongSideInInches} inches.");
                Console.WriteLine($"Cards will have a bleed size of {CardImage.bleedSizeInInches} inches and margin of {CardImage.borderPaddingInInches} inches.");
                Console.WriteLine($"Saving images as {saveConfiguration.GetDisplayValue()}.");
                Console.WriteLine($"Creating images.");

                var images = CreateImages(saveConfiguration);

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

                var dateStamp = DateTime.Now.ToString("yyyyMMddTHHmmss");
                var root = RootDirectoryByPlatform[currentPlatform];
                var directory = Path.Combine(root, "delete", "images", dateStamp);

                Console.WriteLine($"Saving images to {directory}.");

                Directory.CreateDirectory(directory);

                var subdirectories = images
                    .Select(image => image.Subfolder)
                    .Distinct()
                    .ToList();
                foreach (var subdirectory in subdirectories)
                {
                    Directory.CreateDirectory(Path.Combine(directory, subdirectory));
                }

                foreach (var image in images)
                    image.Bitmap.Save(Path.Combine(directory, image.Subfolder, $"{image.Name}.png"), ImageFormat.Png);

                Console.WriteLine("Done!");
                HandleAppExiting();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine("Debug Info:");
                Console.WriteLine(exception);
                if (exception.InnerException != null)
                    Console.WriteLine(exception.InnerException);
                if (exception.InnerException?.InnerException != null)
                    Console.WriteLine(exception.InnerException.InnerException);
                HandleAppExiting();
            }
        }

        private static void HandleAppExiting()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var hConsole = GetConsoleWindow();
                GetWindowThreadProcessId(hConsole, out var hProcessId);
                if (GetCurrentProcessId().Equals(hProcessId))
                {
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        private static IEnumerable<ISaveableImage> CreateImages(SaveConfiguration saveConfiguration)
        {
            switch(saveConfiguration)
            {
                case SaveConfiguration.Page:
                    return CreatePages();
                case SaveConfiguration.SingleImage:
                    return CreateIndividualImages();
                default:
                    throw new InvalidOperationException($"Invalid save configuration encountered: {saveConfiguration}.");
            }
        }

        private static IEnumerable<ISaveableImage> CreateIndividualImages()
        {
            var allSuits = Enum.GetValues(typeof(CardSuit))
                .Cast<CardSuit>()
                .ToList();
            var imageCreator = new GloryToRomeImageCreator();

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

        private static IEnumerable<ISaveableImage> CreatePages()
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

            var jackImageFront = imageCreator.CreateJackImageSword();
            var jackImageBack = imageCreator.CreateJackImageQuill();
            jackImageBack.Bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);

            var merchantBonusFrontCards = allSuits.Select(suit => imageCreator.CreateMerchantBonusImage(suit)).ToList();
            var merchantBonusBackCards = allSuits.Select(suit => imageCreator.CreateMerchantBonusImage(suit)).ToList();
            foreach (var merchantBonusBackCard in merchantBonusBackCards)
                merchantBonusBackCard.Bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);

            var leaderImageFront = imageCreator.CreateLeaderImage();
            var leaderImageBack = imageCreator.CreateLeaderImage();
            leaderImageBack.Bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);

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
            var strings = File.ReadAllLines($".{Path.DirectorySeparatorChar}OrderCardData.txt");
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
