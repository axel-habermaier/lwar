namespace Lwar.Network
{
	using System;
	using Pegasus;

	/// <summary>
	///     Provides extension methods for the MessageType enumeration.
	/// </summary>
	public static class MessageTypeExtensions
	{
		/// <summary>
		///     Checks whether the message type represents a reliable message.
		/// </summary>
		/// <param name="type">The message type that should be checked.</param>
		public static bool IsReliable(this MessageType type)
		{
			Assert.ArgumentInRange(type);
			return (int)type < 100;
		}

		/// <summary>
		///     Checks whether the message type represents an unreliable message.
		/// </summary>
		/// <param name="type">The message type that should be checked.</param>
		public static bool IsUnreliable(this MessageType type)
		{
			Assert.ArgumentInRange(type);
			return (int)type > 100;
		}
	}
}