namespace Lwar.UserInterface.ViewModels
{
	using System;
	using System.Text;
	using Network;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.UserInterface.Input;
	using Scripting;

	/// <summary>
	///     Shows the chat input field during an active game session.
	/// </summary>
	public class ChatViewModel : NotifyPropertyChanged
	{
		/// <summary>
		///     Indicates whether the scoreboard should be shown.
		/// </summary>
		private bool _isVisible;

		/// <summary>
		///     The chat message entered by the user.
		/// </summary>
		private string _message;

		/// <summary>
		///     Gets the maximum allowed length of a chat message. When characters are entered that require more than one UTF8 code
		///     point, the length exceeded property might be true even before max length characters are entered.
		/// </summary>
		public int MaxMessageLength
		{
			get { return Specification.ChatMessageLength; }
		}

		/// <summary>
		///     Gets or sets the chat message entered by the user.
		/// </summary>
		public string Message
		{
			private get { return _message; }
			set
			{
				Assert.ArgumentNotNull(value);

				ChangePropertyValue(ref _message, value);
				OnPropertyChanged("LengthExceeded");
			}
		}

		/// <summary>
		///     Gets a value indicating whether the scoreboard should be shown.
		/// </summary>
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				if (_isVisible != value)
					Message = String.Empty;

				ChangePropertyValue(ref _isVisible, value);
			}
		}

		/// <summary>
		///     Gets a value indicating whether the chat message entered by the user exceeds the maximum allowed length.
		/// </summary>
		private bool LengthExceeded
		{
			get { return Encoding.UTF8.GetByteCount(Message) > Specification.ChatMessageLength; }
		}

		/// <summary>
		///     Sends the current message to the server, if the message is non-empty and does not exceed the length limit, or cancels
		///     the input.
		/// </summary>
		public void Submit(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				IsVisible = false;
			}

			if (LengthExceeded || (e.Key != Key.Return && e.Key != Key.NumpadEnter))
				return;

			if (!String.IsNullOrWhiteSpace(Message))
				Commands.Say(Message);

			e.Handled = true;
			IsVisible = false;
		}
	}
}