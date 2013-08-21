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
		/// <param name="obj">The UI element the trigger should be applied to.</param>
		internal override void Apply(UIElement obj)
		{
			Assert.ArgumentNotNull(obj);
			obj.AddChangedHandler(_property, PropertyChanged);
		}

		/// <summary>
		///   Unsets the trigger from the given UI element.
		/// </summary>
		/// <param name="obj">The UI element the trigger should be unset from.</param>
		internal override void Unset(UIElement obj)
		{
			Assert.ArgumentNotNull(obj);

			obj.RemoveChangedHandler(_property, PropertyChanged);
			UnsetSetters(obj);
		}

		/// <summary>
		///   Invoked when the trigger property has changed. Compares the new value with the trigger value and applies the setters
		///   if the trigger is triggered.
		/// </summary>
		private void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<T> args)
		{
			var uiElement = obj as UIElement;
			Assert.NotNull(uiElement, "Expected an UI element.");

			var value = obj.GetValue(args.Property);
			if (EqualityComparer<T>.Default.Equals(value, _value))
				ApplySetters(uiElement);
			else
				UnsetSetters(uiElement);
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
	}
}