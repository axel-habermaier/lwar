namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using Controls;
	using Scripting;
	using Views;

	/// <summary>
	///     Displays a message box.
	/// </summary>
	internal class MessageBoxViewModel : NotifyPropertyChanged
	{
		/// <summary>
		///     The z-index used to display message boxes.
		/// </summary>
		private const int ZIndex = Int32.MaxValue - 100;

		/// <summary>
		///     The header of the message box.
		/// </summary>
		private string _header;

		/// <summary>
		///     The message of the message box.
		/// </summary>
		private string _message;

		/// <summary>
		///     The message box UI element.
		/// </summary>
		private MessageBoxView _view;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="header">The header of the message box.</param>
		/// <param name="message">The message that the message box should display.</param>
		public MessageBoxViewModel(string header, string message)
		{
			Assert.ArgumentNotNullOrWhitespace(header);
			Assert.ArgumentNotNullOrWhitespace(message);

			_header = header;
			_message = message;
		}

		/// <summary>
		///     Gets or sets the header of the message box.
		/// </summary>
		public string Header
		{
			get { return _header; }
			set { ChangePropertyValue(ref _header, value); }
		}

		/// <summary>
		///     Gets or sets the message of the message box.
		/// </summary>
		public string Message
		{
			get { return _message; }
			set { ChangePropertyValue(ref _message, value); }
		}

		/// <summary>
		///     Closes the message box.
		/// </summary>
		public void Close()
		{
			Application.Current.Window.LayoutRoot.Remove(_view);
		}

		/// <summary>
		///     Shows the message box.
		/// </summary>
		public void Show()
		{
			_view = new MessageBoxView { DataContext = this };
			Panel.SetZIndex(_view, ZIndex);

			Application.Current.Window.LayoutRoot.Add(_view);
			Commands.ShowConsole(false);
			_view.Focus();
		}
	}
}