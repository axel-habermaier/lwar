using System;

namespace Lwar.Client.Network.Messages
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;

	public class FullMessage : Message<FullMessage>, IUnreliableMessage
	{
		/// <summary>
		///   Gets or sets the timestamp of the message.
		/// </summary>
		public uint Timestamp { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="buffer">The buffer from which the instance should be deserialized.</param>
		public static FullMessage Create(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			return GetInstance();
		}
	}
}