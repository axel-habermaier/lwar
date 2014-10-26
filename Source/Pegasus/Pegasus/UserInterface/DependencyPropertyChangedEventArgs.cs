namespace Pegasus.UserInterface
{
	using System;
	using Utilities;

	/// <summary>
	///     Provides information about a change notification for a dependency property.
	/// </summary>
	/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
	public struct DependencyPropertyChangedEventArgs<T>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="property">The property that has been changed.</param>
		/// <param name="oldValue">The value of the property before the change.</param>
		/// <param name="newValue">The value of the property after the change.</param>
		internal DependencyPropertyChangedEventArgs(DependencyProperty<T> property, T oldValue, T newValue)
			: this()
		{
			Assert.ArgumentNotNull(property);

			Property = property;
			OldValue = oldValue;
			NewValue = newValue;
		}

		/// <summary>
		///     Gets the property that has been changed.
		/// </summary>
		public DependencyProperty<T> Property { get; private set; }

		/// <summary>
		///     Gets the value of the property before the change.
		/// </summary>
		public T OldValue { get; private set; }

		/// <summary>
		///     Gets the value of the property after the change.
		/// </summary>
		public T NewValue { get; private set; }
	}
}