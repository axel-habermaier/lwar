using System;

namespace Lwar.Client
{
	using System.Text;
	using Gameplay;
	using Pegasus.Framework;
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
			buffer.WriteUInt16(identifier.Id);
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
		///   Writes a fixed-size string of the given length into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the string should be written into.</param>
		/// <param name="s">The string that should be written into the buffer.</param>
		/// <param name="length">The fixed length of the string, including the termiating '\0'.</param>
		public static void WriteString(this BufferWriter buffer, string s, int length)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			Assert.ArgumentNotNull(s, () => s);
			Assert.ArgumentInRange(length, () => length, 1, Int32.MaxValue);
			Assert.ArgumentSatisfies(Encoding.UTF8.GetByteCount(s) < length - 1, () => s, "String is too long.");

			var bytes = Encoding.UTF8.GetBytes(s);
			foreach (var b in bytes)
				buffer.WriteByte(b);

			// Fill the remaining space with 0s
			for (var i = 0; i < length - bytes.Length; ++i)
				buffer.WriteByte(0);
		}

		/// <summary>
		///   Reads a fixed-size string of the given maximum length from the buffer.
		/// </summary>
		/// <param name="buffer">The buffer the string should be read from.</param>
		/// <param name="length">The fixed length of the string, including the termiating '\0'.</param>
		public static string ReadString(this BufferReader buffer, int length)
		{
			Assert.ArgumentNotNull(buffer, () => buffer);
			Assert.ArgumentInRange(length, () => length, 1, Int32.MaxValue);

			var bytes = new byte[length];
			var count = 0;
			for (var i = 0; i < length; ++i)
			{
				bytes[i] = buffer.ReadByte();
				if (bytes[i] != 0)
					++count;
			}

			return Encoding.UTF8.GetString(bytes, 0, count);
		}
	}
}