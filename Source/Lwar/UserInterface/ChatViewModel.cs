namespace Lwar.UserInterface
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
	public class ChatViewModel : DisposableNotifyPropertyChanged
	{
		/// <summary>
		///     The input trigger that is used to determine whether the chat input should be shown.
		/// </summary>
		private readonly LogicalInput _activate = new LogicalInput(Cvars.InputChatCvar, InputLayers.Game);

		/// <summary>
		///     Indicates whether the scoreboard should be shown.
		/// </summary>
		private bool _isVisible;

		/// <summary>
		///     The chat message entered by the user.
		/// </summary>
		private string _message;

		//private readonly LogicalInput _submit = new LogicalInput(Key.Return.WentDown() | Key.NumpadEnter.WentDown(), InputLayers.Chat);

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public ChatViewModel()
		{
			Application.Current.Window.InputDevice.Add(_activate);
			IsVisible = true;
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

				_message = value;
				OnPropertyChanged("LengthExceeded");
			}
		}

		/// <summary>
		///     Gets a value indicating whether the scoreboard should be shown.
		/// </summary>
		public bool IsVisible
		{
			get { return _isVisible; }
			private set
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

			if (LengthExceeded || String.IsNullOrWhiteSpace(Message) || (e.Key != Key.Return && e.Key != Key.NumpadEnter))
				return;

			Commands.Say(Message);
			e.Handled = true;
			IsVisible = false;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Application.Current.Window.InputDevice.Remove(_activate);
		}
	}
}