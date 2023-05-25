using Gdk;
using Gtk;

namespace OxyPlot.GtkSharp
{
	/// <summary>
	/// Gtk4-specific extension methods.
	/// </summary>
	internal static class Gtk4Extensions
	{
		/// <summary>
		/// Creates the mouse down event arguments.
		/// </summary>
		/// <param name="e">The instance containing the event data.</param>
		/// <returns>Mouse event arguments.</returns>
		public static OxyMouseDownEventArgs ToMouseDownEventArgs(this GestureClick.PressedSignalArgs e, GestureSingle controller)
		{
			return new OxyMouseDownEventArgs
			{
				ChangedButton = controller.ConvertButton(),
				ClickCount = e.NPress,
				Position = new ScreenPoint(e.X, e.Y),
				ModifierKeys = controller.GetModifiers()
			};
		}

        /// <summary>
        /// Creates the mouse up event arguments.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        /// <returns>Mouse event arguments.</returns>
        public static OxyMouseEventArgs ToMouseUpEventArgs(this GestureClick.ReleasedSignalArgs e, EventController controller)
        {
            return new OxyMouseEventArgs
            {
                Position = new ScreenPoint(e.X, e.Y),
                ModifierKeys = controller.GetModifiers()
            };
        }

        /// <summary>
        /// Creates the mouse event arguments.
        /// </summary>
        /// <param name="e">The motion event args.</param>
        /// <returns>Mouse event arguments.</returns>
        public static OxyMouseEventArgs ToMouseEventArgs(this EventControllerMotion.MotionSignalArgs args, EventControllerMotion controller)
        {
            return new OxyMouseEventArgs
            {
                Position = new ScreenPoint(args.X, args.Y),
                ModifierKeys = controller.GetModifiers()
            };
        }

        /// <summary>
        /// Creates the mouse event arguments.
        /// </summary>
        /// <param name="e">The motion event args.</param>
        /// <returns>Mouse event arguments.</returns>
        public static OxyMouseEventArgs ToMouseEventArgs(this EventControllerMotion.EnterSignalArgs args, EventControllerMotion controller)
        {
            return new OxyMouseEventArgs
            {
                Position = new ScreenPoint(args.X, args.Y),
                ModifierKeys = controller.GetModifiers()
            };
        }

        /// <summary>
        /// Creates the key event arguments.
        /// </summary>
        /// <param name="e">The key event args.</param>
        /// <returns>Key event arguments.</returns>
        public static OxyKeyEventArgs ToKeyEventArgs(this EventControllerKey.KeyPressedSignalArgs args, EventControllerKey controller)
        {
            return new OxyKeyEventArgs
			{
				ModifierKeys = controller.GetModifiers(),
				Key = args.Keycode.Convert()
			};
        }

		/// <summary>
		/// Get the modifier keys active for the current event in the specified
		/// event controller.
		/// </summary>
		/// <param name="controller">An event controller.</param>
		/// <returns>The modifier keys active during the event.</returns>
		public static OxyModifierKeys GetModifiers(this EventController controller)
		{
			var @event = controller.GetCurrentEvent();
			if (@event == null || @event.Handle.Equals(System.IntPtr.Zero))
				return OxyModifierKeys.None;
			ModifierType modifier = @event.GetModifierState();
			return modifier.GetModifiers();
		}

        /// <summary>
        /// Converts the changed button.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        /// <returns>The mouse button.</returns>
        private static OxyMouseButton ConvertButton(this GestureSingle controller)
        {
            switch (controller.GetButton())
            {
                case 1:
                    return OxyMouseButton.Left;
                case 2:
                    return OxyMouseButton.Middle;
                case 3:
                    return OxyMouseButton.Right;
                case 4:
                    return OxyMouseButton.XButton1;
                case 5:
                    return OxyMouseButton.XButton2;
            }

            return OxyMouseButton.Left;
        }

