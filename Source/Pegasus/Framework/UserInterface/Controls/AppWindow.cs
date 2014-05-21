namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Memory;
	using Rendering;

	/// <summary>
	///     Represents the default window of an application.
	/// </summary>
	partial class AppWindow
	{
		/// <summary>
		///     The camera that is used to draw the console and the debug overlay.
		/// </summary>
		private readonly Camera _camera;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="title">The title of the window.</param>
		/// <param name="position">The screen position of the window's top left corner.</param>
		/// <param name="size">The size of the window's rendering area.</param>
		/// <param name="mode">Indicates the window mode.</param>
		internal AppWindow(string title, Vector2i position, Size size, WindowMode mode)
			: base(title, position, size, mode)
		{
			InitializeComponents();

			_camera = new Camera2D(Application.Current.GraphicsDevice);
			Console = new ConsoleViewModel(this, InputDevice);
		}

		/// <summary>
		///     Gets the layout root of the application window.
		/// </summary>
		public LayoutRoot LayoutRoot
		{
			get { return _layoutRoot; }
		}

		/// <summary>
		///     Gets or sets the console that should be drawn on top of the window's backbuffer.
		/// </summary>
		internal ConsoleViewModel Console { get; set; }

		/// <summary>
		///     Invoked when the window is being closed.
		/// </summary>
		protected override void OnClosing()
		{
			_camera.SafeDispose();
			Console.SafeDispose();

			base.OnClosing();
		}
	}
}