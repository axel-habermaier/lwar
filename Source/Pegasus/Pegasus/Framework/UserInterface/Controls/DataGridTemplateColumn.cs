namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     Represents a column of a data grid.
	/// </summary>
	public class DataGridTemplateColumn : DependencyObject
	{
		/// <summary>
		///     The width of the column.
		/// </summary>
		public static readonly DependencyProperty<double> WidthProperty =
			new DependencyProperty<double>(defaultValue: Double.NaN, affectsMeasure: true);

		/// <summary>
		///     The minimum width of the column.
		/// </summary>
		public static readonly DependencyProperty<double> MinWidthProperty =
			new DependencyProperty<double>(affectsMeasure: true);

		/// <summary>
		///     The maximum width of the column.
		/// </summary>
		public static readonly DependencyProperty<double> MaxWidthProperty =
			new DependencyProperty<double>(defaultValue: Double.MaxValue, affectsMeasure: true);

		/// <summary>
		///     The content of the column header.
		/// </summary>
		public static readonly DependencyProperty<object> HeaderProperty = new DependencyProperty<object>(affectsMeasure: true);

		/// <summary>
		///     The template that defines the visualization of the column cells.
		/// </summary>
		public static readonly DependencyProperty<DataTemplate> CellTemplateProperty =
			new DependencyProperty<DataTemplate>(affectsMeasure: true);

		/// <summary>
		///     The header of the column.
		/// </summary>
		private DataGridColumnHeader _columnHeader;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public DataGridTemplateColumn()
		{
			CellElements = new List<UIElement>();
		}

		/// <summary>
		///     Gets or sets the template that defines the visualization of the column cells.
		/// </summary>
		public DataTemplate CellTemplate
		{
			get { return GetValue(CellTemplateProperty); }
			set { SetValue(CellTemplateProperty, value); }
		}

		/// <summary>
		///     Gets the cell elements of the column.
		/// </summary>
		internal List<UIElement> CellElements { get; private set; }

		/// <summary>
		///     Gets or sets the width of the column.
		/// </summary>
		public double Width
		{
			get { return GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}

		/// <summary>
		///     Gets or sets the minimum width of the column.
		/// </summary>
		public double MinWidth
		{
			get { return GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}

		/// <summary>
		///     Gets or sets the maximum width of the column.
		/// </summary>
		public double MaxWidth
		{
			get { return GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); }
		}

		/// <summary>
		///     Gets or sets the content of the column header.
		/// </summary>
		public object Header
		{
			get { return GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		/// <summary>
		///     Creates the column header at the given column index.
		/// </summary>
		/// <param name="style">The style that should be applied to the header.</param>
		/// <param name="columnIndex">The zero-based index of the column.</param>
		internal UIElement CreateColumnHeader(Style style, int columnIndex)
		{
			var headerElement = Header as UIElement;
			if (headerElement != null)
			{
				headerElement.ChangeLogicalParent(null);
				headerElement.VisualParent = null;
			}

			_columnHeader = _columnHeader ?? new DataGridColumnHeader();
			_columnHeader.Style = style;
			_columnHeader.Content = Header;

			Grid.SetColumn(_columnHeader, columnIndex);
			return _columnHeader;
		}

		/// <summary>
		///     Creates a cell UI element for the given item.
		/// </summary>
		/// <param name="item">The item the UI element should be created for.</param>
		internal UIElement CreateCellElement(object item)
		{
			Assert.NotNull(CellTemplate);

			var child = CellTemplate();
			child.DataContext = item;
			return child;
		}
	}
}