namespace Pegasus.UserInterface.Views
{
	using System;
	using System.Windows.Controls;

	public partial class DebugOverlayView : UserControl
	{
		public DebugOverlayView()
		{
			InitializeComponent();
		}
	}

	internal class DebugOverlayViewModel
	{
		public string Platform
		{
			get { return "Windows (x64)"; }
		}

		public string DebugMode
		{
			get { return "true"; }
		}

		public string Renderer
		{
			get { return "Direct3D 11"; }
		}

		public string GarbageCollections
		{
			get { return "2"; }
		}

		public string GpuTime
		{
			get { return "0.1"; }
		}

		public string CpuTime
		{
			get { return "1.1"; }
		}

		public string UpdateTime
		{
			get { return "0.1"; }
		}

		public string RenderTime
		{
			get { return "1.0"; }
		}

		public bool IsVisible
		{
			get { return true; }
		}

		public int ParticleCount
		{
			get { return 101; }
		}

		public string ParticleUpdateTime
		{
			get { return "2.3"; }
		}

		public string ParticleRenderTime
		{
			get { return "0.1"; }
		}
	}
}