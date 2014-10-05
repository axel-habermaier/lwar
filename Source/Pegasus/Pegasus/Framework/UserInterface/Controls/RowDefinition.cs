namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	/// <summary>
	///     Represents a row of a Grid layout.
	/// </summary>
	public class RowDefinition : DependencyObject
	{
		/// <summary>
		///     The height of the row. Can be NaN to indicate that the row should automatically size itself to the
		///     height of its children.
		/// </summary>
		public static readonly DependencyProperty<float> HeightProperty =
			new DependencyProperty<float>(defaultValue: Single.NaN, affectsMeasure: true);

		/// <summary>
		///     The minimum height of the row.
		/// </summary>
		public static readonly DependencyProperty<float> MinHeightProperty =
			new DependencyProperty<float>(affectsMeasure: true);

		/// <summary>
		///     The maximum height of the row.
		/// </summary>
		public static readonly DependencyProperty<float> MaxHeightProperty =
			new DependencyProperty<float>(defaultValue: Single.PositiveInfinity, affectsMeasure: true);

		/// <summary>
		///     Gets or sets the vertical offset of the row.
		/// </summary>
		internal float Offset { get; set; }

		/// <summary>
		///     Gets the effective maximum height of the row, depending on whether a height has explicitly been set.
		/// </summary>
		internal float EffectiveMaxHeight
		{
			get
			{
				if (Single.IsNaN(Height))
					return MaxHeight;

				return Height;
			}
		}

		/// <summary>
		///     Gets or sets the height of the row. Can be NaN to indicate that the row should automatically size itself to the
		///     height of its children.
		/// </summary>
		public float Height
		{
			get { return GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}

		/// <summary>
		///     Gets or sets the minimum height of the row.
		/// </summary>
		public float MinHeight
		{
			get { return GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}

		/// <summary>
		///     Gets or sets the maximum height of the row.
		/// </summary>
		public float MaxHeight
		{
			get { return GetValue(MaxHeightProperty); }
			set { SetValue(MaxHeightProperty, value); }
		}

		/// <summary>
		///     Gets the actual height of the row.
		/// </summary>
		internal float ActualHeight { get; private set; }

		/// <summary>
		///     Resets the actual height to the default value as if the row contained no children.
		/// </summary>
		internal void ResetActualHeight()
		{
			Assert.That(MinHeight >= 0, "Row has invalid negative minimum height.");
			Assert.That(MaxHeight >= 0, "Row has invalid negative maximum height.");
			Assert.That(Height >= 0 || Single.IsNaN(Height), "Row has invalid negative height.");

			if (Single.IsNaN(Height))
				ActualHeight = MinHeight;
			else
				ActualHeight = Height;
		}

		/// <summary>
		///     Registers the child height on the row. If possible, the row resizes itself to the accommodate the child.
		/// </summary>
		/// <param name="height">The height of the child UI element that should be taken into account.</param>
		internal void RegisterChildHeight(float height)
		{
			if (!Single.IsNaN(Height))
				return;

			if (height > ActualHeight)
				ActualHeight = Math.Min(height, MaxHeight);
		}
	}
}