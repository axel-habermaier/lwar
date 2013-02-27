using System;

namespace Lwar.Client.GameStates
{
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Rendering.UserInterface;

	/// <summary>
	///   Shows the main menu.
	/// </summary>
	public class MainMenu : GameState
	{
		/// <summary>
		///   The label that is used to draw the menu.
		/// </summary>
		private Label _label;

		/// <summary>
		///   Initializes the game state.
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
		///   Updates the game state.
		/// </summary>
		/// <param name="topmost">Indicates whether the game screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			_label.Area = new Rectangle(0, 0, Window.Width, Window.Height);
		}

		/// <summary>
		///   Draws the game state.
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