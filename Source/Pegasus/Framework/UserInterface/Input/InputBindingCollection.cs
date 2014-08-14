namespace Pegasus.Framework.UserInterface.Input
{
	using System;

	/// <summary>
	///     Represents a collection of input bindings of an UI element.
	/// </summary>
	public class InputBindingCollection : CustomCollection<InputBinding>
	{
		/// <summary>
		///     The UI element the input bindings are associated with.
		/// </summary>
		private readonly UIElement _uiElement;

		/// <summary>
		///     Indicates whether the bindings are currently active.
		/// </summary>
		private bool _active;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="uiElement">The UI element the input bindings are associated with.</param>
		public InputBindingCollection(UIElement uiElement)
		{
			Assert.ArgumentNotNull(uiElement);
			_uiElement = uiElement;
		}

		/// <summary>
		///     Gets or sets a value indicating whether the bindings are currently active.
		/// </summary>
		internal bool Active
		{
			get { return _active; }
			set
			{
				if (_active == value)
					return;

				_active = value;

				if (_active)
					OnActivated();
				else
					OnDeactivated();
			}
		}

		/// <summary>
		///     Invoked when the bindings have been activated.
		/// </summary>
		private void OnActivated()
		{
			var dataContext = _uiElement.DataContext;
			foreach (var binding in this)
				binding.BindToDataContext(dataContext);
		}

		/// <summary>
		///     Invoked when the bindings have been deactivated.
		/// </summary>
		private void OnDeactivated()
		{
			foreach (var binding in this)
				binding.BindToDataContext(null);
		}

		/// <summary>
		///     Inserts an element into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which the item that should be inserted.</param>
		/// <param name="item">The item that should be inserted.</param>
		protected override void InsertItem(int index, InputBinding item)
		{
			base.InsertItem(index, item);

			item.Seal();
			if (_active)
				item.BindToDataContext(_uiElement.DataContext);
		}

		/// <summary>
		///     Replaces the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element that should be replaced.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		protected override void SetItem(int index, InputBinding item)
		{
			base.SetItem(index, item);

			item.Seal();
			if (_active)
				item.BindToDataContext(_uiElement.DataContext);
		}

		/// <summary>
		///     Handles the given event, checking whether any input bindings are triggered.
		/// </summary>
		/// <param name="args">The arguments of the event that should be handled.</param>
		internal void HandleEvent(RoutedEventArgs args)
		{
			Assert.ArgumentNotNull(args);

			if (!_active)
				return;

			foreach (var binding in this)
				binding.HandleEvent(args);
		}

		/// <summary>
		///     Updates the bound target methods of all input bindings when a new data context has been set on the associated UI
		///     element.
		/// </summary>
		/// <param name="dataContext">The new data context that should be bound to.</param>
		internal void BindToDataContext(object dataContext)
		{
			if (!_active)
				return;

			foreach (var binding in this)
				binding.BindToDataContext(dataContext);
		}
	}
}