namespace Pegasus.Framework.UserInterface
{
	using System;
	using Controls;

	/// <summary>
	///   Binds a target UI element/dependency property pair to a dependency property of the target element's templated parent.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	/// <remarks>This class assumes that the templated parent of a UI element never changes.</remarks>
	internal sealed class TemplateBinding<T> : Binding<T>
	{
		/// <summary>
		///   The source object that is the templated parent of the target object.
		/// </summary>
		private readonly Control _sourceObject;

		/// <summary>
		///   The dependency property defined by the source object from which the bound value is retrieved.
		/// </summary>
		private readonly DependencyProperty<T> _sourceProperty;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="sourceObject">The source object that is the templated parent of the target object.</param>
		/// <param name="sourceProperty">
		///   The dependency property defined by the source object from which the bound value is retrieved.
		/// </param>
		internal TemplateBinding(Control sourceObject, DependencyProperty<T> sourceProperty)
		{
			Assert.ArgumentNotNull(sourceObject);
			Assert.ArgumentNotNull(sourceProperty);

			_sourceObject = sourceObject;
			_sourceProperty = sourceProperty;
		}

		/// <summary>
		///   Initializes the binding.
		/// </summary>
		protected override void Initialize()
		{
			_targetObject.SetValue(_targetProperty, _sourceObject.GetValue(_sourceProperty));
			_sourceObject.AddChangedHandler(_sourceProperty, OnPropertyChanged);
		}

		/// <summary>
		///   Updates the value of the target dependency property after the value of the source dependency property has been
		///   changed.
		/// </summary>
		private void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<T> args)
		{
			_targetObject.SetValue(_targetProperty, args.NewValue);
		}
	}
}