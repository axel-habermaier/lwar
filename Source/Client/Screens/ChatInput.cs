using System;

namespace Lwar.Client.Screens
{
	using System.Text;
	using Gameplay;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;
	using Scripting;

	/// <summary>
	///   Shows the chat input field during an active game session.
	/// </summary>
	public class ChatInput : DisposableObject
	{
		/// <summary>
		///   The margin of the chat input and the border of the screen.
		/// </summary>
		private const int Margin = 100;

		/// <summary>
		///   The input trigger that is used to determine whether the chat input should be shown.
		/// </summary>
		private readonly LogicalInput _activate = new LogicalInput(Cvars.InputChatCvar, InputLayers.Game);

		/// <summary>
		///   The input trigger that cancels the chat input.
		/// </summary>
		private readonly LogicalInput _cancel = new LogicalInput(Key.Escape.WentDown(), InputLayers.Chat);

		/// <summary>
		///   The frame around the chat input.
		/// </summary>
		private readonly Frame _frame = new Frame();

		/// <summary>
		///   The game session that is running.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///   The input device that is used to check for user input.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///   The label that informs the user if the text is too long.
		/// </summary>
		private readonly Label _lengthWarning;

		/// <summary>
		///   The network session that is used to synchronize the game state between the client and the server.
		/// </summary>
		private readonly NetworkSession _networkSession;

		/// <summary>
		///   The chat input prompt.
		/// </summary>
		private readonly Label _prompt;

		/// <summary>
		/// The label that explains how the chat input is used.
		/// </summary>
		private readonly Label _help;

		/// <summary>
		///   The input trigger that submits a non-empty chat message.
		/// </summary>
		private readonly LogicalInput _submit = new LogicalInput(Key.Return.WentDown() | Key.NumpadEnter.WentDown(), InputLayers.Chat);

		/// <summary>
		///   The chat input text box.
		/// </summary>
		private readonly TextBox _textBox;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="inputDevice">The input device that should be used to check for user input.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		/// <param name="gameSession">The game session that is running.</param>
		/// <param name="networkSession">The network session that synchronizes the game state between the client and the server.</param>
		public ChatInput(LogicalInputDevice inputDevice, AssetsManager assets, GameSession gameSession, NetworkSession networkSession)
		{
			Assert.ArgumentNotNull(inputDevice);
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(networkSession);

			_inputDevice = inputDevice;
			_gameSession = gameSession;
			_networkSession = networkSession;

			_inputDevice.Add(_activate);
			_inputDevice.Add(_submit);
			_inputDevice.Add(_cancel);

			_inputDevice.Keyboard.CharEntered += OnCharEntered;
			_inputDevice.Keyboard.KeyPressed += OnKeyPressed;

			var font = assets.LoadFont("Fonts/Liberation Mono 12");
			_prompt = new Label(font, "Say: ");
			_textBox = new TextBox(font);
			_help = new Label(font, "Press [Enter] to send the message.\nPress [Escape] to cancel.") { Color = new Color(0xFF3AC984) };
			_lengthWarning = new Label(font, "The message exceeds the maximum allowed width for a chat message and cannot be sent.")
			{
				Color = new Color(255, 0, 0, 255)
			};
		}

		/// <summary>
		///   Gets a value indicating whether the chat message entered by the user exceeds the maximum allowed length.
		/// </summary>
		private bool LengthExceeded
		{
			get { return Encoding.UTF8.GetByteCount(_textBox.Text) > Specification.ChatMessageLength; }
		}

		/// <summary>
		///   Gets a value indicating whether the chat input is currently active.
		/// </summary>
		private bool Active
		{
			get { return _inputDevice.InputLayer == InputLayers.Chat; }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputDevice.Keyboard.CharEntered -= OnCharEntered;
			_inputDevice.Keyboard.KeyPressed -= OnKeyPressed;

			if (Active)
				Hide();

			_inputDevice.Remove(_activate);
			_inputDevice.Remove(_submit);
			_inputDevice.Remove(_cancel);
			_textBox.SafeDispose();
		}

		/// <summary>
		///   Shows the chat input.
		/// </summary>
		private void Show()
		{
			_inputDevice.ActivateLayer(InputLayers.Chat);
			_inputDevice.TextInputEnabled = true;
		}

		/// <summary>
		///   Hides the chat input and clears the currently entered chat message.
		/// </summary>
		private void Hide()
		{
			_inputDevice.DeactivateLayer(InputLayers.Chat);
			_inputDevice.TextInputEnabled = false;
			_textBox.Text = String.Empty;
		}

		/// <summary>
		///   Sends the current message to the server, if the message is non-empty and does not exceed the length limit. Returns
		///   false to indicate that there was a problem sending the message.
		/// </summary>
		private bool Send()
		{
			if (LengthExceeded)
				return false;

			// Ignore empty messages
			if (!String.IsNullOrWhiteSpace(_textBox.Text))
				_networkSession.Send(Message.Say(_gameSession.Players.LocalPlayer, _textBox.Text));

			return true;
		}

		/// <summary>
		///   Updates the chat input's state.
		/// </summary>
		/// <param name="size">The size of the window.</param>
		public void Update(Size size)
		{
			// Check if the user activated the chat input
			if (_activate.IsTriggered)
				Show();

			if (!Active)
				return;

			// Hide the chat input if the user canceled the input or if the user submitted the input and sending was successful
			if (_cancel.IsTriggered || (_submit.IsTriggered && Send()))
				Hide();

			// Update the chat input's layout
			var right = size.Width - Margin;
			var top = size.Height - 2 * Margin;
			_prompt.Area = new Rectangle(Margin, top, _prompt.Font.MeasureWidth(_prompt.Text), 0);

			var messageLeft = _prompt.ActualArea.Right;
			_textBox.Area = new Rectangle(messageLeft, top, right - messageLeft, 0);
			_lengthWarning.Area = new Rectangle(messageLeft, _textBox.ActualArea.Bottom + _lengthWarning.Font.LineHeight, right - messageLeft, 0);

			var bottom = _textBox.ActualArea.Bottom;
			if (LengthExceeded)
				bottom = _lengthWarning.ActualArea.Bottom;

			_help.Area = new Rectangle(messageLeft, bottom + _help.Font.LineHeight, right - messageLeft, 0);
			_frame.ContentArea = new Rectangle(Margin, top, right - Margin, _help.ActualArea.Bottom - top);
		}

		/// <summary>
		///   Draws the chat input, if it is active.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if (!Active)
				return;

			// Draw the frame
			_frame.Draw(spriteBatch);

			// Draw the prompt, the textbox, and the help label
			_prompt.Draw(spriteBatch);
			_textBox.Draw(spriteBatch);
			_help.Draw(spriteBatch);

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