using System;

namespace Pegasus.Rendering.UserInterface
{
	using System.Collections.ObjectModel;

	/// <summary>
	///   Represents an ordered collection of UI elements.
	/// </summary>
	public class UIElementCollection : Collection<UIElement>
	{
		/// <summary>
		///   The logical parent of the UI elements contained in the collection.
		/// </summary>
		private readonly UIElement _logicalParent;

		/// <summary>
		///   The version of the collection. Each modification of the collection increments the version number by one.
		/// </summary>
		private int _version;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="logicalParent">The logical parent of the UI elements contained in the collection.</param>
		public UIElementCollection(UIElement logicalParent)
		{
			Assert.ArgumentNotNull(logicalParent);
			_logicalParent = logicalParent;
		}

		/// <summary>
		///   Removes all elements from the collection.
		/// </summary>
		protected override void ClearItems()
		{
			++_version;

			foreach (var element in this)
				element.ChangeLogicalParent(null);

			base.ClearItems();
		}

		/// <summary>
		///   Inserts an element into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the item that should be inserted.</param>
		/// <param name="item">The item that should be inserted.</param>
		protected override void InsertItem(int index, UIElement item)
		{
			Assert.ArgumentNotNull(item);

			item.ChangeLogicalParent(_logicalParent);

			++_version;
			base.InsertItem(index, item);
		}

		/// <summary>
		///   Removes the element at the specified index of the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be removed.</param>
		protected override void RemoveItem(int index)
		{
			this[index].ChangeLogicalParent(null);

			++_version;
			base.RemoveItem(index);
		}

		/// <summary>
		///   Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be replaced.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		protected override void SetItem(int index, UIElement item)
		{
			Assert.ArgumentNotNull(item);

			this[index].ChangeLogicalParent(null);
			item.ChangeLogicalParent(_logicalParent);

			++_version;
			base.SetItem(index, item);
		}

		/// <summary>
		///   Gets an enumerator for the collection.
		/// </summary>
		public new Enumerator GetEnumerator()
		{
			return Enumerator.FromElements(this);
		}

		/// <summary>
		///   Enumerates an UI element collection.
		/// </summary>
		public struct Enumerator
		{
			/// <summary>
			///   Creates an enumerator that does not enumerate any elements.
			/// </summary>
			public static readonly Enumerator Empty = new Enumerator();

			/// <summary>
			///   The elements that are enumerated.
			/// </summary>
			private UIElementCollection _collection;

			/// <summary>
			///   The index of the current enumerated element.
			/// </summary>
			private int _current;

			/// <summary>
			///   The single element that is enumerated.
			/// </summary>
			private UIElement _singleElement;

			/// <summary>
			///   The version of the collection when the enumerator was created.
			/// </summary>
			private int _version;

			/// <summary>
			///   Gets the element at the current position of the enumerator.
			/// </summary>
			public UIElement Current { get; private set; }

			/// <summary>
			///   Creates an enumerator for a single UI element.
			/// </summary>
			/// <param name="element">The UI element that should be enumerated.</param>
			public static Enumerator FromElement(UIElement element)
			{
				Assert.ArgumentNotNull(element);
				return new Enumerator { _singleElement = element };
			}

			/// <summary>
			///   Creates an enumerator for a single UI element.
			/// </summary>
			/// <param name="collection">The UI elements that should be enumerated.</param>
			public static Enumerator FromElements(UIElementCollection collection)
			{
				Assert.ArgumentNotNull(collection);
				return new Enumerator { _collection = collection, _version = collection._version };
			}

			/// <summary>
			///   Advances the enumerator to the next UI element.
			/// </summary>
			public bool MoveNext()
			{
				// If we neither have a single element or a collection, we're done
				if (_singleElement == null && _collection == null)
					return false;

				// If we have a single element, enumerate it and make sure the next call to MoveNext returns false
				if (_singleElement != null)
				{
					Current = _singleElement;
					_singleElement = null;

					return true;
				}

				Assert.That(_collection._version == _version, "The collection has been modified while it is enumerated.");

				// If we've reached the end of the collection, we're done
				if (_current == _collection.Count)
				{
					_collection = null;
					return false;
				}

				// Otherwise, enumerate the next element
				Current = _collection[_current++];
				return true;
			}

			/// <summary>
			///   Gets the enumerator that can be used with C#'s foreach loops.
			/// </summary>
			/// <remarks>
			///   This method just returns the enumerator object. It is only required to enable foreach support.
			/// </remarks>
			public Enumerator GetEnumerator()
			{
				return this;
			}
		}
	}
}