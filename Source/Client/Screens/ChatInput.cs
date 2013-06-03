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
		///   The input trigger that cancels the chat input.
		/// </summary>
		private readonly LogicalInput _cancel = new LogicalInput(Key.Escape.WentDown(), InputLayers.Chat);

		/// <summary>
		///   The frame around the chat input.
		/// </summary>
		private readonly Frame _frame = new Frame();

		/// <summary>
		///   The game session that is loaded.
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
		///   The chat input text box.
		/// </summary>
		private readonly TextBox _textBox;

		/// <summary>
		///   The input trigger that is used to determine whether the chat input should be shown or whether the chat input
		///   should be sent to the server.
		/// </summary>
		private readonly LogicalInput _trigger = new LogicalInput(Key.Return.WentDown() | Key.NumpadEnter.WentDown(),
																  InputLayers.Game | InputLayers.Chat);

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="inputDevice">The input device that should be used to check for user input.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		/// <param name="gameSession">The game session that should be loaded.</param>
		/// <param name="networkSession">The network session that synchronizes the game state between the client and the server.</param>
		public ChatInput(LogicalInputDevice inputDevice, AssetsManager assets, GameSession gameSession, NetworkSession networkSession)
		{
			Assert.ArgumentNotNull(inputDevice);
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNull(networkSession);

			_inputDevice = inputDevice;
			_gameSession = gameSession;
			_networkSession = networkSession;

			_inputDevice.Add(_trigger);
			_inputDevice.Add(_cancel);

			_inputDevice.Keyboard.CharEntered += OnCharEntered;
			_inputDevice.Keyboard.KeyPressed += OnKeyPressed;

			var font = assets.LoadFont("Fonts/Liberation Mono 12");
			_prompt = new Label(font, "Say: ");
			_textBox = new TextBox(font);
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
			get { return Encoding.UTF8.GetByteCount(_textBox.Text) > Specification.MaximumChatMessageLength; }
		}

		/// <summary>
		///   Gets a value indicating whether the chat input is currently active.
		/// </summary>
		private bool Active
		{
			get { return _inputDevice.InputLayer.Contains(InputLayers.Chat); }
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputDevice.Keyboard.CharEntered -= OnCharEntered;
			_inputDevice.Keyboard.KeyPressed -= OnKeyPressed;

			if (Active)
				_inputDevice.DeactivateLayer(InputLayers.Chat);

			_inputDevice.Remove(_trigger);
			_inputDevice.Remove(_cancel);
			_textBox.SafeDispose();
		}

		/// <summary>
		///   Updates the chat input's state.
		/// </summary>
		/// <param name="size">The size of the window.</param>
		public void Update(Size size)
		{
			// Show or hide the chat input
			if (_trigger.IsTriggered && !Active)
			{
				_inputDevice.ActivateLayer(InputLayers.Chat);
				_inputDevice.TextInputEnabled = true;
			}
			else if ((_cancel.IsTriggered || _trigger.IsTriggered) && Active)
			{
				// Do not do anything if the user tries to send a message that is too long
				if (!_trigger.IsTriggered || !LengthExceeded)
				{
					_inputDevice.DeactivateLayer(InputLayers.Chat);
					_inputDevice.TextInputEnabled = false;

					// If a message has been entered, send it to the server and hide the chat input
					if (!_cancel.IsTriggered && !String.IsNullOrWhiteSpace(_textBox.Text))
						_networkSession.Send(Message.Say(_gameSession.Players.LocalPlayer, _textBox.Text));

					_textBox.Text = String.Empty;
				}
			}

			if (!Active)
				return;

			// Update the chat input's layout
			var right = size.Width - Margin;
			_prompt.Area = new Rectangle(Margin, Margin, _prompt.Font.MeasureWidth(_prompt.Text), 0);

			var messageLeft = _prompt.ActualArea.Right;
			_textBox.Area = new Rectangle(messageLeft, Margin, right - messageLeft, 0);
			_lengthWarning.Area = new Rectangle(messageLeft, _textBox.ActualArea.Bottom + _textBox.Font.LineHeight, right - messageLeft, 0);

			var bottom = _textBox.ActualArea.Bottom;
			if (LengthExceeded)
				bottom = _lengthWarning.ActualArea.Bottom;

			_frame.ContentArea = new Rectangle(Margin, Margin, right - Margin, bottom - Margin);
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