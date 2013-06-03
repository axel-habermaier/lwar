using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using Math;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Memory;

	/// <summary>
	///   Control for text input.
	/// </summary>
	public sealed class TextBox : DisposableObject
	{
		/// <summary>
		///   The caret of the text box.
		/// </summary>
		private readonly Caret _caret = new Caret();

		/// <summary>
		///   The layout of the text box's text.
		/// </summary>
		private TextLayout _layout;

		/// <summary>
		///   The renderer for the text box' text.
		/// </summary>
		private TextRenderer _textRenderer;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="font">The font that is used to draw the text.</param>
		public TextBox(Font font)
		{
			Text = String.Empty;
			Color = new Color(255, 255, 255, 255);

			_layout = new TextLayout(font, String.Empty);
			_layout.LayoutChanged += () => _textRenderer.RebuildCache(Font, Text, _layout.LayoutData);

			_caret.TextChanged += text => _layout.Text = text;
		}

		/// <summary>
		///   Gets or sets the label's area.
		/// </summary>
		public Rectangle Area
		{
			get { return _layout.DesiredArea; }
			set
			{
				// Subtract the caret width from the desired area's size to ensure that we always have
				// enough space left to draw the caret
				var width = value.Width - _caret.GetWidth(_layout.Font);
				_layout.DesiredArea = new Rectangle(value.Left, value.Top, width, value.Height);
			}
		}

		/// <summary>
		///   Gets the actual text rendering area. Usually, the actual area is smaller than the desired size.
		///   If any words overlap, however, the actual area is bigger.
		/// </summary>
		public Rectangle ActualArea
		{
			get
			{
				// Ensure that the layout is up to date
				_layout.UpdateLayout();
				return _layout.ActualArea;
			}
		}

		/// <summary>
		///   Gets or sets the text that is shown in the text box.
		/// </summary>
		public string Text
		{
			get { return _layout.Text; }
			set
			{
				_layout.Text = value;
				_caret.Text = value;
			}
		}

		/// <summary>
		///   Gets or sets the font that is used to draw the text.
		/// </summary>
		public Font Font
		{
			get { return _layout.Font; }
			set { _layout.Font = value; }
		}

		/// <summary>
		///   Gets or sets the text color.
		/// </summary>
		public Color Color
		{
			get { return _textRenderer.Color; }
			set { _textRenderer.Color = value; }
		}

		/// <summary>
		///   Inserts the given character at the current caret position.
		/// </summary>
		/// <param name="c">The character that should be inserted.</param>
		public void InsertCharacter(char c)
		{
			_caret.InsertCharacter(c);
		}

		/// <summary>
		///   Injects a key press event.
		/// </summary>
		/// <param name="args">The key that was pressed.</param>
		public void InjectKeyPress(KeyEventArgs args)
		{
			if (args.Key == Key.Right)
				_caret.Move(1);

			if (args.Key == Key.Left)
				_caret.Move(-1);

			if (args.Key == Key.Home)
				_caret.MoveToBeginning();

			if (args.Key == Key.End)
				_caret.MoveToEnd();

			if (args.Key == Key.Back)
				_caret.RemovePreviousCharacter();

			if (args.Key == Key.Delete)
				_caret.RemoveCurrentCharacter();
		}

		/// <summary>
		///   Draws the text box.
		/// </summary>
		public void Draw(SpriteBatch spriteBatch)
		{
			_layout.UpdateLayout();
			_layout.DrawDebugVisualizations(spriteBatch);
			_textRenderer.DrawCached(spriteBatch, _layout.Font.Texture);

			var position = _layout.ComputeCaretPosition(_caret.Position);
			_caret.Draw(spriteBatch, _layout.Font, position);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_caret.SafeDispose();
		}
	}
}