// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   The demo program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace GtkSharpDemo
{
    using System;

    using Gtk;

    using OxyPlot;
    using OxyPlot.Series;

    /// <summary>
    /// The demo program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        private static void Main()
        {
#if GTKSHARP4
            PangoCairo.Module.Initialize();
            Pango.Module.Initialize();
            Cairo.Module.Initialize();
            var app = Gtk.Application.New("org.oxyplot.gtksharp4demo2", Gio.ApplicationFlags.FlagsNone);
            app.OnActivate += (sender, args) =>
            {
#else
            Application.Init();
#endif

            const string title = "GtkSharp Demo";
#if GTKSHARP4
            var window = Gtk.ApplicationWindow.New((Gtk.Application)sender);
            window.Title = title;
#else
            using (var window = new Window(title))
#endif
            {
                var plotModel = new PlotModel
                {
                    Title = "Trigonometric functions",
                    Subtitle = "Example using the FunctionSeries",
                    PlotType = PlotType.Cartesian,
                    Background = OxyColors.White
                };
                plotModel.Series.Add(new FunctionSeries(Math.Sin, -10, 10, 0.1, "sin(x)"));
                plotModel.Series.Add(new FunctionSeries(Math.Cos, -10, 10, 0.1, "cos(x)"));
                plotModel.Series.Add(new FunctionSeries(t => 5 * Math.Cos(t), t => 5 * Math.Sin(t), 0, 2 * Math.PI, 0.1,
                    "cos(t),sin(t)"));

                var plotView = new OxyPlot.GtkSharp.PlotView { Model = plotModel, Visible = true };
#if !GTKSHARP4
                using (plotView)
#endif
                {
                    window.SetSizeRequest(800, 600);
#if GTKSHARP4
                    window.Child = plotView;
#else
                    window.Add(plotView);
#endif
                    plotView.GrabFocus();
                    window.Show();
#if !GTKSHARP4
                    window.DeleteEvent += (s, a) =>
                    {
                        Application.Quit();
                        a.RetVal = true;
                    };
                    Application.Run();
#endif
                }
            }
#if GTKSHARP4
            };
            app.Run();
#endif
        }
    }
}