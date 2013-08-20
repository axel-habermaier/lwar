using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Handles a change notification for a property.
	/// </summary>
	/// <param name="obj">The object the changed property belongs to.</param>
	/// <param name="property">The name of the property that has changed.</param>
	public delegate void PropertyChangedHandler(object obj, string property);

	/// <summary>
	///   Handles a strongly-typed changed notification for a dependency property.
	/// </summary>
	/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
	/// <param name="obj">The dependency object the changed dependency property belongs to.</param>
	/// <param name="args">
	///   Provides information about the dependency property that has been changed and its previous and
	///   current values.
	/// </param>
	public delegate void DependencyPropertyChangedHandler<T>(DependencyObject obj, DependencyPropertyChangedEventArgs<T> args);

	/// <summary>
	///   Handles an untyped changed notification for a dependency property.
	/// </summary>
	/// <param name="obj">The dependency object the changed dependency property belongs to.</param>
	/// <param name="property">The dependency property that has been changed.</param>
	internal delegate void DependencyPropertyChangedHandler(DependencyObject obj, DependencyProperty property);

	/// <summary>
	///   Handles a change notification for a resource dictionary.
	/// </summary>
	/// <param name="resourceDictionary">The resource dictionary that has been changed.</param>
	/// <param name="key">The key of the resource that has been changed.</param>
	internal delegate void ResourceKeyChangedHandler(ResourceDictionary resourceDictionary, string key);
}