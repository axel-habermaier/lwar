using System;

namespace Lwar.Client.Screens
{
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;

	/// <summary>
	///   Shows the main menu.
	/// </summary>
	public class MainMenu : Screen
	{
		/// <summary>
		///   The label that is used to draw the menu.
		/// </summary>
		private Label _label;

		/// <summary>
		///   Initializes the screen.
		/// </summary>
		public override void Initialize()
		{
			IsOpaque = true;

			_label = new Label(Assets.LoadFont("Fonts/Liberation Mono 12"))
			{
				Text = "Welcome to lwar!",
				Alignment = TextAlignment.Centered | TextAlignment.Middle
			};
		}

		/// <summary>
		///   Updates the screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the app screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			_label.Area = new Rectangle(0, 0, Window.Width, Window.Height);
		}

		/// <summary>
		///   Draws the user interface elements of the app screen.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			_label.Draw(spriteBatch);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
		}
	}
}