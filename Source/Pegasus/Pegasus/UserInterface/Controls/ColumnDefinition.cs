namespace Pegasus.UserInterface.Controls
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents a column of a Grid layout.
	/// </summary>
	public class ColumnDefinition : DependencyObject
	{
		/// <summary>
		///     The width of the column. Can be NaN to indicate that the column should automatically size itself to the
		///     width of its children.
		/// </summary>
		public static readonly DependencyProperty<float> WidthProperty =
			new DependencyProperty<float>(defaultValue: Single.NaN, affectsMeasure: true);

		/// <summary>
		///     The minimum width of the column.
		/// </summary>
		public static readonly DependencyProperty<float> MinWidthProperty =
			new DependencyProperty<float>(affectsMeasure: true);

		/// <summary>
		///     The maximum width of the column.
		/// </summary>
		public static readonly DependencyProperty<float> MaxWidthProperty =
			new DependencyProperty<float>(defaultValue: Single.PositiveInfinity, affectsMeasure: true);

		/// <summary>
		///     Gets or sets the width of the column. Can be NaN to indicate that the column should automatically size itself to the
		///     width of its children.
		/// </summary>
		public float Width
		{
			get { return GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		/// <summary>
		///     Gets or sets the minimum width of the column.
		/// </summary>
		public float MinWidth
		{
			get { return GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}

		/// <summary>
		///     Gets or sets the maximum width of the column.
		/// </summary>
		public float MaxWidth
		{
			get { return GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); }
		}

		/// <summary>
		///     Gets the actual width of the column.
		/// </summary>
		internal float ActualWidth { get; private set; }

		/// <summary>
		///     Gets the effective maximum width of the column, depending on whether a width has explicitly been set.
		/// </summary>
		internal float EffectiveMaxWidth
		{
			get
			{
				if (Single.IsNaN(Width))
					return MaxWidth;

				return Width;
			}
		}

		/// <summary>
		///     Gets or sets the horizontal offset of the column.
		/// </summary>
		internal float Offset { get; set; }

		/// <summary>
		///     Resets the actual width to the default value as if the column contained no children.
		/// </summary>
		internal void ResetActualWidth()
		{
			Assert.That(MinWidth >= 0, "Column has invalid negative minimum width.");
			Assert.That(MaxWidth >= 0, "Column has invalid negative maximum width.");
			Assert.That(Width >= 0 || Single.IsNaN(Width), "Column has invalid negative width.");

			if (Single.IsNaN(Width))
				ActualWidth = MinWidth;
			else
				ActualWidth = Width;
		}

		/// <summary>
		///     Registers the child width on the column. If possible, the column resizes itself to the accommodate the child.
		/// </summary>
		/// <param name="width">The width of the child UI element that should be taken into account.</param>
		internal void RegisterChildWidth(float width)
		{
			if (!Single.IsNaN(Width))
				return;

			if (width > ActualWidth)
				ActualWidth = Math.Min(width, MaxWidth);
		}
	}
}