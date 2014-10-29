namespace Lwar.Network.Messages
{
	using System;
	using System.Linq;
	using Pegasus.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Informs a server about the input state of a client.
	/// </summary>
	[UnreliableTransmission(MessageType.PlayerInput)]
	public sealed class PlayerInputMessage : Message
	{
		/// <summary>
		///     Initializes the type.
		/// </summary>
		static PlayerInputMessage()
		{
			ConstructorCache.Register(() => new PlayerInputMessage());
		}

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private PlayerInputMessage()
		{
			FireWeapons = new byte[NetworkProtocol.WeaponSlotCount];
		}

		/// <summary>
		///     Gets the boolean state value for the backwards input, including the seven previous states.
		/// </summary>
		public byte Backward { get; private set; }

		/// <summary>
		///     Gets the boolean state value for the forward input, including the seven previous states.
		/// </summary>
		public byte Forward { get; private set; }

		/// <summary>
		///     Gets the monotonically increasing frame number, starting at 1.
		/// </summary>
		public uint FrameNumber { get; private set; }

		/// <summary>
		///     Gets the boolean state value for the after burner input, including the seven previous states.
		/// </summary>
		public byte AfterBurner { get; private set; }

		/// <summary>
		///     Gets the player that generated the input.
		/// </summary>
		public Identity Player { get; private set; }

		/// <summary>
		///     Gets the boolean state values for the weapon slots, including the seven previous states.
		/// </summary>
		public byte[] FireWeapons { get; private set; }

		/// <summary>
		///     Gets the boolean state value for the strafe left input, including the seven previous states.
		/// </summary>
		public byte StrafeLeft { get; private set; }

		/// <summary>
		///     Gets the boolean state value for the strafe right input, including the seven previous states.
		/// </summary>
		public byte StrafeRight { get; private set; }

		/// <summary>
		///     Gets the position of the client's target relative to the client's ship.
		/// </summary>
		public Vector2 Target { get; private set; }

		/// <summary>
		///     Gets the boolean state value for the turn left input, including the seven previous states.
		/// </summary>
		public byte TurnLeft { get; private set; }

		/// <summary>
		///     Gets the boolean state value for the turn right input, including the seven previous states.
		/// </summary>
		public byte TurnRight { get; private set; }

		/// <summary>
		///     Serializes the message using the given writer.
		/// </summary>
		/// <param name="writer">The writer that should be used to serialize the message.</param>
		public override void Serialize(BufferWriter writer)
		{
			writer.WriteIdentifier(Player);
			writer.WriteUInt32(FrameNumber);
			writer.WriteByte(Forward);
			writer.WriteByte(Backward);
			writer.WriteByte(TurnLeft);
			writer.WriteByte(TurnRight);
			writer.WriteByte(StrafeLeft);
			writer.WriteByte(StrafeRight);
			writer.WriteByte(AfterBurner);

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				writer.WriteByte(FireWeapons[i]);

			writer.WriteVector2(Target);
		}

		/// <summary>
		///     Deserializes the message using the given reader.
		/// </summary>
		/// <param name="reader">The reader that should be used to deserialize the message.</param>
		public override void Deserialize(BufferReader reader)
		{
			Player = reader.ReadIdentifier();
			FrameNumber = reader.ReadUInt32();
			Forward = reader.ReadByte();
			Backward = reader.ReadByte();
			TurnLeft = reader.ReadByte();
			TurnRight = reader.ReadByte();
			StrafeLeft = reader.ReadByte();
			StrafeRight = reader.ReadByte();
			AfterBurner = reader.ReadByte();

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				FireWeapons[i] = reader.ReadByte();

			Target = reader.ReadVector2();
		}

		/// <summary>
		///     Dispatches the message to the given dispatcher.
		/// </summary>
		/// <param name="handler">The dispatcher that should be used to dispatch the message.</param>
		/// <param name="sequenceNumber">The sequence number of the message.</param>
		public override void Dispatch(IMessageHandler handler, uint sequenceNumber)
		{
			handler.OnPlayerInput(this);
		}

		/// <summary>
		///     Creates a chat message that the server broadcasts to all players.
		/// </summary>
		/// <param name="poolAllocator">The pool allocator that should be used to allocate the message.</param>
		/// <param name="player">The player that generated the input.</param>
		/// <param name="frameNumber">The number of the frame during which the input was generated, starting at 1.</param>
		/// <param name="target"></param>
		/// <param name="forward">The boolean state value for the forward input, including the seven previous states.</param>
		/// <param name="backward">The boolean state value for the backwards input, including the seven previous states.</param>
		/// <param name="strafeLeft">The boolean state value for the strafe left input, including the seven previous states.</param>
		/// <param name="strafeRight">The boolean state value for the strafe right input, including the seven previous states.</param>
		/// <param name="turnLeft">The boolean state value for the turn left input, including the seven previous states.</param>
		/// <param name="turnRight">The boolean state value for the turn right input, including the seven previous states.</param>
		/// <param name="afterBurner">The boolean state value for the after burner input, including the seven previous states.</param>
		/// <param name="fireWeapons">The boolean state values for the shooting inputs, including the seven previous states.</param>
		public static PlayerInputMessage Create(PoolAllocator poolAllocator, Identity player, uint frameNumber, Vector2 target,
												byte forward, byte backward,
												byte strafeLeft, byte strafeRight,
												byte turnLeft, byte turnRight,
			byte afterBurner,
												byte[] fireWeapons)
		{
			Assert.ArgumentNotNull(poolAllocator);
			Assert.ArgumentNotNull(fireWeapons);
			Assert.ArgumentSatisfies(fireWeapons.Length == NetworkProtocol.WeaponSlotCount, "Unexpected array length.");

			var message = poolAllocator.Allocate<PlayerInputMessage>();
			message.Player = player;
			message.FrameNumber = frameNumber;
			message.Target = target;
			message.Forward = forward;
			message.Backward = backward;
			message.StrafeLeft = strafeLeft;
			message.StrafeRight = strafeRight;
			message.TurnLeft = turnLeft;
			message.TurnRight = turnRight;
			message.AfterBurner = afterBurner;

			for (var i = 0; i < NetworkProtocol.WeaponSlotCount; ++i)
				message.FireWeapons[i] = fireWeapons[i];

			return message;
		}

		/// <summary>
		///     Returns a string that represents the message.
		/// </summary>
		public override string ToString()
		{
			return String.Format("{0}, Player={1}, FrameNumber={2}, Target={{{3}}}, Forward={4}, Backward={5}, StrafeLeft={6}, StrafeRight={7}, " +
								 "TurnLeft={8}, TurnRight={9}, AfterBurner={10}, {11}",
				MessageType, Player, FrameNumber, Target, Forward, Backward, StrafeLeft, StrafeRight, TurnLeft, TurnRight, AfterBurner,
				String.Join(", ", FireWeapons.Select((fire, index) => String.Format("FireWeapon{0}={1}", index + 1, fire))));
		}
	}
}