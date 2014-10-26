namespace Pegasus.UserInterface
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     A base class for disposable classes requiring property change notifications.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public abstract class DisposableNotifyPropertyChanged : DisposableObject, INotifyPropertyChanged
	{
		/// <summary>
		///     Raised when a property of the view model has been changed.
		/// </summary>
		public event PropertyChangedHandler PropertyChanged;

		/// <summary>
		///     Changes the property value and raises the property changed event.
		/// </summary>
		/// <param name="field">The backing field that stores the property value.</param>
		/// <param name="newValue">The new value that should be set on the property.</param>
		/// <param name="propertyName">The name of the property that is being changed.</param>
		protected void ChangePropertyValue<T>(ref T field, T newValue, [CallerMemberName] string propertyName = "")
		{
			Assert.ArgumentNotNullOrWhitespace(propertyName);

			if (EqualityComparer<T>.Default.Equals(field, newValue))
				return;

			field = newValue;
			OnPropertyChanged(propertyName);
		}

		/// <summary>
		///     Raises the changed event for the given property.
		/// </summary>
		/// <param name="propertyName">The name of the property that was changed.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, propertyName);
		}
	}
}