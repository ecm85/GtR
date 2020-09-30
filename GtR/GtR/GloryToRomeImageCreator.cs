using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GtR
{
    public class GloryToRomeImageCreator
    {
        private static readonly FontFamily headerFontFamily = new FontFamily("Neuzeit Grotesk"); //TODO
        private const int orderCardHeaderFontSize = (int)(18.5 * GraphicsUtilities.dpiFactor); //TODO
        private const int orderCardTextFontSize = (int)(11 * GraphicsUtilities.dpiFactor); //TODO
        private const int siteCardCostTextFontSize = (int)(13 * GraphicsUtilities.dpiFactor); //TODO
        
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

        public CardImage CreateJackImage1()
        {
            return CreateJackImage(@"Misc\Jack1");
        }

        public CardImage CreateJackImage2()
        {
            return CreateJackImage(@"Misc\Jack2");
        }

        private CardImage CreateJackImage(string path)
        {
            var cardImage = new CardImage("JackImage", ImageOrientation.Portrait);
            var graphics = cardImage.Graphics;
            var fullRectangle = cardImage.FullRectangle;
            var usableRectangle = cardImage.UsableRectangle;
            cardImage.PrintCardBorderAndBackground(Color.Black, Color.Black);
            var imageOffset = (int)(fullRectangle.Height * JackImageHeightOffsetPercentage);
            var imageHeight = GraphicsUtilities.PrintFullWidthPng(graphics, path, fullRectangle.X, fullRectangle.Y + imageOffset, fullRectangle.Width);
            var xOffset = 0;
            var yOffset = (int)(usableRectangle.Width * .15f);
            var brush = new SolidBrush(Color.FromArgb(208, 208, 208));
            PrintCardName("Jack", cardImage, brush, false, xOffset, usableRectangle.Width, yOffset);
            var bottomOfImage = fullRectangle.Top + imageOffset + imageHeight;
            PrintCardText(
                "Lead or follow any role",
                cardImage,
                bottomOfImage,
                usableRectangle.Width,
                usableRectangle.Bottom - bottomOfImage,
                xOffset,
                false,
                brush);
            return cardImage;
        }

        internal CardImage CreateMerchantBonusImage(CardSuit suit)
        {
            var cardImage = new CardImage($"MerchantBonus_{suit.ResourceName()}", ImageOrientation.Portrait);
            var graphics = cardImage.Graphics;
            cardImage.PrintCardBorderAndBackground(Color.White, Color.White);
            var fullRectangle = cardImage.FullRectangle;
            var usableRectangle = cardImage.UsableRectangle;
            var xOffset = usableRectangle.Width / 5;
            var yOffset = (int)(usableRectangle.Width * .05f);
            PrintCardName("Merchant Bonus", cardImage, GraphicsUtilities.BlackBrush, false, xOffset, 3 * usableRectangle.Width / 5, yOffset);
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
                $@"Resources\{suit.ResourceName()}",
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
                false,
                GraphicsUtilities.BlackBrush);

            var centerPoint = usableRectangle.X + usableRectangle.Width / 2;

            var coinOffsetCount = -1.5f;
            var siteCoinPadding = SiteCoinPadding(cardImage);
            var siteCoinWidth = SiteCoinWidth(cardImage);
            var coinXOffset = (int)(coinOffsetCount * siteCoinWidth + (coinOffsetCount + .5f) * siteCoinPadding);

            for (var i = 0; i < 3; i++)
                GraphicsUtilities.PrintScaledPng(
                    graphics,
                    $@"Misc\Coin",
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
            var cardImage = new CardImage(name, ImageOrientation.Portrait);
            var graphics = cardImage.Graphics;
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
                $@"Resources\{suit.ResourceName()}",
                costImageRectangle.X,
                costImageRectangle.Y,
                costImageRectangle.Width,
                costImageRectangle.Height);

            var cardNameFont = new Font(headerFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var text = AddHairSpaces(suit.ResourceName().ToUpper());
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
                    $@"Misc\Coin",
                    centerPoint + xOffset + (i * (siteCoinWidth + siteCoinPadding)),
                    usableRectangle.Y,
                    siteCoinWidth,
                    siteCoinWidth);

            return cardImage;
        }

        private string AddHairSpaces(string value)
        {
            var hairSpace = (char)0x200a;
            var withSpaces = string.Join(hairSpace.ToString(), value.ToCharArray());
            return $"{hairSpace}{withSpaces}{hairSpace}";
        }

        private string AddNonBreakingNarrowSpaces(string value)
        {
            return string.Join(((char)0x202f).ToString(), value.ToCharArray());
        }

        public CardImage CreateSiteBack(CardSuit suit)
        {
            var name = $"{suit.ResourceName()}_SiteBack";
            var cardImage = new CardImage(name, ImageOrientation.Portrait);
            var graphics = cardImage.Graphics;
            var fullRectangle = cardImage.FullRectangle;
            var usableRectangle = cardImage.UsableRectangle;
            cardImage.PrintCardBorderAndBackground(Color.White, Color.White);

            var thickness = fullRectangle.Width / diagonalLinesPerCard;
            var width = (int)Math.Sqrt(Math.Pow(thickness, 2) / 2);
            var pen = new Pen(suit.Color(), width);
            const int extra = 25;
            for (var xOffset = -2 * fullRectangle.Width + 10; xOffset < fullRectangle.Width; xOffset += thickness * 2)
                graphics.DrawLine(pen, fullRectangle.Left + xOffset - extra, fullRectangle.Top - extra, fullRectangle.Left + fullRectangle.Height + xOffset + extra, fullRectangle.Bottom + extra);

            var cardNameFont = new Font(headerFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var costFont = new Font(headerFontFamily, siteCardCostTextFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            var resourceNameText = AddHairSpaces(suit.ResourceName().ToUpper());
            var outOfTownSiteText = AddHairSpaces("out of town site");
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
            //graphics.FillRectangle(new SolidBrush(Color.Blue), textRectangle);
            GraphicsUtilities.DrawString(graphics, resourceNameText, cardNameFont, GraphicsUtilities.BlackBrush, resourceNameTextRectangle, GraphicsUtilities.HorizontalCenterAlignment);
            GraphicsUtilities.DrawString(graphics, outOfTownSiteText, costFont, GraphicsUtilities.BlackBrush, outOfTownSiteTextRectangle, GraphicsUtilities.HorizontalCenterAlignment);

            return cardImage;
        }

        private void DrawSiteCost(CardImage cardImage, CardSuit suit)
        {
            var graphics = cardImage.Graphics;
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
            var costText = AddHairSpaces($"foundation +{suit.Cost()} {material}");
            var costFont = new Font(headerFontFamily, siteCardCostTextFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
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

        private int CardTextWidth(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * (1 - (RoleIconPercentage + SetIndicatorPercentage)));
        private int RoleIconWidth(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * RoleIconPercentage);

        private readonly Font CardTextFont = new Font(headerFontFamily, orderCardTextFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
        private readonly Font BoldCardTextFont = new Font(headerFontFamily, orderCardTextFontSize, FontStyle.Bold, GraphicsUnit.Pixel);

        public CardImage CreateOrderCardBack()
        {
            var name = "OrderCardBack";
            var cardImage = new CardImage(name, ImageOrientation.Portrait);
            var graphics = cardImage.Graphics;
            var usableRectangle = cardImage.UsableRectangle;
            cardImage.PrintCardBorderAndBackground(Color.Black, Color.Black);
            GraphicsUtilities.PrintScaledPng(
                graphics,
                $@"Misc\GloryToRome",
                usableRectangle.X + (int)(usableRectangle.Width * .07f),
                usableRectangle.Y + (int)(usableRectangle.Height * .15f),
                (int)(usableRectangle.Width * (1 - (.07f * 2))),
                (int)(usableRectangle.Height * .3f));
            GraphicsUtilities.PrintScaledPng(
                graphics,
                $@"Misc\GloryToRome",
                usableRectangle.X + (int)(usableRectangle.Width * .07f),
                usableRectangle.Bottom - (int)(usableRectangle.Height * (.15f + .3f)),
                (int)(usableRectangle.Width * (1 - (.07f * 2))),
                (int)(usableRectangle.Height * .3f),
                RotateFlipType.Rotate180FlipNone);
            GraphicsUtilities.PrintScaledPng(
                graphics,
                $@"Misc\OrderBackSeparator",
                usableRectangle.X + (usableRectangle.Width/2 - (int)((usableRectangle.Width * .4f)/2)),
                usableRectangle.Y + (usableRectangle.Height/2 - (int)((usableRectangle.Height* .05f)/2)),
                (int)(usableRectangle.Width * .4f),
                (int)(usableRectangle.Height * .05f));
            return cardImage;
        }

        public CardImage CreateOrderCardFront(OrderCard orderCard, int index)
        {
            var name = $"{orderCard.CardName}_{index}";
            var cardImage = new CardImage(name, ImageOrientation.Portrait);
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
            PrintCardName(orderCard.CardName, cardImage, GraphicsUtilities.BlackBrush, true, xOffset, maxTextBoxWidth, yOffset);
        }

        private void PrintCardName(string name, CardImage cardImage, Brush brush, bool addTranslucentBackground, int xOffset, int maxTextBoxWidth, int yOffset)
        {
            var graphics = cardImage.Graphics;
            var usableRectangle = cardImage.UsableRectangle;
            var cardNameFont = new Font(headerFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var text = string.Join(
                "  ",
                name.ToUpper()
                    .Split(' ')
                    .Select(token => AddNonBreakingNarrowSpaces(token))
                    .ToList());
            var initialRectangle = new Rectangle(usableRectangle.X + xOffset, usableRectangle.Y, maxTextBoxWidth, usableRectangle.Height);
            var textMeasurement = graphics.MeasureString(text, cardNameFont, new SizeF(initialRectangle.Width, initialRectangle.Height), GraphicsUtilities.HorizontalCenterAlignment);
            var textHeight = (int)Math.Ceiling(textMeasurement.Height);
            var textRectangle = new Rectangle(usableRectangle.X + xOffset, usableRectangle.Y + yOffset, maxTextBoxWidth, textHeight);
            if (addTranslucentBackground)
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.White)), textRectangle);
            //graphics.FillRectangle(new SolidBrush(Color.Blue), textRectangle);
            GraphicsUtilities.DrawString(graphics, text, cardNameFont, brush, textRectangle, GraphicsUtilities.HorizontalCenterAlignment);
        }

        private int PrintCardImage(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var fullRectangle = cardImage.FullRectangle;
            var imageWidth = fullRectangle.Width;
            var imageHeight = GraphicsUtilities.PrintFullWidthPng(
                graphics,
                $@"CardImages\{orderCard.CardName}",
                fullRectangle.X + (orderCard.ImageIsRoughlyCentered ? CenteredImageOffset(cardImage) : 0),
                fullRectangle.Y,
                imageWidth);
            return fullRectangle.Y + imageHeight;
        }

        private void PrintRoleIconAndName(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var usableRectangle = cardImage.UsableRectangle;

            var cardNameFont = new Font(headerFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var suit = orderCard.CardSuit;
            var roleName = suit.RoleName();
            var iconImageWidth = RoleIconWidth(cardImage);
            var iconImageHeight = (int)(iconImageWidth * 190f / 140f);
            GraphicsUtilities.PrintScaledPng(
                graphics,
                $@"RoleIcons\{roleName}",
                usableRectangle.X,
                usableRectangle.Y,
                iconImageWidth,
                iconImageHeight);

            var text = new string(roleName.ToUpper().ToCharArray().SelectMany(character => character == 'M' ? new[] { character } : new[] { character, (char)0x2009 }).ToArray());
            var brush = BrushesByCardSuit[suit];
            var singleCharacterMeasurement = graphics.MeasureString("M", cardNameFont, new SizeF(usableRectangle.Width, usableRectangle.Height), GraphicsUtilities.HorizontalNearAlignment);
            var textBoxWidth = (int)singleCharacterMeasurement.Width;
            var xOffset = (int)(iconImageWidth/2.0f - textBoxWidth/2);
            var yOffset = iconImageHeight;
            var rectangle = new Rectangle(usableRectangle.X + xOffset, usableRectangle.Y + yOffset, textBoxWidth, usableRectangle.Height);
            var textMeasurement = graphics.MeasureString(text, cardNameFont, new SizeF(rectangle.Width, rectangle.Height), GraphicsUtilities.HorizontalCenterAlignment);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, Color.White)), new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, (int)textMeasurement.Height));
            GraphicsUtilities.DrawString(graphics, text, cardNameFont, brush, rectangle);
        }

        private void PrintInfluence(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var usableRectangle = cardImage.UsableRectangle;
            var influenceImageSide = InfluenceImageSide(cardImage);

            var suit = orderCard.CardSuit;

            for (var i = 0; i < suit.Influence(); i++)
            {
                GraphicsUtilities.PrintScaledPng(
                    graphics,
                    $@"Influence\{suit.ColorText()}Influence",
                    usableRectangle.X + influenceImageSide * i,
                    usableRectangle.Bottom - influenceImageSide,
                    influenceImageSide,
                    influenceImageSide);
            }
        }

        private void PrintResourceType(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var usableRectangle = cardImage.UsableRectangle;
            var cardNameFont = new Font(headerFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var suit = orderCard.CardSuit;
            var resourceName = suit.ResourceName().ToUpper();
            var text = AddHairSpaces(resourceName);
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
            var graphics = cardImage.Graphics;
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
                        $@"Misc\{set.SetText()}",
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
                textRectangleHeight = usableRectangle.Bottom - (influenceImageSide + bottomOfCardImage);
            }
            var xOffset = RoleIconWidth(cardImage);
            PrintCardText(orderCard.CardText, cardImage, top, textBoxWidth, textRectangleHeight, xOffset, true, GraphicsUtilities.BlackBrush);
        }

        private void PrintCardText(string text, CardImage cardImage, int top, int textBoxWidth, int textRectangleHeight, int xOffset, bool addTranslucentBackground, Brush defaultBrush)
        {
            var graphics = cardImage.Graphics;
            var usableRectangle = cardImage.UsableRectangle;
            var rectangle = new Rectangle(usableRectangle.X + xOffset, top, textBoxWidth, textRectangleHeight);
            var words = text.Split(new[] { " " }, StringSplitOptions.None);
            var fragments = words
                .Select(word => GetFragmentForWord(word, defaultBrush))
                .ToList();

            GraphicsUtilities.DrawFragmentsCentered(graphics, fragments, rectangle, addTranslucentBackground);
        }

        private TextFragment GetFragmentForWord(string word, Brush defaultBrush)
        {
            var text = word;
            var forcesNewline = false;
            if (text[0] == '|')
            {
                forcesNewline = true;
                text = text.Substring(1);
            }
            var matchingSuitKeyword = SuitsByKeyword.Keys.FirstOrDefault(keyword => text.Contains(keyword));
            var isSuitKeyword = matchingSuitKeyword != null;
            var isNonSuitKeyword = NonSuitKeywords.Any(keyword => text.Contains(keyword));
            return new TextFragment
            {
                Text = AddHairSpaces($"{text}"),
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
