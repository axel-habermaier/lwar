namespace Pegasus.AssetsCompiler
{
	using System;
	using System.IO;
	using System.Text;
	using Platform;
	using Platform.Memory;

	/// <summary>
	///     Writes binary data to a memory stream using a little endian encoding.
	/// </summary>
	public class BufferWriter : IDisposable
	{
		/// <summary>
		///     The buffer the data is written to.
		/// </summary>
		private readonly MemoryStream _buffer = new MemoryStream(1024 * 1024);

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_buffer.Dispose();
		}

		/// <summary>
		///     Appends the given byte value to the end of the payload.
		/// </summary>
		/// <param name="value">The value that should be appended.</param>
		private void Append(byte value)
		{
			_buffer.WriteByte(value);
		}

		/// <summary>
		///     Writes a Boolean value.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteBoolean(bool value)
		{
			Append((byte)(value ? 1 : 0));
		}

		/// <summary>
		///     Writes a signed byte.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteSByte(sbyte value)
		{
			Append((byte)value);
		}

		/// <summary>
		///     Writes an unsigned byte.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteByte(byte value)
		{
			Append(value);
		}

		/// <summary>
		///     Writes a 2 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteInt16(short value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			Append((byte)value);
			Append((byte)(value >> 8));
		}

		/// <summary>
		///     Writes a 2 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteUInt16(ushort value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			Append((byte)value);
			Append((byte)(value >> 8));
		}

		/// <summary>
		///     Writes an UTF-16 character.
		/// </summary>
		/// <param name="character">The value that should be written.</param>
		public void WriteCharacter(char character)
		{
			WriteUInt16(character);
		}

		/// <summary>
		///     Writes a 4 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteInt32(int value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			Append((byte)value);
			Append((byte)(value >> 8));
			Append((byte)(value >> 16));
			Append((byte)(value >> 24));
		}

		/// <summary>
		///     Writes a 4 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteUInt32(uint value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			Append((byte)value);
			Append((byte)(value >> 8));
			Append((byte)(value >> 16));
			Append((byte)(value >> 24));
		}

		/// <summary>
		///     Writes an 8 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteInt64(long value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			Append((byte)value);
			Append((byte)(value >> 8));
			Append((byte)(value >> 16));
			Append((byte)(value >> 24));
			Append((byte)(value >> 32));
			Append((byte)(value >> 40));
			Append((byte)(value >> 48));
			Append((byte)(value >> 56));
		}

		/// <summary>
		///     Writes an 8 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteUInt64(ulong value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			Append((byte)value);
			Append((byte)(value >> 8));
			Append((byte)(value >> 16));
			Append((byte)(value >> 24));
			Append((byte)(value >> 32));
			Append((byte)(value >> 40));
			Append((byte)(value >> 48));
			Append((byte)(value >> 56));
		}

		// ReSharper restore InconsistentNaming

		/// <summary>
		///     Writes an UTF8 string.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteString(string value)
		{
			Assert.ArgumentNotNull(value);
			WriteByteArray(Encoding.UTF8.GetBytes(value));
		}

		/// <summary>
		///     Writes a byte array.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteByteArray(byte[] value)
		{
			Assert.ArgumentNotNull(value);

			WriteInt32(value.Length);
			Copy(value);
		}

		/// <summary>
		///     Copies the given byte array into the buffer.
		/// </summary>
		/// <param name="data">The data that should be copied.</param>
		public void Copy(byte[] data)
		{
			Assert.ArgumentNotNull(data);
			_buffer.Write(data, 0, data.Length);
		}

		/// <summary>
		///     Converts the buffer to a byte array.
		/// </summary>
		public byte[] ToArray()
		{
			return _buffer.ToArray();
		}
	}
}