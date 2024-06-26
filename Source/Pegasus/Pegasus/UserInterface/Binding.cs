﻿namespace Pegasus.UserInterface
{
	using System;
	using Utilities;

	/// <summary>
	///     A base class for bindings that automatically ensures that values are synchronized between source and target.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	internal abstract class Binding<T>
	{
		/// <summary>
		///     Indicates whether the binding is currently active.
		/// </summary>
		private bool _active;

		/// <summary>
		///     The target UI element that defines the target dependency property.
		/// </summary>
		protected DependencyObject _targetObject;

		/// <summary>
		///     The target dependency property whose value is bound.
		/// </summary>
		protected DependencyProperty<T> _targetProperty;

		/// <summary>
		///     Gets or sets a value indicating whether the binding is currently active.
		/// </summary>
		internal bool Active
		{
			get { return _active; }
			set
			{
				if (_active == value)
					return;

				_active = value;

				if (_active)
					Activate();
				else
					Deactivate();
			}
		}

		/// <summary>
		///     Invoked when the binding has been activated.
		/// </summary>
		protected abstract void Activate();

		/// <summary>
		///     Initializes the binding.
		/// </summary>
		/// <param name="targetObject">The target dependency object that defines the target dependency property.</param>
		/// <param name="targetProperty">The target dependency property whose value should be bound.</param>
		internal void Initialize(DependencyObject targetObject, DependencyProperty<T> targetProperty)
		{
			Assert.ArgumentNotNull(targetObject);
			Assert.ArgumentNotNull(targetProperty);

			_targetObject = targetObject;
			_targetProperty = targetProperty;
		}

		/// <summary>
		///     Removes the binding. The binding can decide to ignore the removal if it would be overwritten by a local value. True is
		///     returned to indicate that the binding was removed.
		/// </summary>
		/// <param name="overwrittenByLocalValue">Indicates whether the binding is removed because it was overriden by a local value.</param>
		internal abstract bool Deactivate(bool overwrittenByLocalValue = false);
	}
}