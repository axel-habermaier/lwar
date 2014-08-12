﻿namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	public class ScrollViewer : System.Windows.Controls.ScrollViewer
	{
		public object ScrollController { get; set; }
		public double VerticalScrollStep { get; set; }
		public double HorizontalScrollStep { get; set; }
	}
}