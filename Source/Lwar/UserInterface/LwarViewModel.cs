namespace Lwar.UserInterface
{
	using System;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Platform.Memory;

	/// <summary>
	///     A base class for Lwar specific view models.
	/// </summary>
	public abstract class LwarViewModel : ViewModel
	{
		/// <summary>
		///     The view model's child in the view model stack.
		/// </summary>
		private LwarViewModel _child;

		/// <summary>
		///     A value indicating whether the view model is currently active.
		/// </summary>
		private bool _isActive;

		/// <summary>
		///     A value indicating whether the view model's view is modal.
		/// </summary>
		private bool _isModal;

		/// <summary>
		///     The view associated with the view model.
		/// </summary>
		private UserControl _view;

		/// <summary>
		///     Gets the view model's parent view model in the view model stack. The parent is null for the root view model as well as
		///     for view models that are not in the stack.
		/// </summary>
		protected LwarViewModel Parent { get; private set; }

		/// <summary>
		///     Gets or sets the view model's child in the view model stack.
		/// </summary>
		public LwarViewModel Child
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
		///     Gets or sets a value indicating whether the view model's view is modal.
		/// </summary>
		protected bool IsModal
		{
			get
			{
				Assert.NotDisposed(this);
				return _isModal;
			}
			set
			{
				Assert.NotDisposed(this);
				Assert.That(!_isActive, "The IsModal property cannot be changed when the view model is active.");

				_isModal = value;
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
					_view.ViewModel = this;
			}
		}

		/// <summary>
		///     Activates the view model, presenting its content and view on the UI.
		/// </summary>
		public void Activate()
		{
			Assert.That(!_isActive, "The view model is already active.");
			Assert.That(_view != null || GetType() == typeof(RootViewModel), "No view has been set for the view model.");
			Assert.NotDisposed(this);

			if (_isModal && _view != null)
				Application.Current.Window.LayoutRoot.AddModal(_view);
			else if (_view != null)
				Application.Current.Window.LayoutRoot.Add(_view);

			OnActivated();
			_isActive = true;

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
		///     Replaces the current view model with the given one, optionally disposing the current view model.
		/// </summary>
		/// <param name="viewModel">The new view model the current one should be replaced with.</param>
		/// <param name="disposeSelf">Indicates whether the current view model should be disposed after it has been replaced.</param>
		protected void ReplaceSelf(LwarViewModel viewModel, bool disposeSelf)
		{
			Assert.That(Parent != null && Parent.Child == this, "The current view model cannot be replaced.");
			Assert.NotDisposed(this);

			Parent.Child = viewModel;

			if (disposeSelf)
				this.SafeDispose();
		}
	}
}