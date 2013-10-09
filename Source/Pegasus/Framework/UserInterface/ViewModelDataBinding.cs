namespace Pegasus.Framework.UserInterface
{
	using System;

	/// <summary>
	///   Binds a target dependency object/dependency property pair to an UI element's view model and a path selector.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	/// <remarks>
	///   Data bindings to the view model have to be treated differently from other data bindings, unfortunately: As the
	///   view model is untyped, the compiled expression must be regenerated every time the view model type changes.
	///   The current implementation regenerates the expression every time the view model instance changes, though.
	/// </remarks>
	internal sealed class ViewModelDataBinding<T> : DataBinding<T>
	{
		/// <summary>
		///   The UI element that is used as the source object of the binding.
		/// </summary>
		private readonly UIElement _uiElement;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="uiElement">The UI element that should be used as the source object of the binding.</param>
		/// <param name="path">The property path that should be evaluated on the source object to get the source value.</param>
		public ViewModelDataBinding(UIElement uiElement, string path)
			: base(uiElement.ViewModel, path)
		{
			_uiElement = uiElement;
		}

		/// <summary>
		///   Initializes the binding.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
			_uiElement.AddChangedHandler(UIElement.ViewModelProperty, OnViewModelChanged);
		}

		/// <summary>
		///   Reinitializes the binding after the view model has been changed.
		/// </summary>
		private void OnViewModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs<ViewModel> args)
		{
			_sourceObject = _uiElement.ViewModel;
			Initialize();
		}
	}
}