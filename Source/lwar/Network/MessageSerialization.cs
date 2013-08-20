using System;

namespace Lwar.Network
{
	using System.Collections.Generic;
	using Gameplay.Entities;
	using Messages;
	using Pegasus;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;

	/// <summary>
	///   Serializes and deserializes messages from and to buffers.
	/// </summary>
	public static class MessageSerialization
	{
		/// <summary>
		///   A cached list instance that is used to return multiple deserialized messages.
		/// </summary>
		private static readonly List<Message> Messages = new List<Message>();

		/// <summary>
		///   Serializes the message into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer into which the message should be serialized.</param>
		/// <param name="message">The message that should be serialized.</param>
		public static void Serialize(BufferWriter buffer, Message message)
		{
			Assert.ArgumentNotNull(buffer);

			buffer.WriteByte((byte)message.Type);
			buffer.WriteUInt32(message.SequenceNumber);

			switch (message.Type)
			{
				case MessageType.Chat:
					buffer.WriteIdentifier(message.Chat.Player);
					buffer.WriteString(message.Chat.Message, Specification.ChatMessageLength);
					break;
				case MessageType.Connect:
					buffer.WriteByte(message.Connect.NetworkRevision);
					buffer.WriteString(message.Connect.Name, Specification.PlayerNameLength);
					break;
				case MessageType.Disconnect:
					// No payload
					break;
				case MessageType.Input:
					buffer.WriteIdentifier(message.Input.Player);
					buffer.WriteUInt32(message.Input.FrameNumber);
					buffer.WriteByte(message.Input.Forward);
					buffer.WriteByte(message.Input.Backward);
					buffer.WriteByte(message.Input.TurnLeft);
					buffer.WriteByte(message.Input.TurnRight);
					buffer.WriteByte(message.Input.StrafeLeft);
					buffer.WriteByte(message.Input.StrafeRight);
					buffer.WriteByte(message.Input.Shooting1);
					buffer.WriteByte(message.Input.Shooting2);
					buffer.WriteByte(message.Input.Shooting3);
					buffer.WriteByte(message.Input.Shooting4);
					buffer.WriteInt16((short)message.Input.Target.X);
					buffer.WriteInt16((short)message.Input.Target.Y);
					break;
				case MessageType.Name:
					buffer.WriteIdentifier(message.Name.Player);
					buffer.WriteString(message.Name.Name, Specification.PlayerNameLength);
					break;
				case MessageType.Selection:
					buffer.WriteIdentifier(message.Selection.Player);
					buffer.WriteByte((byte)message.Selection.ShipType);
					buffer.WriteByte((byte)message.Selection.WeaponType1);
					buffer.WriteByte((byte)message.Selection.WeaponType2);
					buffer.WriteByte((byte)message.Selection.WeaponType3);
					buffer.WriteByte((byte)message.Selection.WeaponType4);
					break;
				case MessageType.Add:
				case MessageType.Collision:
				case MessageType.Reject:
				case MessageType.Join:
				case MessageType.Leave:
				case MessageType.Remove:
				case MessageType.Stats:
				case MessageType.Synced:
				case MessageType.Kill:
				case MessageType.Update:
				case MessageType.UpdatePosition:
				case MessageType.UpdateRay:
				case MessageType.UpdateCircle:
					Assert.That(false, "The client is not allowed to send a message of type '{0}'.", message.Type);
					break;
				default:
					throw new InvalidOperationException("Unknown message type.");
			}
		}

		/// <summary>
		///   Deserializes a message or messages from the buffer and enqueues them in a list. For some message types sent by the
		///   server, several Message instances are placed in the list (for instance, the Update message sent by the server
		///   is deserialized into one update message per updated entity). The returned list remains valid only until the next call
		///   to this method.
		/// </summary>
		/// <param name="buffer">The buffer from which the message should be deserialized.</param>
		public static List<Message> Deserialize(BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer);

			int count;
			var message = new Message { Type = (MessageType)buffer.ReadByte() };
			Messages.Clear();

			Assert.InRange(message.Type);
			message.SequenceNumber = buffer.ReadUInt32();

