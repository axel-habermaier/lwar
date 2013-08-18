using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Represents an object that exposes dependency properties.
	/// </summary>
	public abstract class DependencyObject
	{
		/// <summary>
		///   Stores the value's of the dependency object's dependency properties.
		/// </summary>
		private DependencyPropertyStore _propertyStore = new DependencyPropertyStore();

		/// <summary>
		///   Sets the value of the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="value">The value that should be set.</param>
		public void SetValue<T>(DependencyProperty<T> property, T value)
		{
			Assert.ArgumentNotNull(property);

			var previousValue = GetValue(property);
			var propertyValue = _propertyStore.GetValue(property, addIfNotFound: true);

			propertyValue.SetLocalValue(value);
			RaiseChangeEvent(property, propertyValue, previousValue);
		}

		/// <summary>
		///   Sets the value of the dependency property originating from an animation.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be set.</param>
		/// <param name="value">The value that should be set.</param>
		internal void SetAnimatedValue<T>(DependencyProperty<T> property, T value)
		{
			Assert.ArgumentNotNull(property);

			var previousValue = GetValue(property);
			var propertyValue = _propertyStore.GetValue(property, addIfNotFound: true);

			propertyValue.SetAnimatedValue(value);
			RaiseChangeEvent(property, propertyValue, previousValue);
		}

		/// <summary>
		///   Unsets the last value of the dependency property that originated from an animation.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be unset.</param>
		internal void UnsetAnimatedValue<T>(DependencyProperty<T> property)
		{
			Assert.ArgumentNotNull(property);

			var previousValue = GetValue(property);
			var propertyValue = _propertyStore.GetValue(property, addIfNotFound: false);

			propertyValue.UnsetAnimatedValue();
			RaiseChangeEvent(property, propertyValue, previousValue);
		}

		/// <summary>
		///   Raises the change event for the given property if its effective value has been changed.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property the change event should be raised for.</param>
		/// <param name="propertyValue">The current property value.</param>
		/// <param name="previousValue">The previous effective value of the property.</param>
		private void RaiseChangeEvent<T>(DependencyProperty<T> property, DependencyPropertyValue<T> propertyValue, T previousValue)
		{
			if (propertyValue.ChangeHandlers == null)
				return;

			var newValue = GetValue(property);
			if (!previousValue.Equals(newValue))
				propertyValue.ChangeHandlers(this, property, newValue);
		}

		/// <summary>
		///   Gets the value of the dependency property.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property whose value should be returned.</param>
		public T GetValue<T>(DependencyProperty<T> property)
		{
			Assert.ArgumentNotNull(property);

			var value = _propertyStore.GetValue(property, addIfNotFound: false);
			if (value == null || !value.HasEffectiveValue)
				return property.DefaultValue;

			return value.EffectiveValue;
		}

		/// <summary>
		///   Adds the change handler to the dependency property's changed event.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property the change handler should be added to.</param>
		/// <param name="changeHandler">The change handler that should be added.</param>
		public void AddChangeHandler<T>(DependencyProperty<T> property, DependencyPropertyChangedHandler<T> changeHandler)
		{
			Assert.ArgumentNotNull(property);
			Assert.ArgumentNotNull(changeHandler);

			_propertyStore.GetValue(property, addIfNotFound: true).ChangeHandlers += changeHandler;
		}

		/// <summary>
		///   Removes the change handler from the dependency property's changed event.
		/// </summary>
		/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
		/// <param name="property">The dependency property the change handler should be removed from.</param>
		/// <param name="changeHandler">The change handler that should be removed.</param>
		public void RemoveChangeHandler<T>(DependencyProperty<T> property, DependencyPropertyChangedHandler<T> changeHandler)
		{
			Assert.ArgumentNotNull(property);
			Assert.ArgumentNotNull(changeHandler);

			_propertyStore.GetValue(property, addIfNotFound: false).ChangeHandlers -= changeHandler;
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
	}
}