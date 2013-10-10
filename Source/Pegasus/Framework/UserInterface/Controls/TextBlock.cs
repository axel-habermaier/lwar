using System;

namespace Pegasus.Framework.UserInterface.Controls
{
	using Math;
	using Platform.Graphics;
	using Rendering;
	using Rendering.UserInterface;
	using Math = System.Math;

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
		/// <param name="constraint">
		///   The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///   to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///   element might be able to use scrolling in this case.
		/// </param>
		protected override SizeD MeasureCore(SizeD constraint)
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
			var x = (int)Math.Round(VisualOffset.X);
			var y = (int)Math.Round(VisualOffset.Y);

			using (var text = Rendering.UserInterface.Text.Create(Text))
				TextRenderer.Draw(spriteBatch, Font, text, Color.FromRgba(0, 255, 0, 255), new Vector2i(x, y));
		}
	}
}