namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///     Represents a collection of UI elements that belongs to an UI element. When an UI element is added to or removed from the
	///     collection, its logical parent is updated accordingly.
	/// </summary>
	public class UIElementCollection : CustomCollection<UIElement>
	{
		/// <summary>
		///     The logical parent of the UI elements contained in the collection.
		/// </summary>
		private readonly UIElement _logicalParent;

		/// <summary>
		///     The visual parent of the UI elements contained in the collection.
		/// </summary>
		private readonly UIElement _visualParent;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="logicalParent">The logical parent of the UI elements contained in the collection.</param>
		/// <param name="visualParent"> The visual parent of the UI elements contained in the collection.</param>
		public UIElementCollection(UIElement logicalParent, UIElement visualParent)
		{
			Assert.ArgumentNotNull(logicalParent);
			Assert.ArgumentNotNull(visualParent);

			_logicalParent = logicalParent;
			_visualParent = visualParent;
		}

		/// <summary>
		///     Removes all elements from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			foreach (var element in this)
			{
				element.ChangeLogicalParent(null);
				element.VisualParent = null;
			}

			_logicalParent.OnVisualChildrenChanged();
			base.ClearItems();
		}

		/// <summary>
		///     Inserts an element into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the item that should be inserted.</param>
		/// <param name="item">The item that should be inserted.</param>
		protected override void InsertItem(int index, UIElement item)
		{
			Assert.ArgumentNotNull(item);

			base.InsertItem(index, item);

			item.ChangeLogicalParent(_logicalParent);
			item.VisualParent = _visualParent;
			_logicalParent.OnVisualChildrenChanged();
		}

		/// <summary>
		///     Removes the element at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be removed.</param>
		protected override void RemoveItem(int index)
		{
			this[index].ChangeLogicalParent(null);
			this[index].VisualParent = null;

			base.RemoveItem(index);

			_logicalParent.OnVisualChildrenChanged();
		}

		/// <summary>
		///     Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be replaced.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		protected override void SetItem(int index, UIElement item)
		{
			Assert.ArgumentNotNull(item);

			base.SetItem(index, item);

			this[index].ChangeLogicalParent(null);
			this[index].VisualParent = null;

			item.ChangeLogicalParent(_logicalParent);
			item.VisualParent = _visualParent;

			_logicalParent.OnVisualChildrenChanged();
		}
	}
}