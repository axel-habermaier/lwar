namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     Represents a base class for columns of a data grid.
	/// </summary>
	public abstract class DataGridColumn : DependencyObject
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
		///     Initializes a new instance.
		/// </summary>
		protected DataGridColumn()
		{
			CellElements = new List<UIElement>();
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
		///     Creates a cell UI element for the given item.
		/// </summary>
		/// <param name="item">The item the UI element should be created for.</param>
		internal abstract UIElement CreateCellElement(object item);
	}
}