﻿using System;

namespace Lwar.Client.Network
{
	using System.Text;
	using Gameplay;
	using Pegasus.Framework;
	using Pegasus.Framework.Network;
	using Pegasus.Framework.Platform;

	/// <summary>
	///   Provides extension methods for buffer readers and writers.
	/// </summary>
	public static class BufferExtensions
	{
		/// <summary>
		///   Writes the given identifier into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the identifier should be written into.</param>
		/// <param name="identifier">The identifier that should be written into the buffer.</param>
		public static void WriteIdentifier(this BufferWriter buffer, Identifier identifier)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			buffer.WriteUInt16(identifier.Generation);
			buffer.WriteUInt16(identifier.Identity);
		}

		/// <summary>
		///   Reads an identifier from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the identifier should be read from.</param>
		public static Identifier ReadIdentifier(this BufferReader buffer)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);

			var generation = buffer.ReadUInt16();
			var id = buffer.ReadUInt16();
			return new Identifier(id, generation);
		}

		/// <summary>
		///   Writes a string of the given maximum length into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the string should be written into.</param>
		/// <param name="s">The string that should be written into the buffer.</param>
		/// <param name="length">The maximum length of the string.</param>
		public static void WriteString(this BufferWriter buffer, string s, int length)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			Assert.ArgumentNotNull(s, () => s);
			Assert.ArgumentInRange(length, () => length, 1, Byte.MaxValue);
			Assert.ArgumentSatisfies(Encoding.UTF8.GetByteCount(s) <= length, () => s, "String is too long.");

			var bytes = Encoding.UTF8.GetBytes(s);
			buffer.WriteByte((byte)bytes.Length);
			foreach (var b in bytes)
				buffer.WriteByte(b);
		}

		/// <summary>
		///   Reads a string of the given length from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the string should be read from.</param>
		/// <param name="length">The maximum length of the string.</param>
		public static string ReadString(this BufferReader buffer, int length)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			Assert.ArgumentInRange(length, () => length, 1, Byte.MaxValue);

			int size = buffer.ReadByte();
			var skipBytes = 0;

			if (size > length)
			{
				NetworkLog.ClientWarn("Received a string that exceeds the maximum allowed length. String truncated.");
				skipBytes = size - length;
				size = length;
			}

			var bytes = new byte[size];
			for (var i = 0; i < size; ++i)
				bytes[i] = buffer.ReadByte();

			// Skip the remaining bytes if the string is too long
			for (var i = 0; i < skipBytes; ++i)
				buffer.ReadByte();

			return Encoding.UTF8.GetString(bytes);
		}
	}
}