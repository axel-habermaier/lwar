namespace Pegasus.Framework
{
	using System;
	using System.Collections;

	/// <summary>
	///     Handles a change notification for a property.
	/// </summary>
	/// <param name="collection">The collection that has been changed.</param>
	/// <param name="args">Describes the changes that have been made to the collection.</param>
	public delegate void CollectionChangedHandler(IEnumerable collection, CollectionChangedEventArgs args);
}