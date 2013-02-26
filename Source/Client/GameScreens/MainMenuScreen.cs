using System;

namespace Lwar.Client.GameScreens
{
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Rendering.UserInterface;

	/// <summary>
	///   Shows the main menu.
	/// </summary>
	public class MainMenuScreen : GameScreen
	{
		/// <summary>
		///   The label that is used to draw the menu.
		/// </summary>
		private Label _label;

		/// <summary>
		///   Initializes the game screen.
		/// </summary>
		public override void Initialize()
		{
			_label = new Label(Assets.LoadFont("Fonts/Liberation Mono 12"))
			{
				Text = "Welcome to lwar!",
				Alignment = TextAlignment.Centered | TextAlignment.Middle
			};
		}

		/// <summary>
		///   Updates the game screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the game screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			_label.Area = new Rectangle(0, 0, Window.Width, Window.Height);
		}

		/// <summary>
		///   Draws the game screen.
		/// </summary>
		public override void Draw()
		{
			_label.Draw(SpriteBatch);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
		}
	}
}