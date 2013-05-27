﻿using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using Math;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Memory;
	using Scripting;

	/// <summary>
	///   A Quake-like in-game console.
	/// </summary>
	internal sealed class Console : DisposableObject
	{
		/// <summary>
		///   The display color of error messages.
		/// </summary>
		private static readonly Color ErrorColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

		/// <summary>
		///   The display color of warnings.
		/// </summary>
		private static readonly Color WarningColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);

		/// <summary>
		///   The display color of normal messages.
		/// </summary>
		private static readonly Color InfoColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		/// <summary>
		///   The background color of the console.
		/// </summary>
		private static readonly Color BackgroundColor = new Color(0xEE333333);

		/// <summary>
		///   The display color of debug messages.
		/// </summary>
		private static readonly Color DebugInfoColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);

		/// <summary>
		///   The command registry that is used to look up commands.
		/// </summary>
		private readonly CommandRegistry _commands;

		/// <summary>
		///   The font used by the console.
		/// </summary>
		private readonly Font _font;

		/// <summary>
		///   The current input for the console.
		/// </summary>
		private readonly ConsoleInput _input;

		/// <summary>
		///   The console's prompt.
		/// </summary>
		private readonly ConsolePrompt _prompt;

		/// <summary>
		///   The sprite batch that is used for drawing.
		/// </summary>
		private readonly SpriteBatch _spriteBatch;

		/// <summary>
		///   Indicates whether the console is currently active.
		/// </summary>
		private bool _active;

		/// <summary>
		///   The console's content.
		/// </summary>
		private ConsoleContent _content;

		/// <summary>
		///   The margin between the console borders and the rows.
		/// </summary>
		private Size _margin = new Size(5, 5);

		/// <summary>
		///   The current size of the console.
		/// </summary>
		private Size _size;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="inputDevice">The input device that provides the user input.</param>
		/// <param name="spriteBatch">The sprite batch that should be used for drawing.</param>
		/// <param name="font">The font that should be used for drawing.</param>
		/// <param name="commands">The command registry that should be used to look up commands.</param>
		/// <param name="cvars">The cvar registry that should be used to look up cvars.</param>
		public Console(GraphicsDevice graphicsDevice, LogicalInputDevice inputDevice, SpriteBatch spriteBatch, Font font,
					   CommandRegistry commands, CvarRegistry cvars)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(inputDevice, () => inputDevice);
			Assert.ArgumentNotNull(spriteBatch, () => spriteBatch);
			Assert.ArgumentNotNull(font, () => font);
			Assert.ArgumentNotNull(commands, () => commands);
			Assert.ArgumentNotNull(cvars, () => cvars);

			_spriteBatch = spriteBatch;
			_font = font;

			_content = new ConsoleContent(_font);
			_prompt = new ConsolePrompt(_font, InfoColor, commands, cvars);
			_input = new ConsoleInput(inputDevice);
			_commands = commands;

			_commands.OnShowConsole += ShowConsole;
			Log.OnError += ShowError;
			Log.OnWarning += ShowWarning;
			Log.OnInfo += ShowInfo;
			Log.OnDebugInfo += ShowDebugInfo;

			_input.CharEntered += OnCharEntered;
			_input.KeyPressed += OnKeyPressed;
		}

		/// <summary>
		///   Gets or sets a value indicating whether the console is currently active.
		/// </summary>
		private bool Active
		{
			get { return _active; }
			set
			{
				if (value == _active)
					return;

				_active = value;
				_prompt.Clear();
				_input.OnActivationChanged(value);
			}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_commands.OnShowConsole -= ShowConsole;
			Log.OnError -= ShowError;
			Log.OnWarning -= ShowWarning;
			Log.OnInfo -= ShowInfo;
			Log.OnDebugInfo -= ShowDebugInfo;

			_input.CharEntered -= OnCharEntered;
			_input.KeyPressed -= OnKeyPressed;
			_input.SafeDispose();

			_prompt.SafeDispose();
		}

		/// <summary>
		///   Invoked whenever a printable character is entered.
		/// </summary>
		/// <param name="c">The character that has been entered.</param>
		private void OnCharEntered(char c)
		{
			if (_active)
				_prompt.InsertCharacter(c);
		}

		/// <summary>
		///   Invoked whenever a key is pressed.
		/// </summary>
		/// <param name="key">The key that was pressed.</param>
		private void OnKeyPressed(KeyEventArgs key)
		{
			if (!_active)
				return;

			_prompt.InjectKeyPress(key);
		}

		/// <summary>
		///   Draws the console.
		/// </summary>
		public void Draw()
		{
			// Only draw the console when it is currently active
			if (!_active)
				return;

			_spriteBatch.UseScissorTest = false;
			_spriteBatch.WorldMatrix = Matrix.Identity;

			// Draw the background
			var consoleArea = new Rectangle(0, 0, _size.Width, _prompt.ActualArea.Bottom + _margin.Height);
			_spriteBatch.Draw(consoleArea, Texture2D.White, BackgroundColor);

			// Draw the prompt and content
			_prompt.Draw(_spriteBatch);
			_content.Draw(_spriteBatch);
			_spriteBatch.DrawBatch();

			_spriteBatch.UseScissorTest = false;
			_spriteBatch.WorldMatrix = Matrix.Identity;
		}

		/// <summary>
		///   Resizes the console.
		/// </summary>
		/// <param name="size">The new size.</param>
		internal void Resize(Size size)
		{
			_size.Width = size.Width;
			_size.Height = size.Height / 2;

			// Calculate the prompt area
			var promptArea = new Rectangle(_margin.Width, _size.Height - _font.LineHeight - _margin.Height,
										   _size.Width - 2 * _margin.Width, _font.LineHeight);
			_prompt.Resize(promptArea);

			// Resize the content area
			var contentArea = new Rectangle(_margin.Width, 0, _size.Width - 2 * _margin.Width, promptArea.Top);
			_content.Resize(contentArea);
		}

		/// <summary>
		///   Handles the user input relevant to the console of the given device.
		/// </summary>
		internal void HandleInput()
		{
			// Check wheter the console should be toggle on or off
			if (_input.Toggle.IsTriggered)
				Active = !Active;

			// Don't handle any other input if the console isn't active
			if (!Active)
				return;

			if (_input.Submit.IsTriggered)
			{
				var input = _prompt.Submit();
				if (!String.IsNullOrWhiteSpace(input))
				{
					Log.Info("{0}{1}", ConsolePrompt.Prompt, input);
					_commands.Execute(input);
				}
			}

			if (_input.ClearPrompt.IsTriggered)
				_prompt.Clear();

			if (_input.ShowOlderHistory.IsTriggered)
				_prompt.ShowOlderHistoryEntry();

			if (_input.ShowNewerHistory.IsTriggered)
				_prompt.ShowNewerHistoryEntry();

			if (_input.Clear.IsTriggered)
			{
				_content.Clear();
				_prompt.Clear();
			}

			if (_input.ScrollUp.IsTriggered)
				_content.ScrollUp();

			if (_input.ScrollDown.IsTriggered)
				_content.ScrollDown();

			if (_input.ScrollToTop.IsTriggered)
				_content.ScrollToTop();

			if (_input.ScrollToBottom.IsTriggered)
				_content.ScrollToBottom();

			if (_input.AutoComplete.IsTriggered)
				_prompt.AutoComplete();
		}

		/// <summary>
		///   Shows or hides the console.
		/// </summary>
		/// <param name="show">Indicates whether the console should be shown.</param>
		private void ShowConsole(bool show)
		{
			Active = show;
		}

		/// <summary>
		///   Outputs an error on the console.
		/// </summary>
		/// <param name="message">The error message that should be displayed.</param>
		public void ShowError(string message)
		{
			_content.Add(message, ErrorColor);
		}

		/// <summary>
		///   Outputs a warning on the console.
		/// </summary>
		/// <param name="message">The warning that should be displayed.</param>
		public void ShowWarning(string message)
		{
			_content.Add(message, WarningColor);
		}

		/// <summary>
		///   Outputs an informational message on the console.
		/// </summary>
		/// <param name="message">The message that should be displayed.</param>
		public void ShowInfo(string message)
		{
			_content.Add(message, InfoColor);
		}

		/// <summary>
		///   Outputs a debug message on the console.
		/// </summary>
		/// <param name="message">The warning that should be displayed.</param>
		public void ShowDebugInfo(string message)
		{
			_content.Add(message, DebugInfoColor);
		}
	}
}