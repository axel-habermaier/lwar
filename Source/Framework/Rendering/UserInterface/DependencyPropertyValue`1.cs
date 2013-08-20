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
		///   The change handlers that are invoked when the effective value of the dependency property has changed.
		/// </summary>
		public DependencyPropertyChangedHandler<T> ChangedHandlers;

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
		///   Gets a value indicating whether the property has an effective value.
		/// </summary>
		public bool HasEffectiveValue
		{
			get { return _sources != 0; }
		}

		/// <summary>
		///   Sets the property's value to the value provided directly or through a data binding.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		public void SetLocalValue(T value)
		{
			_baseValue = value;
			_sources |= ValueSources.Local;
			_sources &= ~ValueSources.Style;
		}

		/// <summary>
		///   Sets the property's value to the value provided by a style.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		public void SetStyleValue(T value)
		{
			if ((_sources & ValueSources.Local) == ValueSources.Local)
				return;

			_baseValue = value;
			_sources |= ValueSources.Style;
		}

		/// <summary>
		///   Sets the property's value to the value provided by a style trigger.
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
		///   Unsets the property's value that has been set by a style.
		/// </summary>
		public void UnsetStyleValue()
		{
			if ((_sources & ValueSources.Style) != ValueSources.Style)
				return;

			_baseValue = default(T);
			_sources &= ~ValueSources.Style;
		}

		/// <summary>
		///   Unsets the property's value that has been set by a style trigger.
		/// </summary>
		public void UnsetStyleTriggeredValue()
		{
			if ((_sources & ValueSources.StyleTrigger) != ValueSources.StyleTrigger)
				return;

			_triggeredValue = default(T);
			_sources &= ~ValueSources.StyleTrigger;
		}

		/// <summary>
		///   Sets the property's animated value.
		/// </summary>
		/// <param name="value">The value that should be set.</param>
		public void SetAnimatedValue(T value)
		{
			_animatedValue = value;
			_sources |= ValueSources.Animation;
		}

		/// <summary>
		///   Unsets the property's animated value.
		/// </summary>
		public void UnsetAnimatedValue()
		{
			_animatedValue = default(T);
			_sources &= ~ValueSources.Animation;
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