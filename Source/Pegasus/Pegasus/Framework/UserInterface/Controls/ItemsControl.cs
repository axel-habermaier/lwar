﻿namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using System.Collections;

	/// <summary>
	///     Represents a control that presents a collection of items.
	/// </summary>
	public class ItemsControl : Control
	{
		/// <summary>
		///     The default template that defines the visual appearance of an items control.
		/// </summary>
		private static readonly ControlTemplate DefaultTemplate = control => new StackPanel { IsItemsHost = true };

		/// <summary>
		///     The default item template that defines the visual appearance and structure of an item of an items control.
		/// </summary>
		private static readonly DataTemplate DefaultItemTemplate = () =>
		{
			var presenter = new ContentPresenter();
			presenter.CreateDataBinding(ContentPresenter.ContentProperty, BindingMode.OneWay);
			return presenter;
		};

		/// <summary>
		///     The collection that is used to generate the contents of the items control.
		/// </summary>
		public static readonly DependencyProperty<IEnumerable> ItemsSourceProperty = new DependencyProperty<IEnumerable>(affectsMeasure: true);

		/// <summary>
		///     The template that specifies the visual appearance and structure of an item generated by the items control.
		/// </summary>
		public static readonly DependencyProperty<DataTemplate> ItemTemplateProperty = new DependencyProperty<DataTemplate>(affectsMeasure: true);

		/// <summary>
		///     Initializes the type.
		/// </summary>
		static ItemsControl()
		{
			ItemsSourceProperty.Changed += OnItemsSourceChanged;
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ItemsControl()
		{
			SetStyleValue(TemplateProperty, DefaultTemplate);
			SetStyleValue(ItemTemplateProperty, DefaultItemTemplate);
		}

		/// <summary>
		///     Gets the panel that hosts the items of the items control.
		/// </summary>
		protected Panel ItemsHost { get; private set; }

		/// <summary>
		///     Gets or sets the template that specifies the visual appearance and structure of an item generated by the items control.
		/// </summary>
		public DataTemplate ItemTemplate
		{
			get { return GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		/// <summary>
		///     Gets or sets the collection that is used to generate the contents of the items control.
		/// </summary>
		public IEnumerable ItemsSource
		{
			get { return GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		/// <summary>
		///     Invoked when the items source has changed.
		/// </summary>
		private static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<IEnumerable> args)
		{
			var itemsControl = obj as ItemsControl;
			if (itemsControl == null)
				return;

			itemsControl.RegenerateItems();

			var observable = args.OldValue as INotifyCollectionChanged;
			if (observable != null)
				observable.CollectionChanged -= itemsControl.OnCollectionChanged;

			observable = args.NewValue as INotifyCollectionChanged;
			if (observable != null)
				observable.CollectionChanged += itemsControl.OnCollectionChanged;
		}

		/// <summary>
		///     Replays the changes to the collection to the items host.
		/// </summary>
		private void OnCollectionChanged(IEnumerable collection, CollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case CollectionChangedAction.Add:
					if (ItemsHost != null)
						AddItem(args.Item, args.Index);
					break;
				case CollectionChangedAction.Remove:
					if (ItemsHost != null)
						RemoveItem(args.Index);
					break;
				case CollectionChangedAction.Replace:
					if (ItemsHost != null)
						ReplaceItem(args.Item, args.Index);
					break;
				case CollectionChangedAction.Reset:
					RegenerateItems();
					break;
				default:
					Assert.NotReached("Unknown change action.");
					break;
			}
		}

		/// <summary>
		///     Invoked when the template has been changed.
		/// </summary>
		/// <param name="templateRoot">The new root element of the template.</param>
		protected override void OnTemplateChanged(UIElement templateRoot)
		{
			if (templateRoot == null)
			{
				if (ItemsHost != null)
				{
					ClearItems();
					ItemsHost = null;
				}
			}
			else
			{
				ItemsHost = FindItemsHost(templateRoot);
				Assert.NotNull(ItemsHost, "The items control has control template that does not define an items host panel.");

				if (ItemsSource != null)
					RegenerateItems();
			}
		}

		/// <summary>
		///     Recursively searches through the logical tree with the given element at the root until it finds a panel that is an items
		///     host. This method returns the first items host that is found.
		/// </summary>
		/// <param name="element">The root UI element that should be searched.</param>
		private static Panel FindItemsHost(UIElement element)
		{
			var panel = element as Panel;
			if (panel != null && panel.IsItemsHost)
				return panel;

			foreach (var child in element.LogicalChildren)
			{
				panel = FindItemsHost(child);
				if (panel != null)
					return panel;
			}

			return null;
		}

		/// <summary>
		///     Adds the given item to the items control at the given index.
		/// </summary>
		/// <param name="item">The item that should be added.</param>
		/// <param name="index">The zero-based index of the new item.</param>
		protected virtual void AddItem(object item, int index)
		{
			ItemsHost.Children.Insert(index, CreateChildElement(item));
		}

		/// <summary>
		///     Removes the item at the given index from the items control.
		/// </summary>
		/// <param name="index">The zero-based index of the item that should be removed.</param>
		protected virtual void RemoveItem(int index)
		{
			ItemsHost.Children.RemoveAt(index);
		}

		/// <summary>
		///     Replaces the item at the given index with the given new one.
		/// </summary>
		/// <param name="item">The new item that should replace the previous one.</param>
		/// <param name="index">The zero-based index of the item that should be replaced.</param>
		protected virtual void ReplaceItem(object item, int index)
		{
			ItemsHost.Children[index] = CreateChildElement(item);
		}

		/// <summary>
		///     Removes all items of the items control.
		/// </summary>
		protected virtual void ClearItems()
		{
			ItemsHost.Clear();
		}

		/// <summary>
		///     Regenerates all items of the items control.
		/// </summary>
		protected void RegenerateItems()
		{
			if (ItemsHost == null)
				return;

			ClearItems();

			var items = ItemsSource;
			if (items == null)
				return;

			var template = ItemTemplate;
			Assert.NotNull(template, "ItemTemplate cannot be null.");

			foreach (var item in items)
				AddItem(item, ItemsHost.Children.Count);
		}

		/// <summary>
		///     Creates a child UI element for the given item.
		/// </summary>
		/// <param name="item">The item the UI element should be created for.</param>
		private UIElement CreateChildElement(object item)
		{
			var template = ItemTemplate;
			Assert.NotNull(template, "ItemTemplate cannot be null.");

			var child = template();
			child.DataContext = item;
			return child;
		}
	}
}