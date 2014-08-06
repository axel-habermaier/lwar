namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using Controls;
	using Platform.Memory;

	/// <summary>
	///     A base class for stacked view models.
	/// </summary>
	public abstract class StackedViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The view model's child in the view model stack.
		/// </summary>
		private StackedViewModel _child;

		/// <summary>
		///     A value indicating whether the view model is currently active.
		/// </summary>
		private bool _isActive;

		/// <summary>
		///     The view associated with the view model.
		/// </summary>
		private UserControl _view;

		/// <summary>
		///     Gets the view model's parent view model in the view model stack. The parent is null for the root view model as well as
		///     for view models that are not in the stack.
		/// </summary>
		protected StackedViewModel Parent { get; private set; }

		/// <summary>
		///     Gets the view model's root view model in the view model stack. The root is null for the root view model as well as
		///     for view models that are not in the stack.
		/// </summary>
		protected StackedViewModel Root
		{
			get
			{
				if (this is RootViewModel)
					return this;

				Assert.NotNull(Parent, "The view model has no parent.");
				return Parent.Root;
			}
		}

		/// <summary>
		///     Gets or sets the view model's child in the view model stack.
		/// </summary>
		public StackedViewModel Child
		{
			get
			{
				Assert.NotDisposed(this);
				return _child;
			}
			set
			{
				Assert.NotDisposed(this);

				if (_child != null)
				{
					_child.Parent = null;

					if (_isActive)
						_child.Deactivate();
				}

				_child = value;

				if (_child != null)
				{
					_child.Parent = this;

					if (_isActive)
						_child.Activate();
				}
			}
		}

		/// <summary>
		///     Gets or sets the view associated with the view model.
		/// </summary>
		protected UserControl View
		{
			get
			{
				Assert.NotDisposed(this);
				return _view;
			}
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.That(!_isActive, "The View property cannot be changed when the view model is active.");
				Assert.NotDisposed(this);

				_view = value;

				if (_view != null)
					_view.DataContext = this;
			}
		}

		/// <summary>
		///     Creates a view model that can be used as the root of the view model stack.
		/// </summary>
		public static StackedViewModel CreateRoot()
		{
			return new RootViewModel();
		}

		/// <summary>
		///     Activates the view model, presenting its content and view on the UI.
		/// </summary>
		public void Activate()
		{
			Assert.That(!_isActive, "The view model is already active.");
			Assert.That(_view != null || GetType() == typeof(RootViewModel), "No view has been set for the view model.");
			Assert.NotDisposed(this);

			OnActivated();

			_isActive = true;

			if (_view != null)
				Application.Current.Window.LayoutRoot.Add(_view);

			if (_child != null)
				_child.Activate();
		}

		/// <summary>
		///     Deactivates the view model, removing its content and view from the UI.
		/// </summary>
		public void Deactivate()
		{
			Assert.That(_isActive, "The view model is not active.");
			Assert.NotDisposed(this);

			if (_view != null)
				Application.Current.Window.LayoutRoot.Remove(_view);

			OnDeactivated();
			_isActive = false;

			if (_child != null)
				_child.Deactivate();
		}

		/// <summary>
		///     Updates the view model's state.
		/// </summary>
		public void Update()
		{
			Assert.NotDisposed(this);

			if (_child != null)
				_child.Update();

			OnUpdate();
		}

		/// <summary>
		///     Activates the view model, presenting its content and view on the UI.
		/// </summary>
		protected virtual void OnActivated()
		{
		}

		/// <summary>
		///     Deactivates the view model, removing its content and view from the UI.
		/// </summary>
		protected virtual void OnDeactivated()
		{
		}

		/// <summary>
		///     Updates the view model's state.
		/// </summary>
		protected virtual void OnUpdate()
		{
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override sealed void OnDisposing()
		{
			_child.SafeDispose();
			_child = null;

			if (_isActive)
				Deactivate();
		}

		/// <summary>
		///     Replaces the current view model's child with the given one, disposing the current child view model.
		/// </summary>
		/// <param name="viewModel">The new view model the current child view model should be replaced with.</param>
		public void ReplaceChild(StackedViewModel viewModel)
		{
			Assert.NotDisposed(this);

			var currentChild = Child;
			Child = viewModel;

			if (currentChild != null)
				currentChild.SafeDispose();
		}

		/// <summary>
		///     Represents the non-visible root of the view model stack.
		/// </summary>
		private class RootViewModel : StackedViewModel
		{
		}
	}
}