// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotViewGtk3.cs" company="OxyPlot">
//   Copyright (c) 2015 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Cairo;
using Gdk;
using Gtk;

namespace OxyPlot.GtkSharp
{
    public partial class PlotView
    {
        /// <summary>
        /// Gtk version-specific initialisation.
        /// </summary>
        private void Initialise()
        {
            // gdk4 cursor names listed here:
            // https://docs.gtk.org/gdk4/ctor.Cursor.new_from_name.html
            this.PanCursor = Cursor.NewFromName("pointer", null);
            this.ZoomRectangleCursor = Cursor.NewFromName("nesw-resize", null);
            this.ZoomHorizontalCursor = Cursor.NewFromName("ew-resize", null);
            this.ZoomVerticalCursor = Cursor.NewFromName("ns-resize", null);
        }

        /// <summary>
        /// Sets the cursor type.
        /// </summary>
        /// <param name="cursorType">The cursor type.</param>
        public void SetCursorType(OxyPlot.CursorType cursorType)
        {
            Window window = GetParentWindow();
            if (window == null)
                return; // ?

            switch (cursorType)
            {
                case OxyPlot.CursorType.Pan:
                    window.Cursor = this.PanCursor;
                    break;
                case OxyPlot.CursorType.ZoomRectangle:
                    window.Cursor = this.ZoomRectangleCursor;
                    break;
                case OxyPlot.CursorType.ZoomHorizontal:
                    window.Cursor = this.ZoomHorizontalCursor;
                    break;
                case OxyPlot.CursorType.ZoomVertical:
                    window.Cursor = this.ZoomVerticalCursor;
                    break;
                default:
                    window.Cursor = Gdk.Cursor.NewFromName("default", null);
                    break;
            }
        }

        /// <summary>
        /// Get the toplevel parent window.
        /// </summary>
        private Window GetParentWindow()
        {
            Widget parent = Parent;
            while (parent != null)
                parent = parent.Parent;
            return parent as Window;
        }

        /// <summary>
        /// Called when the view is drawn.
        /// </summary>
        /// <param name="cr">The drawing context.</param>
        /// <returns><c>true</c> if handled, <c>false</c> otherwise.</returns>
        protected override void OnDrawn (DrawingArea drawingArea, Context cr, int width, int height)
        {
            this.DrawPlot (cr);
            if (tracker != null)
                ShowText(tracker.Text, tracker.BgColour, tracker.FgColour, tracker.X, tracker.Y);
            // return base.OnDrawn (cr);
        }

        /// <summary>
        /// Called when the mouse button is pressed.
        /// </summary>
        /// <param name="e">An instance that contains the event data.</param>
        /// <returns><c>true</c> if the event was handled.</returns>
        protected override void OnButtonPressEvent(GestureClick sender, GestureClick.PressedSignalArgs args)
        {
            this.GrabFocus();
            this.ActualController.HandleMouseDown(this, args.ToMouseDownEventArgs(sender));
        }

        /// <summary>
        /// Called on mouse move events.
        /// </summary>
        /// <param name="e">An instance that contains the event data.</param>
        /// <returns><c>true</c> if the event was handled.</returns>
        protected override void OnMotionNotifyEvent(EventControllerMotion sender, EventControllerMotion.MotionSignalArgs args)
        {
            this.ActualController.HandleMouseMove(this, args.ToMouseEventArgs(sender));
        }

        /// <summary>
        /// Called when the mouse button is released.
        /// </summary>
        /// <param name="e">An instance that contains the event data.</param>
        /// <returns><c>true</c> if the event was handled.</returns>
        protected override void OnButtonReleaseEvent(GestureClick sender, GestureClick.ReleasedSignalArgs args)
        {
            this.ActualController.HandleMouseUp(this, args.ToMouseUpEventArgs(sender));
        }

        /// <summary>
        /// Called when the mouse wheel is scrolled.
        /// </summary>
        /// <param name="e">An instance that contains the event data.</param>
        protected override bool OnScrollEvent(EventControllerScroll sender, EventControllerScroll.ScrollSignalArgs args)
        {
            Event evnt = sender.GetCurrentEvent();
            if (evnt == null)
                return false;
            // fixme - get mouse coords
            double x = 0;
            double y = 0;
            return this.ActualController.HandleMouseWheel(this, GetMouseWheelEventArgs(args, evnt.GetModifierState(), x, y));
        }

        /// <summary>
        /// Called when the mouse enters the widget.
        /// </summary>
        /// <param name="e">An instance that contains the event data.</param>
        /// <returns><c>true</c> if the event was handled.</returns>
        protected override void OnEnterNotifyEvent(EventControllerMotion sender, EventControllerMotion.EnterSignalArgs args)
        {
            // If mouse has entered from an inferior window (ie the tracker label),
            // further propagation of the event could be dangerous; e.g. if it results in
            // the label being moved, it will cause further LeaveNotify and MotionNotify
            // events being fired under X11.
            if (sender.ContainsPointer && !sender.IsPointer)
                return;
            this.ActualController.HandleMouseEnter(this, args.ToMouseEventArgs(sender));
        }

        /// <summary>
        /// Called when the mouse leaves the widget.
        /// </summary>
        /// <param name="e">An instance that contains the event data.</param>
        /// <returns><c>true</c> if the event was handled.</returns>
        protected override void OnLeaveNotifyEvent(EventControllerMotion sender, EventArgs args)
        {
            // If mouse has left via an inferior window (ie the tracker label),
            // further propagation of the event could be dangerous; e.g. if it results in
            // the label being moved, it will cause further LeaveNotify and MotionNotify
            // events being fired under X11.
            if (sender.ContainsPointer && ! sender.IsPointer)
                return;
            this.ActualController.HandleMouseLeave(this, new OxyMouseEventArgs()
            {
                Position = new ScreenPoint(0, 0),
                ModifierKeys = sender.GetModifiers()
            });
        }

        /// <summary>
        /// Called on KeyPress event.
        /// </summary>
        /// <param name="e">An instance that contains the event data.</param>
        /// <returns>True if event was handled?</returns>
        protected override bool OnKeyPressEvent(EventControllerKey sender, EventControllerKey.KeyPressedSignalArgs args)
        {
            return this.ActualController.HandleKeyDown(this, args.ToKeyEventArgs(sender));
        }

        /// <summary>
        /// Creates the mouse wheel event arguments.
        /// </summary>
        /// <param name="e">The scroll event args.</param>
        /// <returns>Mouse event arguments.</returns>
        private static OxyMouseWheelEventArgs GetMouseWheelEventArgs(EventControllerScroll.ScrollSignalArgs e, ModifierType modifiers, double x, double y)
        {
            // fixme
            int delta = (int)Math.Sqrt(e.Dx * e.Dx + e.Dy * e.Dy);

            return new OxyMouseWheelEventArgs
            {
                Delta = delta,
                Position = new ScreenPoint(x, y),
                ModifierKeys = ConverterExtensions.GetModifiers(modifiers)
            };
        }
    }
}

