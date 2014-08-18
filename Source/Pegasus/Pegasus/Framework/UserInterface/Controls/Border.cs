namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Platform.Graphics;
	using Rendering;

	/// <summary>
	///     Draws a border, a background, or both around another element.
	/// </summary>
	public class Border : Decorator
	{
		/// <summary>
		///     The color of the border.
		/// </summary>
		public static readonly DependencyProperty<Color?> BorderBrushProperty = new DependencyProperty<Color?>();

		/// <summary>
		///     The thickness of the border.
		/// </summary>
		public static readonly DependencyProperty<Thickness> BorderThicknessProperty =
			new DependencyProperty<Thickness>(defaultValue: new Thickness(1), affectsMeasure: true);

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

			spriteBatch.DrawLine(area.TopLeft, area.TopRight, color, thickness.Top);
			spriteBatch.DrawLine(area.TopLeft, area.BottomLeft, color, thickness.Left);
			spriteBatch.DrawLine(area.TopRight, area.BottomRight, color, thickness.Right);
			spriteBatch.DrawLine(area.BottomLeft, area.BottomRight, color, thickness.Bottom);
		}
	}
}