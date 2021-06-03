using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Gui
{

    /////////////////////////////// FontDesc /////////////////////////////////

    /// <summary>
    /// Container for font/text attributes.
    /// </summary>
    public class FontDesc
    {
        private Brush _brush;
        private Font _font;

        public FontDesc(Brush brush, Font font)
        {
            _brush = brush;
            _font = font;
        }

        public Brush Brush { get => _brush; }
        public Font Font { get => _font; }
    }

    ////////////////////////////// ReportBase ////////////////////////////////

    /// <summary>
    /// Base class for report generators.
    /// </summary>
    public abstract class BaseReportGenerator
    {

        /// <summary>
        /// Rendering target.
        /// </summary>
        protected Graphics _graphics;

        /// <summary>
        /// Generate a page of the report.
        /// </summary>
        /// <param name="graphics">To render to.</param>
        /// <param name="margins">Margins, typically same as or within graphics visible clip bounds.</param>
        /// <returns>true if more pages follow</returns>
        public abstract bool PrintPage(Graphics graphics, Rectangle margins);

        /// <summary>
        /// Draw text on the report.
        /// </summary>
        /// <param name="text">To be rendered.</param>
        /// <param name="layout">Font and other layout info.</param>
        /// <param name="x">X of top left.</param>
        /// <param name="y">Y of top left.</param>
        /// <returns>Y of next available top left.</returns>
        protected float DrawTextReturnNewY(string text, FontDesc layout, float x, float y)
        {
            _graphics.DrawString(text, layout.Font, layout.Brush, x, y);
            return y + layout.Font.Height + 2;
        }

        /// <summary>
        /// Draw text on the report.
        /// </summary>
        /// <param name="text">To be rendered.</param>
        /// <param name="layout">Font and other layout info.</param>
        /// <param name="x">X of top left.</param>
        /// <param name="y">Y of top left.</param>
        /// <returns>X of next available top left.</returns>
        protected float DrawTextReturnNewX(string text, FontDesc layout, float x, float y)
        {
            _graphics.DrawString(text, layout.Font, layout.Brush, x, y);
            return x + _graphics.MeasureString(text, layout.Font).Width + 2;
        }

    }
}
