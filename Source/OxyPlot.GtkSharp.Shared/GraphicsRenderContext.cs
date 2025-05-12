// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicsRenderContext.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   The graphics render context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.GtkSharp
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Cairo;
    using Pango;

    using Gdk;

#if GTKSHARP4
    using GdkPixbuf;
#endif

    /// <summary>
    /// The graphics render context.
    /// </summary>
    public class GraphicsRenderContext : RenderContextBase
    {
        /// <summary>
        /// The image cache.
        /// </summary>
        private readonly Dictionary<OxyImage, Pixbuf> imageCache = new Dictionary<OxyImage, Pixbuf>();

        /// <summary>
        /// The images in use.
        /// </summary>
        private readonly HashSet<OxyImage> imagesInUse = new HashSet<OxyImage>();

        /// <summary>
        /// The GDI+ drawing surface.
        /// </summary>
        private Cairo.Context g;

#if GTKSHARP34
        /// <summary>
        /// The text layout context
        /// </summary>
        private Pango.Context c;
#endif

        /// <summary>
        /// The number of nested clips applied to the graphics context.
        /// </summary>
        private int clipCount;

        /// <summary>
        /// The number of nested clips applied to the graphics context.
        /// </summary>
        public override int ClipCount => clipCount;

        /// <summary>
        /// Sets the graphics target.
        /// </summary>
        /// <param name="graphics">The graphics surface.</param>
        public void SetGraphicsTarget(Cairo.Context graphics)
        {
            this.g = graphics;
            this.g.Antialias = Antialias.Subpixel; // TODO  .TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
#if GTKSHARP3
            this.c = Pango.CairoHelper.CreateContext(this.g);
#elif GTKSHARP4
            this.c = PangoCairo.Functions.CreateContext(graphics);
#endif
        }

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="fill">The fill color.</param>
        /// <param name="stroke">The stroke color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="edgeRenderingMode">The edge rendering mode.</param>
        /// <remarks>
        /// todo: implement edge rendering modes
        /// </remarks>
        public override void DrawEllipse(OxyRect rect, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
            // center of ellipse
            var ex = rect.Left + (rect.Width / 2.0);
            var ey = rect.Top + (rect.Height / 2.0);

            // ellipse dimensions
            var ew = rect.Width;
            var eh = rect.Height;

            if (fill.IsVisible())
            {
                this.g.Save();

                this.g.Translate(ex, ey); // make (ex, ey) == (0, 0)
                this.g.Scale(ew / 2.0, eh / 2.0); // for width: ew / 2.0 == 1.0, eh / 2.0 == 1.0

                this.g.Arc(0.0, 0.0, 1.0, 0.0, 2.0 * Math.PI); // 'circle' centered at (0, 0)
                this.g.ClosePath();
                this.g.SetSourceColor(fill);
                this.g.Fill();
                this.g.Restore();
            }

            if (stroke.IsVisible() && thickness > 0)
            {
                this.g.Save();

                // g.SmoothingMode = SmoothingMode.HighQuality; // TODO
                this.g.Translate(ex, ey); // make (ex, ey) == (0, 0)
                this.g.Scale(ew / 2.0, eh / 2.0); // for width: ew / 2.0 == 1.0

                // for height: eh / 2.0 == 1.0
                this.g.Arc(0.0, 0.0, 1.0, 0.0, 2.0 * Math.PI); // 'circle' centered at (0, 0)
                this.g.SetSourceColor(stroke);
                this.g.LineWidth = thickness * 2.0 / ew;
                this.g.Stroke();
                this.g.Restore();
            }
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="dashArray">The dash array.</param>
        /// <param name="lineJoin">The line join.</param>
        /// <param name="aliased">if set to <c>true</c> [aliased].</param>
        public override void DrawLine(
            IList<ScreenPoint> points,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            OxyPlot.LineJoin lineJoin)
        {
            if (stroke.IsVisible() && thickness > 0 && points.Count >= 2)
            {
                // g.SmoothingMode = aliased ? SmoothingMode.None : SmoothingMode.HighQuality; // TODO: Smoothing modes
                this.g.Save();
                this.g.SetSourceColor(stroke);
                this.g.LineJoin = lineJoin.ToLineJoin();
                this.g.LineWidth = thickness;
                if (dashArray != null)
                {
                    this.g.SetDash(dashArray, 0);
                }

                bool aliased = !ShouldUseAntiAliasingForLine(edgeRenderingMode, points);
                this.g.MoveTo(points[0].ToPointD(aliased));
                foreach (var point in points.Skip(1))
                {
                    this.g.LineTo(point.ToPointD(aliased));
                }

                this.g.Stroke();
                this.g.Restore();
            }
        }

        /// <summary>
        /// Draws the polygon.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="fill">The fill.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="renderingMode">The edge rendering mode.</param>
        /// <param name="dashArray">The dash array.</param>
        /// <param name="lineJoin">The line join.</param>
        public override void DrawPolygon(
            IList<ScreenPoint> points,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode renderingMode,
            double[] dashArray,
            OxyPlot.LineJoin lineJoin)
        {
            bool aliased = !ShouldUseAntiAliasingForEllipse(renderingMode);
            if (fill.IsVisible() && points.Count >= 2)
            {
                // g.SmoothingMode = aliased ? SmoothingMode.None : SmoothingMode.HighQuality; // TODO: Smoothing modes
                this.g.Save();
                this.g.SetSourceColor(fill);
                this.g.LineJoin = lineJoin.ToLineJoin();
                this.g.LineWidth = thickness;
                if (dashArray != null)
                {
                    this.g.SetDash(dashArray, 0);
                }

                this.g.MoveTo(points[0].ToPointD(aliased));
                foreach (var point in points.Skip(1))
                {
                    this.g.LineTo(point.ToPointD(aliased));
                }

                // g.LineTo(points[0].ToPointD(aliased));
                this.g.ClosePath();
                this.g.Fill();
                this.g.Restore();
            }

            if (stroke.IsVisible() && thickness > 0 && points.Count >= 2)
            {
                // g.SmoothingMode = aliased ? SmoothingMode.None : SmoothingMode.HighQuality; // TODO: Smoothing modes
                this.g.Save();
                this.g.SetSourceColor(stroke);
                this.g.LineJoin = lineJoin.ToLineJoin();
                this.g.LineWidth = thickness;
                if (dashArray != null)
                {
                    this.g.SetDash(dashArray, 0);
                }

                this.g.MoveTo(points[0].ToPointD(aliased));
                foreach (var point in points.Skip(1))
                {
                    this.g.LineTo(point.ToPointD(aliased));
                }

                this.g.ClosePath();
                this.g.Stroke();
                this.g.Restore();
            }
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="fill">The fill color.</param>
        /// <param name="stroke">The stroke color.</param>
        /// <param name="thickness">The stroke thickness.</param>
        /// <param name="renderingMode">The edge rendering mode.</param>
        public override void DrawRectangle(OxyRect rect, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode renderingMode)
        {
            bool aliased = !ShouldUseAntiAliasingForRect(renderingMode);
            if (fill.IsVisible())
            {
                this.g.Save();
                this.g.Rectangle(rect.ToRect(aliased));
                this.g.SetSourceColor(fill);
                this.g.Fill();
                this.g.Restore();
            }

            if (stroke.IsVisible() && thickness > 0)
            {
                this.g.Save();
                this.g.SetSourceColor(stroke);
                this.g.LineWidth = thickness;
                this.g.Rectangle(rect.ToRect(aliased));
                this.g.Stroke();
                this.g.Restore();
            }
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="text">The text.</param>
        /// <param name="fill">The fill color.</param>
        /// <param name="fontFamily">The font family.</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="fontWeight">The font weight.</param>
        /// <param name="rotate">The rotation angle.</param>
        /// <param name="halign">The horizontal alignment.</param>
        /// <param name="valign">The vertical alignment.</param>
        /// <param name="maxSize">The maximum size of the text.</param>
        public override void DrawText(
            ScreenPoint p,
            string text,
            OxyColor fill,
            string fontFamily,
            double fontSize,
            double fontWeight,
            double rotate,
            HorizontalAlignment halign,
            VerticalAlignment valign,
            OxySize? maxSize)
        {
            Pango.Layout layout = CreateLayout(text, fontFamily, fontSize, fontWeight, rotate);

#if GTKSHARP4
            // fixme: gircore rectangle handling is clunky.
            // Pango.Rectangle inkRect = new Pango.Rectangle();
            // Pango.Rectangle tmpSize = new Pango.Rectangle();
            // Pango.Internal.Layout.GetExtents(layout.Handle, inkRect.Handle, tmpSize.Handle);
            layout.GetExtents(out Pango.Rectangle inkRect, out Pango.Rectangle size);

            // Pango.Internal.RectangleData data = System.Runtime.InteropServices.Marshal.PtrToStructure<Pango.Internal.RectangleData>(tmpSize.Handle.DangerousGetHandle());
			// Rectangle size = new Rectangle(data.X, data.Y, data.Width, data.Height);
#else
            Pango.Rectangle inkRect;
            Pango.Rectangle size;
            layout.GetExtents(out inkRect, out size);
#endif
            size.Width /= GetPangoScale();
            size.Height /= GetPangoScale();

            if (maxSize != null)
            {
                int maxWidth = (int)Math.Min((Double)Int32.MaxValue, maxSize.Value.Width);
                int maxHeight = (int)Math.Min((Double)Int32.MaxValue, maxSize.Value.Height);
                size.Width = Math.Min(size.Width, maxWidth);
                size.Height = Math.Min(size.Height, maxHeight);
            }
            this.g.Save();
            double dx = 0;
            if (halign == HorizontalAlignment.Center)
            {
                dx = -size.Width / 2;
            }

            if (halign == HorizontalAlignment.Right)
            {
                dx = -size.Width;
            }

            double dy = 0;
            if (valign == VerticalAlignment.Middle)
            {
                dy = -size.Height / 2;
            }

            if (valign == VerticalAlignment.Bottom)
            {
                dy = -size.Height;
            }

            this.g.Translate(p.X, p.Y);
            if (Math.Abs(rotate) > double.Epsilon)
            {
                this.g.Rotate(rotate * Math.PI / 180.0);
            }

            this.g.Translate(dx, dy);

            g.Rectangle(0, 0, size.Width + 0.1f, size.Height + 0.1f);
            g.Clip();
            this.g.SetSourceColor(fill);
#if GTKSHARP4
            PangoCairo.Functions.ShowLayout(this.g, layout);
#else
            Pango.CairoHelper.ShowLayout(this.g, layout);
#endif
            layout.Dispose();
            this.g.Restore();
        }

		private FontDescription CreateFontDescription()
		{
#if GTKSHARP4
            return Pango.FontDescription.New();
#else
            return new Pango.FontDescription();
#endif
		}

		/// <summary>
		/// The measure text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="fontFamily">The font family.</param>
		/// <param name="fontSize">The font size.</param>
		/// <param name="fontWeight">The font weight.</param>
		/// <returns>The size of the text.</returns>
		public override OxySize MeasureText(string text, string fontFamily, double fontSize, double fontWeight)
        {
            if (text == null)
            {
                return OxySize.Empty;
            }
            this.g.Save();
            Pango.Layout layout = CreateLayout(text, fontFamily, fontSize, fontWeight, 0);

            Pango.Rectangle inkRect;
            Pango.Rectangle logicalRect;
#if GTKSHARP4
            layout.GetExtents(out inkRect, out logicalRect);
            int width = logicalRect.GetWidth();
            int height = logicalRect.GetHeight();
#else
            layout.GetExtents(out inkRect, out logicalRect);
            double width = logicalRect.Width;
            double height = logicalRect.Height;
#endif
            width /= GetPangoScale();
            height /= GetPangoScale();

            this.g.Restore();
            layout.Dispose();
            return new OxySize(width, height);
        }

        /// <summary>
        /// Create a pango layout.
        /// </summary>
		private Pango.Layout CreateLayout(string text, string fontFamily, double fontSize, double fontWeight, double rotate)
		{
            Pango.Layout layout = CreateLayout();
            Pango.Weight weight = (fontWeight >= 700) ? Pango.Weight.Bold : Pango.Weight.Normal;
            double size = fontSize * GetPangoScale();
#if GTKSHARP4
            // fixme - gircore doesn't yet support struct methods/properties
            Pango.FontDescription font = Pango.FontDescription.FromString(fontFamily);
            font.SetWeight(weight);
            font.SetAbsoluteSize(size);
            layout.SetFontDescription(font);
            
            // Pango.Internal.FontDescriptionData fontData = System.Runtime.InteropServices.Marshal.PtrToStructure<Pango.Internal.FontDescriptionData>(font.Handle.DangerousGetHandle());
#else
            Pango.FontDescription font = CreateFontDescription();
            font.Family = fontFamily;
            font.Weight = weight;
            font.AbsoluteSize = (int)size;
            layout.FontDescription = font;
#endif
            layout.SetText(text);
            return layout;
        }

        private Pango.Layout CreateLayout()
        {
#if GTKSHARP3
            return new Layout(this.c);
#elif GTKSHARP4
            return Pango.Layout.New(this.c);
#else
            return Pango.CairoHelper.CreateLayout(this.g);
#endif
		}

		/// <summary>
		/// The clean up.
		/// </summary>
		public override void CleanUp()
        {
            var imagesToRelease = this.imageCache.Keys.Where(i => !this.imagesInUse.Contains(i)).ToList();
            foreach (var i in imagesToRelease)
            {
                var image = this.GetImage(i);
                image.Dispose();
                this.imageCache.Remove(i);
            }

            this.imagesInUse.Clear();
#if GTKSHARP3
            this.c.Dispose();
#endif
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="srcX">The source executable.</param>
        /// <param name="srcY">The source asynchronous.</param>
        /// <param name="srcWidth">Width of the source.</param>
        /// <param name="srcHeight">Height of the source.</param>
        /// <param name="x">The executable.</param>
        /// <param name="y">The asynchronous.</param>
        /// <param name="w">The forward.</param>
        /// <param name="h">The authentication.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="interpolate">Interpolate if set to <c>true</c>.</param>
        public override void DrawImage(
            OxyImage source,
            double srcX,
            double srcY,
            double srcWidth,
            double srcHeight,
            double x,
            double y,
            double w,
            double h,
            double opacity,
            bool interpolate)
        {
            var image = this.GetImage(source);
            if (image != null)
            {
                // TODO: srcX, srcY
                this.g.Save();

                /*
                                ImageAttributes ia = null;
                                if (opacity < 1)
                                {
                                    var cm = new ColorMatrix
                                                 {
                                                     Matrix00 = 1f,
                                                     Matrix11 = 1f,
                                                     Matrix22 = 1f,
                                                     Matrix33 = 1f,
                                                     Matrix44 = (float)opacity
                                                 };

                                    ia = new ImageAttributes();
                                    ia.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                                }

                */
                double scalex = w / image.Width;
                double scaley = h / image.Height;

                // This alternative handles opacity, but can exhibit unattractive antialiasing artifacts when scaling up by large factors.
                // Pixbuf imageScaled = new Pixbuf(image.Colorspace, image.HasAlpha, image.BitsPerSample, (int)w, (int)h);
                // image.Composite(image, 0, 0, (int)w, (int)h, srcX, srcY, scalex, scaley, interpolate ? InterpType.Bilinear : InterpType.Nearest, (int)(opacity * 255));
                // image = imageScaled;

                if (!interpolate)
                {
                    image = image.ScaleSimple((int)w, (int)h, InterpType.Nearest);
                    scalex = 1.0;
                    scaley = 1.0;
                }
                this.g.Translate(x, y);
                this.g.Scale(scalex, scaley);
                this.g.Rectangle(0, 0, image.Width, image.Height);
                this.g.SetSourcePixbuf(image, 0.0, 0.0);
                this.g.Fill();
                this.g.Restore();
            }
        }

        /// <summary>
        /// Sets the clip rectangle.
        /// </summary>
        /// <param name="rect">The clip rectangle.</param>
        /// <returns>True if the clip rectangle was set.</returns>
        public override void PushClip(OxyRect rect)
        {
            clipCount++;
            this.g.Save();
            this.g.Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
            this.g.Clip();
        }

        /// <summary>
        /// Resets the clip rectangle.
        /// </summary>
        public override void PopClip()
        {
            if (clipCount < 1)
                throw new InvalidOperationException($"Clip count is already 0 - programmer may be missing a call to {nameof(PushClip)}");
            clipCount--;
            this.g.Restore();
        }

        /// <summary>
        /// Gets the cached <see cref="Pixbuf" /> of the specified <see cref="OxyImage" />.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>The <see cref="Pixbuf" />.</returns>
        private Pixbuf GetImage(OxyImage source)
        {
            if (source == null)
            {
                return null;
            }

            if (!this.imagesInUse.Contains(source))
            {
                this.imagesInUse.Add(source);
            }

            Pixbuf src;
            if (this.imageCache.TryGetValue(source, out src))
            {
                return src;
            }

            Pixbuf btm;
#if GTKSHARP4
            // For the typical case where the inline pixbuf is read-only static
            // data, you don’t need to copy the pixel data unless you intend to
            // write to it.
            btm = Pixbuf.NewFromInline(source.GetData(), false);
#else
            using (var ms = new MemoryStream(source.GetData()))
            {
                btm = new Pixbuf(ms);
            }
#endif
            this.imageCache.Add(source, btm);
            return btm;
        }

        private static int GetPangoScale()
        {
#if GTKSHARP4
            return Pango.Constants.SCALE;
#else
            return (int)Pango.Scale.PangoScale;
#endif
        }
    }
}