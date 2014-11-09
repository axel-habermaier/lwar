namespace Pegasus.UserInterface
{
	using System;
	using Utilities;

	/// <summary>
	///     Describes a modification of a collection.
	/// </summary>
	public struct CollectionChangedEventArgs
	{
		/// <summary>
		///     The zero-based index of the item within the collection.
		/// </summary>
		private readonly int _index;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="action">Indicates what kind of change was made to the collection.</param>
		/// <param name="index">The zero-based index of the item within the collection.</param>
		private CollectionChangedEventArgs(CollectionChangedAction action, int index)
			: this()
		{
			Assert.ArgumentInRange(action);
			Assert.ArgumentSatisfies(index >= 0, "Invalid index.");

			Action = action;
			_index = index;
		}

		/// <summary>
		///     Gets a value indicating what kind of change was made to the collection.
		/// </summary>
		public CollectionChangedAction Action { get; private set; }

		/// <summary>
		///     Gets the zero-based index of the item within the collection.
		/// </summary>
		public int Index
		{
			get
			{
				Assert.That(Action != CollectionChangedAction.Reset, "The item has been moved.");
				return _index;
			}
		}

		/// <summary>
		///     Initializes a new instance describing the addition of a new item to the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the new item within the collection.</param>
		public static CollectionChangedEventArgs ItemAdded(int index)
		{
			return new CollectionChangedEventArgs(CollectionChangedAction.Add, index);
		}

		/// <summary>
		///     Initializes a new instance describing the removal of an item to the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the item within the collection.</param>
		public static CollectionChangedEventArgs ItemRemoved(int index)
		{
			return new CollectionChangedEventArgs(CollectionChangedAction.Remove, index);
		}

		/// <summary>
		///     Initializes a new instance describing a replacement of an item in the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the item within the collection.</param>
		public static CollectionChangedEventArgs ItemReplaced(int index)
		{
			return new CollectionChangedEventArgs(CollectionChangedAction.Replace, index);
		}

		/// <summary>
		///     Initializes a new instance describing a reset of the collection.
		/// </summary>
		public static CollectionChangedEventArgs Reset()
		{
			return new CollectionChangedEventArgs(CollectionChangedAction.Reset, 0);
		}
	}
}