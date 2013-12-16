namespace Lwar.Screens
{
	using System;
	using Pegasus.Platform;
	using Pegasus.Platform.Assets;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Input;

	/// <summary>
	///     Represents the context of an application, providing access to all framework objects that can be used by an
	///     application.
	/// </summary>
	public struct AppContext
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that renders the output of the application.</param>
		/// <param name="window">The application window the application renders to.</param>
		/// <param name="assets">The asset manager that manages the assets of the application.</param>
		/// <param name="inputDevice">The logical input device that handles all user input to the application.</param>
		public AppContext(GraphicsDevice graphicsDevice, NativeWindow window, AssetsManager assets, LogicalInputDevice inputDevice)
			: this()
		{
			GraphicsDevice = graphicsDevice;
			Window = window;
			Assets = assets;
			InputDevice = inputDevice;
		}

		/// <summary>
		///     Gets the graphics device that renders the output of the application.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///     Gets the application window the application renders to.
		/// </summary>
		public NativeWindow Window { get; private set; }

		/// <summary>
		///     Gets the asset manager that manages the assets of the application.
		/// </summary>
		public AssetsManager Assets { get; private set; }

		/// <summary>
		///     Gets the logical input device that handles all user input to the application.
		/// </summary>
		public LogicalInputDevice InputDevice { get; private set; }
	}
}