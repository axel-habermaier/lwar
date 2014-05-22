namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;

	public class Texture2D
	{
	}

	public class RenderOutputPanel : Border
	{
		public static readonly DependencyProperty SizeCallbackProperty = DependencyProperty.Register(
			"SizeCallback", typeof(Action<Size>), typeof(RenderOutputPanel), new PropertyMetadata(default(Action<Size>)));

		public static readonly DependencyProperty Texture2DProperty = DependencyProperty.Register(
			"Texture2D", typeof(Texture2D), typeof(RenderOutputPanel), new PropertyMetadata(default(Texture2D)));

		public static readonly DependencyProperty DrawCallbackProperty = DependencyProperty.Register(
			"DrawCallback", typeof(Action), typeof(RenderOutputPanel), new PropertyMetadata(default(Action)));

		public RenderOutputPanel()
		{
			Background = new SolidColorBrush(Colors.Black);
		}

		public Action DrawCallback
		{
			get { return (Action)GetValue(DrawCallbackProperty); }
			set { SetValue(DrawCallbackProperty, value); }
		}

		public Texture2D Texture2D
		{
			get { return (Texture2D)GetValue(Texture2DProperty); }
			set { SetValue(Texture2DProperty, value); }
		}

		public Action<Size> SizeCallback
		{
			get { return (Action<Size>)GetValue(SizeCallbackProperty); }
			set { SetValue(SizeCallbackProperty, value); }
		}
	}
}