        /// <summary>
        /// Converts the specified Gdk.Constants.KEY_
        /// </summary>
        /// <param name="key">The key to convert.</param>
        /// <returns>The converted key.</returns>
        public static OxyKey Convert(this uint key)
        {
			if (key == Gdk.Constants.KEY_A
				|| key == Gdk.Constants.KEY_a)
				return OxyKey.A;
			if (key == Gdk.Constants.KEY_plus
				|| key == Gdk.Constants.KEY_KP_Add)
				return OxyKey.Add;
			if (key == Gdk.Constants.KEY_B
				|| key == Gdk.Constants.KEY_b)
				return OxyKey.B;
			if (key == Gdk.Constants.KEY_BackSpace)
				return OxyKey.Backspace;
			if (key == Gdk.Constants.KEY_C
				|| key == Gdk.Constants.KEY_c)
				return OxyKey.C;
			if (key == Gdk.Constants.KEY_D
				|| key == Gdk.Constants.KEY_d)
				return OxyKey.D;
			if (key == Gdk.Constants.KEY_0)
				return OxyKey.D0;
			if (key == Gdk.Constants.KEY_1)
				return OxyKey.D1;
			if (key == Gdk.Constants.KEY_2)
				return OxyKey.D2;
			if (key == Gdk.Constants.KEY_3)
				return OxyKey.D3;
			if (key == Gdk.Constants.KEY_4)
				return OxyKey.D4;
			if (key == Gdk.Constants.KEY_5)
				return OxyKey.D5;
			if (key == Gdk.Constants.KEY_6)
				return OxyKey.D6;
			if (key == Gdk.Constants.KEY_7)
				return OxyKey.D7;
			if (key == Gdk.Constants.KEY_8)
				return OxyKey.D8;
			if (key == Gdk.Constants.KEY_9)
				return OxyKey.D9;
			if (key == Gdk.Constants.KEY_KP_Decimal)
				return OxyKey.Decimal;
			if (key == Gdk.Constants.KEY_Delete
				|| key == Gdk.Constants.KEY_KP_Delete)
				return OxyKey.Delete;
			if (key == Gdk.Constants.KEY_KP_Divide)
				return OxyKey.Divide;
			if (key == Gdk.Constants.KEY_Down
				|| key == Gdk.Constants.KEY_KP_Down)
				return OxyKey.Down;
			if (key == Gdk.Constants.KEY_E
				|| key == Gdk.Constants.KEY_e)
				return OxyKey.E;
			if (key == Gdk.Constants.KEY_End
				|| key == Gdk.Constants.KEY_KP_End)
				return OxyKey.End;
			if (key == Gdk.Constants.KEY_Return
				|| key == Gdk.Constants.KEY_KP_Enter)
				return OxyKey.Enter;
			if (key == Gdk.Constants.KEY_Escape)
				return OxyKey.Escape;
			if (key == Gdk.Constants.KEY_F
				|| key == Gdk.Constants.KEY_f)
				return OxyKey.F;
			if (key == Gdk.Constants.KEY_F1)
				return OxyKey.F1;
			if (key == Gdk.Constants.KEY_F10)
				return OxyKey.F10;
			if (key == Gdk.Constants.KEY_F11)
				return OxyKey.F11;
			if (key == Gdk.Constants.KEY_F12)
				return OxyKey.F12;
			if (key == Gdk.Constants.KEY_F2)
				return OxyKey.F2;
			if (key == Gdk.Constants.KEY_F3)
				return OxyKey.F3;
			if (key == Gdk.Constants.KEY_F4)
				return OxyKey.F4;
			if (key == Gdk.Constants.KEY_F5)
				return OxyKey.F5;
			if (key == Gdk.Constants.KEY_F6)
				return OxyKey.F6;
			if (key == Gdk.Constants.KEY_F7)
				return OxyKey.F7;
			if (key == Gdk.Constants.KEY_F8)
				return OxyKey.F8;
			if (key == Gdk.Constants.KEY_F9)
				return OxyKey.F9;
			if (key == Gdk.Constants.KEY_G
				|| key == Gdk.Constants.KEY_g)
				return OxyKey.G;
			if (key == Gdk.Constants.KEY_H
				|| key == Gdk.Constants.KEY_h)
				return OxyKey.H;
			if (key == Gdk.Constants.KEY_Home
				|| key == Gdk.Constants.KEY_KP_Home)
				return OxyKey.Home;
			if (key == Gdk.Constants.KEY_I
				|| key == Gdk.Constants.KEY_i)
				return OxyKey.I;
			if (key == Gdk.Constants.KEY_Insert)
				return OxyKey.Insert;
			if (key == Gdk.Constants.KEY_J
				|| key == Gdk.Constants.KEY_j)
				return OxyKey.J;
			if (key == Gdk.Constants.KEY_K
				|| key == Gdk.Constants.KEY_k)
				return OxyKey.K;
			if (key == Gdk.Constants.KEY_L
				|| key == Gdk.Constants.KEY_l)
				return OxyKey.L;
			if (key == Gdk.Constants.KEY_Left)
				return OxyKey.Left;
			if (key == Gdk.Constants.KEY_M
				|| key == Gdk.Constants.KEY_m)
				return OxyKey.M;
			if (key == Gdk.Constants.KEY_asterisk
				|| key == Gdk.Constants.KEY_KP_Multiply)
				return OxyKey.Multiply;
			if (key == Gdk.Constants.KEY_N
				|| key == Gdk.Constants.KEY_n)
				return OxyKey.N;
			if (key == Gdk.Constants.KEY_KP_0)
				return OxyKey.NumPad0;
			if (key == Gdk.Constants.KEY_KP_1)
				return OxyKey.NumPad1;
			if (key == Gdk.Constants.KEY_KP_2)
				return OxyKey.NumPad2;
			if (key == Gdk.Constants.KEY_KP_3)
				return OxyKey.NumPad3;
			if (key == Gdk.Constants.KEY_KP_4)
				return OxyKey.NumPad4;
			if (key == Gdk.Constants.KEY_KP_5)
				return OxyKey.NumPad5;
			if (key == Gdk.Constants.KEY_KP_6)
				return OxyKey.NumPad6;
			if (key == Gdk.Constants.KEY_KP_7)
				return OxyKey.NumPad7;
			if (key == Gdk.Constants.KEY_KP_8)
				return OxyKey.NumPad8;
			if (key == Gdk.Constants.KEY_KP_9)
				return OxyKey.NumPad9;
			if (key == Gdk.Constants.KEY_O
				|| key == Gdk.Constants.KEY_o)
				return OxyKey.O;
			if (key == Gdk.Constants.KEY_P
				|| key == Gdk.Constants.KEY_p)
				return OxyKey.P;
			if (key == Gdk.Constants.KEY_Page_Down)
				return OxyKey.PageDown;
			if (key == Gdk.Constants.KEY_Page_Up)
				return OxyKey.PageUp;
			if (key == Gdk.Constants.KEY_Q
				|| key == Gdk.Constants.KEY_q)
				return OxyKey.Q;
			if (key == Gdk.Constants.KEY_R
				|| key == Gdk.Constants.KEY_r)
				return OxyKey.R;
			if (key == Gdk.Constants.KEY_Right)
				return OxyKey.Right;
			if (key == Gdk.Constants.KEY_S
				|| key == Gdk.Constants.KEY_s)
				return OxyKey.S;
			if (key == Gdk.Constants.KEY_space
				|| key == Gdk.Constants.KEY_KP_Space)
				return OxyKey.Space;
			if (key == Gdk.Constants.KEY_minus
				|| key == Gdk.Constants.KEY_KP_Subtract)
				return OxyKey.Subtract;
			if (key == Gdk.Constants.KEY_T
				|| key == Gdk.Constants.KEY_t)
				return OxyKey.T;
			if (key == Gdk.Constants.KEY_Tab
				|| key == Gdk.Constants.KEY_KP_Tab)
				return OxyKey.Tab;
			if (key == Gdk.Constants.KEY_U
				|| key == Gdk.Constants.KEY_u)
				return OxyKey.U;
			if (key == Gdk.Constants.KEY_Up)
				return OxyKey.Up;
			if (key == Gdk.Constants.KEY_V
				|| key == Gdk.Constants.KEY_v)
				return OxyKey.V;
			if (key == Gdk.Constants.KEY_W
				|| key == Gdk.Constants.KEY_w)
				return OxyKey.W;
			if (key == Gdk.Constants.KEY_X
				|| key == Gdk.Constants.KEY_x)
				return OxyKey.X;
			if (key == Gdk.Constants.KEY_Y
				|| key == Gdk.Constants.KEY_y)
				return OxyKey.Y;
			if (key == Gdk.Constants.KEY_Z
				|| key == Gdk.Constants.KEY_z)
				return OxyKey.Z;
			return OxyKey.Unknown;
        }

