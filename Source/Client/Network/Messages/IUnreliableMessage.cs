using System;

namespace Lwar.Client.Network.Messages
{
	/// <summary>
	///   Represents an unreliable message.
	/// </summary>
	public interface IUnreliableMessage : IMessage
	{
		/// <summary>
		///   Gets or sets the timestamp of the message.
		/// </summary>
		uint Timestamp { get; set; }
	}
}