namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Collections.Generic;
	using Math;

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

	/// <summary>
	///     Provides a flexible grid layout that consists of rows and columns.
	/// </summary>
	public class Grid : Panel
	{
		/// <summary>
		///     Indicates which column, identified by its zero-based index, a child UI element of the grid should appear in.
		/// </summary>
		public static readonly DependencyProperty<int> ColumnProperty = new DependencyProperty<int>(defaultValue: 0, affectsArrange: true);

		/// <summary>
		///     Indicates which row, identified by its zero-based index, a child UI element of the grid should appear in.
		/// </summary>
		public static readonly DependencyProperty<int> RowProperty = new DependencyProperty<int>(defaultValue: 0, affectsArrange: true);

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Grid()
		{
			ColumnDefinitions = new List<ColumnDefinition>();
			RowDefinitions = new List<RowDefinition>();
		}

		/// <summary>
		///     Gets the columns of this grid.
		/// </summary>
		public List<ColumnDefinition> ColumnDefinitions { get; private set; }

		/// <summary>
		///     Gets the rows of this grid.
		/// </summary>
		public List<RowDefinition> RowDefinitions { get; private set; }

		/// <summary>
		///     Gets a value indicating the column, identified by its zero-based index, the child UI element of the grid appears in.
		/// </summary>
		/// <param name="element">The element the zero-based column index should be returned for.</param>
		public static int GetColumn(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(ColumnProperty);
		}

		/// <summary>
		///     Sets a value indicating the column, identified by its zero-based index, the child UI element of the grid should appear
		///     in.
		/// </summary>
		/// <param name="element">The element the zero-based column index should be set for.</param>
		/// <param name="index">The zero-based column index that should be set for the UI element.</param>
		public static void SetColumn(UIElement element, int index)
		{
			Assert.ArgumentNotNull(element);
			element.SetValue(ColumnProperty, index);
		}

		/// <summary>
		///     Gets a value indicating the row, identified by its zero-based index, the child UI element of the grid appears in.
		/// </summary>
		/// <param name="element">The element the zero-based row index should be returned for.</param>
		public static int GetRow(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(RowProperty);
		}

		/// <summary>
		///     Sets a value indicating the row, identified by its zero-based index, the child UI element of the grid should appear in.
		/// </summary>
		/// <param name="element">The element the zero-based row index should be set for.</param>
		/// <param name="index">The zero-based row index that should be set for the UI element.</param>
		public static void SetRow(UIElement element, int index)
		{
			Assert.ArgumentNotNull(element);
			element.SetValue(RowProperty, index);
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
			// Reset all rows and columns
			foreach (var column in ColumnDefinitions)
				column.ResetActualWidth();

			foreach (var row in RowDefinitions)
				row.ResetActualHeight();

			// Register all children on the rows and columns
			foreach (var child in Children)
			{
				var columnIndex = GetColumn(child);
				var rowIndex = GetRow(child);

				Assert.InRange(columnIndex, ColumnDefinitions);
				Assert.InRange(rowIndex, RowDefinitions);

				var column = ColumnDefinitions[columnIndex];
				var row = RowDefinitions[rowIndex];

				child.Measure(new SizeD(column.EffectiveMaxWidth, row.EffectiveMaxHeight));
				var desiredSize = child.DesiredSize;

				column.RegisterChildWidth(desiredSize.Width);
				row.RegisterChildHeight(desiredSize.Height);
			}

			// Compute the desired size of the grid and the row and column offsets
			var gridSize = new SizeD();
			foreach (var column in ColumnDefinitions)
			{
				column.Offset = gridSize.Width;
				gridSize.Width += column.ActualWidth;
			}

			foreach (var row in RowDefinitions)
			{
				row.Offset = gridSize.Height;
				gridSize.Height += row.ActualHeight;
			}

			return gridSize;
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
			foreach (var child in Children)
			{
				var column = ColumnDefinitions[GetColumn(child)];
				var row = RowDefinitions[GetRow(child)];

				child.Arrange(new RectangleD(column.Offset, row.Offset, column.ActualWidth, row.ActualHeight));
			}

			return finalSize;
		}
	}
}