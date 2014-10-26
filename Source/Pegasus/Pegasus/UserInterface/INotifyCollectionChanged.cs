namespace Pegasus.UserInterface
{
	using System;

	/// <summary>
	///     Raises an event when items of a collection were added, replaced, or removed.
	/// </summary>
	public interface INotifyCollectionChanged
	{
		/// <summary>
		///     Raised when the collection has been changed.
		/// </summary>
		event CollectionChangedHandler CollectionChanged;
	}
}