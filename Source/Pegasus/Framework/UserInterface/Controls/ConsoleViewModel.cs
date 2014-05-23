namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Assets;
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
		private readonly LayoutRoot _root;
		private readonly SpriteBatch _spriteBatch;
		private readonly ConsoleView _view = new ConsoleView();

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
			Camera = new Camera2D(Application.Current.GraphicsDevice);

			_root = window.LayoutRoot;
			_root.AddTopmost(_view);
			_view.ViewModel = this;

			_spriteBatch = new SpriteBatch(Application.Current.GraphicsDevice, Application.Current.Assets)
			{
				BlendState = BlendState.Premultiplied,
				DepthStencilState = DepthStencilState.DepthDisabled,
				SamplerState = SamplerState.PointClampNoMipmaps
			};
		}

		public Camera2D Camera { get; private set; }

		/// <summary>
		///     Gets the in-game console.
		/// </summary>
		public Console Console { get; private set; }

		public Action<RenderOutput> OnDraw { get; private set; }

		private void Draw(RenderOutput renderOutput)
		{
			renderOutput.ClearColor(new Color());

			Console.Update(renderOutput.Viewport.Size);
			Console.Draw(_spriteBatch);

			_spriteBatch.DrawBatch(renderOutput);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Console.SafeDispose();
			_spriteBatch.SafeDispose();
			_root.Remove(_view);
			Camera.SafeDispose();
		}
	}
}