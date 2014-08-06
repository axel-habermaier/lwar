namespace Pegasus.Framework.UserInterface
{
	using System;
	using System.Diagnostics;
	using System.Reflection;

	/// <summary>
	///     Represents a strongly-typed property that has multiple sources (such as data bindings, style setters, animation,
	///     etc.).
	/// </summary>
	/// <typeparam name="T">The type of the value stored by the dependency property.</typeparam>
	public sealed class DependencyProperty<T> : DependencyProperty
	{
		/// <summary>
		///     The callback that is used to validate whether the given value is a valid value for the dependency property.
		/// </summary>
		private readonly DependencyPropertyValidationCallback<T> _validationCallback;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="defaultValue">The default value of the dependency property.</param>
		/// <param name="validationCallback">
		///     The callback that should be used to validate whether the given value is a valid value
		///     for the dependency property.
		/// </param>
		/// <param name="inherits">Indicates whether the value of the dependency property is inheritable.</param>
		/// <param name="affectsMeasure">
		///     Indicates that changes to the value of the dependency property potentially affect the measure pass of the layout
		///     engine. Implies affectsArrange and affectsRender.
		/// </param>
		/// <param name="affectsArrange">
		///     Indicates that changes to the value of the dependency property potentially affect the arrange pass of the layout
		///     engine. Implies affectsRender.
		/// </param>
		/// <param name="affectsRender">
		///     Indicates that changes to the value of the dependency property potentially requires a redraw, without affecting
		///     measurement or arrangement.
		/// </param>
		/// <param name="prohibitsAnimations"> Indicates that the dependency property cannot be animated.</param>
		/// <param name="prohibitsDataBinding">Indicates that the dependency property does not support data binding.</param>
		/// <param name="isReadOnly">
		///     Indicates that the dependency property is read-only and can only be set internally by the
		///     framework.
		/// </param>
		/// <param name="defaultBindingMode">The default data binding mode of the dependency property.</param>
		public DependencyProperty(T defaultValue = default(T),
								  DependencyPropertyValidationCallback<T> validationCallback = null,
								  bool inherits = false,
								  bool affectsMeasure = false,
								  bool affectsArrange = false,
								  bool affectsRender = false,
								  bool prohibitsAnimations = false,
								  bool prohibitsDataBinding = false,
								  bool isReadOnly = false,
								  BindingMode defaultBindingMode = BindingMode.OneWay)
			: base(inherits, affectsMeasure, affectsArrange, affectsRender, prohibitsAnimations,
				prohibitsDataBinding, isReadOnly, defaultBindingMode)
		{
			DefaultValue = defaultValue;
			_validationCallback = validationCallback;
		}

		/// <summary>
		///     Gets the default value of the dependency property.
		/// </summary>
		public T DefaultValue { get; private set; }

		/// <summary>
		///     Gets the type of the value stored by the dependency property.
		/// </summary>
		internal override Type ValueType
		{
			get { return typeof(T); }
		}

		/// <summary>
		///     Raised when the value of the dependency property has been changed on any dependency object.
		/// </summary>
		internal event DependencyPropertyChangedHandler<T> Changed;

		/// <summary>
		///     In debug builds, checks whether the given value is a valid value for the dependency property.
		/// </summary>
		/// <param name="value">The value that should be checked.</param>
		[Conditional("DEBUG")]
		internal void ValidateValue(T value)
		{
			// For enumeration types, check if the given literal is defined
			if (typeof(T).GetTypeInfo().IsEnum)
				Assert.That(Enum.IsDefined(typeof(T), value), "The given value is not defined by the enumeration.");

			Assert.That(_validationCallback == null || _validationCallback(value), "Attempted to set an invalid value.");
		}

		/// <summary>
		///     Adds an untyped changed handler to the dependency property for the given dependency object.
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
		///     Removes an untyped changed handler from the dependency property for the given dependency object.
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
		///     Gets the untyped value of the dependency property for the given dependency object.
		/// </summary>
		/// <param name="obj">The dependency object the value should be returned for.</param>
		internal override object GetValue(DependencyObject obj)
		{
			Assert.ArgumentNotNull(obj);
			return obj.GetValue(this);
		}

		/// <summary>
		///     Copies the dependency property's inherited value from source to target.
		/// </summary>
		/// <param name="source">The dependency object the value should be retrieved from.</param>
		/// <param name="target">The dependency object the value should be set for.</param>
		internal override void CopyInheritedValue(DependencyObject source, DependencyObject target)
		{
			Assert.ArgumentNotNull(source);
			Assert.ArgumentNotNull(target);
			Assert.That(Inherits, "The dependency property does not support value inheritance.");

			target.SetInheritedValue(this, source.GetValue(this));
		}

		/// <summary>
		///     Unsets the inherited value of the given dependency object.
		/// </summary>
		/// <param name="obj">The dependency object whose inherited value should be unset.</param>
		internal override void UnsetInheritedValue(DependencyObject obj)
		{
			Assert.ArgumentNotNull(obj);
			Assert.That(Inherits, "The dependency property does not support value inheritance.");

			obj.UnsetInheritedValue(this);
		}

		/// <summary>
		///     Raises the Changed event.
		/// </summary>
		internal void OnValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs<T> changedEventArgs)
		{
			if (Changed != null)
				Changed(dependencyObject, changedEventArgs);
		}
	}
}