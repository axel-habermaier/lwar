namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using Views;

	/// <summary>
	///     Displays the in-game console.
	/// </summary>
	internal class ConsoleViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The console view inside the UI.
		/// </summary>
		private readonly ConsoleView _view = new ConsoleView();

		/// <summary>
		///     The window the console is displayed in.
		/// </summary>
		private readonly AppWindow _window;

		/// <summary>
		///     The desired height of the console overlay.
		/// </summary>
		private double _height;

		/// <summary>
		///     Indicates whether the console is visible.
		/// </summary>
		private bool _isVisible;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="window">The window in which the console should be shown.</param>
		public ConsoleViewModel(AppWindow window)
		{
			Assert.ArgumentNotNull(window);

			_window = window;
			_window.LayoutRoot.Add(_view);
			_view.DataContext = this;
		}

		/// <summary>
		///     Gets or sets a value indicating whether the console is visible.
		/// </summary>
		public bool IsVisible
		{
			get { return _isVisible; }
			set { ChangePropertyValue(ref _isVisible, value); }
		}

		/// <summary>
		///     Gets or sets the desired height of the console overlay.
		/// </summary>
		public double Height
		{
			get { return _height; }
			set { ChangePropertyValue(ref _height, value); }
		}

		/// <summary>
		///     Updates the state of the console.
		/// </summary>
		public void Update()
		{
			Height = _window.ActualHeight / 2;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_window.LayoutRoot.Remove(_view);
		}
	}
}