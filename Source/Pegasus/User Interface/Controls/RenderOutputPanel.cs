namespace Pegasus.UserInterface.Controls
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;
	using Platform.Graphics;

	public class RenderOutputPanel : Border
	{
		public static readonly DependencyProperty HasDepthStencilProperty = DependencyProperty.Register(
			"HasDepthStencil", typeof(bool), typeof(RenderOutputPanel), new PropertyMetadata(false));

		public static readonly DependencyProperty DepthStencilFormatProperty = DependencyProperty.Register(
			"DepthStencilFormat", typeof(SurfaceFormat), typeof(RenderOutputPanel), new PropertyMetadata(SurfaceFormat.Depth24Stencil8));

		public static readonly DependencyProperty ColorBufferFormatProperty = DependencyProperty.Register(
			"ColorBufferFormat", typeof(SurfaceFormat), typeof(RenderOutputPanel), new PropertyMetadata(SurfaceFormat.Rgba8));

		public static readonly DependencyProperty RenderOutputProperty = DependencyProperty.Register(
			"RenderOutput", typeof(string), typeof(RenderOutputPanel), new PropertyMetadata(default(string)));

		public static readonly DependencyProperty ResolutionSourceProperty = DependencyProperty.Register(
			"ResolutionSource", typeof(ResolutionSource), typeof(RenderOutputPanel), new PropertyMetadata(default(ResolutionSource)));

		public static readonly DependencyProperty ResolutionProperty = DependencyProperty.Register(
			"Resolution", typeof(Size), typeof(RenderOutputPanel), new PropertyMetadata(default(Size)));

		public RenderOutputPanel()
		{
			Background = new SolidColorBrush(Colors.Black);
		}

		public Size Resolution
		{
			get { return (Size)GetValue(ResolutionProperty); }
			set { SetValue(ResolutionProperty, value); }
		}

		public ResolutionSource ResolutionSource
		{
			get { return (ResolutionSource)GetValue(ResolutionSourceProperty); }
			set { SetValue(ResolutionSourceProperty, value); }
		}

		public bool HasDepthStencil
		{
			get { return (bool)GetValue(HasDepthStencilProperty); }
			set { SetValue(HasDepthStencilProperty, value); }
		}

		public SurfaceFormat DepthStencilFormat
		{
			get { return (SurfaceFormat)GetValue(DepthStencilFormatProperty); }
			set { SetValue(DepthStencilFormatProperty, value); }
		}

		public SurfaceFormat ColorBufferFormat
		{
			get { return (SurfaceFormat)GetValue(ColorBufferFormatProperty); }
			set { SetValue(ColorBufferFormatProperty, value); }
		}

		public string RenderOutput
		{
			get { return (string)GetValue(RenderOutputProperty); }
			set { SetValue(RenderOutputProperty, value); }
		}
	}
}