namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Collections.Generic;
	using Math;

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

				var width = Math.Min(column.EffectiveMaxWidth, availableSize.Width);
				for (var c = columnIndex; c > 0; --c)
					width -= ColumnDefinitions[c - 1].ActualWidth;

				var height = Math.Min(row.EffectiveMaxHeight, availableSize.Height);
				for (var r = rowIndex; r > 0; --r)
					height -= RowDefinitions[r - 1].ActualHeight;

				child.Measure(new SizeD(width, height));
				var desiredSize = child.DesiredSize;

				column.RegisterChildWidth(desiredSize.Width);
				row.RegisterChildHeight(desiredSize.Height);
			}

			// Compute the desired size of the grid and update the row and column offsets
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