namespace OxyPlot.GtkSharp
{
	internal class TrackerState
	{
		public TrackerState(string text, int x, int y, OxyColor bgColour, OxyColor fgColour)
		{
			this.Text = text;
			this.X = x;
			this.Y = y;
			this.BgColour = bgColour;
			this.FgColour = fgColour;
		}

		public string Text { get; private init; }
		public int X { get; private init; }
		public int Y { get; private init; }
		public OxyColor BgColour { get; private init; }
		public OxyColor FgColour { get; private init; }
	}
}