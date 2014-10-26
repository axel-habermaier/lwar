namespace Pegasus.UserInterface
{
	using System;

	/// <summary>
	///     Indicates what kind of change was made to a collection.
	/// </summary>
	public enum CollectionChangedAction
	{
		/// <summary>
		///     Indicates that an item was added to the collection.
		/// </summary>
		Add,

		/// <summary>
		///     Indicates that an item was removed from the collection.
		/// </summary>
		Remove,

		/// <summary>
		///     Indicates that an item of the collection was replaced.
		/// </summary>
		Replace,

		/// <summary>
		///     Indicates that a large change has been made to the collection, affecting many of its items.
		/// </summary>
		Reset,
	}
}