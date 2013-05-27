﻿using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using Math;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Memory;
	using Scripting;

	/// <summary>
	///   The input prompt of the console.
	/// </summary>
	internal class ConsolePrompt : DisposableObject
	{
		/// <summary>
		///   The prompt token.
		/// </summary>
		internal const string Prompt = "]";

		/// <summary>
		///   The maximum history size.
		/// </summary>
		private const int MaxHistory = 64;

		/// <summary>
		///   The command registry that is used to look up commands.
		/// </summary>
		private readonly CommandRegistry _commands;

		/// <summary>
		///   The cvar registry that is used to look up cvars.
		/// </summary>
		private readonly CvarRegistry _cvars;

		/// <summary>
		///   The input history.
		/// </summary>
		private readonly string[] _history;

		/// <summary>
		///   The input text box.
		/// </summary>
		private readonly TextBox _input;

		/// <summary>
		///   The prompt label.
		/// </summary>
		private readonly Label _prompt;

		/// <summary>
		///   The area of the prompt.
		/// </summary>
		private Rectangle _area;

		/// <summary>
		///   The currently active auto completion list.
		/// </summary>
		private IEnumerator<string> _autoCompletionList;

		/// <summary>
		///   Stores the current index into the input history.
		/// </summary>
		private int _historyIndex;

		/// <summary>
		///   The number of history slots that are currently used.
		/// </summary>
		private int _numHistory;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="font">The font that should be used to draw the prompt.</param>
		/// <param name="color">The text color of the prompt.</param>
		/// <param name="commands">The command registry that should be used to look up commands.</param>
		/// <param name="cvars">The cvar registry that should be used to look up cvars.</param>
		public ConsolePrompt(Font font, Color color, CommandRegistry commands, CvarRegistry cvars)
		{
			Assert.ArgumentNotNull(font);
			Assert.ArgumentNotNull(commands);
			Assert.ArgumentNotNull(cvars);

			_history = new string[MaxHistory];
			_input = new TextBox(font) { Color = color };
			_prompt = new Label(font, Prompt) { Color = color };
			_commands = commands;
			_cvars = cvars;
		}

		/// <summary>
		///   Gets the actual area of the prompt.
		/// </summary>
		public Rectangle ActualArea
		{
			get { return new Rectangle(_area.Left, _area.Top, _area.Width, _input.ActualArea.Height); }
		}

		/// <summary>
		///   Injects a key press event.
		/// </summary>
		/// <param name="args">The key that was pressed.</param>
		public void InjectKeyPress(KeyEventArgs args)
		{
			_input.InjectKeyPress(args);
		}

		/// <summary>
		///   Draws the prompt.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the prompt.</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			_prompt.Draw(spriteBatch);
			_input.Draw(spriteBatch);
		}

		/// <summary>
		///   Resizes the prompt area.
		/// </summary>
		/// <param name="area">The new prompt area.</param>
		public void Resize(Rectangle area)
		{
			_area = area;

			_input.Area = new Rectangle(area.Left + _prompt.ActualArea.Width, area.Top,
										area.Width - _prompt.ActualArea.Width, 0);
			_prompt.Area = new Rectangle(area.Left, area.Top, Int16.MaxValue, 0);
		}

		/// <summary>
		///   Inserts the given character at the current caret position.
		/// </summary>
		/// <param name="c">The character that should be inserted.</param>
		public void InsertCharacter(char c)
		{
			_input.InsertCharacter(c);
			_autoCompletionList = null;
		}

		/// <summary>
		///   Submits the current user input to the input history, returns the current input, and
		///   resets the input.
		/// </summary>
		public string Submit()
		{
			if (!String.IsNullOrWhiteSpace(_input.Text))
			{
				// Store the input in the input history, if it differs from the last entry
				if (_numHistory == 0 || _history[_numHistory - 1] != _input.Text)
				{
					// If the history is full, remove the oldest entry; this could be implemented more efficiently
					if (_numHistory == MaxHistory)
					{
						Array.Copy(_history, 1, _history, 0, MaxHistory - 1);
						--_numHistory;
					}

					_history[_numHistory++] = _input.Text;
					_historyIndex = _numHistory;
				}
			}

			// Reset and return the input
			var input = _input.Text;
			Clear();
			return input;
		}

		/// <summary>
		///   Clears the user input.
		/// </summary>
		public void Clear()
		{
			_input.Text = String.Empty;
			_historyIndex = _numHistory;
			_autoCompletionList = null;
		}

		/// <summary>
		///   Shows the next newer history entry, if any.
		/// </summary>
		public void ShowNewerHistoryEntry()
		{
			ShowHistory(_historyIndex + 1);
		}

		/// <summary>
		///   Shows the next older history entry, if any.
		/// </summary>
		public void ShowOlderHistoryEntry()
		{
			ShowHistory(_historyIndex - 1);
		}

		/// <summary>
		///   Shows the next auto-completed value if completion is possible.
		/// </summary>
		public void AutoComplete()
		{
			if (_autoCompletionList == null)
				_autoCompletionList = GetAutoCompletionList();

			if (_autoCompletionList.MoveNext())
				_input.Text = _autoCompletionList.Current + " ";
			else
				_autoCompletionList = null;
		}

		/// <summary>
		///   Gets the auto completion list for the current input.
		/// </summary>
		private IEnumerator<string> GetAutoCompletionList()
		{
			if (String.IsNullOrWhiteSpace(_input.Text))
				yield break;

			var commands = _commands.AllInstances
									.Where(command => command.Name.ToLower().StartsWith(_input.Text.ToLower()))
									.Select(command => command.Name);

			var cvars = _cvars.AllInstances
							  .Where(cvar => cvar.Name.ToLower().StartsWith(_input.Text.ToLower()))
							  .Select(cvar => cvar.Name);

			var items = cvars.Union(commands).OrderBy(item => item).ToArray();
			if (items.Length == 0)
				yield break;

			var i = 0;
			while (true)
			{
				yield return items[i];
				i = (i + 1) % items.Length;
			}
		}

		/// <summary>
		///   Shows the history entry at the given index.
		/// </summary>
		/// <param name="index">The index of the entry that should be shown.</param>
		private void ShowHistory(int index)
		{
			if (_numHistory == 0)
				return;

			// Ensure the index is inside the bounds
			if (index < 0)
				index = 0;

			// Empty the input box when going past the newest entry
			if (index >= _numHistory)
			{
				_input.Text = String.Empty;
				index = _numHistory;
			}
			else
				_input.Text = _history[index];

			_historyIndex = index;
			_autoCompletionList = null;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_input.SafeDispose();
		}
	}
}