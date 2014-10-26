namespace Pegasus.UserInterface
{
	using System;
	using Utilities;

	/// <summary>
	///     Stores the values that have been set on a dependency property, managing their precedences. The effective value of the
	///     property is calculated based on the values that have been set. For instance, if the base value has been set by a
	///     style  and a style trigger changes the property's value, the effective value also changes. On the other hand, if the
	///     base value has been set directly and a style trigger changes the property's value, the effective value remains
	///     unchanged.
	/// </summary>
	/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
	internal sealed class DependencyPropertyValue<T> : DependencyPropertyValue
	{
		/// <summary>
		///     The change handlers that are invoked when the effective value of the dependency property has changed.
		/// </summary>
		public DependencyPropertyChangedHandler<T> ChangedHandlers;

		/// <summary>
		///     The property's value that has been set by the animation system. The animated value has the highest precedence.
		/// </summary>
		private T _animatedValue;

		/// <summary>
		///     The base value of the property that has either been set directly, through data binding, by a a style, or via
		///     inheritance.
		/// </summary>
		private T _baseValue;

		/// <summary>
		///     The binding that determines the value of the property.
		/// </summary>
		private Binding<T> _binding;

		/// <summary>
		///     The value of the property that has been set by a trigger.
		/// </summary>
		private T _triggeredValue;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="property">The dependency property the value belongs to.</param>
		public DependencyPropertyValue(DependencyProperty<T> property)
			: base(property)
		{
		}

		/// <summary>
		///     Gets the property's effective value.
		/// </summary>
		public T EffectiveValue
		{
			get
			{
				Assert.That(HasEffectiveValue, "The property has no effective value.");

				if ((_sources & ValueSources.Animation) == ValueSources.Animation)
					return _animatedValue;

				if ((_sources & ValueSources.Local) == ValueSources.Local || (_sources & ValueSources.Binding) == ValueSources.Binding)
					return _baseValue;

				if ((_sources & ValueSources.StyleTrigger) == ValueSources.StyleTrigger ||
					(_sources & ValueSources.TemplateTrigger) == ValueSources.TemplateTrigger)
					return _triggeredValue;

				return _baseValue;
			}
		}

		/// <summary>
		///     Changes the activation state of the binding that is set on the dependency property, if any.
		/// </summary>
		/// <param name="activated">Indicates whether the binding should be activated.</param>
		public override void SetBindingActivationState(bool activated)
		{
			if (_binding != null)
				_binding.Active = activated;
		}

		/// <summary>
		///     Sets the property's value to the value provided directly.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		public void SetLocalValue(T value)
		{
			if (_binding != null && _binding.Deactivate(overwrittenByLocalValue: true))
			{
				_sources &= ~ValueSources.Binding;
				_binding = null;
			}

			_baseValue = value;

			_sources |= ValueSources.Local;
			_sources &= ~ValueSources.Style;
			_sources &= ~ValueSources.Inherited;
		}

		/// <summary>
		///     Sets the property's value to the value provided by a binding.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		public void SetBoundValue(T value)
		{
			Assert.NotNull(_binding, "No binding has been set for this property.");
			_baseValue = value;
		}

		/// <summary>
		///     Sets the property's value to the value provided by a style.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		public void SetStyleValue(T value)
		{
			if ((_sources & ValueSources.Local) == ValueSources.Local)
				return;

			_baseValue = value;
			_sources |= ValueSources.Style;
			_sources &= ~ValueSources.Inherited;
		}

		/// <summary>
		///     Sets the property's value to the given inherited value.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		public void SetInheritedValue(T value)
		{
			if ((_sources & ValueSources.Local) == ValueSources.Local || (_sources & ValueSources.Style) == ValueSources.Style)
				return;

			_baseValue = value;
			_sources |= ValueSources.Inherited;
		}

		/// <summary>
		///     Sets the property's value to the value provided by a style trigger.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		public void SetStyleTriggeredValue(T value)
		{
			if ((_sources & ValueSources.TemplateTrigger) == ValueSources.TemplateTrigger)
				return;

			_triggeredValue = value;
			_sources |= ValueSources.StyleTrigger;
		}

		/// <summary>
		///     Unsets the property's value that has been set by a style.
		/// </summary>
		public void UnsetStyleValue()
		{
			if ((_sources & ValueSources.Style) != ValueSources.Style)
				return;

			_baseValue = default(T);
			_sources &= ~ValueSources.Style;
		}

		/// <summary>
		///     Unsets the property's inherited value.
		/// </summary>
		public void UnsetInheritedValue()
		{
			if ((_sources & ValueSources.Inherited) != ValueSources.Inherited)
				return;

			_baseValue = default(T);
			_sources &= ~ValueSources.Inherited;
		}

		/// <summary>
		///     Unsets the property's value that has been set by a style trigger.
		/// </summary>
		public void UnsetStyleTriggeredValue()
		{
			if ((_sources & ValueSources.StyleTrigger) != ValueSources.StyleTrigger)
				return;

			_triggeredValue = default(T);
			_sources &= ~ValueSources.StyleTrigger;
		}

		/// <summary>
		///     Sets the property's animated value.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		public void SetAnimatedValue(T value)
		{
			_animatedValue = value;
			_sources |= ValueSources.Animation;
		}

		/// <summary>
		///     Unsets the property's animated value.
		/// </summary>
		public void UnsetAnimatedValue()
		{
			_animatedValue = default(T);
			_sources &= ~ValueSources.Animation;
		}

		/// <summary>
		///     Sets the property's binding.
		/// </summary>
		/// <param name="binding">The binding that should be set.</param>
		public void SetBinding(Binding<T> binding)
		{
			Assert.ArgumentNotNull(binding);

			if (_binding != null)
				_binding.Deactivate();

			_binding = binding;
			_sources |= ValueSources.Binding;
		}

		/// <summary>
		///     Unsets the property's binding.
		/// </summary>
		public void UnsetBinding()
		{
			Assert.NotNull(_binding, "There is no binding set for this property.");

			_binding = null;
			_sources &= ~ValueSources.Binding;
		}
	}
}