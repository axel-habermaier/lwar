using System;

namespace Pegasus.Rendering.UserInterface
{
	/// <summary>
	///   Raises an event when a property has been changed.
	/// </summary>
	public interface INotifyPropertyChanged
	{
		/// <summary>
		///   Raised when a property of the current object has been changed.
		/// </summary>
		event PropertyChangedHandler PropertyChanged;
	}
}