			switch (message.Type)
			{
				case MessageType.Chat:
					message.Chat.Player = buffer.ReadIdentifier();
					message.Chat.Message = buffer.ReadString(Specification.ChatMessageLength);
					Messages.Add(message);
					break;
				case MessageType.Name:
					message.Name.Player = buffer.ReadIdentifier();
					message.Name.Name = buffer.ReadString(Specification.PlayerNameLength);
					Messages.Add(message);
					break;
				case MessageType.Selection:
					message.Selection.Player = buffer.ReadIdentifier();
					message.Selection.ShipType = (EntityType)buffer.ReadByte();
					message.Selection.WeaponType1 = (EntityType)buffer.ReadByte();
					message.Selection.WeaponType2 = (EntityType)buffer.ReadByte();
					message.Selection.WeaponType3 = (EntityType)buffer.ReadByte();
					message.Selection.WeaponType4 = (EntityType)buffer.ReadByte();

					Assert.InRange(message.Selection.ShipType);
					Assert.InRange(message.Selection.WeaponType1);
					Assert.InRange(message.Selection.WeaponType2);
					Assert.InRange(message.Selection.WeaponType3);
					Assert.InRange(message.Selection.WeaponType4);
					Messages.Add(message);
					break;
				case MessageType.Add:
					message.Add.Entity = buffer.ReadIdentifier();
					message.Add.Player = buffer.ReadIdentifier();
					message.Add.Type = (EntityType)buffer.ReadByte();

					Assert.InRange(message.Add.Type);
					Messages.Add(message);
					break;
				case MessageType.Collision:
					message.Collision.Entity1 = buffer.ReadIdentifier();
					message.Collision.Entity2 = buffer.ReadIdentifier();
					message.Collision.Position = new Vector2(buffer.ReadInt16(), buffer.ReadInt16());
					Messages.Add(message);
					break;
				case MessageType.Kill:
					message.Kill.Killer = buffer.ReadIdentifier();
					message.Kill.Victim = buffer.ReadIdentifier();
					Messages.Add(message);
					break;
				case MessageType.Reject:
					message.Reject = (RejectReason)buffer.ReadByte();

					Assert.InRange(message.Reject);
					Messages.Add(message);
					break;
				case MessageType.Synced:
					// No payload data
					Messages.Add(message);
					break;
				case MessageType.Join:
					message.Join.Player = buffer.ReadIdentifier();
					message.Join.IsLocalPlayer = false;
					message.Join.Name = buffer.ReadString(Specification.PlayerNameLength);
					Messages.Add(message);
					break;
				case MessageType.Leave:
					message.Leave.Player = buffer.ReadIdentifier();
					message.Leave.Reason = (LeaveReason)buffer.ReadByte();

					Assert.InRange(message.Leave.Reason);
					Messages.Add(message);
					break;
				case MessageType.Remove:
					message.Remove = buffer.ReadIdentifier();
					Messages.Add(message);
					break;
				case MessageType.Stats:
					count = buffer.ReadByte();
					for (var i = 0; i < count; ++i)
					{
						message.Stats.Player = buffer.ReadIdentifier();
						message.Stats.Kills = buffer.ReadUInt16();
						message.Stats.Deaths = buffer.ReadUInt16();
						message.Stats.Ping = buffer.ReadUInt16();

						Messages.Add(message);
					}
					break;
				case MessageType.Update:
					count = buffer.ReadByte();
					for (var i = 0; i < count; ++i)
					{
						message.Update.Entity = buffer.ReadIdentifier();
						message.Update.Position = new Vector2(buffer.ReadInt16(), buffer.ReadInt16());
						message.Update.Rotation = buffer.ReadUInt16() / Specification.AngleFactor;
						message.Update.Health = buffer.ReadByte();

						Messages.Add(message);
					}
					break;
				case MessageType.UpdatePosition:
					count = buffer.ReadByte();
					for (var i = 0; i < count; ++i)
					{
						message.UpdatePosition.Entity = buffer.ReadIdentifier();
						message.UpdatePosition.Position = new Vector2(buffer.ReadInt16(), buffer.ReadInt16());

						Messages.Add(message);
					}
					break;
				case MessageType.UpdateRay:
					count = buffer.ReadByte();
					for (var i = 0; i < count; ++i)
					{
						message.UpdateRay.Entity = buffer.ReadIdentifier();
						message.UpdateRay.Origin = new Vector2(buffer.ReadInt16(), buffer.ReadInt16());
						message.UpdateRay.Direction = buffer.ReadUInt16() / Specification.AngleFactor;
						message.UpdateRay.Length = buffer.ReadUInt16();
						message.UpdateRay.Target = buffer.ReadIdentifier();

						Messages.Add(message);
					}
					break;
				case MessageType.UpdateCircle:
					count = buffer.ReadByte();
					for (var i = 0; i < count; ++i)
					{
						message.UpdateCircle.Entity = buffer.ReadIdentifier();
						message.UpdateCircle.Center = new Vector2(buffer.ReadInt16(), buffer.ReadInt16());
						message.UpdateCircle.Radius = buffer.ReadUInt16();

						Messages.Add(message);
					}
					break;
				case MessageType.Connect:
				case MessageType.Disconnect:
				case MessageType.Input:
					Assert.That(false, "The client is not allowed to receive a message of type '{0}'.", message.Type);
					return Messages;
				default:
					throw new InvalidOperationException("Unknown message type.");
			}

			Assert.That(Messages.Count != 0, "The message has not been deserialized properly.");
			return Messages;
		}
	}
}