namespace Lwar.Screens
{
	using System;
	using Assets;
	using Network;
	using Pegasus.Math;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Pegasus.Rendering.UserInterface;

	/// <summary>
	///     Shows the main menu.
	/// </summary>
	public class MainMenu : Screen
	{
		/// <summary>
		///     The service that is used to automatically discover server instances.
		/// </summary>
		private readonly ServerDiscoveryService _discoveryService = new ServerDiscoveryService();

		/// <summary>
		///     The label that is used to draw the menu.
		/// </summary>
		private Label _label;

		/// <summary>
		///     Initializes the screen.
		/// </summary>
		public override void Initialize()
		{
			IsOpaque = true;

			_label = new Label(Assets.Load(Fonts.LiberationMono11))
			{
				Text = "Welcome to lwar!",
				Alignment = TextAlignment.Centered | TextAlignment.Middle
			};

			_discoveryService.ServerDiscovered += s => Log.Info("Discovered {0}.", s);
			_discoveryService.ServerHasShutdown += s => Log.Info("Removed discovered server {0}.", s);
		}

		/// <summary>
		///     Updates the screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the app screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			_label.Area = new Rectangle(0, 0, (int)Window.ActualWidth, (int)Window.ActualHeight);
			_discoveryService.Update();
		}

		/// <summary>
		///     Draws the user interface elements of the app screen.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			_label.Draw(spriteBatch);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_label.SafeDispose();
			_discoveryService.SafeDispose();
		}
	}
}