using System;

namespace Lwar.Client.GameScreens
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Represents a game screen that shows and manages, for instance, a menu or a game session.
	/// </summary>
	public abstract class GameScreen : DisposableObject
	{
		/// <summary>
		///   Gets or sets the screen manager this game screen belongs to.
		/// </summary>
		public ScreenManager ScreenManager { get; set; }

		/// <summary>
		///   Gets or sets the window that displays the game session.
		/// </summary>
		public Window Window
		{
			get { return ScreenManager.Window; }
		}

		/// <summary>
		///   Gets the logical input device that provides all the user input to the game session.
		/// </summary>
		public LogicalInputDevice InputDevice
		{
			get { return ScreenManager.InputDevice; }
		}

		/// <summary>
		///   Gets the graphics device that is used to draw the game session.
		/// </summary>
		public GraphicsDevice GraphicsDevice
		{
			get { return ScreenManager.GraphicsDevice; }
		}

		/// <summary>
		///   Gets the assets manager that manages all assets of the game session.
		/// </summary>
		public AssetsManager Assets
		{
			get { return ScreenManager.Assets; }
		}

		/// <summary>
		///   Gets the sprite batch that all game screens should use to draw 2D content on the screen.
		/// </summary>
		public SpriteBatch SpriteBatch
		{
			get { return ScreenManager.SpriteBatch; }
		}

		/// <summary>
		///   Gets a value indicating whether the screen is opaque and all game screens below it do not need to be drawn.
		/// </summary>
		public bool IsOpaque { get; protected set; }

		/// <summary>
		///   Initializes the game screen.
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		///   Updates the game screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the game screen is the topmost one.</param>
		public abstract void Update(bool topmost);

		/// <summary>
		///   Draws the game screen.
		/// </summary>
		public abstract void Draw();
	}
}