using System;

namespace Lwar.Client.Network.Messages
{
	using System.Runtime.InteropServices;
	using Gameplay;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Contains updated data sent by the server.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct UpdateRecord
	{
		/// <summary>
		///   The type of the update record.
		/// </summary>
		[FieldOffset(0)]
		public UpdateRecordType Type;

		/// <summary>
		/// The identifier of the entity that is updated.
		/// </summary>
		[FieldOffset(4)]
		public Identifier EntityId;

		/// <summary>
		///   The data of a full update.
		/// </summary>
		[FieldOffset(8)]
		public FullUpdate Full;

		/// <summary>
		///   The data of a ray update.
		/// </summary>
		[FieldOffset(8)]
		public RayUpdate Ray;

		/// <summary>
		///   The data of a circle update.
		/// </summary>
		[FieldOffset(8)]
		public CircleUpdate Circle;

		/// <summary>
		///   The data of a position update.
		/// </summary>
		[FieldOffset(8)]
		public Vector2 Position;

		public struct FullUpdate
		{
			public int Health;
			public Vector2 Position;
			public ushort Rotation;
		}

		public struct RayUpdate
		{
			public float Direction;
			public float Length;
			public Vector2 Origin;
		}

		public struct CircleUpdate
		{
			public Vector2 Center;
			public float Radius;
		}
	}
}