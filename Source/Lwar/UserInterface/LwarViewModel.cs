namespace Lwar.UserInterface
{
	using System;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface.Controls;

	/// <summary>
	///     A base class for Lwar specific view models.
	/// </summary>
	public abstract class LwarViewModel
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
			get { return _child; }
			set
			{
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
			get { return _isModal; }
			set
			{
				Assert.That(!_isActive, "The IsModal property cannot be changed when the view model is active.");
				_isModal = value;
			}
		}

		/// <summary>
		///     Gets or sets the view associated with the view model.
		/// </summary>
		protected UserControl View
		{
			get { return _view; }
			set
			{
				Assert.ArgumentNotNull(value);
				Assert.That(!_isActive, "The View property cannot be changed when the view model is active.");

				_view = value;
			}
		}

		/// <summary>
		///     Activates the view model, presenting its content and view on the UI.
		/// </summary>
		public void Activate()
		{
			Assert.That(!_isActive, "The view model is already active.");
			Assert.That(_view != null || GetType() == typeof(RootViewModel), "No view has been set for the view model.");

			if (_isModal && _view != null)
				Application.Current.Window.LayoutRoot.AddModal(_view);
			else if (_view != null)
				Application.Current.Window.LayoutRoot.Add(_view);

			_isActive = true;
			OnActivated();

			if (_child != null)
				_child.Activate();
		}

		/// <summary>
		///     Deactivates the view model, removing its content and view from the UI.
		/// </summary>
		public void Deactivate()
		{
			Assert.That(_isActive, "The view model is not active.");

			if (_isModal && _view != null)
				Application.Current.Window.LayoutRoot.RemoveModal(_view);
			else if (_view != null)
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
	}
}