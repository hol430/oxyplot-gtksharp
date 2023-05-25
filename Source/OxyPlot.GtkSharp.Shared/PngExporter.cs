// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PngExporter.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides a png exporter based on GTK#.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.GtkSharp
{
    using System;
    using System.IO;

    using Cairo;

    /// <summary>
    /// Provides a png exporter based on GTK#.
    /// </summary>
    public class PngExporter
    {
        /// <summary>
        /// Gets or sets the width of the output image.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the output image.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the resolution of the output image.
        /// </summary>
        public int Resolution { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public OxyColor Background { get; set; }

        private static Format GetFormat()
        {
#if GTKSHARP4
            return Format.Argb32;
#else
            return Format.ARGB32;
#endif
        }

        /// <summary>
        /// Exports the specified <see cref="PlotModel" /> to a png file.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="fileName">Name of the output file.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="background">The background color.</param>
        public static void Export(IPlotModel model, string fileName, int width, int height, Pattern background = null)
        {
            var bm = new ImageSurface(GetFormat(), width, height);
#if !GTKSHARP4
            using (bm)
#endif
            {
                var g = new Context(bm);
#if !GTKSHARP4
                using (g)
#endif
                {
                    if (background != null)
                    {
                        g.Save();
                        g.SetSource(background);
                        g.Rectangle(0, 0, width, height);
                        g.Fill();
                        g.Restore();
                    }

                    var rc = new GraphicsRenderContext { RendersToScreen = false };
                    rc.SetGraphicsTarget(g);
                    model.Update(true);
                    OxyRect rect = new OxyRect(0, 0, width, height);
                    model.Render(rc, rect);
#if GTKSHARP4
                    // todo: cairo_surface_write_to_png not included in GIR file
                    // for some reason.
                    throw new NotImplementedException();
#else
                    bm.WriteToPng(fileName);
#endif
                }
            }
        }

        /// <summary>
        /// Exports the specified <see cref="PlotModel" /> to the specified <see cref="Stream" />.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The output stream.</param>
        public void Export(IPlotModel model, Stream stream)
        {
            var bm = new ImageSurface(GetFormat(), this.Width, this.Height);
#if !GTKSHARP4
            using (bm)
#endif
            {
                var g = new Context(bm);
#if !GTKSHARP4
                using (g)
#endif
                {
                    if (this.Background.IsVisible())
                    {
                        g.Save();
                        var pattern = CreateSolidPattern(this.Background.R, this.Background.G, this.Background.B, this.Background.A);
#if !GTKSHARP4
                        using (pattern)
#endif
                        {
                            g.SetSource(pattern);
                            g.Rectangle(0, 0, this.Width, this.Height);
                            g.Fill();
                        }

                        g.Restore();
                    }

                    var rc = new GraphicsRenderContext { RendersToScreen = false };
                    rc.SetGraphicsTarget(g);
                    model.Update(true);
                    model.Render(rc, new OxyRect(0, 0, Width, Height));

                    // write to a temporary file
                    var tmp = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".png");
#if GTKSHARP4
                    // todo: cairo_surface_write_to_png not included in GIR file
                    // for some reason.
                    throw new NotImplementedException();
#else
                    bm.WriteToPng(tmp);
                    var bytes = File.ReadAllBytes(tmp);

                    // write to the stream
                    stream.Write(bytes, 0, bytes.Length);

                    // delete the temporary file
                    File.Delete(tmp);
#endif
                }
            }
        }

		private SolidPattern CreateSolidPattern(byte r, byte g, byte b, byte a)
		{
#if GTKSHARP4
            return SolidPattern.CreateRgba(r / 255.0, g / 255.0, b / 255.0, a / 255.0);
#else
            return new SolidPattern(r, g, b, a);
#endif
		}
	}
}