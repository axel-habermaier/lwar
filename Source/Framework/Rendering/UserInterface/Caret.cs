using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using System.Globalization;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Math = System.Math;

	/// <summary>
	///   An indicator that shows the current insertion and deletion position within an editable text.
	/// </summary>
	internal struct Caret
	{
		/// <summary>
		///   The frequency of the caret (in times per second).
		/// </summary>
		private const int Frequency = 2;

		/// <summary>
		///   The string that is used to visualize the caret.
		/// </summary>
		private const string CaretVisual = "_";

		/// <summary>
		///   Raised when an editing operation changed the text.
		/// </summary>
		public Action<string> TextChanged;

		/// <summary>
		///   The logical position of the caret, corresponding to an index of a character of the editable text.
		/// </summary>
		private int _position;

		/// <summary>
		///   The text that can be edited with the caret.
		/// </summary>
		private string _text;

		/// <summary>
		///   The time used to determine whether the caret should be visible.
		/// </summary>
		private Time _time;

		/// <summary>
		///   Gets or sets the text that can be edited with the caret. If the text is changed, the caret
		///   is automatically moved to the end of the text.
		/// </summary>
		public string Text
		{
			get { return _text; }
			set
			{
				Assert.ArgumentNotNull(value, () => value);

				if (_text != value)
					_position = value.Length;

				ChangeText(value);
			}
		}

		/// <summary>
		///   Gets or sets the logical position of the cursor. The position is always clamped into the valid range.
		/// </summary>
		public int Position
		{
			get { return _position; }
			private set
			{
				_position = Math.Min(_text.Length, value);
				_position = Math.Max(0, _position);
				_time.Offset -= _time.Seconds;
			}
		}

		/// <summary>
		///   Changes the text, raising the text changed event if necessary.
		/// </summary>
		/// <param name="text">The new text.</param>
		private void ChangeText(string text)
		{
			Assert.ArgumentNotNull(text, () => text);

			if (_text == text)
				return;

			_text = text;

			if (TextChanged != null)
				TextChanged(_text);
		}

		/// <summary>
		///   Moves the caret by the given delta.
		/// </summary>
		/// <param name="delta">The delta by which the caret should be moved.</param>
		public void Move(int delta)
		{
			Position += delta;
		}

		/// <summary>
		///   Moves the caret to the beginning of the text.
		/// </summary>
		public void MoveToBeginning()
		{
			Position = 0;
		}

		/// <summary>
		///   Moves the caret to the end of the text.
		/// </summary>
		public void MoveToEnd()
		{
			Position = _text.Length;
		}

		/// <summary>
		///   Inserts the given character at the current caret position.
		/// </summary>
		/// <param name="c">The character that should be inserted.</param>
		public void InsertCharacter(char c)
		{
			if (_position >= Text.Length)
				ChangeText(_text + c);
			else
				ChangeText(_text.Insert(_position, c.ToString(CultureInfo.InvariantCulture)));

			Move(1);
		}

		/// <summary>
		///   Removes the character at the current caret position (similar to pressing the
		///   Delete key in a Windows text box).
		/// </summary>
		public void RemoveCurrentCharacter()
		{
			if (_position >= Text.Length)
				return;

			if (_position == 0)
				ChangeText(_text.Substring(1));
			else
				ChangeText(_text.Remove(_position, 1));

			// The caret position doesn't change, but we have to ensure that it does not get out of bounds
			Move(0);
		}

		/// <summary>
		///   Removes the character that is immediately before the current caret position (similar to
		///   pressing the Backspace key in a Windows text box).
		/// </summary>
		public void RemovePreviousCharacter()
		{
			if (_position <= 0)
				return;

			if (_position == Text.Length)
				ChangeText(_text.Substring(0, _text.Length - 1));
			else
				ChangeText(_text.Remove(_position - 1, 1));

			Move(-1);
		}

		/// <summary>
		///   Draws the caret.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used for drawing the caret.</param>
		/// <param name="font">The font that should be used to draw the caret.</param>
		/// <param name="position">The position of the caret's top left corner.</param>
		public void Draw(SpriteBatch spriteBatch, Font font, Vector2i position)
		{
			// Show and hide the caret depending on the frequency and offset
			if (((int)Math.Round(_time.Seconds * Frequency)) % 2 != 0)
				return;

			TextRenderer.Draw(spriteBatch, font, CaretVisual, Color.White, position);
			TextRenderer.Draw(spriteBatch, font, CaretVisual, Color.White, position + new Vector2i(0, 1));
		}

		/// <summary>
		///   Gets the width of the visualization of the caret.
		/// </summary>
		/// <param name="font">The font that is used to draw the caret.</param>
		public int GetWidth(Font font)
		{
			return font.MeasureWidth(CaretVisual);
		}
	}
}