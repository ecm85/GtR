using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GtR
{
    public class GloryToRomeImageCreator
    {
        private const int orderCardHeaderFontSize = (int)(18.5 * GraphicsUtilities.dpiFactor);
        private const int orderCardTextFontSize = (int)(11 * GraphicsUtilities.dpiFactor);
        private const int siteCardCostTextFontSize = (int)(13 * GraphicsUtilities.dpiFactor);
        
        private const float InfluenceImagePercentage = .13f;
        private const float RoleIconPercentage = .18f;
        private const float SiteCostRegionHeightPercentage = .36f;
        private const float SiteResourceHeightPercentage = .35f;
        private const float SiteResourceSectionPaddingPercentage = .15f;
        private const float SetIndicatorPercentage = .10f;
        private const float CenteredImageOffsetPercentage = (RoleIconPercentage - SetIndicatorPercentage)/2;
        private const float SiteCoinWidthPercentage = .2f;
        private const float SiteCoinPaddingPercentage = .05f;
        private const int diagonalLinesPerCard = 16;
        private const float JackImageHeightOffsetPercentage = .30f;

        private int CardTextWidth(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * (1 - (RoleIconPercentage + SetIndicatorPercentage)));
        private int RoleIconWidth(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * RoleIconPercentage);

        private readonly Font CardTextFont;
        private readonly Font BoldCardTextFont;

        private GtrConfig GtrConfig { get; }

        public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private readonly FontFamily boldFontFamily;
        private readonly FontFamily regularFontFamily;


        public CardImage CreateJackImageSword()
        {
            return CreateJackImage("JackSword", "JackSword");
        }

        public CardImage CreateJackImageQuill()
        {
            return CreateJackImage("JackQuill", "JackQuill");
        }

        public GloryToRomeImageCreator(GtrConfig gtrConfig)
        {
            GtrConfig = gtrConfig;
            var fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(Path.Combine(CurrentPath, "Fonts", "NeuzeitGro-BolModified.ttf"));
            fontCollection.AddFontFile(Path.Combine(CurrentPath, "Fonts", "NeuzeitGro-RegModified.ttf"));
            regularFontFamily = fontCollection.Families.Single(family => family.Name == "Neuzeit Grotesk");
            boldFontFamily = fontCollection.Families.Single(family => family.Name == "Neuzeit Grotesk Bold");
            CardTextFont = new Font(regularFontFamily, orderCardTextFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            BoldCardTextFont = new Font(boldFontFamily, orderCardTextFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        private CardImage CreateJackImage(string path, string fileName)
        {
            var cardImage = new CardImage(GtrConfig, fileName, "Misc", ImageOrientation.Portrait);
            var graphics = Graphics.FromImage(cardImage.Bitmap);
            var fullRectangle = cardImage.FullRectangle;
            var usableRectangle = cardImage.UsableRectangle;
            cardImage.PrintCardBorderAndBackground(Color.Black, Color.Black);
            var imageOffset = (int)(fullRectangle.Height * JackImageHeightOffsetPercentage);
            var imageHeight = GraphicsUtilities.PrintFullWidthPng(graphics, "Misc", path, fullRectangle.X, fullRectangle.Y + imageOffset, fullRectangle.Width);
            var xOffset = 0;
            var yOffset = (int)(usableRectangle.Width * .15f);
            var brush = new SolidBrush(Color.FromArgb(208, 208, 208));
            PrintCardName("Jack", cardImage, brush, 0, xOffset, usableRectangle.Width, yOffset);
            var bottomOfImage = fullRectangle.Top + imageOffset + imageHeight;
            PrintCardText(
                "Lead or follow any role",
                cardImage,
                bottomOfImage,
                usableRectangle.Width,
                usableRectangle.Bottom - bottomOfImage,
                xOffset,
                0,
                brush);
            return cardImage;
        }

        public CardImage CreateLeaderImage()
        {
            var name = "Leader";
            var cardImage = new CardImage(GtrConfig, name, "Misc", ImageOrientation.Portrait);
            cardImage.PrintCardBorderAndBackground(Color.White, Color.White);
            var graphics = Graphics.FromImage(cardImage.Bitmap);
            var fullRectangle = cardImage.FullRectangle;
            var imageWidth = fullRectangle.Width;
            var imageHeight = GraphicsUtilities.PrintFullWidthPng(
                graphics,
                "Misc",
                "Leader",
                fullRectangle.X,
                fullRectangle.Y,
                imageWidth);
            var bottomOfImage = fullRectangle.Y + imageHeight;

            var imageXOffset = 0;
            var maxTextBoxWidth = cardImage.UsableRectangle.Width;
            var yOffset = (int)(cardImage.UsableRectangle.Width * .05f);
            PrintCardName("Leader", cardImage, GraphicsUtilities.BlackBrush, 100, imageXOffset, maxTextBoxWidth, yOffset);

            var usableRectangle = cardImage.UsableRectangle;
            var textBoxWidth = cardImage.UsableRectangle.Width;
            var influenceImageSide = InfluenceImageSide(cardImage);
            var textRectangleHeight = usableRectangle.Bottom - (influenceImageSide + bottomOfImage);
            var top = bottomOfImage;
            var textXOffset = 0;
            PrintCardText("LEAD |a role from your hand |or |THINK |and draw new cards", cardImage, top, textBoxWidth, textRectangleHeight, textXOffset, 200, GraphicsUtilities.BlackBrush);

            return cardImage;
        }

        internal CardImage CreateMerchantBonusImage(CardSuit suit)
        {
            var cardImage = new CardImage(GtrConfig, $"MerchantBonus_{suit.ResourceName()}", "Misc", ImageOrientation.Portrait);
            var graphics = Graphics.FromImage(cardImage.Bitmap);
            cardImage.PrintCardBorderAndBackground(Color.White, Color.White);
            var fullRectangle = cardImage.FullRectangle;
            var usableRectangle = cardImage.UsableRectangle;
            var xOffset = usableRectangle.Width / 5;
            var yOffset = (int)(usableRectangle.Width * .05f);
            PrintCardName("Merchant Bonus", cardImage, GraphicsUtilities.BlackBrush, 0, xOffset, 3 * usableRectangle.Width / 5, yOffset);
            var costImagePaddingHeight = (int)(fullRectangle.Height * SiteResourceSectionPaddingPercentage);
            var costImageTopPaddingHeight = costImagePaddingHeight / 2;
            var costImageBottomPaddingHeight = costImagePaddingHeight / 2;
            var costImageSectionHeight = (int)(fullRectangle.Height * SiteResourceHeightPercentage);
            var costImageHeight = costImageSectionHeight - costImagePaddingHeight;
            var costImageRectangle = new Rectangle(
                usableRectangle.X,
                fullRectangle.Y + (fullRectangle.Bottom - ((int)(fullRectangle.Height * .30f) + costImageBottomPaddingHeight + costImageHeight)),
                usableRectangle.Width,
                costImageHeight);
            //graphics.FillRectangle(new SolidBrush(Color.Blue), costImageRectangle);
            GraphicsUtilities.PrintScaledAndCenteredPng(
                graphics,
                "Resources",
                suit.ResourceName(),
                costImageRectangle.X,
                costImageRectangle.Y,
                costImageRectangle.Width,
                costImageRectangle.Height);
            PrintCardText(
                $"+3 victory points for |the player with the most |{suit.ResourceName().ToUpper()} in his vault at |the end of the game.",
                cardImage,
                fullRectangle.Bottom - ((int)(fullRectangle.Height * .35f)),
                usableRectangle.Width,
                (int)(fullRectangle.Height * .25f),
                0,
                0,
                GraphicsUtilities.BlackBrush);

            var centerPoint = usableRectangle.X + usableRectangle.Width / 2;

            var coinOffsetCount = -1.5f;
            var siteCoinPadding = SiteCoinPadding(cardImage);
            var siteCoinWidth = SiteCoinWidth(cardImage);
            var coinXOffset = (int)(coinOffsetCount * siteCoinWidth + (coinOffsetCount + .5f) * siteCoinPadding);

            for (var i = 0; i < 3; i++)
                GraphicsUtilities.PrintScaledPng(
                    graphics,
                    "Misc",
                    "Coin",
                    centerPoint + coinXOffset + (i * (siteCoinWidth + siteCoinPadding)),
                    usableRectangle.Y + (int)(usableRectangle.Width * .26f),
                    siteCoinWidth,
                    siteCoinWidth);

            return cardImage;
        }

        private int InfluenceImageSide(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * InfluenceImagePercentage);
        private int CardNameWidth(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * (1 - (RoleIconPercentage + SetIndicatorPercentage)));
        private int CenteredImageOffset(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * CenteredImageOffsetPercentage);
        private int SiteCoinWidth(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * SiteCoinWidthPercentage);
        private int SiteCoinPadding(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * SiteCoinPaddingPercentage);

        public CardImage CreateSiteFront(CardSuit suit)
        {
            var name = $"{suit.ResourceName()}_Site";
            var cardImage = new CardImage(GtrConfig, name, "Sites", ImageOrientation.Portrait);
            var graphics = Graphics.FromImage(cardImage.Bitmap);
            var fullRectangle = cardImage.FullRectangle;
            var usableRectangle = cardImage.UsableRectangle;
            cardImage.PrintCardBorderAndBackground(Color.White, Color.White);

            DrawSiteCost(cardImage, suit);

            var costRegionHeight = (int)(fullRectangle.Height * SiteCostRegionHeightPercentage);
            var costImagePaddingHeight = (int)(fullRectangle.Height * SiteResourceSectionPaddingPercentage);
            var costImageTopPaddingHeight = costImagePaddingHeight / 2;
            var costImageBottomPaddingHeight = costImagePaddingHeight / 2;
            var costImageSectionHeight = (int)(fullRectangle.Height * SiteResourceHeightPercentage);
            var costImageHeight = costImageSectionHeight - costImagePaddingHeight;
            var costImageRectangle = new Rectangle(
                usableRectangle.X,
                fullRectangle.Y + (fullRectangle.Bottom - (costRegionHeight + costImageBottomPaddingHeight + costImageHeight)),
                usableRectangle.Width,
                costImageHeight);
            //graphics.FillRectangle(new SolidBrush(Color.Blue), costImageRectangle);
            GraphicsUtilities.PrintScaledAndCenteredPng(
                graphics,
                "Resources",
                suit.ResourceName(),
                costImageRectangle.X,
                costImageRectangle.Y,
                costImageRectangle.Width,
                costImageRectangle.Height);

            var cardNameFont = new Font(boldFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var text = AddFourPerEmSpaces(suit.ResourceName().ToUpper());
            var maxTextBoxWidth = usableRectangle.Width;
            var initialRectangle = new Rectangle(usableRectangle.X, usableRectangle.Y, maxTextBoxWidth, usableRectangle.Height);
            var textMeasurement = graphics.MeasureString(text, cardNameFont, new SizeF(initialRectangle.Width, initialRectangle.Height), GraphicsUtilities.HorizontalCenterAlignment);
            var textHeight = (int)Math.Ceiling(textMeasurement.Height);
            var yOffset = fullRectangle.Bottom - (costRegionHeight + costImageSectionHeight + textHeight);
            var textRectangle = new Rectangle(usableRectangle.X, usableRectangle.Y + yOffset, maxTextBoxWidth, textHeight);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.White)), textRectangle);
            //graphics.FillRectangle(new SolidBrush(Color.Blue), textRectangle);
            GraphicsUtilities.DrawString(graphics, text, cardNameFont, GraphicsUtilities.BlackBrush, textRectangle, GraphicsUtilities.HorizontalCenterAlignment);

            var centerPoint = usableRectangle.X + usableRectangle.Width / 2;

            var coinOffsetCount = 0f;
            switch(suit.Cost())
            {
                case 1:
                    coinOffsetCount = -.5f;
                    break;
                case 2:
                    coinOffsetCount = -1;
                    break;
                case 3:
                    coinOffsetCount = -1.5f;
                    break;
            }
            var siteCoinPadding = SiteCoinPadding(cardImage);
            var siteCoinWidth = SiteCoinWidth(cardImage);
            var xOffset = (int)(coinOffsetCount * siteCoinWidth + (coinOffsetCount + .5f) * siteCoinPadding);

            for(var i = 0; i < suit.Cost(); i++)
                GraphicsUtilities.PrintScaledPng(
                    graphics,
                    "Misc",
                    "Coin",
                    centerPoint + xOffset + (i * (siteCoinWidth + siteCoinPadding)),
                    usableRectangle.Y,
                    siteCoinWidth,
                    siteCoinWidth);

            return cardImage;
        }

        private string AddThreePerEmSpaces(string value)
        {
            var hairSpace = (char)0x2004;
            var withSpaces = string.Join(hairSpace.ToString(), value.ToCharArray());
            return $"{hairSpace}{withSpaces}{hairSpace}";
        }

        private string AddFourPerEmSpaces(string value)
        {
            var hairSpace = (char)0x2005;
            var withSpaces = string.Join(hairSpace.ToString(), value.ToCharArray());
            return $"{withSpaces}{hairSpace}";
        }

        //Thin
        private string AddFivePerEmSpaces(string value)
        {
            var hairSpace = (char)0x2009;
            var withSpaces = string.Join(hairSpace.ToString(), value.ToCharArray());
            return $"{hairSpace}{withSpaces}{hairSpace}";
        }

        //SixPerEm
        private string AddSixPerEmSpaces(string value)
        {
            var hairSpace = (char)0x2006;
            var withSpaces = string.Join(hairSpace.ToString(), value.ToCharArray());
            return $"{hairSpace}{withSpaces}{hairSpace}";
        }

        //Hair
        private string AddTwelvePerEmSpaces(string value)
        {
            var hairSpace = (char)0x200a;
            var withSpaces = string.Join(hairSpace.ToString(), value.ToCharArray());
            return $"{hairSpace}{withSpaces}{hairSpace}";
        }


        public CardImage CreateSiteBack(CardSuit suit)
        {
            var name = $"{suit.ResourceName()}_SiteBack";
            var cardImage = new CardImage(GtrConfig, name, "Sites", ImageOrientation.Portrait);
             var graphics = Graphics.FromImage(cardImage.Bitmap);
            var fullRectangle = cardImage.FullRectangle;
            var usableRectangle = cardImage.UsableRectangle;
            cardImage.PrintCardBorderAndBackground(Color.White, Color.White);

            var thickness = fullRectangle.Width / diagonalLinesPerCard;
            var width = (int)Math.Sqrt(Math.Pow(thickness, 2) / 2);
            var pen = new Pen(suit.Color(), width);
            const int extra = 25;
            for (var xOffset = -2 * fullRectangle.Width + 10; xOffset < fullRectangle.Width; xOffset += thickness * 2)
                graphics.DrawLine(pen, fullRectangle.Left + xOffset - extra, fullRectangle.Top - extra, fullRectangle.Left + fullRectangle.Height + xOffset + extra, fullRectangle.Bottom + extra);

            var cardNameFont = new Font(boldFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var costFont = new Font(regularFontFamily, siteCardCostTextFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            var resourceNameText = AddFourPerEmSpaces(suit.ResourceName().ToUpper());
            var outOfTownSiteText = AddFourPerEmSpaces("out of town site");
            var maxTextBoxWidth = usableRectangle.Width;
            var initialRectangle = new Rectangle(usableRectangle.X, usableRectangle.Y, maxTextBoxWidth, usableRectangle.Height);
            var resourceNameTextMeasurement = graphics.MeasureString(outOfTownSiteText, cardNameFont, new SizeF(initialRectangle.Width, initialRectangle.Height), GraphicsUtilities.HorizontalCenterAlignment);
            var outOfTownSiteTextMeasurement = graphics.MeasureString(outOfTownSiteText, cardNameFont, new SizeF(initialRectangle.Width, initialRectangle.Height), GraphicsUtilities.HorizontalCenterAlignment);
            var resourceNameTextHeight = (int)Math.Ceiling(resourceNameTextMeasurement.Height);
            var outOfTownSiteTextHeight = (int)Math.Ceiling(outOfTownSiteTextMeasurement.Height);
            var textHeight = resourceNameTextHeight + outOfTownSiteTextHeight;
            var yOffset = usableRectangle.Height/2 - textHeight/2;
            var resourceNameTextRectangle = new Rectangle(usableRectangle.X, usableRectangle.Y + yOffset - resourceNameTextHeight, maxTextBoxWidth, textHeight);
            var outOfTownSiteTextRectangle = new Rectangle(usableRectangle.X, usableRectangle.Y + yOffset, maxTextBoxWidth, textHeight);
            if (GtrConfig.ShowSiteBackTextBackground)
			{
                var backgroundRectangle = new Rectangle(usableRectangle.X + (int)(maxTextBoxWidth * .2), usableRectangle.Y + yOffset - (int)(resourceNameTextHeight * 1.5), (int) (maxTextBoxWidth * .6), resourceNameTextHeight + textHeight);
                graphics.FillRectangle(new SolidBrush(Color.White), backgroundRectangle);
            }
            //graphics.FillRectangle(new SolidBrush(Color.Blue), textRectangle);
            GraphicsUtilities.DrawString(graphics, resourceNameText, cardNameFont, GraphicsUtilities.BlackBrush, resourceNameTextRectangle, GraphicsUtilities.HorizontalCenterAlignment);
            GraphicsUtilities.DrawString(graphics, outOfTownSiteText, costFont, GraphicsUtilities.BlackBrush, outOfTownSiteTextRectangle, GraphicsUtilities.HorizontalCenterAlignment);

            return cardImage;
        }

        private void DrawSiteCost(CardImage cardImage, CardSuit suit)
        {
             var graphics = Graphics.FromImage(cardImage.Bitmap);
            var fullRectangle = cardImage.FullRectangle;
            var usableRectangle = cardImage.UsableRectangle;
            var costRegionHeight = (int)(fullRectangle.Height * SiteCostRegionHeightPercentage);
            var costRectangle = new Rectangle(fullRectangle.Left, fullRectangle.Bottom - costRegionHeight, fullRectangle.Width, costRegionHeight);
            var costRegion = new Region(costRectangle);
            var oldClip = graphics.Clip;
            graphics.Clip = costRegion;
            var barThickness = (int)(fullRectangle.Width / diagonalLinesPerCard);
            var strokeWidth = (int)Math.Sqrt(Math.Pow(barThickness, 2) / 2);
            var pen = new Pen(suit.Color(), strokeWidth);
            const int extra = 25;
            for (var xOffset = -2 * fullRectangle.Width + 10; xOffset < fullRectangle.Width; xOffset += barThickness * 2)
                graphics.DrawLine(pen, fullRectangle.Left + xOffset - extra, fullRectangle.Bottom + extra, fullRectangle.Left + fullRectangle.Height + xOffset + extra, fullRectangle.Top - extra);
            graphics.Clip = oldClip;
            var material = suit.Cost() > 1 ? "materials" : "material";
            var costText = AddFourPerEmSpaces($"foundation +{suit.Cost()} {material}");
            var costFont = new Font(regularFontFamily, siteCardCostTextFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            var costTextMeasurement = graphics.MeasureString(costText, costFont, fullRectangle.Width);
            var costTextWidth = (int)Math.Ceiling(costTextMeasurement.Width);
            var costTextHeight = (int)costTextMeasurement.Height;
            var costTextRectangle = new Rectangle(
                fullRectangle.Width / 2 - costTextWidth / 2,
                (int)(costRectangle.Top + costRectangle.Height * .1f),
                costTextWidth,
                costTextHeight);
            graphics.FillRectangle(new SolidBrush(Color.White), costTextRectangle);
            GraphicsUtilities.DrawString(graphics, costText, costFont, GraphicsUtilities.BlackBrush, costTextRectangle);
        }

        public CardImage CreateOrderCardBack()
        {
            var name = "OrderCardBack";
            var cardImage = new CardImage(GtrConfig, name, "OrderCards", ImageOrientation.Portrait);
             var graphics = Graphics.FromImage(cardImage.Bitmap);
            var usableRectangle = cardImage.UsableRectangle;
            cardImage.PrintCardBorderAndBackground(Color.Black, Color.Black);
            GraphicsUtilities.PrintScaledPng(
                graphics,
                "Misc",
                "GloryToRome",
                usableRectangle.X + (int)(usableRectangle.Width * .07f),
                usableRectangle.Y + (int)(usableRectangle.Height * .15f),
                (int)(usableRectangle.Width * (1 - (.07f * 2))),
                (int)(usableRectangle.Height * .3f));
            GraphicsUtilities.PrintScaledPng(
                graphics,
                "Misc",
                "GloryToRome",
                usableRectangle.X + (int)(usableRectangle.Width * .07f),
                usableRectangle.Bottom - (int)(usableRectangle.Height * (.15f + .3f)),
                (int)(usableRectangle.Width * (1 - (.07f * 2))),
                (int)(usableRectangle.Height * .3f),
                RotateFlipType.Rotate180FlipNone);
            GraphicsUtilities.PrintScaledPng(
                graphics,
                "Misc",
                "OrderBackSeparator",
                usableRectangle.X + (usableRectangle.Width/2 - (int)((usableRectangle.Width * .4f)/2)),
                usableRectangle.Y + (usableRectangle.Height/2 - (int)((usableRectangle.Height* .05f)/2)),
                (int)(usableRectangle.Width * .4f),
                (int)(usableRectangle.Height * .05f));
            return cardImage;
        }

        public CardImage CreateOrderCardFront(OrderCard orderCard, int index)
        {
            var name = $"{orderCard.CardName}_{index}";
            var cardImage = new CardImage(GtrConfig, name, "OrderCards", ImageOrientation.Portrait);
            cardImage.PrintCardBorderAndBackground(Color.White, Color.White);
            var bottomOfImage = PrintCardImage(orderCard, cardImage);
            PrintRoleIconAndName(orderCard, cardImage);
            PrintOrderCardName(orderCard, cardImage);
            PrintResourceType(orderCard, cardImage);
            PrintSetIndicator(orderCard, cardImage);
            PrintOrderCardText(orderCard, cardImage, bottomOfImage);
            PrintInfluence(orderCard, cardImage);
            return cardImage;
        }

        private void PrintOrderCardName(OrderCard orderCard, CardImage cardImage)
        {
            var xOffset = RoleIconWidth(cardImage);
            var maxTextBoxWidth = CardNameWidth(cardImage);
            var yOffset = (int)(cardImage.UsableRectangle.Width * .05f);
            PrintCardName(orderCard.CardName, cardImage, GraphicsUtilities.BlackBrush, 100, xOffset, maxTextBoxWidth, yOffset);
        }

        private void PrintCardName(string name, CardImage cardImage, Brush brush, int translucentBackgroundOpacity, int xOffset, int maxTextBoxWidth, int yOffset)
        {
             var graphics = Graphics.FromImage(cardImage.Bitmap);
            var usableRectangle = cardImage.UsableRectangle;
            var cardNameFont = new Font(boldFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var fragments = name.ToUpper().Split(" ")
               .Select((word, index) => new TextFragment
               {
                   Brush = brush,
                   Font = cardNameFont,
                   ForcesNewline = index > 0,
                   Text = AddThreePerEmSpaces(word)
               })
               .ToList();
            var textRectangle = new Rectangle(usableRectangle.X + xOffset, usableRectangle.Y + yOffset, maxTextBoxWidth, usableRectangle.Height);
            GraphicsUtilities.DrawFragmentsCentered(graphics, fragments, textRectangle, translucentBackgroundOpacity, false, false);
        }

        private int PrintCardImage(OrderCard orderCard, CardImage cardImage)
        {
             var graphics = Graphics.FromImage(cardImage.Bitmap);
            var fullRectangle = cardImage.FullRectangle;
            var imageWidth = fullRectangle.Width;
            var imageHeight = GraphicsUtilities.PrintFullWidthPng(
                graphics,
                "CardImages",
                orderCard.CardName,
                fullRectangle.X + (orderCard.ImageIsRoughlyCentered ? CenteredImageOffset(cardImage) : 0),
                fullRectangle.Y,
                imageWidth);
            return fullRectangle.Y + imageHeight;
        }

        private void PrintRoleIconAndName(OrderCard orderCard, CardImage cardImage)
        {
             var graphics = Graphics.FromImage(cardImage.Bitmap);
            var usableRectangle = cardImage.UsableRectangle;

            var cardNameFont = new Font(boldFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var suit = orderCard.CardSuit;
            var roleName = suit.RoleName();
            var iconImageWidth = RoleIconWidth(cardImage);
            var iconImageHeight = (int)(iconImageWidth * 190f / 140f);
            GraphicsUtilities.PrintScaledPng(
                graphics,
                "RoleIcons",
                roleName,
                usableRectangle.X,
                usableRectangle.Y,
                iconImageWidth,
                iconImageHeight);

            var text = roleName.ToUpper();
            var brush = BrushesByCardSuit[suit];
            var letters = text.ToList().Select(letter => letter.ToString()).ToList(); ;
            var singleCharacterMeasurement = letters.Max(letter =>  graphics.MeasureString(letter, cardNameFont, new SizeF(usableRectangle.Width, usableRectangle.Height), GraphicsUtilities.HorizontalNearAlignment).Width);
            var textBoxWidth = (int)singleCharacterMeasurement;
            var xOffset = (int)(iconImageWidth/2.0f - textBoxWidth/2);
            var yOffset = iconImageHeight;
            var rectangle = new Rectangle(usableRectangle.X + xOffset, usableRectangle.Y + yOffset, textBoxWidth, usableRectangle.Height);
            var fragments = letters
                .Select(character => new TextFragment
                {
                    Brush = brush,
                    Font = cardNameFont,
                    ForcesNewline = true,
                    Text = character
                })
                .ToList();
            GraphicsUtilities.DrawFragmentsCentered(graphics, fragments, rectangle, 200, false, false);
        }

        private void PrintInfluence(OrderCard orderCard, CardImage cardImage)
        {
             var graphics = Graphics.FromImage(cardImage.Bitmap);
            var usableRectangle = cardImage.UsableRectangle;
            var influenceImageSide = InfluenceImageSide(cardImage);

            var suit = orderCard.CardSuit;

            for (var i = 0; i < suit.Influence(); i++)
            {
                GraphicsUtilities.PrintScaledPng(
                    graphics,
                    "Influence",
                    $"{suit.ColorText()}Influence",
                    usableRectangle.X + influenceImageSide * i,
                    usableRectangle.Bottom - influenceImageSide,
                    influenceImageSide,
                    influenceImageSide);
            }
        }

        private void PrintResourceType(OrderCard orderCard, CardImage cardImage)
        {
             var graphics = Graphics.FromImage(cardImage.Bitmap);
            var usableRectangle = cardImage.UsableRectangle;
            var cardNameFont = new Font(boldFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var suit = orderCard.CardSuit;
            var resourceName = suit.ResourceName().ToUpper();
            var text = AddFourPerEmSpaces(resourceName);
            var brush = new SolidBrush(suit.Color());
            var textMeasurement = graphics.MeasureString(text, cardNameFont, new SizeF(usableRectangle.Width, usableRectangle.Height), GraphicsUtilities.HorizontalCenterAlignment);
            var textBoxHeight = (int)textMeasurement.Height;
            var rectangle = new Rectangle(
                usableRectangle.X,
                usableRectangle.Bottom - textBoxHeight,
                usableRectangle.Width,
                textBoxHeight);
            GraphicsUtilities.DrawString(
                graphics,
                text,
                cardNameFont,
                brush,
                rectangle,
                GraphicsUtilities.HorizontalFarAlignment);
        }

        private void PrintSetIndicator(OrderCard orderCard, CardImage cardImage)
        {
             var graphics = Graphics.FromImage(cardImage.Bitmap);
            var usableRectangle = cardImage.UsableRectangle;

            var setIndicatorCircleWidth = (int)(usableRectangle.Width * SetIndicatorPercentage);
            var setIndicatorImageSide = (int)(setIndicatorCircleWidth * .7f);
            var suit = orderCard.CardSuit;
            var brush = new SolidBrush(suit.Color());
            var haloBrush = new SolidBrush(Color.FromArgb(200, Color.White));
            var set = orderCard.CardSet;

            for (var i = 0; i < suit.Points(); i++)
            {
                var circleRectangle = new Rectangle(
                    usableRectangle.Right - setIndicatorCircleWidth,
                    usableRectangle.Y + (i * setIndicatorCircleWidth) + (i > 0 ? i * (int)(setIndicatorCircleWidth * .2f) : 0),
                    setIndicatorCircleWidth,
                    setIndicatorCircleWidth);
                var haloSize = (int)(circleRectangle.Width * 1.2f);
                var haloRectangle = new Rectangle(
                    circleRectangle.X - (haloSize - circleRectangle.Width)/2,
                    circleRectangle.Y - (haloSize - circleRectangle.Width)/2,
                    haloSize,
                    haloSize);

                graphics.FillEllipse(haloBrush, haloRectangle);
                graphics.FillEllipse(brush, circleRectangle);
                if (set.HasSetImage())
                    GraphicsUtilities.PrintScaledPng(
                        graphics,
                        "Misc",
                        set.SetText(),
                        circleRectangle.X + (int)(circleRectangle.Width * ((1 - .7f)/2)),
                        circleRectangle.Y + (int)(circleRectangle.Height * ((1 - .7f)/2)),
                        setIndicatorImageSide,
                        setIndicatorImageSide);
            }
        }

        private void PrintOrderCardText(OrderCard orderCard, CardImage cardImage, int bottomOfCardImage)
        {
            var usableRectangle = cardImage.UsableRectangle;
            var textBoxWidth = CardTextWidth(cardImage);
            var influenceImageSide = InfluenceImageSide(cardImage);
            var textRectangleHeight = usableRectangle.Bottom - (influenceImageSide + bottomOfCardImage);
            var top = bottomOfCardImage;
            if (textRectangleHeight < 0)
            {
                top -= usableRectangle.Height / 2;
                textRectangleHeight = usableRectangle.Bottom - (influenceImageSide + top);
            }
            var xOffset = RoleIconWidth(cardImage);
            PrintCardText(orderCard.CardText, cardImage, top, textBoxWidth, textRectangleHeight, xOffset, 200, GraphicsUtilities.BlackBrush);
        }

        private void PrintCardText(string text, CardImage cardImage, int top, int textBoxWidth, int textRectangleHeight, int xOffset, int backgroundOpacity, Brush defaultBrush)
        {
             var graphics = Graphics.FromImage(cardImage.Bitmap);
            var usableRectangle = cardImage.UsableRectangle;
            var rectangle = new Rectangle(usableRectangle.X + xOffset, top, textBoxWidth, textRectangleHeight);
            var words = text.Split(new[] { " " }, StringSplitOptions.None);
            var fragments = words
                .Select(word => GetFragmentForWord(word, defaultBrush))
                .ToList();

            GraphicsUtilities.DrawFragmentsCentered(graphics, fragments, rectangle, backgroundOpacity, true, true);
        }

        private TextFragment GetFragmentForWord(string word, Brush defaultBrush)
        {
            var text = word;
            var forcesNewline = false;
            if (text[0] == '|')
            {
                forcesNewline = true;
                var isForcedParagraph = text[1] == '|';
                text = isForcedParagraph ? AddFourPerEmSpaces("") : text.Substring(1);
            }
            var matchingSuitKeyword = SuitsByKeyword.Keys.FirstOrDefault(keyword => text.Contains(keyword));
            var isSuitKeyword = matchingSuitKeyword != null;
            var isNonSuitKeyword = NonSuitKeywords.Any(keyword => text.Contains(keyword));
            return new TextFragment
            {
                Text = AddFourPerEmSpaces($"{text}"),
                Font = isSuitKeyword || isNonSuitKeyword ? BoldCardTextFont : CardTextFont,
                Brush = isSuitKeyword ? BrushesByCardSuit[SuitsByKeyword[matchingSuitKeyword]] : defaultBrush,
                ForcesNewline = forcesNewline
            };
        }

        private readonly IDictionary<CardSuit, SolidBrush> BrushesByCardSuit = Enum.GetValues(typeof(CardSuit))
            .Cast<CardSuit>()
            .ToDictionary(suit => suit, suit => new SolidBrush(suit.Color()));

        private readonly IList<string> NonSuitKeywords = new List<string>
        {
            "LEAD",
            "THINK",
            "THINKER",
            "JACK",
            "JACKS",
            "x2",
            "+1",
            "+2",
            "+3",
            "+4"
        };

        private readonly Dictionary<string, CardSuit> SuitsByKeyword = new Dictionary<string, CardSuit>
        {
            ["LEGIONARY"] = CardSuit.RedLegionaryBrick,
            ["CRAFTSMAN"] = CardSuit.GreenCraftsmanWood,
            ["PATRON"] = CardSuit.PurplePatronMarble,
            ["MERCHANT"] = CardSuit.BlueMerchantStone,
            ["ARCHITECT"] = CardSuit.GreyArchitectConcrete,
            ["LABORER"] = CardSuit.YellowLaborerRubble,
            ["STONE"] = CardSuit.BlueMerchantStone,
            ["WOOD"] = CardSuit.GreenCraftsmanWood,
            ["CONCRETE"] = CardSuit.GreyArchitectConcrete,
            ["MARBLE"] = CardSuit.PurplePatronMarble,
            ["BRICK"] = CardSuit.RedLegionaryBrick,
            ["RUBBLE"] = CardSuit.YellowLaborerRubble
        };
    }
}
