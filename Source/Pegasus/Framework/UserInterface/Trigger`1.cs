using System;

namespace Pegasus.Framework.UserInterface
{
	using System.Collections.Generic;

	/// <summary>
	///   Applies property values when a certain condition is met.
	/// </summary>
	public class Trigger<T> : Trigger
	{
		/// <summary>
		///   The dependency property whose value is checked to determine whether the trigger is triggered.
		/// </summary>
		private readonly DependencyProperty<T> _property;

		/// <summary>
		///   The value that is compared with the trigger property.
		/// </summary>
		private readonly T _value;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="property">
		///   The dependency property whose value should be checked to determine whether the trigger is triggered.
		/// </param>
		/// <param name="value">The value that should be compared with the trigger property.</param>
		public Trigger(DependencyProperty<T> property, T value)
		{
			Assert.ArgumentNotNull(property);

			_property = property;
			_value = value;
		}

		/// <summary>
		///   Applies the trigger to the given UI element.
		/// </summary>
		/// <param name="element">The UI element the trigger should be applied to.</param>
		internal override void Apply(UIElement element)
		{
			Assert.ArgumentNotNull(element);

			element.AddChangedHandler(_property, PropertyChanged);
			ApplyIfTriggered(element, element.GetValue(_property), unsetIfNotTriggered: false);
		}

		/// <summary>
		///   Unsets the all triggered values from the given UI element.
		/// </summary>
		/// <param name="element">The UI element the style should be unset from.</param>
		internal override void Unset(UIElement element)
		{
			Assert.ArgumentNotNull(element);

			element.RemoveChangedHandler(_property, PropertyChanged);
			UnsetSetters(element);
		}

		/// <summary>
		///   Reapplys all setters of the trigger if it is currently triggered.
		/// </summary>
		/// <param name="element">The UI element the triggered setters should be reapplied to.</param>
		internal override void Reapply(UIElement element)
		{
			Assert.NotNull(element, "Expected an UI element.");
			ApplyIfTriggered(element, element.GetValue(_property), unsetIfNotTriggered: false);
		}

		/// <summary>
		///   Invoked when the trigger property has changed. Compares the new value with the trigger value and applies the setters
		///   if the trigger is triggered.
		/// </summary>
		private void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<T> args)
		{
			var uiElement = obj as UIElement;
			Assert.NotNull(uiElement, "Expected an UI element.");

			ApplyIfTriggered(uiElement, args.NewValue, unsetIfNotTriggered: true);
			RaiseTriggerStateChanged(uiElement);
		}

		/// <summary>
		///   Applies the setters of the trigger to the given UI element.
		/// </summary>
		/// <param name="element">The UI element the trigger should be applied to.</param>
		private void ApplySetters(UIElement element)
		{
			foreach (var setter in _setters)
				setter.ApplyTriggered(element);
		}

		/// <summary>
		///   Unsets the setters of the trigger from the given UI element.
		/// </summary>
		/// <param name="element">The UI element the trigger should be unset from.</param>
		private void UnsetSetters(UIElement element)
		{
			foreach (var setter in _setters)
				setter.UnsetTriggered(element);
		}

		/// <summary>
		///   Applies the setters of the trigger if the trigger is currently triggered; unsets them otherwise.
		/// </summary>
		/// <param name="element">The UI element the trigger should be applied to.</param>
		/// <param name="value">The current value of the triggering dependency property.</param>
		/// <param name="unsetIfNotTriggered">Indicates whether the value should be unset if it is not triggered.</param>
		private void ApplyIfTriggered(UIElement element, T value, bool unsetIfNotTriggered)
		{
			var isTriggered = EqualityComparer<T>.Default.Equals(value, _value);

			if (isTriggered)
				ApplySetters(element);
			else if (unsetIfNotTriggered)
				UnsetSetters(element);
		}
	}
}