using System;

namespace Lwar.Client.GameStates
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Represents a game state that shows and manages, for instance, a menu or a game session.
	/// </summary>
	public abstract class GameState : DisposableObject
	{
		/// <summary>
		///   Gets or sets the state manager this game state belongs to.
		/// </summary>
		public StateManager StateManager { get; set; }

		/// <summary>
		///   Gets or sets the window that displays the game.
		/// </summary>
		protected Window Window
		{
			get { return StateManager.Window; }
		}

		/// <summary>
		///   Gets the logical input device that provides all the user input to the game.
		/// </summary>
		protected LogicalInputDevice InputDevice
		{
			get { return StateManager.InputDevice; }
		}

		/// <summary>
		///   Gets the graphics device that is used to draw the game.
		/// </summary>
		protected GraphicsDevice GraphicsDevice
		{
			get { return StateManager.GraphicsDevice; }
		}

		/// <summary>
		///   Gets the render target that the state should render into.
		/// </summary>
		protected RenderTarget RenderTarget
		{
			get { return StateManager.RenderTarget; }
		}

		/// <summary>
		///   Gets the assets manager that manages all assets of the game.
		/// </summary>
		protected AssetsManager Assets
		{
			get { return StateManager.Assets; }
		}

		/// <summary>
		///   Gets a value indicating whether the state is opaque and all game state below it do not need to be drawn.
		/// </summary>
		public bool IsOpaque { get; protected set; }

		/// <summary>
		///   Initializes the game state.
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		///   Updates the game state.
		/// </summary>
		/// <param name="topmost">Indicates whether the game state is the topmost one.</param>
		public abstract void Update(bool topmost);

		/// <summary>
		///   Draws the game state.
		/// </summary>
		public virtual void Draw()
		{
		}

		/// <summary>
		///   Draws the user interface elements of the game state.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public virtual void DrawUserInterface(SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		///   Shows a message box with the given message, optionally removing the current state from the state manager.
		/// </summary>
		/// <param name="message">The message that should be displayed.</param>
		/// <param name="logType">The type of the message that should be logged.</param>
		/// <param name="removeState">Indicates whether the current state should be removed from the state manager.</param>
		protected void ShowMessageBox(string message, LogType logType, bool removeState = false)
		{
			new LogEntry(logType, message).RaiseLogEvent();
			StateManager.Add(new MessageBox(message));

			if (removeState)
				StateManager.Remove(this);
		}
	}
}