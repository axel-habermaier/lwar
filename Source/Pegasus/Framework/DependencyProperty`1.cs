using System;

namespace Pegasus.Framework
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
		/// <param name="inherits">Indicates whether the value of the dependency property is inheritable.</param>
		public DependencyProperty(T defaultValue = default(T), bool inherits = false)
			: base(inherits)
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

		/// <summary>
		///   Adds an untyped changed handler to the dependency property for the given dependency object.
		/// </summary>
		/// <param name="obj">The dependency object the handler should be added to.</param>
		/// <param name="handler">The handler that should be added.</param>
		internal override Delegate AddUntypedChangeHandler(DependencyObject obj, DependencyPropertyChangedHandler handler)
		{
			Assert.ArgumentNotNull(obj);
			Assert.ArgumentNotNull(handler);

			DependencyPropertyChangedHandler<T> typedHandler = (o, e) => handler(o, e.Property);
			obj.AddChangedHandler(this, typedHandler);

			return typedHandler;
		}

		/// <summary>
		///   Removes an untyped changed handler from the dependency property for the given dependency object.
		/// </summary>
		/// <param name="obj">The dependency object the handler should be removed from.</param>
		/// <param name="handler">The handler that should be removed.</param>
		internal override void RemoveUntypedChangeHandler(DependencyObject obj, Delegate handler)
		{
			Assert.ArgumentNotNull(obj);
			Assert.ArgumentNotNull(handler);

			var typedHandler = (DependencyPropertyChangedHandler<T>)handler;
			obj.RemoveChangedHandler(this, typedHandler);
		}

		/// <summary>
		///   Gets the untyped value of the dependency property for the given dependency object.
		/// </summary>
		/// <param name="obj">The dependency object the value should be returned for.</param>
		internal override object GetValue(DependencyObject obj)
		{
			Assert.ArgumentNotNull(obj);
			return obj.GetValue(this);
		}
	}
}