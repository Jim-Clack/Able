using AbleCheckbook.Logic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Gui
{
    public static class UiHelperMethods
    {
        /// <summary>
        /// Draw a pie chart with a list of color-coded captions underneath.
        /// </summary>
        /// <param name="graphics">To be drawn in.</param>
        /// <param name="x">Horizontal offset in graphics area.</param>
        /// <param name="y">Vertical offset in graphics area.</param>
        /// <param name="width">How wide to make it. (Height depends on number of captions)</param>
        /// <param name="amounts">Array of pie-slice monetary amounts.</param>
        /// <param name="captions">Captions in the same sequence as proportions[].</param>
        /// <returns>maximum (bottom-most) y value rendered.</returns>
        public static int DrawPieChart(Graphics graphics,
            int x, int y, int width, long[] amounts, string[] captions)
        {
            if (amounts == null || amounts.Length < 1)
            {
                return 0;
            }
            int count = amounts.Length;
            long sum = 0;
            for (int i = 0; i < count; ++i)
            {
                amounts[i] = amounts[i] < 0 ? 0 : amounts[i];
                sum += amounts[i];
            }
            Color[] colors = CreateColorSelections(count);
            Point location = new Point(x, y);
            Size size = new Size(width, width);
            Font font = new Font(FontFamily.GenericMonospace, width / 25, FontStyle.Bold);
            if (amounts.Length > 7)
            {
                font = new Font(FontFamily.GenericMonospace, width / 35, FontStyle.Bold);
            }
            long prevAmount = 0;
            int prevTextY = width + 10 + y;
            for (int i = 0; i < count; i++)
            {
                SolidBrush brush = new SolidBrush(colors[i]);
                graphics.FillPie(brush, new Rectangle(location, size),
                    Convert.ToSingle(prevAmount * 360.0 / sum),
                    Convert.ToSingle(amounts[i] * 360.0 / sum));
                prevAmount += amounts[i];
                string caption = "       -.--  Unspecified";
                if (captions != null && captions.Length > i)
                {
                    string amount = UtilityMethods.FormatCurrency(Math.Abs(amounts[i]), 11);
                    caption = "" + amount + "  " + captions[i];
                }
                graphics.DrawString(caption, font, brush, x, prevTextY);
                prevTextY += font.Height;
                Pen pen = new Pen(Color.Black, 3);
                graphics.DrawEllipse(pen, new Rectangle(location, size));
            }
            return prevTextY;
        }

        /// <summary>
        /// Select colors based on an even distribution across the spectrum.
        /// </summary>
        /// <param name="count">Number of color selections desrired.</param>
        /// <returns>Practical aray of dissimilar colors.</returns>
        private static Color[] CreateColorSelections(int count)
        {
            Color[] colors = new Color[count]; // assemble colors array
            for (int i = 0; i < count; ++i)
            {
                int hue = (int)(i * 256 + 0.5) / count;
                // if hue=0, s/b r=180, b=0, g=180
                int red = Math.Max(0, 255 - (Math.Abs(hue - 42) % 256) * 2);
                int grn = Math.Max(0, 255 - (Math.Abs(hue - 127) % 256) * 2);
                int blu = Math.Max(0, 255 - (Math.Abs(hue - 213) % 256) * 2);
                colors[i] = Color.FromArgb(255, red, grn, blu);
            }
            return colors;
        }

        /// <summary>
        /// Screen capture of a form.
        /// </summary>
        /// <param name="form">To be captured as a jpg image.</param>
        /// <param name="filePath">File to be created. (omit for random filename in log folder)</param>
        /// <returns>Name of file. Null on error.</returns>
        public static string FormCapture(Form form, string filePath = null)
        {
            long counter = 0;
            if (filePath == null)
            {
                filePath = Path.Combine(Configuration.Instance.DirectoryLogs, "FC" + ++counter + ".jpg");
            }
            // Set up codec
            ImageCodecInfo encoder = null;
            EncoderParameters encoderParameters = null;
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    encoder = codec;
                    encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 20L);
                }
            }
            // Detect screen scaling, i.e. 125%
            double scale = 1.0;
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop\\WindowMetrics"))
                {
                    if (key != null)
                    {
                        Object obj = key.GetValue("AppliedDPI");
                        if (obj != null)
                        {
                            int value = (int)obj;
                            scale = (double)value / 96.0;
                        }
                    }
                }
            }
            catch (Exception) 
            {
                scale = 1.0;
            }
            // Capture from screen
            try
            {

                int x = (int)(form.Bounds.X * scale + 10);
                int y = (int)(form.Bounds.Y * scale + 10);
                int width = (int)(form.Bounds.Width * scale - 20);
                int height = (int)(form.Bounds.Height * scale - 20);
                Bitmap bitmap = new Bitmap(width, height, form.CreateGraphics());
                Graphics.FromImage(bitmap).CopyFromScreen(x, y, 0, 0, new Size(width, height));
                File.Delete(filePath);
                if (encoderParameters == null)
                {
                    bitmap.Save(filePath, ImageFormat.Jpeg);
                }
                else
                {
                    bitmap.Save(filePath, encoder, encoderParameters);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return filePath;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

    }

}
