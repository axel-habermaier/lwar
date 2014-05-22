namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Assets;
	using Math;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Memory;
	using Rendering;
	using Console = Rendering.UserInterface.Console;

	/// <summary>
	///     Displays the in-game console. TODO: Convert to UI framework
	/// </summary>
	internal class ConsoleViewModel : ViewModel
	{
		private readonly Camera2D _camera2D;
		private readonly LayoutRoot _root;
		private readonly SpriteBatch _spriteBatch;
		private readonly ConsoleView _view = new ConsoleView();
		private Texture2D _outputTexture;
		private RenderOutput _renderOutput;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window in which the console should be shown.</param>
		/// <param name="inputDevice">The logical input device that should be used to listen for user input.</param>
		public ConsoleViewModel(AppWindow window, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(window);
			Assert.ArgumentNotNull(inputDevice);

			var font = Application.Current.Assets.Load(Fonts.LiberationMono11);
			Console = new Console(inputDevice, font);
			Console.Update(window.Size);
			OnDraw = Draw;
			OnSizeChanged = InitializeRenderOutput;

			_root = window.LayoutRoot;
			_root.AddTopmost(_view);
			_view.ViewModel = this;
			_camera2D = new Camera2D(Application.Current.GraphicsDevice);

			InitializeRenderOutput(window.Size);

			_spriteBatch = new SpriteBatch(Application.Current.GraphicsDevice, Application.Current.Assets)
			{
				BlendState = BlendState.Premultiplied,
				DepthStencilState = DepthStencilState.DepthDisabled,
				SamplerState = SamplerState.PointClampNoMipmaps
			};
		}

		public Texture2D OutputTexture
		{
			get { return _outputTexture; }
			set { ChangePropertyValue(ref _outputTexture, value); }
		}

		/// <summary>
		///     Gets the in-game console.
		/// </summary>
		public Console Console { get; private set; }

		public Action OnDraw { get; private set; }

		public Action<Size> OnSizeChanged { get; private set; }

		private void Draw()
		{
			_renderOutput.ClearColor(new Color());

			Console.Update(_renderOutput.Viewport.Size);
			Console.Draw(_spriteBatch);

			_spriteBatch.DrawBatch(_renderOutput);
		}

		private void InitializeRenderOutput(Size size)
		{
			DisposeRenderOutput();

			if (size.Width == 0 || size.Height == 0)
				return;

			OutputTexture = new Texture2D(Application.Current.GraphicsDevice, size, SurfaceFormat.Rgba8, TextureFlags.RenderTarget);
			OutputTexture.SetName("RenderOutputPanel.ColorBuffer");

			_camera2D.Viewport = new Rectangle(0, 0, size);

			_renderOutput = new RenderOutput(Application.Current.GraphicsDevice)
			{
				RenderTarget = new RenderTarget(Application.Current.GraphicsDevice, null, OutputTexture),
				Viewport = _camera2D.Viewport,
				Camera = _camera2D
			};
		}

		private void DisposeRenderOutput()
		{
			if (_renderOutput != null)
				_renderOutput.RenderTarget.SafeDispose();

			OutputTexture.SafeDispose();
			_renderOutput.SafeDispose();

			OutputTexture = null;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DisposeRenderOutput();
			Console.SafeDispose();
			_spriteBatch.SafeDispose();
			_root.Remove(_view);
			_camera2D.SafeDispose();
		}
	}
}