namespace Pegasus.Framework
{
	using System;

	/// <summary>
	///   A base class for bindings that automatically ensures that values are synchronized between source and target.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	internal abstract class Binding<T>
	{
		/// <summary>
		///   The target UI element that defines the target dependency property.
		/// </summary>
		protected DependencyObject _targetObject;

		/// <summary>
		///   The target dependency property whose value is bound.
		/// </summary>
		protected DependencyProperty<T> _targetProperty;

		/// <summary>
		///   Gets a value indicating whether the binding has already been bound to a dependency property.
		/// </summary>
		internal bool IsBound { get; private set; }

		/// <summary>
		///   Initializes the binding.
		/// </summary>
		/// <param name="targetObject">The target dependency object that defines the target dependency property.</param>
		/// <param name="targetProperty">The target dependency property whose value should be bound.</param>
		internal void Initialize(DependencyObject targetObject, DependencyProperty<T> targetProperty)
		{
			Assert.ArgumentNotNull(targetObject);
			Assert.ArgumentNotNull(targetProperty);
			Assert.That(!IsBound, "The binding has already been bound to a dependency property.");

			_targetObject = targetObject;
			_targetProperty = targetProperty;

			Initialize();

			IsBound = true;
		}

		/// <summary>
		///   Removes the binding.
		/// </summary>
		internal abstract void Remove();

		/// <summary>
		///   Initializes the binding.
		/// </summary>
		protected abstract void Initialize();
	}
}