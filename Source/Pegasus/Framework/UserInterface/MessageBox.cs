namespace Pegasus.Framework.UserInterface
{
	using System;
	using ViewModels;

	/// <summary>
	///     Provides convenience methods for displaying message boxes.
	/// </summary>
	public static class MessageBox
	{
		/// <summary>
		/// Shows a message box with the given header and message.
		/// </summary>
		/// <param name="header">The header of the message box.</param>
		/// <param name="message">The message that the message box should display.</param>
		public static void Show(string header, string message)
		{
			Assert.ArgumentNotNullOrWhitespace(header);
			Assert.ArgumentNotNullOrWhitespace(message);

			var viewModel = new MessageBoxViewModel(header, message);
			viewModel.Show();
		}
	}
}