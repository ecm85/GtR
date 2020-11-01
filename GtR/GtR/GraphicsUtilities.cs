using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GtR
{
    public class GraphicsUtilities
    {
        public static string CurrentPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public const float dpiFactor = 300.0f / 96;
        public const int dpi = (int)(96 * dpiFactor);

        public static readonly StringFormat HorizontalCenterAlignment = new StringFormat { Alignment = StringAlignment.Center };
        public static readonly StringFormat FullCenterAlignment = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        public static readonly StringFormat HorizontalNearAlignment = new StringFormat { Alignment = StringAlignment.Near };
        public static readonly StringFormat HorizontalFarAlignment = new StringFormat { Alignment = StringAlignment.Far };
        public static readonly StringFormat VerticalText = new StringFormat { FormatFlags = StringFormatFlags.DirectionVertical };
        public static readonly SolidBrush BlackBrush = new SolidBrush(Color.Black);

        private const float textOutlineWidth = .5f * dpiFactor;

        //Note: does not maintain original image aspect ratio!
        public static void PrintImageWithText(
            Graphics graphics,
            string subfolder,
            string fileName,
            int imageX,
            int imageY,
            int imageWidth,
            int imageHeight,
            string text,
            int textImageXOffset,
            int textImageYOffset,
            Font font)
        {
            PrintScaledPng(graphics, subfolder, fileName, imageX, imageY, imageWidth, imageHeight);
            var textX = imageX + textImageXOffset;
            var textY = imageY + textImageYOffset;
            var path = new GraphicsPath();
            path.AddString(
                text,
                font.FontFamily,
                (int)font.Style,
                font.Size,
                new PointF(textX, textY),
                new StringFormat());
            graphics.FillPath(Brushes.White, path);
            graphics.DrawPath(new Pen(Color.Black, textOutlineWidth), path);
        }

        //Note: does not maintain original image aspect ratio!
        public static void PrintImageWithTextCentered(
            Graphics graphics,
            string subfolder,
            string fileName,
            int x,
            int y,
            int imageWidth,
            int imageHeight,
            string text,
            Font font)
        {
            PrintScaledPng(graphics, subfolder, fileName, x, y, imageWidth, imageHeight);
            var path = new GraphicsPath();
            path.AddString(
                text,
                font.FontFamily,
                (int)font.Style,
                font.Size,
                new RectangleF(x, y, imageWidth, imageHeight),
                HorizontalCenterAlignment);
            graphics.FillPath(Brushes.White, path);
            graphics.DrawPath(new Pen(Color.Black, textOutlineWidth), path);
        }

        //Note: does not maintain original image aspect ratio!
        public static void PrintScaledPng(Graphics graphics, string subfolder, string fileName, int x, int y, int width, int height, RotateFlipType? rotateFlipType = null)
        {
            using (var srcImage = Image.FromFile(Path.Combine(CurrentPath, "Images", subfolder, $"{fileName}.png")))
            {
                if (rotateFlipType != null)
                    srcImage.RotateFlip(rotateFlipType.Value);
                PrintScaledImage(graphics, srcImage, x, y, width, height);
            }
        }

        public static void PrintScaledAndCenteredPng(Graphics graphics, string subfolder, string fileName, int x, int y, int maxWidth, int maxHeight, RotateFlipType? rotateFlipType = null)
        {
            using (var image = Image.FromFile(Path.Combine(CurrentPath, "Images", subfolder, $"{fileName}.png")))
            {
                if (rotateFlipType != null)
                    image.RotateFlip(rotateFlipType.Value);
                var originalAspectRatio = (float)image.Width / image.Height;
                var shouldFitToWidth = ShouldFitToWidth(maxWidth, maxHeight, originalAspectRatio);
                var width = shouldFitToWidth ? maxWidth : Math.Round(maxHeight * originalAspectRatio);
                var height = shouldFitToWidth ? Math.Round(maxWidth / originalAspectRatio) : maxHeight;
                var xOffset = (int)Math.Round((maxWidth - width) / 2);
                var yOffset = (int)Math.Round((maxHeight - height) / 2);

                PrintScaledImage(graphics, image, x + xOffset, y + yOffset, (int)width, (int)height);
            }
        }

        private static bool ShouldFitToWidth(
            int maxWidth,
            int  maxHeight,
            float originalAspectRatio)
        {
            var maxAllowedAspectRatio = maxWidth / maxHeight;
            return maxAllowedAspectRatio <= originalAspectRatio;
        }

        public static int PrintFullWidthPng(Graphics graphics, string subfolder, string fileName, int x, int y, int width)
        {
            using (var srcImage = Image.FromFile(Path.Combine(CurrentPath, "Images", subfolder, $"{fileName}.png")))
            {
                var height = width * srcImage.Height / srcImage.Width;
                PrintScaledImage(graphics, srcImage, x, y, width, height);
                return height;
            }
        }

        public static void PrintScaledJpg(Graphics graphics, string subfolder, string fileName, int x, int y, int width, int height)
        {
            using (var srcImage = Image.FromFile(Path.Combine(CurrentPath, "Images", subfolder, $"{fileName}.jpg")))
            {
                PrintScaledImage(graphics, srcImage, x, y, width, height);
            }
        }

        public static void PrintScaledImage(Graphics graphics, Image image, int x, int y, int width, int height)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.DrawImage(image, new Rectangle(x, y, width, height));
        }

        public static Bitmap CreateBitmap(int widthInPixels, int heightInPixels)
        {
            var bitmap = new Bitmap(widthInPixels, heightInPixels);
            bitmap.SetResolution(dpi, dpi);
            return bitmap;
        }

        public static void DrawString(Graphics graphics, string text, Font font, Brush brush, RectangleF rectangle)
        {
            graphics.DrawString(text, font, brush, rectangle, HorizontalCenterAlignment);
        }

        public static void DrawString(Graphics graphics, string text, Font font, Brush brush, RectangleF rectangle, StringFormat stringFormat)
        {
            graphics.DrawString(text, font, brush, rectangle, stringFormat);
        }

        public static void DrawFragmentsCentered(
            Graphics graphics,
            IList<TextFragment> fragments,
            Rectangle rectangle,
            int translucentBackgroundOpacity,
            float characterSpacingPercentage,
            StringAlignment horizontalAlignment,
            StringAlignment verticalAlignment)
        {
            var measuredFragments = fragments
                .Select(fragment => MeasureTextFragment(graphics, fragment, rectangle.Width, characterSpacingPercentage))
                .ToList();
            var fragmentsGroupedByLine = GetFragmentsGroupedByLine(graphics, rectangle, measuredFragments).ToList();
            var totalHeight = fragmentsGroupedByLine.Sum(group => group.Max(measuredFragment => measuredFragment.Height));
            var totalWidth = fragmentsGroupedByLine.Max(group => group.Sum(measuredFragment => measuredFragment.Width));
            var minX = GetMinX(horizontalAlignment, rectangle, totalWidth);
            var minY = GetMinY(verticalAlignment, rectangle, totalHeight);
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(translucentBackgroundOpacity, Color.White)), new RectangleF(minX, minY, totalWidth, totalHeight));
            var currentY = minY;
            foreach (var fragmentGroup in fragmentsGroupedByLine)
            {
                var groupWidth = fragmentGroup.Sum(fragment => fragment.Width);
                var groupHeight = fragmentGroup.Max(fragment => fragment.Height);
                var currentX = GetMinX(horizontalAlignment, rectangle, groupWidth);

                if (horizontalAlignment == StringAlignment.Center)
                    currentX += 5; //Hax!!!
                foreach (var fragment in fragmentGroup)
                {
                    var text = fragment.TextFragment.Text;
                    var wordPosition = currentX;
                    var font = fragment.TextFragment.Font;
                    var characterRanges = Enumerable.Range(0, text.Length).Select(index => new CharacterRange(index, 1)).ToArray();
                    var stringFormat = new StringFormat();
                    stringFormat.SetMeasurableCharacterRanges(characterRanges);
                    var regions = graphics.MeasureCharacterRanges(text, font, rectangle, stringFormat).Select(region => region.GetBounds(graphics)).ToList();
                    for (var i = 0; i < text.Length; i++)
                    {
                        var character = text[i].ToString();

                        var brush = fragment.TextFragment.Brush;
                        graphics.DrawString(character, font, brush, new RectangleF(wordPosition, currentY, rectangle.Width, rectangle.Height));
                        wordPosition += (regions[i].Width) + characterSpacingPercentage * font.Height;
                    }
                    currentX += fragment.Width + characterSpacingPercentage * font.Height;
                }
                currentY += groupHeight;
            }
        }

        private static float GetMinX(StringAlignment horizontalAlignment, Rectangle rectangle, float totalWidth)
        {
            switch (horizontalAlignment)
            {
                case StringAlignment.Center:
                    return rectangle.Left + (rectangle.Width / 2 - totalWidth / 2);
                case StringAlignment.Near:
                    return rectangle.Left;
                case StringAlignment.Far:
                    return rectangle.Right - totalWidth;
                default:
                    throw new InvalidOperationException($"Invalid alignment: {horizontalAlignment}");
            }
        }

        private static int GetMinY(StringAlignment verticalAlignment, Rectangle rectangle, int totalHeight)
        {
            switch (verticalAlignment)
            {
                case StringAlignment.Center:
                    return rectangle.Top + (rectangle.Height / 2 - totalHeight / 2);
                case StringAlignment.Near:
                    return rectangle.Top;
                case StringAlignment.Far:
                    return rectangle.Bottom - totalHeight;
                default:
                    throw new InvalidOperationException($"Invalid alignment: {verticalAlignment}");
            }
        }

        private static MeasuredTextFragment MeasureTextFragment(Graphics graphics, TextFragment textFragment, int maxWidth, float characterSpacingPercentage)
        {
            var letters = textFragment.Text.ToList();
            //var measuredWidth = textFragment.Text.ToList().Sum(character => graphics.MeasureString(character.ToString(), textFragment.Font, maxWidth).Width);
            var measuredWidth = graphics.MeasureString(textFragment.Text, textFragment.Font, maxWidth).Width;
            var width = measuredWidth + (characterSpacingPercentage * textFragment.Font.Height * (letters.Count));
            var height = (int)graphics.MeasureString(textFragment.Text, textFragment.Font, maxWidth).Height;
            return new MeasuredTextFragment(width, height, textFragment);
        }

        private static IEnumerable<IEnumerable<MeasuredTextFragment>> GetFragmentsGroupedByLine(Graphics graphics, Rectangle rectangle, IEnumerable<MeasuredTextFragment> original)
        {
            var remaining = original.ToList();
            while (remaining.Any())
            {
                var tokens = 0;
                var currentWidth = 0f;
                do
                {
                    currentWidth += remaining[tokens].Width;
                    tokens++;
                } while (tokens < remaining.Count && !remaining[tokens].TextFragment.ForcesNewline && currentWidth + remaining[tokens].Width <= rectangle.Width);
                yield return remaining.Take(tokens).ToList();
                remaining = remaining.Skip(tokens).ToList();
            }
        }
    }
}
