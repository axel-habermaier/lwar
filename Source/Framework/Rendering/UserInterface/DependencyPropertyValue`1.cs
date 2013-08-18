using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Stores the values that have been set on a dependency property, managing their precedences. The effective value of the
	///   property is calculated based on the values that have been set. For instance, if the base value has been set by a
	///   style  and a style trigger changes the property's value, the effective value also changes. On the other hand, if the
	///   base value has been set directly and a style trigger changes the property's value, the effective value remains
	///   unchanged.
	/// </summary>
	/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
	internal class DependencyPropertyValue<T> : DependencyPropertyValue
	{
		/// <summary>
		///   The property's value that has been set by the animation system. The animated value has the highest precedence.
		/// </summary>
		private T _animatedValue;

		/// <summary>
		///   The base value of the property that has either been set directly, through data binding, by a a style, etc.
		/// </summary>
		private T _baseValue;

		/// <summary>
		///   The sources of the property's values.
		/// </summary>
		private ValueSources _sources;

		/// <summary>
		///   The value of the property that has been set by a trigger.
		/// </summary>
		private T _triggeredValue;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="propertyIndex">The index of the dependency property the value belongs to.</param>
		public DependencyPropertyValue(int propertyIndex)
			: base(propertyIndex)
		{
		}

		/// <summary>
		///   Gets the property's effective value.
		/// </summary>
		public T EffectiveValue
		{
			get
			{
				if ((_sources & ValueSources.Animation) == ValueSources.Animation)
					return _animatedValue;

				if ((_sources & ValueSources.Local) == ValueSources.Local)
					return _baseValue;

				if ((_sources & ValueSources.StyleTrigger) == ValueSources.StyleTrigger ||
					(_sources & ValueSources.TemplateTrigger) == ValueSources.TemplateTrigger)
					return _triggeredValue;

				return _baseValue;
			}
		}

		/// <summary>
		///   Raised when the effective value of the property has changed.
		/// </summary>
		public event Action<DependencyProperty<T>, T> Changed;

		/// <summary>
		///   Sets the property's value to the value provided directly or through a data binding.
		/// </summary>
		/// <param name="property">The dependency property the value should be set for.</param>
		/// <param name="value">The value that should be set.</param>
		public void SetLocalValue(DependencyProperty<T> property, T value)
		{
			Assert.ArgumentNotNull(property);

			var previousValue = EffectiveValue;

			_baseValue = value;
			_sources |= ValueSources.Local;
			_sources &= ~ValueSources.Style;

			RaiseChangeEvent(property, previousValue);
		}

		/// <summary>
		///   Sets the property's value to the value provided by a style.
		/// </summary>
		/// <param name="property">The dependency property the value should be set for.</param>
		/// <param name="value">The value that should be set.</param>
		public void SetStyleValue(DependencyProperty<T> property, T value)
		{
			Assert.ArgumentNotNull(property);

			if ((_sources & ValueSources.Local) == ValueSources.Local)
				return;

			var previousValue = EffectiveValue;

			_baseValue = value;
			_sources |= ValueSources.Style;

			RaiseChangeEvent(property, previousValue);
		}

		/// <summary>
		///   Sets the property's value to the value provided by a style trigger.
		/// </summary>
		/// <param name="property">The dependency property the value should be set for.</param>
		/// <param name="value">The value that should be set.</param>
		public void SetStyleTriggeredValue(DependencyProperty<T> property, T value)
		{
			Assert.ArgumentNotNull(property);

			if ((_sources & ValueSources.TemplateTrigger) == ValueSources.TemplateTrigger)
				return;

			var previousValue = EffectiveValue;

			_triggeredValue = value;
			_sources |= ValueSources.StyleTrigger;

			RaiseChangeEvent(property, previousValue);
		}

		/// <summary>
		///   Unsets the property's value that has been set by a style trigger.
		/// </summary>
		/// <param name="property">The dependency property the value should be unset for.</param>
		public void UnsetStyleTriggeredValue(DependencyProperty<T> property)
		{
			Assert.ArgumentNotNull(property);

			if ((_sources & ValueSources.StyleTrigger) != ValueSources.StyleTrigger)
				return;

			var previousValue = EffectiveValue;

			_triggeredValue = default(T);
			_sources &= ~ValueSources.StyleTrigger;

			RaiseChangeEvent(property, previousValue);
		}

		/// <summary>
		///   Sets the property's animated value.
		/// </summary>
		/// <param name="property">The dependency property the value should be set for.</param>
		/// <param name="value">The value that should be set.</param>
		public void SetAnimatedValue(DependencyProperty<T> property, T value)
		{
			Assert.ArgumentNotNull(property);

			var previousValue = EffectiveValue;

			_animatedValue = value;
			_sources |= ValueSources.Animation;

			RaiseChangeEvent(property, previousValue);
		}

		/// <summary>
		///   Unsets the property's animated value.
		/// </summary>
		/// <param name="property">The dependency property the value should be unset for.</param>
		public void UnsetAnimatedValue(DependencyProperty<T> property)
		{
			Assert.ArgumentNotNull(property);

			var previousValue = EffectiveValue;

			_animatedValue = default(T);
			_sources &= ~ValueSources.Animation;

			RaiseChangeEvent(property, previousValue);
		}

		/// <summary>
		///   Raises the changed event if the property's effective value has changed.
		/// </summary>
		/// <param name="property">The dependency property the change event should be raised for.</param>
		/// <param name="previousValue">The previous effective value of the property.</param>
		private void RaiseChangeEvent(DependencyProperty<T> property, T previousValue)
		{
			var value = EffectiveValue;

			if (value.Equals(previousValue) && Changed != null)
				Changed(property, value);
		}

		/// <summary>
		///   Identifies the sources of the property's values.
		/// </summary>
		[Flags]
		private enum ValueSources : byte
		{
			/// <summary>
			///   Indicates that the base value has been set directly or through a data binding.
			/// </summary>
			Local = 1,

			/// <summary>
			///   Indicates that the base value has been set by a style.
			/// </summary>
			Style = 2,

			/// <summary>
			///   Indicates that the triggered value has been set by a style trigger.
			/// </summary>
			StyleTrigger = 4,

			/// <summary>
			///   Indicates that the triggered value has been set by a template trigger.
			/// </summary>
			TemplateTrigger = 8,

			/// <summary>
			///   Indicates that the animated value has been set.
			/// </summary>
			Animation = 16
		}
	}
}