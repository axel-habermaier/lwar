namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Collections;
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

		/// <summary>
		///     Notifies all inheriting objects about a change of an inheriting dependency property.
		/// </summary>
		/// <param name="property">The inheriting dependency property that has been changed.</param>
		/// <param name="newValue">The new value that should be inherited.</param>
		protected override void InheritedValueChanged<T>(DependencyProperty<T> property, T newValue)
		{
			// TODO
		}
	}

	/// <summary>
	///     Represents a control that displays data in a grid layout.
	/// </summary>
	public class DataGrid : ItemsControl
	{
		/// <summary>
		///     The default template that defines the visual appearance of a data grid.
		/// </summary>
		private static readonly ControlTemplate DefaultTemplate = control => new Grid { IsItemsHost = true };

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public DataGrid()
		{
			SetStyleValue(TemplateProperty, DefaultTemplate);
			Columns = new ObservableCollection<DataGridColumn>();
		}

		/// <summary>
		///     Gets a collection that contains all the columns of the data grid.
		/// </summary>
		public ObservableCollection<DataGridColumn> Columns { get; private set; }

		/// <summary>
		///     Gets the grid that is used to layout the items of the data grid.
		/// </summary>
		private Grid Grid
		{
			get
			{
				Assert.OfType<Grid>(ItemsHost, "Data grid must have an items host of type '{0}'.", typeof(Grid).FullName);
				return ItemsHost as Grid;
			}
		}

		/// <summary>
		///     Invoked when the UI element is now (transitively) attached to the root of a visual tree.
		/// </summary>
		protected override void OnAttachedToRoot()
		{
			base.OnAttachedToRoot();

			Columns.CollectionChanged += OnColumnsChanged;
			RegenerateColumns();
		}

		/// <summary>
		///     Invoked when the UI element is no longer (transitively) attached to the root of a visual tree.
		/// </summary>
		protected override void OnDetachedFromRoot()
		{
			base.OnDetachedFromRoot();
			Columns.CollectionChanged -= OnColumnsChanged;
		}

		/// <summary>
		///     Applies the changes in the column layout to the data grid.
		/// </summary>
		private void OnColumnsChanged(IEnumerable collection, CollectionChangedEventArgs args)
		{
			RegenerateColumns();
		}

		/// <summary>
		///     Regenerates the columns.
		/// </summary>
		private void RegenerateColumns()
		{
			Grid.ColumnDefinitions.Clear();
			foreach (var column in Columns)
			{
				var gridColumn = new ColumnDefinition();

				BindColumnProperty(gridColumn, ColumnDefinition.WidthProperty, column, "Width");
				BindColumnProperty(gridColumn, ColumnDefinition.MaxWidthProperty, column, "MaxWidth");
				BindColumnProperty(gridColumn, ColumnDefinition.MinWidthProperty, column, "MinWidth");

				Grid.ColumnDefinitions.Add(gridColumn);
			}

			RegenerateItems();
		}

		/// <summary>
		///     Establishes a data binding between a property of a grid layout column and a data grid column.
		/// </summary>
		/// <typeparam name="T">The type of the data bound property.</typeparam>
		/// <param name="gridColumn">The layout grid column that should be the target of the data binding.</param>
		/// <param name="dependencyProperty">The target dependency property that should be bound.</param>
		/// <param name="column">The data grid column that should be the source of the data binding.</param>
		/// <param name="propertyName">The name of the property of the data grid column that should be bound.</param>
		private static void BindColumnProperty<T>(ColumnDefinition gridColumn, DependencyProperty<T> dependencyProperty,
												  DataGridColumn column, string propertyName)
		{
			var binding = new DataBinding<T>(column, BindingMode.OneWay, propertyName);
			gridColumn.SetBinding(dependencyProperty, binding);
			binding.Active = true;
		}

		/// <summary>
		///     Generates the UI elements for the column headers.
		/// </summary>
		private void GenerateHeaderElements()
		{
			if (Grid.RowDefinitions.Count == 0)
				Grid.RowDefinitions.Add(new RowDefinition { Height = Double.NaN });

			var index = 0;
			foreach (var column in Columns)
			{
				if (column.Header == null)
					continue;

				var presenter = new ContentPresenter { Content = column.Header };
				Grid.SetColumn(presenter, index++);

				ItemsHost.Add(presenter);
			}
		}

		/// <summary>
		///     Adds the given item to the items control at the given index.
		/// </summary>
		/// <param name="item">The item that should be added.</param>
		/// <param name="index">The zero-based index of the new item.</param>
		protected override void AddItem(object item, int index)
		{
			while (Grid.RowDefinitions.Count <= index + 1)
				Grid.RowDefinitions.Add(new RowDefinition { Height = Double.NaN });

			var columnIndex = 0;
			foreach (var column in Columns)
			{
				var element = column.CreateCellElement(item);
				Grid.SetColumn(element, columnIndex++);
				Grid.SetRow(element, index + 1);

				column.CellElements.Add(element);
				ItemsHost.Add(element);
			}
		}

		/// <summary>
		///     Removes the item at the given index from the items control.
		/// </summary>
		/// <param name="index">The zero-based index of the item that should be removed.</param>
		protected override void RemoveItem(int index)
		{
			foreach (var column in Columns)
			{
				var element = column.CellElements[index];

				column.CellElements.Remove(element);
				ItemsHost.Remove(element);
			}
		}

		/// <summary>
		///     Replaces the item at the given index with the given new one.
		/// </summary>
		/// <param name="item">The new item that should replace the previous one.</param>
		/// <param name="index">The zero-based index of the item that should be replaced.</param>
		protected override void ReplaceItem(object item, int index)
		{
			var columnIndex = 0;
			foreach (var column in Columns)
			{
				var element = column.CreateCellElement(item);
				Grid.SetColumn(element, columnIndex++);
				Grid.SetRow(element, index + 1);

				column.CellElements[index] = element;
				ItemsHost.Children[index] = element;
			}
		}

		/// <summary>
		///     Removes all items of the items control.
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();

			foreach (var column in Columns)
				column.CellElements.Clear();

			GenerateHeaderElements();
		}
	}
}