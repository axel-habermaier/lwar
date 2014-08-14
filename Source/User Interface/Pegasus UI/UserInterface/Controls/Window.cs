namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Windows;

	public enum WindowMode
	{
	}

	public class Window : System.Windows.Window
	{
		public static readonly DependencyProperty FullscreenProperty = DependencyProperty.Register(
			"Fullscreen", typeof(bool), typeof(Window), new PropertyMetadata(default(bool)));

		public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
			"Size", typeof(Size), typeof(Window), new PropertyMetadata(default(Size)));

		public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
			"Position", typeof(Point), typeof(Window), new PropertyMetadata(default(Point)));

		public static readonly DependencyProperty WindowModeProperty = DependencyProperty.Register(
			"WindowMode", typeof(WindowMode), typeof(Window), new PropertyMetadata(default(WindowMode)));

		public bool Fullscreen
		{
			get { return (bool)GetValue(FullscreenProperty); }
			set { SetValue(FullscreenProperty, value); }
		}

		public Size Size
		{
			get { return (Size)GetValue(SizeProperty); }
			set { SetValue(SizeProperty, value); }
		}

		public Point Position
		{
			get { return (Point)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}

		public WindowMode WindowMode
		{
			get { return (WindowMode)GetValue(WindowModeProperty); }
			set { SetValue(WindowModeProperty, value); }
		}
	}
}