using System;

namespace Lwar.Client.Network.Messages
{
	using System.Collections.Generic;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;

	/// <summary>
	///   Deserializes messages from a packet received from the server.
	/// </summary>
	public static class MessageDeserializer
	{
		/// <summary>
		///   Deserializes all messages from the incoming packet.
		/// </summary>
		/// <param name="packet">The packet from which the messages should be deserialized.</param>
		/// <param name="deliveryManager">The delivery manager that should be used to enforce the message delivery constraints.</param>
		public static IEnumerable<IMessage> Deserialize(IncomingPacket packet, DeliveryManager deliveryManager)
		{
			Assert.ArgumentNotNull(packet, () => packet);

			using (packet)
			{
				var buffer = packet.Reader;
				var header = Header.Create(buffer);

				if (header == null)
					yield break;

				var ignoreUnreliableMessages = deliveryManager.AllowDelivery(header.Value.Timestamp);
				deliveryManager.UpdateLastAckedSequenceNumber(header.Value.Acknowledgement);

				IReliableMessage reliableMessage = null;
				IUnreliableMessage unreliableMessage = null;

				while (!buffer.EndOfBuffer)
				{
					var type = (MessageType)buffer.ReadByte();
					switch (type)
					{
						case MessageType.AddPlayer:
							reliableMessage = AddPlayer.Create(buffer);
							break;
						case MessageType.RemovePlayer:
							reliableMessage = RemovePlayer.Create(buffer);
							break;
						case MessageType.ChatMessage:
							break;
						case MessageType.AddEntity:
							break;
						case MessageType.RemoveEntity:
							break;
						case MessageType.ChangePlayerState:
							break;
						case MessageType.ChangePlayerName:
							break;
						case MessageType.Synced:
							break;
						case MessageType.ServerFull:
							break;
						case MessageType.UpdatePlayerStats:
							break;
						case MessageType.UpdateEntity:
							break;
						case MessageType.Connect:
						case MessageType.Disconnect:
						case MessageType.UpdateClientInput:
							NetworkLog.ClientError("Received unexpected message of type {0}. The rest of the packet is ignored.", type);
							yield break;
						default:
							NetworkLog.ClientError("Received a message of unknown type. The rest of the packet is ignored.");
							yield break;
					}

					if (type.IsReliable() && deliveryManager.AllowDelivery(reliableMessage))
						yield return reliableMessage;
					else if (type.IsUnreliable() && !ignoreUnreliableMessages)
						yield return unreliableMessage;
				}
			}
		}
	}
}