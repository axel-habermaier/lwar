using System;

namespace Lwar.Client.Network.Messages
{
	using System.Runtime.InteropServices;
	using Gameplay;

	/// <summary>
	///   Represents a message that is used for the communication between the server and the client (implemented as a
	///   union-type).
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct Message
	{
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
		///   The payload of a Connect message.
		/// </summary>
		[FieldOffset(StringPayloadOffset)]
		public string Connect;

		/// <summary>
		///   The payload of a Join message.
		/// </summary>
		[FieldOffset(StringPayloadOffset)]
		public JoinMessage Join;

		/// <summary>
		///   The payload of a Leave message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public LeaveMessage Leave;

		/// <summary>
		///   The payload of a Chat message.
		/// </summary>
		[FieldOffset(StringPayloadOffset)]
		public ChatMessage Chat;

		/// <summary>
		///   The payload of an Add message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public AddMessage Add;

		/// <summary>
		///   The payload of a Remove message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public Identifier Remove;

		/// <summary>
		///   The payload of a Selection message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public SelectionMessage Selection;

		/// <summary>
		///   The payload of a Name message.
		/// </summary>
		[FieldOffset(StringPayloadOffset)]
		public NameMessage Name;

		/// <summary>
		///   The payload of a Kill message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public KillMessage Kill;

		/// <summary>
		///   The payload of a Stats message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public StatsMessage Stats;

		/// <summary>
		///   The payload of an Input message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public InputMessage Input;

		/// <summary>
		///   The payload of a Collision message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public CollisionMessage Collision;

		/// <summary>
		///   The payload of an Update message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public UpdateMessage Update;

		/// <summary>
		///   The payload of an UpdatePosition message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public UpdatePositionMessage UpdatePosition;

		/// <summary>
		///   The payload of an UpdateRay message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public UpdateRayMessage UpdateRay;

		/// <summary>
		///   The payload of an UpdateCircle message.
		/// </summary>
		[FieldOffset(PayloadOffset)]
		public UpdateCircleMessage UpdateCircle;
	}
}