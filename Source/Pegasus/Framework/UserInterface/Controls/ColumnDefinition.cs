namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	/// <summary>
	///     Represents a column of a Grid layout.
	/// </summary>
	public class ColumnDefinition
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ColumnDefinition()
		{
			Width = Double.NaN;
			MaxWidth = Double.PositiveInfinity;
		}

		/// <summary>
		///     Gets or sets the minimum allowed width of the column.
		/// </summary>
		public double MinWidth { get; set; }

		/// <summary>
		///     Gets or sets the maximum allowed width of the column.
		/// </summary>
		public double MaxWidth { get; set; }

		/// <summary>
		///     Gets or sets the width of the column. Can be NaN to indicate that the column should automatically size itself to the
		///     width of its children.
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		///     Gets the actual width of the column.
		/// </summary>
		internal double ActualWidth { get; private set; }

		/// <summary>
		///     Gets the effective maximum width of the column, depending on whether a width has explicitly been set.
		/// </summary>
		internal double EffectiveMaxWidth
		{
			get
			{
				if (Double.IsNaN(Width))
					return MaxWidth;

				return Width;
			}
		}

		/// <summary>
		///     Gets or sets the horizontal offset of the column.
		/// </summary>
		public double Offset { get; set; }

		/// <summary>
		///     Resets the actual width to the default value as if the column contained no children.
		/// </summary>
		internal void ResetActualWidth()
		{
			Assert.That(MinWidth >= 0, "Column has invalid negative minimum width.");
			Assert.That(MaxWidth >= 0, "Column has invalid negative maximum width.");
			Assert.That(Width >= 0 || Double.IsNaN(Width), "Column has invalid negative width.");

			if (Double.IsNaN(Width))
				ActualWidth = MinWidth;
			else
				ActualWidth = Width;
		}

		/// <summary>
		///     Registers the child width on the column. If possible, the column resizes itself to the accommodate the child.
		/// </summary>
		/// <param name="width">The width of the child UI element that should be taken into account.</param>
		internal void RegisterChildWidth(double width)
		{
			if (!Double.IsNaN(Width))
				return;

			if (width > ActualWidth)
				ActualWidth = Math.Min(width, MaxWidth);
		}
	}
}