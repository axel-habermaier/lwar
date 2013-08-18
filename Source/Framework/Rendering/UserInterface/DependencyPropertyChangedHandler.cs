using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Handles a change notification for a dependency property.
	/// </summary>
	/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
	/// <param name="obj">The dependency object the changed dependency property belongs to.</param>
	/// <param name="property">The dependency property that has changed.</param>
	/// <param name="newValue">The new value of the dependency property.</param>
	public delegate void DependencyPropertyChangedHandler<T>(DependencyObject obj, DependencyProperty<T> property, T newValue);
}