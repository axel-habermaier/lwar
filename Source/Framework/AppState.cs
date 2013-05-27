using System;

namespace Pegasus.Framework
{
	using Platform;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Memory;
	using Rendering;

	/// <summary>
	///   Represents an app state that shows and manages, for instance, a menu or an app session.
	/// </summary>
	public abstract class AppState : DisposableObject
	{
		/// <summary>
		///   Gets or sets the state manager this app state belongs to.
		/// </summary>
		public AppStateManager StateManager { get; internal set; }

		/// <summary>
		///   Gets the context of the application, providing access to all framework objects that can be used by the application.
		/// </summary>
		protected IAppContext Context
		{
			get { return StateManager.Context; }
		}

		/// <summary>
		///   Gets or sets the window that displays the app.
		/// </summary>
		protected Window Window
		{
			get { return Context.Window; }
		}

		/// <summary>
		///   Gets the logical input device that provides all the user input to the app.
		/// </summary>
		protected LogicalInputDevice InputDevice
		{
			get { return Context.LogicalInputDevice; }
		}

		/// <summary>
		///   Gets the graphics device that is used to draw the app.
		/// </summary>
		protected GraphicsDevice GraphicsDevice
		{
			get { return Context.GraphicsDevice; }
		}

		/// <summary>
		///   Gets the assets manager that manages all assets of the app.
		/// </summary>
		protected AssetsManager Assets
		{
			get { return Context.Assets; }
		}

		/// <summary>
		///   Gets a value indicating whether the state is opaque and all app states below it do not need to be drawn.
		/// </summary>
		public bool IsOpaque { get; protected set; }

		/// <summary>
		///   Initializes the game state.
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		///   Updates the app state.
		/// </summary>
		/// <param name="topmost">Indicates whether the app state is the topmost one.</param>
		public abstract void Update(bool topmost);

		/// <summary>
		///   Draws the app state.
		/// </summary>
		/// <param name="output">The output that the state should render to.</param>
		public virtual void Draw(RenderOutput output)
		{
		}

		/// <summary>
		///   Draws the user interface elements of the app state.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public virtual void DrawUserInterface(SpriteBatch spriteBatch)
		{
		}
	}
}