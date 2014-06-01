namespace Pegasus.Framework
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     Represents a collection that provides change notifications when items are added, removed, or replaced.
	/// </summary>
	public class ObservableCollection<T> : CustomCollection<T>, INotifyPropertyChanged, INotifyCollectionChanged
	{
		/// <summary>
		///     Initializes a new, empty instance.
		/// </summary>
		public ObservableCollection()
		{
		}

		/// <summary>
		///     Initializes a new instance, containing all items from the given list.
		/// </summary>
		/// <param name="list">The list of items that should be contained in the new instance.</param>
		public ObservableCollection(IList<T> list)
			: base(list)
		{
		}

		/// <summary>
		///     Raised when the collection has been changed.
		/// </summary>
		public event CollectionChangedHandler CollectionChanged;

		/// <summary>
		///     Raised when a property of the current object has been changed.
		/// </summary>
		public event PropertyChangedHandler PropertyChanged;

		/// <summary>
		///     Removes all elements from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			base.ClearItems();

			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionChanged(CollectionChangedEventArgs.Reset());
		}

		/// <summary>
		///     Inserts an element into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the item that should be inserted.</param>
		/// <param name="item">The item that should be inserted.</param>
		protected override void InsertItem(int index, T item)
		{
			base.InsertItem(index, item);

			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionChanged(CollectionChangedEventArgs.ItemAdded(item, index));
		}

		/// <summary>
		///     Removes the element at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be removed.</param>
		protected override void RemoveItem(int index)
		{
			var item = this[index];
			base.RemoveItem(index);

			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnCollectionChanged(CollectionChangedEventArgs.ItemRemoved(item, index));
		}

		/// <summary>
		///     Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be replaced.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		protected override void SetItem(int index, T item)
		{
			base.SetItem(index, item);

			OnPropertyChanged("Item[]");
			OnCollectionChanged(CollectionChangedEventArgs.ItemReplaced(item, index));
		}

		/// <summary>
		///     Raises the property changed event.
		/// </summary>
		/// <param name="property">The name of the property that has been changed.</param>
		private void OnPropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, property);
		}

		/// <summary>
		///     Raises the collection changed event.
		/// </summary>
		/// <param name="args">The arguments describing the collection change.</param>
		private void OnCollectionChanged(CollectionChangedEventArgs args)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, args);
		}
	}
}