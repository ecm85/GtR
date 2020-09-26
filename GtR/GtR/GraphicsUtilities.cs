using System.Drawing;
using System.Drawing.Drawing2D;

namespace GtR
{
    public class GraphicsUtilities
    {
        public const float dpiFactor = 300.0f / 96;
        public const int dpi = (int)(96 * dpiFactor);

        public static readonly StringFormat HorizontalCenterAlignment = new StringFormat { Alignment = StringAlignment.Center };
        public static readonly StringFormat FullCenterAlignment = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        public static readonly StringFormat HorizontalNearAlignment = new StringFormat { Alignment = StringAlignment.Near };
        public static readonly StringFormat HorizontalFarAlignment = new StringFormat { Alignment = StringAlignment.Far };
        public static readonly StringFormat VerticalText = new StringFormat { FormatFlags = StringFormatFlags.DirectionVertical };
        public static readonly SolidBrush BlackBrush = new SolidBrush(Color.Black);

        private const float textOutlineWidth = .5f * dpiFactor;

        public static void PrintImageWithText(
            Graphics graphics,
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
            PrintScaledPng(graphics, fileName, imageX, imageY, imageWidth, imageHeight);
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

        public static void PrintImageWithTextCentered(
            Graphics graphics,
            string fileName,
            int x,
            int y,
            int imageWidth,
            int imageHeight,
            string text,
            Font font)
        {
            PrintScaledPng(graphics, fileName, x, y, imageWidth, imageHeight);
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

        public static void PrintScaledPng(Graphics graphics, string fileName, int x, int y, int width, int height)
        {
            using (var srcImage = Image.FromFile($"Images\\{fileName}.png"))
            {
                PrintScaledImage(graphics, srcImage, x, y, width, height);
            }
        }

        public static void PrintScaledJpg(Graphics graphics, string fileName, int x, int y, int width, int height)
        {
            using (var srcImage = Image.FromFile($"Images\\{fileName}.jpg"))
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

        public static Bitmap CreateBitmap(int width, int height)
        {
            var bitmap = new Bitmap(width, height);
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
    }
}
