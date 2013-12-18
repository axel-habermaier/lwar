namespace Lwar.Screens
{
	using System;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Platform;
	using Pegasus.Platform.Assets;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Represents an app screen that shows and manages, for instance, a menu or an app session.
	/// </summary>
	public abstract class Screen : DisposableObject
	{
		/// <summary>
		///     Gets or sets the screen manager this app screen belongs to.
		/// </summary>
		public ScreenManager ScreenManager { get; internal set; }

		/// <summary>
		///     Gets or sets the window that displays the app.
		/// </summary>
		protected Window Window
		{
			get { return Application.Current.Window; }
		}

		/// <summary>
		///     Gets the logical input device that provides all the user input to the app.
		/// </summary>
		protected LogicalInputDevice InputDevice
		{
			get { return Application.Current.Window.InputDevice; }
		}

		/// <summary>
		///     Gets the graphics device that is used to draw the app.
		/// </summary>
		protected GraphicsDevice GraphicsDevice
		{
			get { return Application.Current.GraphicsDevice; }
		}

		/// <summary>
		///     Gets the assets manager that manages all assets of the app.
		/// </summary>
		protected AssetsManager Assets
		{
			get { return Application.Current.Assets; }
		}

		/// <summary>
		///     Gets a value indicating whether the screen is opaque and all app screens below it do not need to be drawn.
		/// </summary>
		public bool IsOpaque { get; protected set; }

		/// <summary>
		///     Initializes the screen.
		/// </summary>
		public abstract void Initialize();

		/// <summary>
		///     Updates the screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the app screen is the topmost one.</param>
		public abstract void Update(bool topmost);

		/// <summary>
		///     Draws the screen.
		/// </summary>
		/// <param name="output">The output that the screen should render to.</param>
		public virtual void Draw(RenderOutput output)
		{
		}

		/// <summary>
		///     Draws the user interface elements of the app screen.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public virtual void DrawUserInterface(SpriteBatch spriteBatch)
		{
		}
	}
}