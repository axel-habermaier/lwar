namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;

	public class RenderTargetPanel : ContentControl
	{
		public RenderTargetPanel()
		{
			var panel = new DockPanel { Background = new SolidColorBrush(Colors.Black), LastChildFill = true};
			panel.Children.Add(new TextBlock
			{
				Text = "Render Target Panel", 
				Foreground = new SolidColorBrush(Colors.White),
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				TextAlignment = TextAlignment.Center
			});

			Content = panel;
		}
	}
}