namespace Pegasus.Framework
{
	using System;

	/// <summary>
	///     Handles a change notification for a property.
	/// </summary>
	/// <param name="obj">The object the changed property belongs to.</param>
	/// <param name="property">The name of the property that has changed.</param>
	public delegate void PropertyChangedHandler(object obj, string property);

	/// <summary>
	///     Raises an event when a property has been changed.
	/// </summary>
	public interface INotifyPropertyChanged
	{
		/// <summary>
		///     Raised when a property of the current object has been changed.
		/// </summary>
		event PropertyChangedHandler PropertyChanged;
	}
}