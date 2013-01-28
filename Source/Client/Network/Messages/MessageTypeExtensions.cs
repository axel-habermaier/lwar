using System;

namespace Lwar.Client.Network.Messages
{
	using Pegasus.Framework;

	/// <summary>
	///   Provides extension methods for the MessageType enumeration.
	/// </summary>
	public static class MessageTypeExtensions
	{
		/// <summary>
		///   Checks whether the message type represents a reliable message.
		/// </summary>
		/// <param name="type">The type that should be checked.</param>
		public static bool IsReliable(this MessageType type)
		{
			Assert.ArgumentInRange(type, () => type);
			return (int)type > 100 && (int)type < 200;
		}

		/// <summary>
		///   Checks whether the message type represents an unreliable message.
		/// </summary>
		/// <param name="type">The type that should be checked.</param>
		public static bool IsUnreliable(this MessageType type)
		{
			Assert.ArgumentInRange(type, () => type);
			return (int)type > 200 && (int)type < 300;
		}
	}
}