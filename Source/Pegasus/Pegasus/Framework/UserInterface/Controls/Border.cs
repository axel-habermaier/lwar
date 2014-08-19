namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///     Draws a border, a background, or both around another element.
	/// </summary>
	public class Border : Decorator
	{
		/// <summary>
		///     The color of a border.
		/// </summary>
		public static readonly DependencyProperty<Color?> BorderBrushProperty = new DependencyProperty<Color?>();

		/// <summary>
		///     The thickness of a border.
		/// </summary>
		public static readonly DependencyProperty<Thickness> BorderThicknessProperty =
			new DependencyProperty<Thickness>(defaultValue: new Thickness(1), affectsMeasure: true);

		/// <summary>
		///     The padding inside a border.
		/// </summary>
		public static readonly DependencyProperty<Thickness> PaddingProperty = new DependencyProperty<Thickness>(affectsMeasure: true);

		/// <summary>
		///     Gets or sets the padding inside the border.
		/// </summary>
		public Thickness Padding
		{
			get { return GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}

		/// <summary>
		///     Gets or sets the color of the border.
		/// </summary>
		public Color? BorderBrush
		{
			get { return GetValue(BorderBrushProperty); }
			set { SetValue(BorderBrushProperty, value); }
		}

		/// <summary>
		///     Gets or sets the thickness of the border.
		/// </summary>
		public Thickness BorderThickness
		{
			get { return GetValue(BorderThicknessProperty); }
			set { SetValue(BorderThicknessProperty, value); }
		}

		/// <summary>
		///     Computes and returns the desired size of the element given the available space allocated by the parent UI element.
		/// </summary>
		/// <param name="availableSize">
		///     The available space that the parent UI element can allocate to this UI element. Can be infinity if the parent wants
		///     to size itself to its contents. The computed desired size is allowed to exceed the available space; the parent UI
		///     element might be able to use scrolling in this case.
		/// </param>
		protected override SizeD MeasureCore(SizeD availableSize)
		{
			var padding = Padding;
			availableSize = new SizeD(availableSize.Width - padding.Width, availableSize.Height - padding.Height);
			availableSize = base.MeasureCore(availableSize);

			return new SizeD(availableSize.Width + padding.Width, availableSize.Height + padding.Height);
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
		protected override SizeD ArrangeCore(SizeD finalSize)
		{
			var padding = Padding;
			finalSize = new SizeD(finalSize.Width - padding.Width, finalSize.Height - padding.Height);
			finalSize = base.ArrangeCore(finalSize);

			return new SizeD(finalSize.Width + padding.Width, finalSize.Height + padding.Height);
		}

		/// <summary>
		///     Gets the additional offset that should be applied to the visual offset of the UI element's children.
		/// </summary>
		protected override Vector2d GetAdditionalChildrenOffset()
		{
			var padding = Padding;
			return new Vector2d(padding.Left, padding.Top);
		}

		/// <summary>
		///     Draws the UI element using the given sprite batch.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the UI element.</param>
		protected override void DrawCore(SpriteBatch spriteBatch)
		{
			base.DrawCore(spriteBatch);

			var borderColor = BorderBrush;

			if (borderColor == null)
				return;

			var color = borderColor.Value;
			var area = VisualArea;
			var thickness = BorderThickness;

			// Make sure there is no overdraw at the corners
			spriteBatch.DrawLine(area.TopLeft, area.TopRight, color, thickness.Top);
			spriteBatch.DrawLine(area.BottomLeft + new Vector2d(1, -1), area.TopLeft + new Vector2d(1, 1), color, thickness.Left);
			spriteBatch.DrawLine(area.TopRight + new Vector2d(0, 1), area.BottomRight - new Vector2d(0, 1), color, thickness.Right);
			spriteBatch.DrawLine(area.BottomLeft - new Vector2d(0, 1), area.BottomRight - new Vector2d(0, 1), color, thickness.Bottom);
		}
	}
}