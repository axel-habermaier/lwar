namespace Lwar.UserInterface.ViewModels
{
	using System;
	using System.Text;
	using Network;
	using Pegasus.UserInterface;
	using Pegasus.Utilities;
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
			get { return NetworkProtocol.ChatMessageLength; }
		}

		/// <summary>
		///     Gets or sets the chat message entered by the user.
		/// </summary>
		public string Message
		{
			get { return _message; }
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
		public bool LengthExceeded
		{
			get { return Encoding.UTF8.GetByteCount(Message) > NetworkProtocol.ChatMessageLength; }
		}

		/// <summary>
		///     Sends the current message to the server, if the message is non-empty and does not exceed the length limit.
		/// </summary>
		public void Send()
		{
			if (LengthExceeded)
				return;

			if (!String.IsNullOrWhiteSpace(Message))
				Commands.Say(Message);

			IsVisible = false;
		}

		/// <summary>
		///     Cancels and hides the chat input.
		/// </summary>
		public void Cancel()
		{
			IsVisible = false;
		}
	}
}