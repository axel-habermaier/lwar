﻿namespace Lwar.UserInterface
{
	using System;
	using System.Text;
	using Network;
	using Pegasus;
	using Pegasus.Assets;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Input;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;
	using Pegasus.Rendering.UserInterface;
	using Scripting;
	using Fonts = Assets.Fonts;

	/// <summary>
	///     Shows the chat input field during an active game session.
	/// </summary>
	public class ChatInput : DisposableObject
	{
		/// <summary>
		///     The margin of the chat input and the border of the screen.
		/// </summary>
		private const int Margin = 100;

		/// <summary>
		///     The input trigger that is used to determine whether the chat input should be shown.
		/// </summary>
		private readonly LogicalInput _activate = new LogicalInput(Cvars.InputChatCvar, InputLayers.Game);

		/// <summary>
		///     The input trigger that cancels the chat input.
		/// </summary>
		private readonly LogicalInput _cancel = new LogicalInput(Key.Escape.WentDown(), InputLayers.Chat);

		/// <summary>
		///     The frame around the chat input.
		/// </summary>
		private readonly Frame _frame = new Frame();

		/// <summary>
		///     The input device that is used to check for user input.
		/// </summary>
		private readonly LogicalInputDevice _inputDevice;

		/// <summary>
		///     The label that informs the user if the text is too long.
		/// </summary>
		private readonly Label _lengthWarning;

		/// <summary>
		///     The chat input prompt.
		/// </summary>
		private readonly Label _prompt;

		/// <summary>
		///     The input trigger that submits a non-empty chat message.
		/// </summary>
		private readonly LogicalInput _submit = new LogicalInput(Key.Return.WentDown() | Key.NumpadEnter.WentDown(), InputLayers.Chat);

		/// <summary>
		///     The chat input text box.
		/// </summary>
		private readonly TextBox _textBox;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="inputDevice">The input device that should be used to check for user input.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		public ChatInput(LogicalInputDevice inputDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(inputDevice);

			_inputDevice = inputDevice;

			_inputDevice.Add(_activate);
			_inputDevice.Add(_submit);
			_inputDevice.Add(_cancel);

			_inputDevice.Keyboard.CharacterEntered += OnCharacterEntered;
			_inputDevice.Keyboard.KeyPressed += OnKeyPressed;

			var font = assets.Load(Fonts.LiberationMono11);
			_prompt = new Label(font, "Say: ");
			_textBox = new TextBox(font);
			_lengthWarning = new Label(font, "The message exceeds the maximum allowed width for a chat message and cannot be sent.")
			{
				Color = new Color(255, 0, 0, 255)
			};
		}

		/// <summary>
		///     Gets a value indicating whether the chat message entered by the user exceeds the maximum allowed length.
		/// </summary>
		private bool LengthExceeded
		{
			get { return Encoding.UTF8.GetByteCount(_textBox.Text) > Specification.ChatMessageLength; }
		}

		/// <summary>
		///     Gets a value indicating whether the chat input is currently active.
		/// </summary>
		private bool Active
		{
			get { return _inputDevice.InputLayer == InputLayers.Chat; }
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_inputDevice.Keyboard.CharacterEntered -= OnCharacterEntered;
			_inputDevice.Keyboard.KeyPressed -= OnKeyPressed;

			if (Active)
				Hide();

			_inputDevice.Remove(_activate);
			_inputDevice.Remove(_submit);
			_inputDevice.Remove(_cancel);

			_textBox.SafeDispose();
			_lengthWarning.SafeDispose();
			_prompt.SafeDispose();
		}

		/// <summary>
		///     Shows the chat input.
		/// </summary>
		private void Show()
		{
			_inputDevice.ActivateLayer(InputLayers.Chat);
			_inputDevice.TextInputEnabled = true;
		}

		/// <summary>
		///     Hides the chat input and clears the currently entered chat message.
		/// </summary>
		private void Hide()
		{
			_inputDevice.DeactivateLayer(InputLayers.Chat);
			_inputDevice.TextInputEnabled = false;
			_textBox.Text = String.Empty;
		}

		/// <summary>
		///     Sends the current message to the server, if the message is non-empty and does not exceed the length limit. Returns
		///     false to indicate that there was a problem sending the message.
		/// </summary>
		private bool Send()
		{
			if (LengthExceeded)
				return false;

			// Ignore empty messages
			if (!String.IsNullOrWhiteSpace(_textBox.Text))
				Commands.Say(_textBox.Text);

			return true;
		}

		/// <summary>
		///     Updates the chat input's state.
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

			_frame.ContentArea = new Rectangle(Margin, top, right - Margin, bottom - top);
		}

		/// <summary>
		///     Draws the chat input, if it is active.
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
		///     Invoked whenever a printable character is entered.
		/// </summary>
		/// <param name="c">The character that has been entered.</param>
		private void OnCharacterEntered(char c)
		{
			if (Active)
				_textBox.InsertCharacter(c);
		}

		/// <summary>
		///     Invoked whenever a key is pressed.
		/// </summary>
		/// <param name="key">The key that was pressed.</param>
		private void OnKeyPressed(KeyEventArgs key)
		{
			if (Active)
				_textBox.InjectKeyPress(key);
		}
	}
}