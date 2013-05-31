using System;

namespace Lwar.Client.Network
{
	using System.Runtime.InteropServices;
	using System.Text;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents a message that is used for the communication between the server and the client.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct Message
	{
		/// <summary>
		///   Creates a message that instructs the server to change the name of the given player.
		/// </summary>
		/// <param name="player">The player whose name should be changed.</param>
		/// <param name="playerName">The new player name.</param>
		public static Message ChangePlayerName(Player player, string playerName)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentNotNullOrWhitespace(playerName);
			Assert.That(Encoding.UTF8.GetByteCount(playerName) <= Specification.MaximumPlayerNameLength, "Player name is too long.");

			return new Message
			{
				Type = MessageType.Name,
				Name = new NamePayload
				{
					Player = player.Id,
					Name = playerName
				}
			};
		}

		/// <summary>
		///   Creates a chat message that the server broadcasts to all players.
		/// </summary>
		/// <param name="player">The player who wrote the chat message.</param>
		/// <param name="message">The message that should be sent.</param>
		public static Message Say(Player player, string message)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentNotNullOrWhitespace(message);
			Assert.That(Encoding.UTF8.GetByteCount(message) <= Specification.MaximumChatMessageLength, "Chat message is too long.");

			return new Message
			{
				Type = MessageType.Chat,
				Chat = new ChatPayload
				{
					Player = player.Id,
					Message = message
				}
			};
		}

		/// <summary>
		///   Creates a message that instructs the server to change the ship and weapon types of the given player.
		/// </summary>
		/// <param name="player">The player whose ship and weapon types should be changed.</param>
		/// <param name="ship">The new ship type.</param>
		/// <param name="weapon1">The type of the weapon in the first weapon slot.</param>
		/// <param name="weapon2">The type of the weapon in the second weapon slot.</param>
		/// <param name="weapon3">The type of the weapon in the third weapon slot.</param>
		/// <param name="weapon4">The type of the weapon in the fourth weapon slot.</param>
		public static Message ChangeSelection(Player player, EntityType ship,
											  EntityType weapon1, EntityType weapon2,
											  EntityType weapon3, EntityType weapon4)
		{
			Assert.ArgumentNotNull(player);
			Assert.ArgumentInRange(ship);
			Assert.ArgumentInRange(weapon1);
			Assert.ArgumentInRange(weapon2);
			Assert.ArgumentInRange(weapon3);
			Assert.ArgumentInRange(weapon4);

			return new Message
			{
				Type = MessageType.Selection,
				Selection = new SelectionPayload
				{
					Player = player.Id,
					ShipType = ship,
					WeaponType1 = weapon1,
					WeaponType2 = weapon2,
					WeaponType3 = weapon3,
					WeaponType4 = weapon4
				}
			};
		}

		/// <summary>
		///   Returns a string representation of the message.
		/// </summary>
		public override string ToString()
		{
			return Type.ToString();
		}

		/// <summary>
		///   The offset of payload fields that contain strings at offset 0.
		/// </summary>
		private const int StringPayloadOffset = 8;

		/// <summary>
		///   The offset of the payload fields that contain native types only.
		/// </summary>
		/// <remarks>
		///   These fields must lie at offsets after the string, which ends at offset 16 in 64 bit builds.
		/// </remarks>
		private const int PayloadOffset = 16;

		/// <summary>
		///   The type of the message.
		/// </summary>
		[FieldOffset(0)]
		public MessageType Type;

		/// <summary>
		///   The sequence number of a reliable message.
		/// </summary>
		[FieldOffset(4)]
		public uint SequenceNumber;

		/// <summary>
		///   The timestamp of an unreliable message.
		/// </summary>
		[FieldOffset(4)]
		public uint Timestamp;

		/// <summary>
		///   The payload of a Join message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public JoinPayload Join;

		/// <summary>
		///   The payload of a Leave message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public Identifier Leave;

		/// <summary>
		///   The payload of a Chat message.
		/// </summary>
		[FieldOffset(StringPayloadOffset)]
		public ChatPayload Chat;

		/// <summary>
		///   The payload of an Add message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public AddPayload Add;

		/// <summary>
		///   The payload of a Remove message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public Identifier Remove;

		/// <summary>
		///   The payload of a Selection message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public SelectionPayload Selection;

		/// <summary>
		///   The payload of a Name message.
		/// </summary>
		[FieldOffset(StringPayloadOffset)]
		public NamePayload Name;

		/// <summary>
		///   The payload of a Stats message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public StatsPayload Stats;

		/// <summary>
		///   The payload of an Input message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public InputPayload Input;

		/// <summary>
		///   The payload of a Collision message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public CollisionPayload Collision;

		/// <summary>
		///   The payload of an Update message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public UpdatePayload Update;

		/// <summary>
		///   The payload of an UpdatePosition message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public UpdatePositionPayload UpdatePosition;

		/// <summary>
		///   The payload of an UpdateRay message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public UpdateRayPayload UpdateRay;

		/// <summary>
		///   The payload of an UpdateCircle message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public UpdateCirclePayload UpdateCircle;

		/// <summary>
		///   Holds the payload of a Join message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct JoinPayload
		{
			/// <summary>
			///   Indicates whether the joined player is the local player.
			/// </summary>
			public bool IsLocalPlayer;

			/// <summary>
			///   The player that joined the game session.
			/// </summary>
			public Identifier Player;
		}

		/// <summary>
		///   Holds the payload of a Chat message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ChatPayload
		{
			/// <summary>
			///   The message sent by the player.
			/// </summary>
			public string Message;

			/// <summary>
			///   The player that sent the message.
			/// </summary>
			public Identifier Player;
		}

		/// <summary>
		///   Holds the payload of an Add message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct AddPayload
		{
			/// <summary>
			///   The entity that is added.
			/// </summary>
			public Identifier Entity;

			/// <summary>
			///   The player the entity belongs to.
			/// </summary>
			public Identifier Player;

			/// <summary>
			///   The type of the entity that is added.
			/// </summary>
			public EntityType Type;
		}

		/// <summary>
		///   Holds the payload of a Selection message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct SelectionPayload
		{
			/// <summary>
			///   The player whose ship and weapons types are changed.
			/// </summary>
			public Identifier Player;

			/// <summary>
			///   The selected ship type.
			/// </summary>
			public EntityType ShipType;

			/// <summary>
			///   The selected weapon type for the first weapon slot.
			/// </summary>
			public EntityType WeaponType1;

			/// <summary>
			///   The selected weapon type for the second weapon slot.
			/// </summary>
			public EntityType WeaponType2;

			/// <summary>
			///   The selected weapon type for the third weapon slot.
			/// </summary>
			public EntityType WeaponType3;

			/// <summary>
			///   The selected weapon type for the fourth weapon slot.
			/// </summary>
			public EntityType WeaponType4;
		}

		/// <summary>
		///   Holds the payload of a Name message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct NamePayload
		{
			/// <summary>
			///   The new name of the player.
			/// </summary>
			public string Name;

			/// <summary>
			///   The player whose name is changed.
			/// </summary>
			public Identifier Player;
		}

		/// <summary>
		///   Holds the payload of a Stats message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct StatsPayload
		{
			/// <summary>
			///   The number of deaths of the player.
			/// </summary>
			public ushort Deaths;

			/// <summary>
			///   The number of kills scored by the player.
			/// </summary>
			public ushort Kills;

			/// <summary>
			///   The player's network latency.
			/// </summary>
			public ushort Ping;

			/// <summary>
			///   The player whose stats are updated.
			/// </summary>
			public Identifier Player;
		}

		/// <summary>
		///   Holds the payload of an Input message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct InputPayload
		{
			/// <summary>
			///   The boolean state value for the backwards input, including the seven previous states.
			/// </summary>
			public byte Backward;

			/// <summary>
			///   The boolean state value for the forward input, including the seven previous states.
			/// </summary>
			public byte Forward;

			/// <summary>
			///   The monotonically increasing frame number, starting at 1.
			/// </summary>
			public uint FrameNumber;

			/// <summary>
			///   The player that generated the input.
			/// </summary>
			public Identifier Player;

			/// <summary>
			///   The boolean state value for the first shooting input, including the seven previous states.
			/// </summary>
			public byte Shooting1;

			/// <summary>
			///   The boolean state value for the second shooting input, including the seven previous states.
			/// </summary>
			public byte Shooting2;

			/// <summary>
			///   The boolean state value for the third shooting input, including the seven previous states.
			/// </summary>
			public byte Shooting3;

			/// <summary>
			///   The boolean state value for the fourth shooting input, including the seven previous states.
			/// </summary>
			public byte Shooting4;

			/// <summary>
			///   The boolean state value for the strafe left input, including the seven previous states.
			/// </summary>
			public byte StrafeLeft;

			/// <summary>
			///   The boolean state value for the strafe right input, including the seven previous states.
			/// </summary>
			public byte StrafeRight;

			/// <summary>
			///   The position of the client's target in world coodinates.
			/// </summary>
			public Vector2 Target;

			/// <summary>
			///   The boolean state value for the turn left input, including the seven previous states.
			/// </summary>
			public byte TurnLeft;

			/// <summary>
			///   The boolean state value for the turn right input, including the seven previous states.
			/// </summary>
			public byte TurnRight;
		}

		/// <summary>
		///   Holds the payload of a Collision message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct CollisionPayload
		{
			/// <summary>
			///   The first entity involved in the collision.
			/// </summary>
			public Identifier Entity1;

			/// <summary>
			///   The second entity involved in the collision.
			/// </summary>
			public Identifier Entity2;

			/// <summary>
			///   The position of the impact.
			/// </summary>
			public Vector2 Position;
		}

		/// <summary>
		///   Holds the payload of an Update message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct UpdatePayload
		{
			/// <summary>
			///   The entity that is updated.
			/// </summary>
			public Identifier Entity;

			/// <summary>
			///   The new entity health.
			/// </summary>
			public int Health;

			/// <summary>
			///   The new entity position.
			/// </summary>
			public Vector2 Position;

			/// <summary>
			///   The new entity rotation.
			/// </summary>
			public ushort Rotation;
		}

		/// <summary>
		///   Holds the payload of an UpdatePosition message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct UpdatePositionPayload
		{
			/// <summary>
			///   The entity that is updated.
			/// </summary>
			public Identifier Entity;

			/// <summary>
			///   The new entity position.
			/// </summary>
			public Vector2 Position;
		}

		/// <summary>
		///   Holds the payload of an UpdateRay message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct UpdateRayPayload
		{
			/// <summary>
			///   The entity that is updated.
			/// </summary>
			public Identifier Entity;

			/// <summary>
			///   The new ray direction.
			/// </summary>
			public float Direction;

			/// <summary>
			///   The new ray length.
			/// </summary>
			public float Length;

			/// <summary>
			///   The new ray origin.
			/// </summary>
			public Vector2 Origin;
		}

		/// <summary>
		///   Holds the payload of an UpdateCircle message.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct UpdateCirclePayload
		{
			/// <summary>
			///   The entity that is updated.
			/// </summary>
			public Identifier Entity;

			/// <summary>
			///   The new circle center.
			/// </summary>
			public Vector2 Center;

			/// <summary>
			///   The new circle radius.
			/// </summary>
			public float Radius;
		}
	}
}