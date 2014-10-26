namespace Pegasus.UserInterface.Controls
{
	using System;
	using Input;
	using Math;
	using Platform;
	using Platform.Graphics;
	using Rendering;
	using Utilities;

	/// <summary>
	///     Represents a control that can be used to edit text.
	/// </summary>
	public class TextBox : Control, ITextInputControl
	{
		/// <summary>
		///     The default template that defines the visual appearance of an items control.
		/// </summary>
		private static readonly ControlTemplate DefaultTemplate = control =>
		{
			var textBlock = new TextBlock();
			textBlock.CreateTemplateBinding(control, TextProperty, TextBlock.TextProperty);

			var border = new Border { Child = textBlock };
			border.CreateTemplateBinding(control, BorderBrushProperty, Border.BorderBrushProperty);
			border.CreateTemplateBinding(control, BorderThicknessProperty, Border.BorderThicknessProperty);
			border.CreateTemplateBinding(control, PaddingProperty, Border.PaddingProperty);
			return border;
		};

		/// <summary>
		///     The text content of the text block.
		/// </summary>
		public static readonly DependencyProperty<string> TextProperty =
			new DependencyProperty<string>(defaultValue: String.Empty, affectsMeasure: true, prohibitsAnimations: true,
				defaultBindingMode: BindingMode.TwoWay, validationCallback: ValidateText);

		/// <summary>
		///     The maximum number of characters that can be manually entered into the text box.
		/// </summary>
		public static readonly DependencyProperty<int> MaxLengthProperty = new DependencyProperty<int>(validationCallback: ValidateMaxLength);

		/// <summary>
		///     Raised when the text contained in the text box has been changed.
		/// </summary>
		public static readonly RoutedEvent<TextChangedEventArgs> TextChangedEvent =
			new RoutedEvent<TextChangedEventArgs>(RoutingStrategy.Bubble);

		/// <summary>
		///     The caret that is used to insert and delete text.
		/// </summary>
		private Caret _caret;

		/// <summary>
		///     The text block that is used to display the text.
		/// </summary>
		private TextBlock _textBlock;

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static TextBox()
		{
			KeyDownEvent.Raised += OnKeyDown;
			TextInputEvent.Raised += OnTextInput;
			MouseDownEvent.Raised += OnMouseDown;
			IsFocusedProperty.Changed += OnFocused;
			TextProperty.Changed += OnTextChanged;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public TextBox()
			: this(String.Empty)
		{
			SetValue(Cursor.CursorProperty, Cursors.Text);
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="text">The text content of the text box.</param>
		public TextBox(string text)
		{
			Assert.ArgumentNotNull(text);

			SetStyleValue(TemplateProperty, DefaultTemplate);
			Text = text;
			Focusable = true;
			_caret = new Caret(this);
		}

		/// <summary>
		///     Gets or sets the maximum number of characters that can be manually entered into the text box.
		/// </summary>
		public int MaxLength
		{
			get { return GetValue(MaxLengthProperty); }
			set { SetValue(MaxLengthProperty, value); }
		}

		/// <summary>
		///     Gets or sets the text content of the text block.
		/// </summary>
		public string Text
		{
			get { return GetValue(TextProperty); }
			set
			{
				Assert.ArgumentNotNull(value);
				SetValue(TextProperty, value);
			}
		}

		/// <summary>
		///     Raised when the text contained in the text box has been changed.
		/// </summary>
		public event RoutedEventHandler<TextChangedEventArgs> TextChanged
		{
			add { AddHandler(TextChangedEvent, value); }
			remove { RemoveHandler(TextChangedEvent, value); }
		}

		/// <summary>
		///     Validates a value of the text dependency property.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private static bool ValidateText(string value)
		{
			return value != null;
		}

		/// <summary>
		///     Validates a value for the max length property.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		private static bool ValidateMaxLength(int value)
		{
			return value >= 0;
		}

		/// <summary>
		///     Raises the text changed event.
		/// </summary>
		private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<string> args)
		{
			var textBox = obj as TextBox;
			if (textBox == null)
				return;

			textBox.RaiseEvent(TextChangedEvent, TextChangedEventArgs.Create(args.NewValue));
			textBox._caret.MoveToEndIfTextChanged();
		}

		/// <summary>
		///     Sets the caret to the character closest to the mouse.
		/// </summary>
		private static void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox == null || textBox._textBlock == null || e.Button != MouseButton.Left)
				return;

			textBox._caret.Position = textBox._textBlock.GetCharacterIndexAt(e.Position);
			e.Handled = true;
		}

		/// <summary>
		///     Ensures that the caret is visible, even though it might in the hidden phase of its blinking interval at the moment.
		/// </summary>
		private static void OnFocused(DependencyObject obj, DependencyPropertyChangedEventArgs<bool> args)
		{
			var textBox = obj as TextBox;
			if (textBox == null)
				return;

			textBox._caret.Show();
		}

		/// <summary>
		///     Inserts the given text.
		/// </summary>
		private static void OnTextInput(object sender, TextInputEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox == null || textBox._textBlock == null)
				return;

			// Check if we've exceeded the maximum length
			if (textBox.MaxLength > 0 && textBox.Text.Length >= textBox.MaxLength)
				return;

			textBox._caret.InsertCharacter(e.Character);
			e.Handled = true;
		}

		/// <summary>
		///     Handles a key down event, modifying the position of the cursor.
		/// </summary>
		private static void OnKeyDown(object sender, KeyEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox == null || textBox._textBlock == null)
				return;

			switch (e.Key)
			{
				case Key.Right:
					if ((e.Modifiers & KeyModifiers.Control) == KeyModifiers.Control)
						textBox._caret.Position = textBox.GetBeginningOfNextWord();
					else
						textBox._caret.Move(1);
					break;
				case Key.Left:
					if ((e.Modifiers & KeyModifiers.Control) == KeyModifiers.Control)
						textBox._caret.Position = textBox.GetBeginningOfPreviousWord();
					else
						textBox._caret.Move(-1);
					break;
				case Key.Home:
					textBox._caret.MoveToBeginning();
					break;
				case Key.End:
					textBox._caret.MoveToEnd();
					break;
				case Key.Back:
					textBox._caret.RemovePreviousCharacter();
					break;
				case Key.Delete:
				case Key.NumpadDecimal:
					textBox._caret.RemoveCurrentCharacter();
					break;
				default:
					return;
			}

			e.Handled = true;
		}

		/// <summary>
		///     Gets the index of the beginning of the next word.
		/// </summary>
		private int GetBeginningOfNextWord()
		{
			using (var text = new TextString(Text))
			{
				var encounteredWhitespace = false;
				for (var i = _caret.Position; i < text.Length; ++i)
				{
					if (Char.IsWhiteSpace(text[i]))
						encounteredWhitespace = true;
					else if (encounteredWhitespace)
						return i;
				}

				return text.Length;
			}
		}

		/// <summary>
		///     Gets the index of the beginning of the previous word.
		/// </summary>
		private int GetBeginningOfPreviousWord()
		{
			using (var text = new TextString(Text))
			{
				var encounteredNonWhitespace = false;
				for (var i = _caret.Position; i > 0; --i)
				{
					if (!Char.IsWhiteSpace(text[i - 1]))
						encounteredNonWhitespace = true;
					else if (encounteredNonWhitespace)
						return i;
				}

				return 0;
			}
		}

		/// <summary>
		///     Invoked when the template has been changed.
		/// </summary>
		/// <param name="templateRoot">The new root element of the template.</param>
		protected override void OnTemplateChanged(UIElement templateRoot)
		{
			if (templateRoot == null)
				_textBlock = null;
			else
			{
				_textBlock = FindTextBlock(templateRoot);
				Assert.NotNull(_textBlock, "The text box has control template that does not contain a text block.");

				_textBlock.TextAlignment = TextAlignment.Left;
				_textBlock.TextWrapping = TextWrapping.Wrap;
			}
		}

		/// <summary>
		///     Performs a detailed hit test for the given position. The position is guaranteed to lie within the UI element's
		///     bounds. This method should be overridden to implement special hit testing logic that is more precise than a
		///     simple bounding box check.
		/// </summary>
		/// <param name="position">The position that should be checked for a hit.</param>
		/// <returns>Returns true if the UI element is hit; false, otherwise.</returns>
		protected override bool HitTestCore(Vector2 position)
		{
			return true;
		}

		/// <summary>
		///     Recursively searches through the logical tree with the given element at the root until it finds a text block. This
		///     method returns the first text block that is found.
		/// </summary>
		/// <param name="element">The root UI element that should be searched.</param>
		private static TextBlock FindTextBlock(UIElement element)
		{
			var textBlock = element as TextBlock;
			if (textBlock != null)
				return textBlock;

			foreach (var child in element.LogicalChildren)
			{
				textBlock = FindTextBlock(child);
				if (textBlock != null)
					return textBlock;
			}

			return null;
		}

		/// <summary>
		///     Draws the UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element.</param>
		protected override void DrawCore(SpriteBatch spriteBatch)
		{
			base.DrawCore(spriteBatch);

			if (_textBlock != null && IsFocused)
				_caret.Draw(spriteBatch, _textBlock.ComputeCaretPosition(_caret.Position), _textBlock.Font.LineHeight, Foreground);
		}
	}
}