namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Input;
	using Platform.Graphics;
	using Rendering;

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
			textBlock.CreateTemplateBinding(control, TextBlock.TextProperty, TextBlock.TextProperty);

			return textBlock;
		};

		/// <summary>
		///     The text content of the text block.
		/// </summary>
		public static readonly DependencyProperty<string> TextProperty = TextBlock.TextProperty;

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
			IsFocusedProperty.Changed += OnFocused;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public TextBox()
			: this(String.Empty)
		{
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
			if (textBox == null)
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
			if (textBox == null)
				return;

			switch (e.Key)
			{
				case Key.Right:
					textBox._caret.Move(1);
					break;
				case Key.Left:
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
					textBox._caret.RemoveCurrentCharacter();
					break;
				case Key.Escape:
					textBox.Unfocus();
					break;
				default:
					return;
			}

			e.Handled = true;
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
			}
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

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			base.OnDraw(spriteBatch);

			if (_textBlock == null || !IsFocused)
				return;

			_caret.Draw(spriteBatch, _textBlock.ComputeCaretPosition(_caret.Position), _textBlock.Font.LineHeight, Color.White);
		}
	}
}