namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	/// <summary>
	///     Represents a row of a Grid layout.
	/// </summary>
	public class RowDefinition
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public RowDefinition()
		{
			Height = Double.NaN;
			MaxHeight = Double.PositiveInfinity;
		}

		/// <summary>
		///     Gets or sets the vertical offset of the row.
		/// </summary>
		public double Offset { get; set; }

		/// <summary>
		///     Gets the effective maximum height of the row, depending on whether a height has explicitly been set.
		/// </summary>
		internal double EffectiveMaxHeight
		{
			get
			{
				if (Double.IsNaN(Height))
					return MaxHeight;

				return Height;
			}
		}

		/// <summary>
		///     Gets or sets the minimum allowed height of the row.
		/// </summary>
		public double MinHeight { get; set; }

		/// <summary>
		///     Gets or sets the maximum allowed height of the row.
		/// </summary>
		public double MaxHeight { get; set; }

		/// <summary>
		///     Gets or sets the height of the row. Can be NaN to indicate that the row should automatically size itself to the height
		///     of its children.
		/// </summary>
		public double Height { get; set; }

		/// <summary>
		///     Gets the actual height of the row.
		/// </summary>
		internal double ActualHeight { get; private set; }

		/// <summary>
		///     Resets the actual height to the default value as if the row contained no children.
		/// </summary>
		internal void ResetActualHeight()
		{
			Assert.That(MinHeight >= 0, "Row has invalid negative minimum height.");
			Assert.That(MaxHeight >= 0, "Row has invalid negative maximum height.");
			Assert.That(Height >= 0 || Double.IsNaN(Height), "Row has invalid negative height.");

			if (Double.IsNaN(Height))
				ActualHeight = MinHeight;
			else
				ActualHeight = Height;
		}

		/// <summary>
		///     Registers the child height on the row. If possible, the row resizes itself to the accommodate the child.
		/// </summary>
		/// <param name="height">The height of the child UI element that should be taken into account.</param>
		internal void RegisterChildHeight(double height)
		{
			if (!Double.IsNaN(Height))
				return;

			if (height > ActualHeight)
				ActualHeight = Math.Min(height, MaxHeight);
		}
	}
}