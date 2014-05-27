namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Platform;
	using Platform.Memory;

	/// <summary>
	///     A view model can be bound to an UI element, providing both the values that the UI displays and the methods that
	///     handle UI commands.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public abstract class ViewModel : DisposableObject, INotifyPropertyChanged
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
			NotifyPropertyChanged(propertyName);
		}

		/// <summary>
		///     Raises the changed event for the given property.
		/// </summary>
		/// <param name="propertyName">The name of the property that was changed.</param>
		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, propertyName);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
		}
	}
}