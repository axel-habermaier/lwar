namespace Lwar.Network
{
	using System;
	using System.Text;
	using Pegasus.Math;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     Provides extension methods for buffer readers and writers.
	/// </summary>
	public static class BufferExtensions
	{
		/// <summary>
		///     Writes the given identity into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the identity should be written into.</param>
		/// <param name="identity">The identity that should be written into the buffer.</param>
		public static void WriteIdentifier(this BufferWriter buffer, NetworkIdentity identity)
		{
			Assert.ArgumentNotNull(buffer);

			buffer.WriteUInt16(identity.Generation);
			buffer.WriteUInt16(identity.Identifier);
		}

		/// <summary>
		///     Writes the given vector into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the vector should be written into.</param>
		/// <param name="vector">The vector that should be written into the buffer.</param>
		public static void WriteVector2(this BufferWriter buffer, Vector2 vector)
		{
			Assert.ArgumentNotNull(buffer);

			vector = Vector2.Clamp(vector, new Vector2(Int16.MinValue), new Vector2(Int16.MaxValue));
			buffer.WriteInt16((short)vector.X);
			buffer.WriteInt16((short)vector.Y);
		}

		/// <summary>
		///     Writes the given orientation into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the orientation should be written into.</param>
		/// <param name="orientation">The orientation that should be written into the buffer.</param>
		public static void WriteOrientation(this BufferWriter buffer, float orientation)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.InRange(orientation, 0, 360);

			buffer.WriteUInt16((ushort)(orientation * NetworkProtocol.AngleFactor));
		}

		/// <summary>
		///     Reads an identity from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the identity should be read from.</param>
		public static NetworkIdentity ReadIdentifier(this BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer);

			var generation = buffer.ReadUInt16();
			var id = buffer.ReadUInt16();

			Assert.That(id != NetworkProtocol.ReservedEntityIdentity.Identifier || generation == 0,
				"Generation of reserved entity identity must be 0.");
			Assert.That(id != NetworkProtocol.ServerPlayerIdentity.Identifier || generation == 0,
				"Generation of reserved server player identity must be 0.");

			return new NetworkIdentity(id, generation);
		}

		/// <summary>
		///     Reads a vector from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the vector should be read from.</param>
		public static Vector2 ReadVector2(this BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer);
			return new Vector2(buffer.ReadInt16(), buffer.ReadInt16());
		}

		/// <summary>
		///     Reads an orientation from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the orientation should be read from.</param>
		public static float ReadOrientation(this BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer);
			return buffer.ReadUInt16() / NetworkProtocol.AngleFactor;
		}

		/// <summary>
		///     Writes a string of the given maximum length into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the string should be written into.</param>
		/// <param name="s">The string that should be written into the buffer.</param>
		/// <param name="length">The maximum length of the string.</param>
		public static void WriteString(this BufferWriter buffer, string s, int length)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentNotNull(s);
			Assert.ArgumentInRange(length, 1, Byte.MaxValue);
			Assert.ArgumentSatisfies(Encoding.UTF8.GetByteCount(s) <= length, "String is too long.");

			var bytes = Encoding.UTF8.GetBytes(s);
			buffer.WriteByte((byte)bytes.Length);
			foreach (var b in bytes)
				buffer.WriteByte(b);
		}

		/// <summary>
		///     Reads a string of the given length from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the string should be read from.</param>
		/// <param name="length">The maximum length of the string.</param>
		public static string ReadString(this BufferReader buffer, int length)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentInRange(length, 1, Byte.MaxValue);

			int size = buffer.ReadByte();
			var skipBytes = 0;

			if (size > length)
			{
				Log.Warn("Received a string that exceeds the maximum allowed length. String truncated.");
				skipBytes = size - length;
				size = length;
			}

			var bytes = new byte[size];
			for (var i = 0; i < size; ++i)
				bytes[i] = buffer.ReadByte();

			// Skip the remaining bytes if the string is too long
			for (var i = 0; i < skipBytes; ++i)
				buffer.ReadByte();

			return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}
	}
}