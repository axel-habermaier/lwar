using System;

namespace Lwar.Client.Screens
{
	using System.Text;
	using Gameplay;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;

	/// <summary>
	///   Shows the chat input field during an active game session.
	/// </summary>
	public class ChatInput : Screen
	{
		/// <summary>
		///   The width of the chat input's background border.
		/// </summary>
		private const int BorderWidth = 5;

		/// <summary>
		///   The margin of the chat input and the border of the screen.
		/// </summary>
		private const int Margin = 100;

		/// <summary>
		///   The input trigger that cancels the chat input.
		/// </summary>
		private readonly LogicalInput _cancel = new LogicalInput(Key.Escape.WentDown(), InputModes.Chat);

		/// <summary>
		///   The game session that is loaded.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///   The network session that is used to synchronize the game state between the client and the server.
		/// </summary>
		private readonly NetworkSession _networkSession;

		/// <summary>
		///   The input trigger that is used to determine whether the chat input should be shown or whether the chat input
		///   should be sent to the server.
		/// </summary>
		private readonly LogicalInput _trigger = new LogicalInput(Key.Return.WentDown() | Key.NumpadEnter.WentDown(),
																  InputModes.Game | InputModes.Chat);

		/// <summary>
		///   The area covered by the chat input.
		/// </summary>
		private Rectangle _area;

		/// <summary>
		///   The label that informs the user if the text is too long.
		/// </summary>
		private Label _lengthWarning;

		/// <summary>
		///   The chat input prompt.
		/// </summary>
		private Label _prompt;

		/// <summary>
		///   The chat input text box.
		/// </summary>
		private TextBox _textBox;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that should be loaded.</param>
		/// <param name="networkSession">The network session that synchronizes the game state between the client and the server.</param>
		public ChatInput(GameSession gameSession, NetworkSession networkSession)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(networkSession);

			_gameSession = gameSession;
			_networkSession = networkSession;
		}

		/// <summary>
		///   Gets a value indicating whether the chat message entered by the user exceeds the maximum allowed length.
		/// </summary>
		private bool LengthExceeded
		{
			get { return Encoding.UTF8.GetByteCount(_textBox.Text) > Specification.MaximumChatMessageLength; }
		}

		/// <summary>
		///   Gets a value indicating whether the chat input is currently active.
		/// </summary>
		private bool Active
		{
			get { return (InputDevice.Modes & InputModes.Chat) == InputModes.Chat; }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			InputDevice.Keyboard.CharEntered -= OnCharEntered;
			InputDevice.Keyboard.KeyPressed -= OnKeyPressed;

			InputDevice.Remove(_trigger);
			InputDevice.Remove(_cancel);
			_textBox.SafeDispose();
		}

		/// <summary>
		///   Initializes the screen.
		/// </summary>
		public override void Initialize()
		{
			InputDevice.Register(_trigger);
			InputDevice.Register(_cancel);

			InputDevice.Keyboard.CharEntered += OnCharEntered;
			InputDevice.Keyboard.KeyPressed += OnKeyPressed;

			var font = Assets.LoadFont("Fonts/Liberation Mono 12");
			_prompt = new Label(font, "Say: ");
			_textBox = new TextBox(font);
			_lengthWarning = new Label(font, "The message exceeds the maximum allowed width for a chat message and cannot be sent.")
			{
				Color = new Color(255, 0, 0, 255)
			};
		}

		/// <summary>
		///   Updates the screen.
		/// </summary>
		/// <param name="topmost">Indicates whether the app screen is the topmost one.</param>
		public override void Update(bool topmost)
		{
			// Show or hide the chat input
			if (_trigger.IsTriggered && !Active)
			{
				Assert.That(InputDevice.Modes == InputModes.Game, "Unexpected input state.");
				InputDevice.Modes = InputModes.Chat;
			}
			else if ((_cancel.IsTriggered || _trigger.IsTriggered) && Active)
			{
				// Do not do anything if the user tries to send a message that is too long
				if (!_trigger.IsTriggered || !LengthExceeded)
				{
					InputDevice.Modes &= ~InputModes.Chat;
					InputDevice.Modes |= InputModes.Game;

					// If a message has been entered, send it to the server and hide the chat input
					if (!_cancel.IsTriggered && !String.IsNullOrWhiteSpace(_textBox.Text))
						_networkSession.Send(Message.Say(_gameSession.Players.LocalPlayer, _textBox.Text));

					_textBox.Text = String.Empty;
				}
			}

			if (!Active)
				return;

			// Update the chat input's layout
			var right = Window.Width - Margin;
			_prompt.Area = new Rectangle(Margin, Margin, _prompt.Font.MeasureWidth(_prompt.Text), 0);

			var messageLeft = _prompt.ActualArea.Right;
			_textBox.Area = new Rectangle(messageLeft, Margin, right - messageLeft, 0);
			_lengthWarning.Area = new Rectangle(messageLeft, _textBox.ActualArea.Bottom + _textBox.Font.LineHeight, right - messageLeft, 0);

			var bottom = _textBox.ActualArea.Bottom;
			if (LengthExceeded)
				bottom = _lengthWarning.ActualArea.Bottom;

			_area = new Rectangle(Margin, Margin, right - Margin, bottom - Margin);
		}

		/// <summary>
		///   Draws the user interface elements of the app screen.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		public override void DrawUserInterface(SpriteBatch spriteBatch)
		{
			if (!Active)
				return;

			// Draw a background
			spriteBatch.Draw(_area.Enlarge(BorderWidth), Texture2D.White, new Color(32, 32, 32, 16));

			// Draw the prompt and the textbox
			_prompt.Draw(spriteBatch);
			_textBox.Draw(spriteBatch);

			// Draw the length warning
			if (LengthExceeded)
				_lengthWarning.Draw(spriteBatch);
		}

		/// <summary>
		///   Invoked whenever a printable character is entered.
		/// </summary>
		/// <param name="c">The character that has been entered.</param>
		private void OnCharEntered(char c)
		{
			if (Active)
				_textBox.InsertCharacter(c);
		}

		/// <summary>
		///   Invoked whenever a key is pressed.
		/// </summary>
		/// <param name="key">The key that was pressed.</param>
		private void OnKeyPressed(KeyEventArgs key)
		{
			if (Active)
				_textBox.InjectKeyPress(key);
		}
	}
}