namespace Lwar.Gameplay.Client
{
	using System;
	using Network.Messages;
	using Pegasus.Utilities;

	/// <summary>
	///     Dispatches messages received from the server to the game session.
	/// </summary>
	internal partial class MessageHandler
	{
		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnConnect(ClientConnectMessage message)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles the given message.
		/// </summary>
		/// <param name="message">The message that should be dispatched.</param>
		public void OnPlayerInput(PlayerInputMessage message)
		{
			HandleUnsupportedMessage(message);
		}

		/// <summary>
		///     Handles an unsupported message.
		/// </summary>
		/// <param name="message">The unsupported message that should be handled.</param>
		private static void HandleUnsupportedMessage(Message message)
		{
			Assert.That(false, "The client cannot handle a message of type '{0}'.", message.MessageType);
		}
	}
}