namespace Pegasus.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using Platform;
	using Platform.Memory;

	/// <summary>
	///     A base class for pooled classes requiring property change notifications.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	public abstract class OldPooledNotifyPropertyChanged<T> : OldPooledObject<T>, INotifyPropertyChanged
		where T : OldPooledObject<T>, new()
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
		protected void ChangePropertyValue<TProperty>(ref TProperty field, TProperty newValue, [CallerMemberName] string propertyName = "")
		{
			Assert.ArgumentNotNullOrWhitespace(propertyName);

			if (EqualityComparer<TProperty>.Default.Equals(field, newValue))
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