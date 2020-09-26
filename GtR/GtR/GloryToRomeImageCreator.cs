using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GtR
{
    public class GloryToRomeImageCreator
    {
        private readonly FontFamily headerFontFamily = new FontFamily("Neuzeit Grotesk"); //TODO
        private const int orderCardHeaderFontSize = (int)(17 * GraphicsUtilities.dpiFactor); //TODO

        public CardImage CreateOrderCardFront(OrderCard orderCard, int index)
        {
            var name = $"{orderCard.CardName}_{index}";
            var cardImage = new CardImage(name, ImageOrientation.Portrait);
            cardImage.PrintCardBorderAndBackground(Color.White, Color.White);
            //cardImage.PrintCardBorderAndBackground(Color.FromArgb(225, 225, 225));
            PrintCardImage(orderCard, cardImage);
            PrintRoleIconAndName(orderCard, cardImage);
            PrintCardName(orderCard, cardImage);
            PrintInfluence(orderCard, cardImage);
            PrintResourceType(orderCard, cardImage);
            PrintSetIndicator(orderCard, cardImage);
            return cardImage;
        }

        private void PrintCardName(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var usableRectangWithPadding = cardImage.UsableRectangWithPadding;
            var cardNameFont = new Font(headerFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var text = string.Join(
                " ",
                orderCard.CardName.ToUpper()
                    .Split(' ')
                    .Select(token =>
                        string.Join(((char)0x202f).ToString(), token.ToCharArray())))
                        .Replace(" ", "  ");
            var textBoxWidth = (int)(usableRectangWithPadding.Width * (1 - (.18f * 2)));
            var xOffset = (usableRectangWithPadding.Width - textBoxWidth) / 2;
            var yOffset = (int)(usableRectangWithPadding.Width * .05f);
            var rectangle = new Rectangle(usableRectangWithPadding.X + xOffset, usableRectangWithPadding.Y + yOffset, textBoxWidth, usableRectangWithPadding.Height);
            var textMeasurement = graphics.MeasureString(text, cardNameFont, new SizeF(rectangle.Width, rectangle.Height), GraphicsUtilities.HorizontalCenterAlignment);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.White)), new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, (int)textMeasurement.Height));
            GraphicsUtilities.DrawString(graphics, text, cardNameFont, GraphicsUtilities.BlackBrush, rectangle, GraphicsUtilities.HorizontalCenterAlignment);
        }

        private void PrintCardImage(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var fullRectangWithPadding = cardImage.FullRectangWithPadding;
            var imageHeight = fullRectangWithPadding.Height;
            var imageWidth = (int)(imageHeight * (821f / 1121f));
            GraphicsUtilities.PrintScaledPng(
                graphics,
                $@"CardImages\{orderCard.CardName}",
                fullRectangWithPadding.X,
                fullRectangWithPadding.Y,
                imageWidth,
                imageHeight);
        }

        private void PrintRoleIconAndName(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var usableRectangWithPadding = cardImage.UsableRectangWithPadding;

            var cardNameFont = new Font(headerFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var suit = orderCard.CardSuit;
            var roleName = suit.RoleName();
            var iconImageWidth = (int)(usableRectangWithPadding.Width * .18);
            var iconImageHeight = (int)(iconImageWidth * 190f / 140f);
            GraphicsUtilities.PrintScaledPng(
                graphics,
                $@"RoleIcons\{roleName}",
                usableRectangWithPadding.X,
                usableRectangWithPadding.Y,
                iconImageWidth,
                iconImageHeight);

            var text = new string(roleName.ToUpper().ToCharArray().SelectMany(character => character == 'M' ? new[] { character } : new[] { character, (char)0x2009 }).ToArray());
            var brush = new SolidBrush(suit.Color());
            var singleCharacterMeasurement = graphics.MeasureString("M", cardNameFont, new SizeF(usableRectangWithPadding.Width, usableRectangWithPadding.Height), GraphicsUtilities.HorizontalNearAlignment);
            var textBoxWidth = (int)singleCharacterMeasurement.Width;
            var xOffset = (int)(iconImageWidth/2.0f - textBoxWidth/2);
            var yOffset = iconImageHeight;
            var rectangle = new Rectangle(usableRectangWithPadding.X + xOffset, usableRectangWithPadding.Y + yOffset, textBoxWidth, usableRectangWithPadding.Height);
            var textMeasurement = graphics.MeasureString(text, cardNameFont, new SizeF(rectangle.Width, rectangle.Height), GraphicsUtilities.HorizontalCenterAlignment);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(200, Color.White)), new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, (int)textMeasurement.Height));
            GraphicsUtilities.DrawString(graphics, text, cardNameFont, brush, rectangle);
        }

        private void PrintInfluence(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var usableRectangWithPadding = cardImage.UsableRectangWithPadding;

            var influenceImageSide = (int)(usableRectangWithPadding.Width * .13);
            var suit = orderCard.CardSuit;

            for (var i = 0; i < suit.InfluenceAndPoints(); i++)
            {
                GraphicsUtilities.PrintScaledPng(
                    graphics,
                    $@"Influence\{suit.ColorText()}Influence",
                    usableRectangWithPadding.X + influenceImageSide * i,
                    usableRectangWithPadding.Bottom - influenceImageSide,
                    influenceImageSide,
                    influenceImageSide);
            }
        }

        private void PrintResourceType(OrderCard orderCard, CardImage cardImage)
        {
            var graphics = cardImage.Graphics;
            var usableRectangWithPadding = cardImage.UsableRectangWithPadding;
            var cardNameFont = new Font(headerFontFamily, orderCardHeaderFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var suit = orderCard.CardSuit;
            var resourceName = suit.ResourceName().ToUpper();
            var text = string.Join(((char)0x202f).ToString(), resourceName.ToCharArray());
            var brush = new SolidBrush(suit.Color());
            var textMeasurement = graphics.MeasureString(text, cardNameFont, new SizeF(usableRectangWithPadding.Width, usableRectangWithPadding.Height), GraphicsUtilities.HorizontalCenterAlignment);
            var textBoxHeight = (int)textMeasurement.Height;
            var rectangle = new Rectangle(
                usableRectangWithPadding.X,
                usableRectangWithPadding.Bottom - textBoxHeight,
                usableRectangWithPadding.Width,
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
            var usableRectangWithPadding = cardImage.UsableRectangWithPadding;

            var setIndicatorCircleWidth = (int)(usableRectangWithPadding.Width * .10);
            var setIndicatorImageSide = (int)(setIndicatorCircleWidth * .7f);
            var suit = orderCard.CardSuit;
            var brush = new SolidBrush(suit.Color());
            var set = orderCard.CardSet;

            for (var i = 0; i < suit.InfluenceAndPoints(); i++)
            {
                var circleRectangle = new Rectangle(
                    usableRectangWithPadding.Right - setIndicatorCircleWidth,
                    usableRectangWithPadding.Y + (i * setIndicatorCircleWidth) + (i > 0 ? i * (int)(setIndicatorCircleWidth * .2f) : 0),
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
    }
}
