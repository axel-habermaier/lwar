using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Represents an untyped property that has multiple sources (such as data bindings, style setters, animation,
	///   etc.).
	/// </summary>
	public abstract class DependencyProperty
	{
		/// <summary>
		///   The number of dependency properties that have been registered throughout the lifetime of the application.
		/// </summary>
		private static int _propertyCount;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		protected DependencyProperty()
		{
			Index = _propertyCount++;
		}

		/// <summary>
		///   The index of the dependency property that remains unchanged and unique throughout the lifetime of the application.
		/// </summary>
		internal int Index { get; private set; }

		/// <summary>
		///   Gets the type of the value stored by the dependency property.
		/// </summary>
		internal abstract Type ValueType { get; }

		/// <summary>
		///   Adds an untyped changed handler to the dependency property for the given dependency object. Returns the delegate that
		///   must can used to remove the untyped change handler.
		/// </summary>
		/// <param name="obj">The dependency object the handler should be added to.</param>
		/// <param name="handler">The handler that should be added.</param>
		internal abstract Delegate AddUntypedChangeHandler(DependencyObject obj, DependencyPropertyChangedHandler handler);

		/// <summary>
		///   Removes an untyped changed handler from the dependency property for the given dependency object.
		/// </summary>
		/// <param name="obj">The dependency object the handler should be removed from.</param>
		/// <param name="handler">The handler that should be removed.</param>
		internal abstract void RemoveUntypedChangeHandler(DependencyObject obj, Delegate handler);

		/// <summary>
		/// Gets the untyped value of the dependency property for the given dependency object.
		/// </summary>
		/// <param name="obj">The dependency object the value should be returned for.</param>
		internal abstract object GetValue(DependencyObject obj);
	}
}