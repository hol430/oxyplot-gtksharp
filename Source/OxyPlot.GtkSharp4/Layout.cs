using System;
using Cairo;
using Gtk;

namespace OxyPlot.GtkSharp
{
	public abstract class Layout : DrawingArea
	{
		protected Layout()
		{
			this.SetDrawFunc(OnDrawn);

			GestureClick clickController = GestureClick.New();
			clickController.OnPressed += OnButtonPressEvent;
            clickController.OnReleased += OnButtonReleaseEvent;
            // Set button to 0 to capture all events.
            clickController.Button = 0;
			this.AddController(clickController);

            EventControllerMotion motionController = EventControllerMotion.New();
            motionController.OnEnter += OnEnterNotifyEvent;
            motionController.OnLeave += OnLeaveNotifyEvent;
            motionController.OnMotion += OnMotionNotifyEvent;
            this.AddController(motionController);

            EventControllerScroll scrollController = EventControllerScroll.New(EventControllerScrollFlags.BothAxes);
            scrollController.OnScroll += OnScrollEvent;
            this.AddController(scrollController);

            EventControllerKey keypressController = EventControllerKey.New();
            keypressController.OnKeyPressed += OnKeyPressEvent;
            this.AddController(keypressController);
		}

		protected abstract void OnDrawn(DrawingArea drawingArea, Context cr, int width, int height);

        /// <summary>
        /// Called when the mouse button is pressed.
        /// </summary>
        /// <param name="args">An instance that contains the event data.</param>
        protected abstract void OnButtonPressEvent(GestureClick sender, GestureClick.PressedSignalArgs args);

        /// <summary>
        /// Called on mouse move events.
        /// </summary>
        /// <param name="args">An instance that contains the event data.</param>
        protected abstract void OnMotionNotifyEvent(EventControllerMotion sender, EventControllerMotion.MotionSignalArgs args);

        /// <summary>
        /// Called when the mouse button is released.
        /// </summary>
        /// <param name="args">An instance that contains the event data.</param>
        protected abstract void OnButtonReleaseEvent(GestureClick sender, GestureClick.ReleasedSignalArgs args);

        /// <summary>
        /// Called when the mouse wheel is scrolled.
        /// </summary>
        /// <param name="args">An instance that contains the event data.</param>
        /// <returns>True if event was handled?</returns>
        protected abstract bool OnScrollEvent(EventControllerScroll sender, EventControllerScroll.ScrollSignalArgs args);

        /// <summary>
        /// Called when the mouse enters the widget.
        /// </summary>
        /// <param name="args">An instance that contains the event data.</param>
        protected abstract void OnEnterNotifyEvent(EventControllerMotion sender, EventControllerMotion.EnterSignalArgs args);

        /// <summary>
        /// Called when the mouse leaves the widget.
        /// </summary>
        /// <param name="args">An instance that contains the event data.</param>
        protected abstract void OnLeaveNotifyEvent(EventControllerMotion sender, EventArgs args);

        /// <summary>
        /// Called on KeyPress event.
        /// </summary>
        /// <param name="args">An instance that contains the event data.</param>
        /// <returns>True if event was handled?</returns>
        protected abstract bool OnKeyPressEvent(EventControllerKey sender, EventControllerKey.KeyPressedSignalArgs args);
	}
}
