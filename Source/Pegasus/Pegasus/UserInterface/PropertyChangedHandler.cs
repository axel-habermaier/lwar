namespace Pegasus.UserInterface
{
	using System;

	/// <summary>
	///     Handles a change notification for a property.
	/// </summary>
	/// <param name="obj">The object the changed property belongs to.</param>
	/// <param name="property">The name of the property that has changed.</param>
	public delegate void PropertyChangedHandler(object obj, string property);
}