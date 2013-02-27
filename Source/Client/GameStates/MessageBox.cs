using System;

namespace Lwar.Client.GameStates
{
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Rendering.UserInterface;

	/// <summary>
	///   Displays a message to the user.
	/// </summary>
	public class MessageBox : GameState
	{
		/// <summary>
		///   The input that, when triggered, closes the message box.
		/// </summary>
		private readonly LogicalInput _continue;

		/// <summary>
		///   The message that is displayed to the user.
		/// </summary>
		private readonly string _message;

		/// <summary>
		///   The label that draws the message to the screen.
		/// </summary>
		private Label _messageLabel;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="message">The message that should be displayed to the user.</param>
		public MessageBox(string message)
		{
			Assert.ArgumentNotNullOrWhitespace(message, () => message);

			_message = message;
			_continue = new LogicalInput(Key.Space.WentDown(), InputModes.Game);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			InputDevice.Remove(_continue);
		}

		/// <summary>
		///   Initializes the game state.
		/// </summary>
		public override void Initialize()
		{
			_messageLabel = new Label(Assets.LoadFont("Fonts/Liberation Mono 12"))
			{
				Text = _message + "\n\nPress [Space] to continue...",
				Alignment = TextAlignment.Middle | TextAlignment.Centered
			};

			InputDevice.Register(_continue);
		}

		/// <summary>
		///   Updates the game state.
		/// </summary>
		/// <param name="topmost">Indicates whether the game screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			_messageLabel.Area = new Rectangle(0, 0, Window.Width, Window.Height);

			if (topmost && _continue.IsTriggered)
				StateManager.Remove(this);
		}

		/// <summary>
		///   Draws the game state.
		/// </summary>
		public override void Draw()
		{
			SpriteBatch.Draw(_messageLabel.ActualArea, Texture2D.White, new Color(0xEE333333));
			_messageLabel.Draw(SpriteBatch);
		}
	}
}