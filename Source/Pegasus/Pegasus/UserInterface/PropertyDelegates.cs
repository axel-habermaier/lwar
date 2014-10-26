namespace Pegasus.UserInterface
{
	using System;

	/// <summary>
	///     Handles a strongly-typed changed notification for a dependency property.
	/// </summary>
	/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
	/// <param name="obj">The dependency object the changed dependency property belongs to.</param>
	/// <param name="args">
	///     Provides information about the dependency property that has been changed and its previous and
	///     current values.
	/// </param>
	public delegate void DependencyPropertyChangedHandler<T>(DependencyObject obj, DependencyPropertyChangedEventArgs<T> args);

	/// <summary>
	///     Handles an untyped changed notification for a dependency property.
	/// </summary>
	/// <param name="obj">The dependency object the changed dependency property belongs to.</param>
	/// <param name="property">The dependency property that has been changed.</param>
	internal delegate void DependencyPropertyChangedHandler(DependencyObject obj, DependencyProperty property);

	/// <summary>
	///     Validates whether the given value is a valid value for a dependency property. Returns true if the value is valid;
	///     false, otherwise.
	/// </summary>
	/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
	/// <param name="value">The value that should be validated.</param>
	public delegate bool DependencyPropertyValidationCallback<in T>(T value);
}