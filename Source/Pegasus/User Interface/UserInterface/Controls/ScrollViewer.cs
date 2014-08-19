namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Windows;

	public class ScrollViewer : System.Windows.Controls.ScrollViewer
	{
		public static readonly DependencyProperty ScrollControllerProperty = DependencyProperty.Register(
			"ScrollController", typeof(object), typeof(ScrollViewer), new PropertyMetadata(default(object)));

		public object ScrollController
		{
			get { return GetValue(ScrollControllerProperty); }
			set { SetValue(ScrollControllerProperty, value); }
		}

		public double VerticalScrollStep { get; set; }
		public double HorizontalScrollStep { get; set; }
	}
}