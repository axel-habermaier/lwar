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
		///     The writer that is used to write the binary data.
		/// </summary>
		private readonly BinaryWriter _writer;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public BufferWriter()
		{
			_writer = new BinaryWriter(_buffer);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_buffer.Dispose();
			_writer.Dispose();
		}

		/// <summary>
		///     Writes a Boolean value.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteBoolean(bool value)
		{
			_writer.Write((byte)(value ? 1 : 0));
		}

		/// <summary>
		///     Writes a signed byte.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteSByte(sbyte value)
		{
			_writer.Write((byte)value);
		}

		/// <summary>
		///     Writes an unsigned byte.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteByte(byte value)
		{
			_writer.Write(value);
		}

		/// <summary>
		///     Writes a 2 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteInt16(short value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			_writer.Write(value);
		}

		/// <summary>
		///     Writes a 2 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteUInt16(ushort value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			_writer.Write(value);
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

			_writer.Write(value);
		}

		/// <summary>
		///     Writes a 4 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteUInt32(uint value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			_writer.Write(value);
		}

		/// <summary>
		///     Writes an 8 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteInt64(long value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			_writer.Write(value);
		}

		/// <summary>
		///     Writes an 8 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteUInt64(ulong value)
		{
			if (PlatformInfo.Endianess != Endianess.Little)
				value = EndianConverter.Convert(value);

			_writer.Write(value);
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