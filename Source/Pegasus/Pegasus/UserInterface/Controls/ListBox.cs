namespace Pegasus.UserInterface.Controls
{
	using System;
	using Input;
	using Utilities;

	/// <summary>
	///     Represents a control that presents a collection of items. Items can be selected.
	/// </summary>
	public class ListBox : ItemsControl
	{
		/// <summary>
		///     The selected item of the list box.
		/// </summary>
		public static readonly DependencyProperty<object> SelectedItemProperty =
			new DependencyProperty<object>(prohibitsAnimations: true, defaultBindingMode: BindingMode.TwoWay);

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static ListBox()
		{
			SelectedItemProperty.Changed += OnSelectedItemChanged;
			MouseDownEvent.Raised += OnMouseDown;
		}

		/// <summary>
		///     Gets or sets the selected item of the list box.
		/// </summary>
		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		/// <summary>
		///     Checks whether the selection of the list box should be changed.
		/// </summary>
		private static void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			var listBox = sender as ListBox;
			if (listBox == null)
				return;

			// Search for the list box item
			var item = e.Source;
			while (item != null && !(item is ListBoxItem))
				item = item.VisualParent;

			if (item != null)
				listBox.SelectedItem = item.DataContext;
		}

		/// <summary>
		///     Sets the is selected property of the selected child.
		/// </summary>
		private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<object> args)
		{
			var listBox = obj as ListBox;
			if (listBox == null)
				return;

			var items = listBox.ItemsHost.Children;
			var hasSelectedItem = false;

			foreach (var item in items)
			{
				var isSelected = item.DataContext == args.NewValue;
				item.SetReadOnlyValue(ListBoxItem.IsSelectedProperty, isSelected && !hasSelectedItem);
				hasSelectedItem |= isSelected;
			}
		}

		/// <summary>
		///     Adds the given item to the items control at the given index.
		/// </summary>
		/// <param name="item">The item that should be added.</param>
		/// <param name="index">The zero-based index of the new item.</param>
		protected override void AddItem(object item, int index)
		{
			ItemsHost.Children.Insert(index, CreateChildElement(item));
		}

		/// <summary>
		///     Replaces the item at the given index with the given new one.
		/// </summary>
		/// <param name="item">The new item that should replace the previous one.</param>
		/// <param name="index">The zero-based index of the item that should be replaced.</param>
		protected override void ReplaceItem(object item, int index)
		{
			if (ItemsHost.Children[index] == SelectedItem)
				SelectedItem = null;

			ItemsHost.Children[index] = CreateChildElement(item);
		}

		/// <summary>
		///     Removes the item at the given index from the items control.
		/// </summary>
		/// <param name="index">The zero-based index of the item that should be removed.</param>
		protected override void RemoveItem(int index)
		{
			if (ItemsHost.Children[index] == SelectedItem)
				SelectedItem = null;

			base.RemoveItem(index);
		}

		/// <summary>
		///     Removes all items of the items control.
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();
			SelectedItem = null;
		}

		/// <summary>
		///     Creates a child UI element for the given item.
		/// </summary>
		/// <param name="item">The item the UI element should be created for.</param>
		private UIElement CreateChildElement(object item)
		{
			var template = ItemTemplate;
			Assert.NotNull(template, "ItemTemplate cannot be null.");

			return new ListBoxItem { Content = template(), DataContext = item };
		}
	}
}