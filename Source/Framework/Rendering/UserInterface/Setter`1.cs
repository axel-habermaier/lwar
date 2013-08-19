using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Applies a typed property value.
	/// </summary>
	public class Setter<T> : Setter
	{
		/// <summary>
		///   The dependency property that is set by the setter.
		/// </summary>
		private readonly DependencyProperty<T> _dependencyProperty;

		/// <summary>
		///   The value that the dependency property is set to.
		/// </summary>
		private readonly T _value;

		/// <summary>
		///   Initializes a new instance.
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
		///   Applies the setter's value to the given dependency object and seals the setter.
		/// </summary>
		/// <param name="obj">The dependency object the setter should be applied to.</param>
		internal override void Apply(DependencyObject obj)
		{
			Assert.ArgumentNotNull(obj);
			obj.SetStyleValue(_dependencyProperty, _value);
		}
	}
}