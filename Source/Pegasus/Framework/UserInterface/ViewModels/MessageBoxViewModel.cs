namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;

	/// <summary>
	///     Displays a message box.
	/// </summary>
	internal class MessageBoxViewModel : ViewModel
	{
		/// <summary>
		///     The header of the message box.
		/// </summary>
		private string _header;

		/// <summary>
		///     The message of the message box.
		/// </summary>
		private string _message;

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
	}
}