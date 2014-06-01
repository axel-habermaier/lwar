namespace Pegasus.Framework.UserInterface.Controls
{
	using System;

	/// <summary>
	///     Represents a column of a data grid that displays textual content in its cells.
	/// </summary>
	public class DataGridTextColumn : DataGridColumn
	{
		/// <summary>
		///     The property name of the data binding that associates the column with a property value of the data source.
		/// </summary>
		public static readonly DependencyProperty<string> PropertyNameProperty = new DependencyProperty<string>(affectsMeasure: true);

		/// <summary>
		///     Gets or sets the property name of the data binding that associates the column with a property value of the data source.
		/// </summary>
		public string PropertyName
		{
			get { return GetValue(PropertyNameProperty); }
			set { SetValue(PropertyNameProperty, value); }
		}

		/// <summary>
		///     Creates a cell UI element for the given item.
		/// </summary>
		/// <param name="item">The item the UI element should be created for.</param>
		internal override UIElement CreateCellElement(object item)
		{
			var child = new TextBlock { DataContext = item };
			child.CreateDataBinding(TextBlock.TextProperty, BindingMode.OneWay, PropertyName);
			return child;
		}
	}
}