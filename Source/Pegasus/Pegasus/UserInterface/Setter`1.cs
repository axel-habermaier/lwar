namespace Pegasus.UserInterface
{
	using System;
	using Utilities;

	/// <summary>
	///     Applies a typed property value.
	/// </summary>
	public class Setter<T> : Setter
	{
		/// <summary>
		///     The dependency property that is set by the setter.
		/// </summary>
		private readonly DependencyProperty<T> _dependencyProperty;

		/// <summary>
		///     The value that the dependency property is set to.
		/// </summary>
		private readonly T _value;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="dependencyProperty"> The dependency property that should be set by the setter.</param>
		/// <param name="value">he value that the dependency property should be set to.</param>
		public Setter(DependencyProperty<T> dependencyProperty, T value)
		{
			Assert.ArgumentNotNull(dependencyProperty);

			_dependencyProperty = dependencyProperty;
			_value = value;
		}

		/// <summary>
		///     Applies the setter's value to the given UI element.
		/// </summary>
		/// <param name="element">The UI element the setter's value should be applied to.</param>
		internal override void Apply(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			element.SetStyleValue(_dependencyProperty, _value);
		}

		/// <summary>
		///     Applies the setter's value to the given UI element when the setter is applied as the result of a trigger being
		///     triggered.
		/// </summary>
		/// <param name="element">The UI element the setter's value should be applied to.</param>
		internal override void ApplyTriggered(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			element.SetStyleTriggeredValue(_dependencyProperty, _value);
		}

		/// <summary>
		///     Unsets the setter's value from the given UI element when the setter is no longer applied as the result of a
		///     trigger being triggered.
		/// </summary>
		/// <param name="element">The UI element the setter's value should be applied to.</param>
		internal override void UnsetTriggered(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			element.UnsetStyleTriggeredValue(_dependencyProperty);
		}

		/// <summary>
		///     Unsets the setter's value from the given UI element.
		/// </summary>
		/// <param name="element">The UI element the setter's value should be unset from.</param>
		internal override void Unset(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			element.UnsetStyleValue(_dependencyProperty);
		}
	}
}