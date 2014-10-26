namespace Pegasus.UserInterface
{
	using System;
	using System.Collections.Generic;
	using Utilities;
	using ViewModels;

	/// <summary>
	///     Provides convenience methods for displaying message boxes.
	/// </summary>
	public static class MessageBox
	{
		/// <summary>
		///     The message boxes that are currently open.
		/// </summary>
		private static readonly List<MessageBoxViewModel> MessageBoxes = new List<MessageBoxViewModel>();

		/// <summary>
		///     Shows a message box with the given header and message.
		/// </summary>
		/// <param name="header">The header of the message box.</param>
		/// <param name="message">The message that the message box should display.</param>
		public static void Show(string header, string message)
		{
			Assert.ArgumentNotNullOrWhitespace(header);
			Assert.ArgumentNotNullOrWhitespace(message);

			var messageBox = new MessageBoxViewModel(header, message);
			messageBox.Closed += OnClosed;
			messageBox.Show();

			MessageBoxes.Add(messageBox);
		}

		/// <summary>
		///     Closes all open message boxes.
		/// </summary>
		public static void CloseAll()
		{
			while (MessageBoxes.Count > 0)
				MessageBoxes[0].Close();
		}

		/// <summary>
		///     Removes the given message box from the internal list.
		/// </summary>
		/// <param name="messageBox">The message box that has been closed.</param>
		private static void OnClosed(MessageBoxViewModel messageBox)
		{
			Assert.ArgumentNotNull(messageBox);
			Assert.ArgumentSatisfies(MessageBoxes.Contains(messageBox), "Unknown message box.");

			MessageBoxes.Remove(messageBox);
		}
	}
}