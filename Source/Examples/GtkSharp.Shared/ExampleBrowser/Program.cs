// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   The main entry point for the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ExampleBrowser
{
    using System;

    using Gtk;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const int width = 800;
            const int height = 600;
#if GTKSHARP4
            var application = Gtk.Application.New("org.oxyplot.examplebrowsergtk4", Gio.ApplicationFlags.FlagsNone);
            application.OnActivate += (sender, args) =>
            {
                var window = new MainWindow();
                window.Application = (Gtk.Application)sender;
                window.SetDefaultSize(width, height);
                window.Present();
            };
            application.Run();
#else
            Application.Init();
            var window = new MainWindow();
            window.SetDefaultSize (width, height);
            window.Visible = true;
            window.ShowAll();
            Application.Run();
#endif
        }
    }
}