		[System.Runtime.InteropServices.DllImport("libpango-1.0.so", EntryPoint = "pango_font_description_set_weight")]
		private static extern void FontDescriptionSetWeight(nint desc, long weight);

		[System.Runtime.InteropServices.DllImport("libpango-1.0.so", EntryPoint = "pango_font_description_set_family")]
		private static extern void FontDescriptionSetFamily(nint desc, string family);

		[System.Runtime.InteropServices.DllImport("libpango-1.0.so", EntryPoint = "pango_font_description_set_absolute_size")]
		private static extern void FontDescriptionSetAbsoluteSize(nint desc, double size);

		public static void SetWeight(this Pango.FontDescription fontDescription, Pango.Weight weight)
		{
			FontDescriptionSetWeight(fontDescription.GetHandle(), (long)weight);
		}

		public static void SetFamily(this Pango.FontDescription fontDescription, string fontFamily)
		{
			FontDescriptionSetFamily(fontDescription.GetHandle(), fontFamily);
		}

		public static void SetAbsoluteSize(this Pango.FontDescription fontDescription, double size)
		{
			FontDescriptionSetAbsoluteSize(fontDescription.GetHandle(), size);
		}

		private static nint GetHandle(this Pango.FontDescription desc)
		{
			if (desc.Handle.IsInvalid)
				throw new System.InvalidOperationException($"FontDescription handle is stale");
			return desc.Handle.DangerousGetHandle();
		}
	}
}
