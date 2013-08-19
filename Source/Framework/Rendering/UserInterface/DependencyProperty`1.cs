using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	/// <summary>
	///   Represents a strongly-typed property that has multiple sources (such as data bindings, style setters, animation,
	///   etc.).
	/// </summary>
	/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
	public class DependencyProperty<T> : DependencyProperty
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="defaultValue">The default value of the dependency property.</param>
		public DependencyProperty(T defaultValue = default(T))
		{
			DefaultValue = defaultValue;
		}

		/// <summary>
		///   Gets the default value of the dependency property.
		/// </summary>
		public T DefaultValue { get; private set; }

		/// <summary>
		///   Gets the type of the value stored by the dependency property.
		/// </summary>
		internal override Type ValueType
		{
			get { return typeof(T); }
		}
	}
}