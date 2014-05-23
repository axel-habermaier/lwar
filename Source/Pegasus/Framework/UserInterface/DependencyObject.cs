namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     Represents an object that exposes dependency properties.
	/// </summary>
	public abstract class DependencyObject
	{
		/// <summary>
		///     Stores the values of the dependency object's dependency properties.
		/// </summary>
		private DependencyPropertyStore _propertyStore = new DependencyPropertyStore();

		/// <summary>
		///     Sets the value of the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="value">The value that should be set.</param>
		public void SetValue<T>(DependencyProperty<T> property, T value)
		{
			using (var setter = new DependencyPropertyValueSetter<T>(this, property, value))
				setter.PropertyValue.SetLocalValue(value);
		}

		/// <summary>
		///     Sets the inherited value of the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="value">The value that should be set.</param>
		internal void SetInheritedValue<T>(DependencyProperty<T> property, T value)
		{
			Assert.ArgumentSatisfies(property.Inherits, "The property does not support value inheritance.");

			using (var setter = new DependencyPropertyValueSetter<T>(this, property, value))
				setter.PropertyValue.SetInheritedValue(value);
		}

		/// <summary>
		///     Sets the value of the dependency property originating from a style setter.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="value">The value that should be set.</param>
		internal void SetStyleValue<T>(DependencyProperty<T> property, T value)
		{
			using (var setter = new DependencyPropertyValueSetter<T>(this, property, value))
				setter.PropertyValue.SetStyleValue(value);
		}

		/// <summary>
		///     Sets the value of the dependency property originating from a style setter being triggered.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="value">The value that should be set.</param>
		internal void SetStyleTriggeredValue<T>(DependencyProperty<T> property, T value)
		{
			using (var setter = new DependencyPropertyValueSetter<T>(this, property, value))
				setter.PropertyValue.SetStyleTriggeredValue(value);
		}

		/// <summary>
		///     Sets the value of the dependency property originating from an animation.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="value">The value that should be set.</param>
		internal void SetAnimatedValue<T>(DependencyProperty<T> property, T value)
		{
			Assert.ArgumentSatisfies(!property.IsAnimationProhibited, "The property does not support animations.");

			using (var setter = new DependencyPropertyValueSetter<T>(this, property, value))
				setter.PropertyValue.SetAnimatedValue(value);
		}

		/// <summary>
		///     Sets a binding for the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="binding">The binding that should be set.</param>
		internal void SetBinding<T>(DependencyProperty<T> property, Binding<T> binding)
		{
			Assert.ArgumentNotNull(property);
			Assert.ArgumentNotNull(binding);

			_propertyStore.GetValueAddUnknown(property).SetBinding(binding);
			binding.Initialize(this, property);
		}

		/// <summary>
		///     Sets the value of the dependency property originating from a binding.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="value">The value that should be set.</param>
		internal void SetBoundValue<T>(DependencyProperty<T> property, T value)
		{
			using (var setter = new DependencyPropertyValueSetter<T>(this, property, value))
				setter.PropertyValue.SetBoundValue(value);
		}

		/// <summary>
		///     Unsets the last value of the dependency property that originated from a style.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be unset.</param>
		internal void UnsetStyleValue<T>(DependencyProperty<T> property)
		{
			using (var setter = new DependencyPropertyValueSetter<T>(this, property))
				setter.PropertyValue.UnsetStyleValue();
		}

		/// <summary>
		///     Unsets the inherited value of the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be unset.</param>
		internal void UnsetInheritedValue<T>(DependencyProperty<T> property)
		{
			Assert.ArgumentSatisfies(property.Inherits, "The property does not support value inheritance.");

			using (var setter = new DependencyPropertyValueSetter<T>(this, property))
				setter.PropertyValue.UnsetInheritedValue();
		}

		/// <summary>
		///     Unsets the last value of the dependency property that originated from a style setter being triggered.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be unset.</param>
		internal void UnsetStyleTriggeredValue<T>(DependencyProperty<T> property)
		{
			using (var setter = new DependencyPropertyValueSetter<T>(this, property))
				setter.PropertyValue.UnsetStyleTriggeredValue();
		}

		/// <summary>
		///     Unsets the last value of the dependency property that originated from an animation.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be unset.</param>
		internal void UnsetAnimatedValue<T>(DependencyProperty<T> property)
		{
			Assert.ArgumentSatisfies(!property.IsAnimationProhibited, "The property does not support animations.");

			using (var setter = new DependencyPropertyValueSetter<T>(this, property))
				setter.PropertyValue.UnsetAnimatedValue();
		}

		/// <summary>
		///     Unsets the current binding of the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose binding should be removed.</param>
		internal void UnsetBinding<T>(DependencyProperty<T> property)
		{
			_propertyStore.GetValueAddUnknown(property).UnsetBinding();
		}

		/// <summary>
		///     Gets the value of the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be returned.</param>
		public T GetValue<T>(DependencyProperty<T> property)
		{
			Assert.ArgumentNotNull(property);

			// If the property has an effective value, return it
			T value;
			if (TryGetEffectiveValue(property, out value))
				return value;

			// If no value is inherited, return the property's default value
			return property.DefaultValue;
		}

		/// <summary>
		///     Gets the current effective value of the dependency property. Returns true to indicate that an effective value is set.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be returned.</param>
		/// <param name="value">Returns the effective value, if one is set.</param>
		protected bool TryGetEffectiveValue<T>(DependencyProperty<T> property, out T value)
		{
			Assert.ArgumentNotNull(property);

			var propertyValue = _propertyStore.GetValueOrNull(property);
			if (propertyValue != null && propertyValue.HasEffectiveValue)
			{
				value = propertyValue.EffectiveValue;
				return true;
			}

			value = default(T);
			return false;
		}

		/// <summary>
		///     Adds the change handler to the dependency property's changed event.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property the change handler should be added to.</param>
		/// <param name="changeHandler">The change handler that should be added.</param>
		public void AddChangedHandler<T>(DependencyProperty<T> property, DependencyPropertyChangedHandler<T> changeHandler)
		{
			Assert.ArgumentNotNull(property);
			Assert.ArgumentNotNull(changeHandler);

			_propertyStore.GetValueAddUnknown(property).ChangedHandlers += changeHandler;
		}

		/// <summary>
		///     Removes the change handler from the dependency property's changed event.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property the change handler should be removed from.</param>
		/// <param name="changeHandler">The change handler that should be removed.</param>
		public void RemoveChangedHandler<T>(DependencyProperty<T> property, DependencyPropertyChangedHandler<T> changeHandler)
		{
			Assert.ArgumentNotNull(property);
			Assert.ArgumentNotNull(changeHandler);

			var value = _propertyStore.GetValueOrNull(property);
			if (value != null)
				value.ChangedHandlers -= changeHandler;
		}

		/// <summary>
		///     Changes the activation state of all bindings that are set on any of the dependency object's dependency properties.
		/// </summary>
		/// <param name="activated">Indicates whether the bindings should be activated.</param>
		internal void SetBindingsActivationState(bool activated)
		{
			_propertyStore.SetBindingsActivationState(activated);
		}

		/// <summary>
		///     Notifies all inheriting objects about a change of an inheriting dependency property.
		/// </summary>
		/// <param name="property">The inheriting dependency property that has been changed.</param>
		/// <param name="newValue">The new value that should be inherited.</param>
		protected abstract void InheritedValueChanged<T>(DependencyProperty<T> property, T newValue);

		/// <summary>
		///     Invalidates the inherited values of all inheriting dependency properties.
		/// </summary>
		/// <param name="inheritedObject">The new inherited dependency object.</param>
		protected void InvalidateInheritedValues(DependencyObject inheritedObject)
		{
			if (inheritedObject != null)
				inheritedObject._propertyStore.SetInheritedValues(inheritedObject, this);
			else
				_propertyStore.UnsetInheritedValues(this);
		}

		/// <summary>
		///     Abuses the IDisposable interface to allow for a more streamlined implementation of the many Set*Value and Unset*Value
		///     methods that set or unset a dependency property's value.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <remarks>
		///     Alternative implementations would rely on virtual methods or delegates, which probably introduce too much
		///     overhead.
		/// </remarks>
		private struct DependencyPropertyValueSetter<T> : IDisposable
		{
			/// <summary>
			///     The dependency object for which the value is be changed.
			/// </summary>
			private readonly DependencyObject _object;

			/// <summary>
			///     The old value of the dependency property before the change was made.
			/// </summary>
			private readonly T _oldValue;

			/// <summary>
			///     The dependency property that is about to change its value.
			/// </summary>
			private readonly DependencyProperty<T> _property;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="obj">The dependency object for which the value should be changed.</param>
			/// <param name="property">The dependency property that is about to change its value.</param>
			/// <param name="newValue">The new value for the dependency property.</param>
			public DependencyPropertyValueSetter(DependencyObject obj, DependencyProperty<T> property, T newValue)
				: this(obj, property)
			{
				_property.ValidateValue(newValue);
			}

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="obj">The dependency object for which the value should be changed.</param>
			/// <param name="property">The dependency property that is about to change its value.</param>
			public DependencyPropertyValueSetter(DependencyObject obj, DependencyProperty<T> property)
				: this()
			{
				Assert.ArgumentNotNull(obj);
				Assert.ArgumentNotNull(property);

				_object = obj;
				_property = property;

				_oldValue = obj.GetValue(property);
				PropertyValue = obj._propertyStore.GetValueAddUnknown(property);
			}

			/// <summary>
			///     Gets the property value instance for the dependency object's dependency property.
			/// </summary>
			public DependencyPropertyValue<T> PropertyValue { get; private set; }

			/// <summary>
			///     Raises the changed event if the property's value has changed.
			/// </summary>
			public void Dispose()
			{
				// Check if the property's value has changed
				var newValue = _object.GetValue(_property);
				if (EqualityComparer<T>.Default.Equals(_oldValue, newValue))
					return;

				var changedEventArgs = new DependencyPropertyChangedEventArgs<T>(_property, _oldValue, newValue);

				// Invoke the static changed handlers
				_property.OnValueChanged(_object, changedEventArgs);

				// Invoke the instance changed handlers, if any
				if (PropertyValue.ChangedHandlers != null)
					PropertyValue.ChangedHandlers(_object, changedEventArgs);

				// If the property inherits its value, we have to push down the change to all inheriting objects
				if (_property.Inherits)
					_object.InheritedValueChanged(_property, newValue);
			}
		}
	}
}