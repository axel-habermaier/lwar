using System;

namespace Lwar.Client.Network.Messages
{
	/// <summary>
	///   Represents a reliable message.
	/// </summary>
	public interface IReliableMessage : IMessage
	{
		/// <summary>
		///   Gets the sequence number of the message.
		/// </summary>
		uint SequenceNumber { get; }
	}
}