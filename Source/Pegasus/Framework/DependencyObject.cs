using System;

namespace Pegasus.Framework
{
	using System.Collections.Generic;
	using UserInterface;

	/// <summary>
	///   Represents an object that exposes dependency properties.
	/// </summary>
	public abstract class DependencyObject
	{
		/// <summary>
		///   The dependency object this dependency object gets its inherited dependency property values from.
		/// </summary>
		private DependencyObject _inheritedObject;

		/// <summary>
		///   Stores the value's of the dependency object's dependency properties.
		/// </summary>
		private DependencyPropertyStore _propertyStore = new DependencyPropertyStore();

		/// <summary>
		///   Changes the dependency object from which this dependency object gets its inherited dependency property values.
		/// </summary>
		protected void ChangeInheritedObject(DependencyObject obj)
		{
			Assert.ArgumentSatisfies(obj != this, "Detected a loop in the inheritance relationship.");

			_inheritedObject = obj;
			// TODO invalidate!
		}

		/// <summary>
		///   Sets the value of the dependency property.
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
		///   Sets the value of the dependency property originating from a style setter.
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
		///   Sets the value of the dependency property originating from a style setter being triggered.
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
		///   Sets the value of the dependency property originating from an animation.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="value">The value that should be set.</param>
		internal void SetAnimatedValue<T>(DependencyProperty<T> property, T value)
		{
			using (var setter = new DependencyPropertyValueSetter<T>(this, property, value))
				setter.PropertyValue.SetAnimatedValue(value);
		}

		/// <summary>
		///   Unsets the last value of the dependency property that originated from a style.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be unset.</param>
		internal void UnsetStyleValue<T>(DependencyProperty<T> property)
		{
			using (var setter = new DependencyPropertyValueSetter<T>(this, property))
				setter.PropertyValue.UnsetStyleValue();
		}

		/// <summary>
		///   Unsets the last value of the dependency property that originated from a style setter being triggered.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be unset.</param>
		internal void UnsetStyleTriggeredValue<T>(DependencyProperty<T> property)
		{
			using (var setter = new DependencyPropertyValueSetter<T>(this, property))
				setter.PropertyValue.UnsetStyleTriggeredValue();
		}

		/// <summary>
		///   Unsets the last value of the dependency property that originated from an animation.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be unset.</param>
		internal void UnsetAnimatedValue<T>(DependencyProperty<T> property)
		{
			using (var setter = new DependencyPropertyValueSetter<T>(this, property))
				setter.PropertyValue.UnsetAnimatedValue();
		}

		/// <summary>
		///   Gets the value of the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be returned.</param>
		public T GetValue<T>(DependencyProperty<T> property)
		{
			Assert.ArgumentNotNull(property);

			// If the property has an effective value, return it
			var value = _propertyStore.GetValueOrNull(property);
			if (value != null && value.HasEffectiveValue)
				return value.EffectiveValue;

			// If the property is not inheritable, return its default value
			if (!property.Inherits)
				return property.DefaultValue;

			// Otherwise, check whether we inherit an effective value
			var obj = _inheritedObject;
			while (obj != null)
			{
				value = obj._propertyStore.GetValueOrNull(property);
				if (value != null && value.HasEffectiveValue)
					return value.EffectiveValue;

				obj = obj._inheritedObject;
			}

			// If no value is inherited, return the property's default value
			return property.DefaultValue;
		}

		/// <summary>
		///   Adds the change handler to the dependency property's changed event.
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
		///   Removes the change handler from the dependency property's changed event.
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
		///   Attaches a binding to the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property that should be target of the binding.</param>
		/// <param name="binding">The binding that should be set.</param>
		public void SetBinding<T>(DependencyProperty<T> property, Binding<T> binding)
		{
			Assert.ArgumentNotNull(property);
			Assert.ArgumentNotNull(binding);

			binding.Initialize(this, property);
		}

		/// <summary>
		///   Abuses the IDisposable interface to allow for a more streamlined implementation of the many Set*Value and Unset*Value
		///   methods that set or unset a dependency property's value.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <remarks>
		///   Alternative implementations would rely on virtual methods or delegates, which probably introduce too much
		///   overhead.
		/// </remarks>
		private struct DependencyPropertyValueSetter<T> : IDisposable
		{
			/// <summary>
			///   The dependency object for which the value is be changed.
			/// </summary>
			private readonly DependencyObject _object;

			/// <summary>
			///   The old value of the dependency property before the change was made.
			/// </summary>
			private readonly T _oldValue;

			/// <summary>
			///   The dependency property that is about to change its value.
			/// </summary>
			private readonly DependencyProperty<T> _property;

			/// <summary>
			///   Initializes a new instance.
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
			///   Initializes a new instance.
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
			///   Gets the property value instance for the dependency object's dependency property.
			/// </summary>
			public DependencyPropertyValue<T> PropertyValue { get; private set; }

			/// <summary>
			///   Raises the changed event if the property's value has changed.
			/// </summary>
			public void Dispose()
			{
				if (PropertyValue.ChangedHandlers == null)
					return;

				var newValue = _object.GetValue(_property);
				if (!EqualityComparer<T>.Default.Equals(_oldValue, newValue))
					PropertyValue.ChangedHandlers(_object, new DependencyPropertyChangedEventArgs<T>(_property, _oldValue, newValue));
			}
		}
	}
}