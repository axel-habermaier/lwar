namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Windows;

	public class Window : System.Windows.Window
	{
		public static readonly DependencyProperty FullscreenProperty = DependencyProperty.Register(
			"Fullscreen", typeof(bool), typeof(Window), new PropertyMetadata(default(bool)));

		public bool Fullscreen
		{
			get { return (bool)GetValue(FullscreenProperty); }
			set { SetValue(FullscreenProperty, value); }
		}
	}
}