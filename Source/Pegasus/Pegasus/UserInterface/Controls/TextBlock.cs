﻿namespace Pegasus.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Rendering;
	using Utilities;

	/// <summary>
	///     Represents a text block that draws text into the UI.
	/// </summary>
	public class TextBlock : UIElement
	{
		/// <summary>
		///     The text content of the text block.
		/// </summary>
		public static readonly DependencyProperty<string> TextProperty =
			new DependencyProperty<string>(defaultValue: String.Empty, affectsMeasure: true, prohibitsAnimations: true,
				validationCallback: ValidateText);

		/// <summary>
		///     Indicates whether the text should be wrapped when it reaches the edge of the text block.
		/// </summary>
		public static readonly DependencyProperty<TextWrapping> TextWrappingProperty =
			new DependencyProperty<TextWrapping>(defaultValue: TextWrapping.NoWrap, affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///     Indicates whether the text should be left-aligned, right-aligned or centered within the text block.
		/// </summary>
		public static readonly DependencyProperty<TextAlignment> TextAlignmentProperty =
			new DependencyProperty<TextAlignment>(defaultValue: TextAlignment.Left, affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///     The foreground color of the text.
		/// </summary>
		public static readonly DependencyProperty<Color> ForegroundProperty = Control.ForegroundProperty;

		/// <summary>
		///     The cached text layout that is used to layout and draw the text of the text block.
		/// </summary>
		private TextLayout _textLayout = new TextLayout();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public TextBlock()
			: this(String.Empty)
		{
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="text">The text content of the text block.</param>
		public TextBlock(string text)
		{
			Assert.ArgumentNotNull(text);
			Text = text;
		}

		/// <summary>
		///     Gets or sets the foreground color of the text.
		/// </summary>
		public Color Foreground
		{
			get { return GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}

		/// <summary>
		///     Gets or sets a value indicating whether the text should be wrapped when it reaches the edge of the text block.
		/// </summary>
		public TextWrapping TextWrapping
		{
			get { return GetValue(TextWrappingProperty); }
			set { SetValue(TextWrappingProperty, value); }
		}

		/// <summary>
		///     Gets or sets a value indicating whether the text should be left-aligned, right-aligned or centered within the text
		///     block.
		/// </summary>
		public TextAlignment TextAlignment
		{
			get { return GetValue(TextAlignmentProperty); }
			set { SetValue(TextAlignmentProperty, value); }
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
		///     Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected internal override Enumerator<UIElement> LogicalChildren
		{
			get { return Enumerator<UIElement>.Empty; }
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
		///     Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="availableSize">
		///     The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///     to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///     element might be able to use scrolling in this case.
		/// </param>
		protected override Size MeasureCore(Size availableSize)
		{
			Assert.NotNull(Font);

			var width = Single.IsInfinity(availableSize.Width) ? Int32.MaxValue : (int)Math.Round(availableSize.Width);
			var height = Single.IsInfinity(availableSize.Height) ? Int32.MaxValue : (int)Math.Round(availableSize.Height);

			var size = _textLayout.Measure(Font, Text, new Size(width, height), 0, TextAlignment, TextWrapping);
			return new Size(size.Width, size.Height);
		}

		/// <summary>
		///     Determines the size of the UI element and positions all of its children. Returns the actual size used by the UI
		///     element. If this value is smaller than the given size, the UI element's alignment properties position it
		///     appropriately.
		/// </summary>
		/// <param name="finalSize">
		///     The final area allocated by the UI element's parent that the UI element should use to arrange
		///     itself and its children.
		/// </param>
		protected override Size ArrangeCore(Size finalSize)
		{
			var width = Single.IsInfinity(finalSize.Width) ? Int32.MaxValue : (int)Math.Round(finalSize.Width);
			var height = Single.IsInfinity(finalSize.Height) ? Int32.MaxValue : (int)Math.Round(finalSize.Height);

			var size = _textLayout.Arrange(Font, Text, new Size(width, height), 0, TextAlignment, TextWrapping);
			return new Size(size.Width, size.Height);
		}

		/// <summary>
		///     Computes the physical position of the caret at the given logical caret position.
		/// </summary>
		/// <param name="position">The logical position of the caret.</param>
		internal Vector2 ComputeCaretPosition(int position)
		{
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);
			return _textLayout.ComputeCaretPosition(position) + new Vector2(x, y);
		}

		/// <summary>
		///     Gets the index of the character closest to the given position.
		/// </summary>
		/// <param name="position">The position the closest character should be returned for.</param>
		internal int GetCharacterIndexAt(Vector2 position)
		{
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);
			return _textLayout.GetCharacterIndexAt(position - new Vector2(x, y));
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
			return _textLayout.HitTest(position) || base.HitTestCore(position);
		}

		/// <summary>
		///     Draws the UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element.</param>
		protected override void DrawCore(SpriteBatch spriteBatch)
		{
			base.DrawCore(spriteBatch);

			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			++spriteBatch.Layer;
			_textLayout.Draw(spriteBatch, new Vector2(x, y), Foreground);
			--spriteBatch.Layer;
		}
	}
}