namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///   Displays text.
	/// </summary>
	[ContentProperty("Text")]
	public class TextBlock : UIElement
	{
		/// <summary>
		///   The text content of the text block.
		/// </summary>
		public static readonly DependencyProperty<string> TextProperty =
			new DependencyProperty<string>(defaultValue: String.Empty, affectsMeasure: true, prohibitsAnimations: true);

		/// <summary>
		///   The foreground color of the text.
		/// </summary>
		public static readonly DependencyProperty<Color> ForegroundProperty = Control.ForegroundProperty;

		/// <summary>
		///   The background color of the text.
		/// </summary>
		public static readonly DependencyProperty<Color> BackgroundProperty = Control.BackgroundProperty;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public TextBlock()
			: this(String.Empty)
		{
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="text">The text content of the text block.</param>
		public TextBlock(string text)
		{
			Assert.ArgumentNotNull(text);
			Text = text;
		}

		/// <summary>
		///   Gets or sets the foreground color of the text.
		/// </summary>
		public Color Foreground
		{
			get { return GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}

		/// <summary>
		///   Gets or sets the background color of the text.
		/// </summary>
		public Color Background
		{
			get { return GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}

		/// <summary>
		///   Gets or sets the text content of the text block.
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
		///   Gets an enumerator that can be used to enumerate all logical children of the UI element.
		/// </summary>
		protected internal override UIElementCollection.Enumerator LogicalChildren
		{
			get { return UIElementCollection.Enumerator.Empty; }
		}

		/// <summary>
		///   Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="availableSize">
		///   The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///   to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///   element might be able to use scrolling in this case.
		/// </param>
		protected override SizeD MeasureCore(SizeD availableSize)
		{
			Assert.NotNull(Font);
			return new SizeD(Font.MeasureWidth(Text), Font.LineHeight);
		}

		/// <summary>
		///   Determines the size of the UI element and positions all of its children. Returns the actual size used by the UI
		///   element. If this value is smaller than the given size, the UI element's alignment properties position it
		///   appropriately.
		/// </summary>
		/// <param name="finalSize">
		///   The final area allocated by the UI element's parent that the UI element should use to arrange
		///   itself and its children.
		/// </param>
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			Assert.NotNull(Font);
			return new SizeD(Font.MeasureWidth(Text), Font.LineHeight);
		}

		protected override void OnDraw(SpriteBatch spriteBatch)
		{
			var width = (int)Math.Round(ActualWidth);
			var height = (int)Math.Round(ActualHeight);
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			spriteBatch.Draw(new Rectangle(x, y, width, height), Texture2D.White, Background);
			using (var text = Rendering.UserInterface.Text.Create(Text))
				TextRenderer.Draw(spriteBatch, Font, text, Foreground, new Vector2i(x, y));
		}
	}
}