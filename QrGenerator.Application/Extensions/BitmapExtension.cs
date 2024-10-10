using System.Drawing.Text;
using System.Drawing;

namespace QrGenerator.Application.Extensions
{
    internal static class BitmapExtension
    {
        internal static Bitmap AddCaption(this Bitmap bitmap, string? ssid, int textHeight = 50)
        {
            if (string.IsNullOrEmpty(ssid))
            {
                return bitmap;
            }

            // Create a new bitmap with a size of loaded image + rectangle for caption
            var img = new Bitmap(bitmap.Width, bitmap.Height + textHeight);
            var graphics = Graphics.FromImage(img);

            // Draw the loaded image on newly created image
            graphics.DrawImage(bitmap, 0, 0);

            // Draw a rectangle for caption box
            var rectangle = new Rectangle(0, bitmap.Height, bitmap.Width, textHeight);
            graphics.DrawRectangle(
                        new Pen(Color.White, 2),
                        rectangle);
            graphics.FillRectangle(
                        new SolidBrush(Color.White),
                        rectangle);

            // Draw text
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.DrawString(
                        ssid,
                        new Font("Arial", 30, FontStyle.Regular),
                        brush: new SolidBrush(Color.Black),
                        layoutRectangle: rectangle,
                        new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });

            return img;
        }
    }
}
