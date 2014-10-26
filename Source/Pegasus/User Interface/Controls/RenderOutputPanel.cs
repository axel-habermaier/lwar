namespace Pegasus.UserInterface.Controls
{
	using System;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;
	using System.Windows.Media.Media3D;
	using Platform.Graphics;

	public class RenderOutputPanel : Border
	{
		public static readonly DependencyProperty HasDepthStencilProperty = DependencyProperty.Register(
			"HasDepthStencil", typeof(bool), typeof(RenderOutputPanel), new PropertyMetadata(false));

		public static readonly DependencyProperty DepthStencilFormatProperty = DependencyProperty.Register(
			"DepthStencilFormat", typeof(SurfaceFormat), typeof(RenderOutputPanel), new PropertyMetadata(SurfaceFormat.Depth24Stencil8));

		public static readonly DependencyProperty ColorBufferFormatProperty = DependencyProperty.Register(
			"ColorBufferFormat", typeof(SurfaceFormat), typeof(RenderOutputPanel), new PropertyMetadata(SurfaceFormat.Rgba8));

		public static readonly DependencyProperty DrawMethodProperty = DependencyProperty.Register(
			"DrawMethod", typeof(string), typeof(RenderOutputPanel), new PropertyMetadata(default(Action)));

		public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
			"Camera", typeof(Camera), typeof(RenderOutputPanel), new PropertyMetadata(default(Camera)));

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

		public Camera Camera
		{
			get { return (Camera)GetValue(CameraProperty); }
			set { SetValue(CameraProperty, value); }
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

		public string DrawMethod
		{
			get { return (string)GetValue(DrawMethodProperty); }
			set { SetValue(DrawMethodProperty, value); }
		}
	}
}