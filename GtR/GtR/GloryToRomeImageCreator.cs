using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GtR
{
    public class GloryToRomeImageCreator
    {
        private static readonly FontFamily headerFontFamily = new FontFamily("Neuzeit Grotesk"); //TODO
        private const int orderCardHeaderFontSize = (int)(19 * GraphicsUtilities.dpiFactor); //TODO
        private const int orderCardTextFontSize = (int)(14 * GraphicsUtilities.dpiFactor); //TODO
        private const float influenceImagePercentage = .13f;
        private const float RoleIconPercentage = .18f;

        private const float SetIndicatorPercentage = .10f;

        private int InfluenceImageSide(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * influenceImagePercentage);
        private int CardNameWidth(CardImage cardImage) => (int)(cardImage.UsableRectangle.Width * (1 - (RoleIconPercentage + SetIndicatorPercentage)));
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
            PrintCardName(orderCard, cardImage);
            PrintInfluence(orderCard, cardImage);
            PrintResourceType(orderCard, cardImage);
            PrintSetIndicator(orderCard, cardImage);
            PrintCardText(orderCard, cardImage, bottomOfImage);
            return cardImage;
        }

        private void PrintCardName(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var usableRectangle = cardImage.UsableRectangle;
            var cardNameFont = new Font(headerFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var text = string.Join(
                " ",
                orderCard.CardName.ToUpper()
                    .Split(' ')
                    .Select(token =>
                        string.Join(((char)0x202f).ToString(), token.ToCharArray())))
                        .Replace(" ", "  ");
            var textBoxWidth = CardNameWidth(cardImage);
            var xOffset = RoleIconWidth(cardImage);
            var yOffset = (int)(usableRectangle.Width * .05f);
            var rectangle = new Rectangle(usableRectangle.X + xOffset, usableRectangle.Y + yOffset, textBoxWidth, usableRectangle.Height);
            var textMeasurement = graphics.MeasureString(text, cardNameFont, new SizeF(rectangle.Width, rectangle.Height), GraphicsUtilities.HorizontalCenterAlignment);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.White)), new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, (int)textMeasurement.Height));
            //graphics.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, (int)textMeasurement.Height));
            GraphicsUtilities.DrawString(graphics, text, cardNameFont, GraphicsUtilities.BlackBrush, rectangle, GraphicsUtilities.HorizontalCenterAlignment);
        }

        private int PrintCardImage(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var fullRectangle = cardImage.FullRectangle;
            var imageWidth = fullRectangle.Width;
            var imageHeight = GraphicsUtilities.PrintFullWidthPng(
                graphics,
                $@"CardImages\{orderCard.CardName}",
                fullRectangle.X,
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

            for (var i = 0; i < suit.InfluenceAndPoints(); i++)
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
            var text = string.Join(((char)0x202f).ToString(), resourceName.ToCharArray());
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
            var set = orderCard.CardSet;

            for (var i = 0; i < suit.InfluenceAndPoints(); i++)
            {
                var circleRectangle = new Rectangle(
                    usableRectangle.Right - setIndicatorCircleWidth,
                    usableRectangle.Y + (i * setIndicatorCircleWidth) + (i > 0 ? i * (int)(setIndicatorCircleWidth * .2f) : 0),
                    setIndicatorCircleWidth,
                    setIndicatorCircleWidth);

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

        private void PrintCardText(OrderCard orderCard, CardImage cardImage, int bottomOfCardImage)
        {
            var graphics = cardImage.Graphics;
            var usableRectangle = cardImage.UsableRectangle;
            var text = orderCard.CardText;
            var textBoxWidth = CardTextWidth(cardImage);
            var xOffset = RoleIconWidth(cardImage);
            var influenceImageSide = InfluenceImageSide(cardImage);
            var textRectangleHeight = usableRectangle.Bottom - (influenceImageSide + bottomOfCardImage);
            if (textRectangleHeight < 0)
            {
                bottomOfCardImage -= usableRectangle.Height / 2;
                textRectangleHeight = usableRectangle.Bottom - (influenceImageSide + bottomOfCardImage);
            }
            var rectangle = new Rectangle(usableRectangle.X + xOffset, bottomOfCardImage, textBoxWidth, textRectangleHeight);
            var words = text.Split(new[] {" "}, StringSplitOptions.None);
            var fragments = words
                .Select(word => GetFragmentForWord(word))
                .ToList();

            GraphicsUtilities.DrawFragmentsCentered(graphics, fragments, rectangle);
        }

        private TextFragment GetFragmentForWord(string word)
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
                Text = $"{text}",
                Font = isSuitKeyword || isNonSuitKeyword ? BoldCardTextFont : CardTextFont,
                Brush = isSuitKeyword ? BrushesByCardSuit[SuitsByKeyword[matchingSuitKeyword]] : GraphicsUtilities.BlackBrush,
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
            ["MARBLE"] = CardSuit.PurplePatronMarble,
            ["STONE"] = CardSuit.BlueMerchantStone,
            ["RUBBLE"] = CardSuit.YellowLaborerRubble
        };
    }
}
