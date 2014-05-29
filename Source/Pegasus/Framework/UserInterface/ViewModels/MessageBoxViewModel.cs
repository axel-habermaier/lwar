﻿namespace Pegasus.Framework.UserInterface.ViewModels
{
	using System;
	using Controls;

	/// <summary>
	///     Displays a message box.
	/// </summary>
	internal class MessageBoxViewModel : ViewModel
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
		///     Shows the message box.
		/// </summary>
		public void Show()
		{
			var view = new MessageBoxView { ViewModel = this };
			Panel.SetZIndex(view, ZIndex);

			Application.Current.Window.LayoutRoot.Add(view);
		}
	}
}