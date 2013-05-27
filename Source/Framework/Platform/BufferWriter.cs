using System;

namespace Pegasus.Framework.Platform
{
	using System.Diagnostics;
	using System.Text;
	using Math;
	using Memory;

	/// <summary>
	///   Wraps a byte buffer, providing methods for writing fundamental data types to the buffer.
	/// </summary>
	public class BufferWriter : PooledObject<BufferWriter>
	{
		/// <summary>
		///   The buffer to which the data is written.
		/// </summary>
		private ArraySegment<byte> _buffer;

		/// <summary>
		///   Indicates the which endian encoding the buffer uses.
		/// </summary>
		private Endianess _endianess;

		/// <summary>
		///   The current write position.
		/// </summary>
		private int _writePosition;

		/// <summary>
		///   Gets the number of bytes that have been written to the buffer.
		/// </summary>
		public int Count
		{
			get { return _writePosition - _buffer.Offset; }
		}

		/// <summary>
		///   Creates a new instance. Data is therefore written to the buffer within the range [0, buffer.Length).
		/// </summary>
		/// <param name="buffer">The buffer to which the data should be written.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public static BufferWriter Create(byte[] buffer, Endianess endianess = Endianess.Little)
		{
			return Create(buffer, 0, buffer.Length, endianess);
		}

		/// <summary>
		///   Creates a new instance. Data is therefore written to the buffer within the range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer to which the data should be written.</param>
		/// <param name="offset"> The offset to the first byte of the buffer that should be written.</param>
		/// <param name="length">The length of the buffer in bytes.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public static BufferWriter Create(byte[] buffer, int offset, int length, Endianess endianess = Endianess.Little)
		{
			return Create(new ArraySegment<byte>(buffer, offset, length), endianess);
		}

		/// <summary>
		///   Creates a new instance. Data is therefore written to the buffer within the range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer to which the data should be written.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public static BufferWriter Create(ArraySegment<byte> buffer, Endianess endianess = Endianess.Little)
		{
			var writer = GetInstance();
			writer._endianess = endianess;
			writer._buffer = buffer;
			writer.Reset();
			return writer;
		}

		/// <summary>
		///   Resets the write position so that all content can be overwritten.
		/// </summary>
		public void Reset()
		{
			_writePosition = _buffer.Offset;
		}

		/// <summary>
		///   Checks whether the given number of bytes can be written to the buffer.
		/// </summary>
		/// <param name="size">The number of bytes that should be checked.</param>
		public bool CanWrite(int size)
		{
			return _writePosition + size <= _buffer.Offset + _buffer.Count;
		}

		/// <summary>
		///   Checks whether the given number of bytes can be written to the buffer and throws an exception if not.
		/// </summary>
		/// <param name="size">The number of bytes that should be checked.</param>
		[DebuggerHidden]
		private void ValidateCanWrite(int size)
		{
			if (!CanWrite(size))
				throw new IndexOutOfRangeException("Attempted to write past the end of the buffer.");
		}

		/// <summary>
		///   Appends the given byte value to the end of the payload.
		/// </summary>
		/// <param name="value">The value that should be appended.</param>
		private void Append(byte value)
		{
			_buffer.Array[_writePosition++] = value;
		}

		/// <summary>
		///   Writes a Boolean value.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteBoolean(bool value)
		{
			ValidateCanWrite(1);
			Append((byte)(value ? 1 : 0));
		}

		/// <summary>
		///   Writes a signed byte.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteSByte(sbyte value)
		{
			ValidateCanWrite(1);
			Append((byte)value);
		}

		/// <summary>
		///   Writes an unsigned byte.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteByte(byte value)
		{
			ValidateCanWrite(1);
			Append(value);
		}

		/// <summary>
		///   Writes a 2 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteInt16(short value)
		{
			ValidateCanWrite(2);
			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			Append((byte)value);
			Append((byte)(value >> 8));
		}

		/// <summary>
		///   Writes a 2 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteUInt16(ushort value)
		{
			ValidateCanWrite(2);
			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			Append((byte)value);
			Append((byte)(value >> 8));
		}

		/// <summary>
		///   Writes an UTF-16 character.
		/// </summary>
		/// <param name="character">The value that should be written.</param>
		public void WriteCharacter(char character)
		{
			ValidateCanWrite(2);
			WriteUInt16(character);
		}

		/// <summary>
		///   Writes a 4 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteInt32(int value)
		{
			ValidateCanWrite(4);
			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			Append((byte)value);
			Append((byte)(value >> 8));
			Append((byte)(value >> 16));
			Append((byte)(value >> 24));
		}

		/// <summary>
		///   Writes a 4 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteUInt32(uint value)
		{
			ValidateCanWrite(4);
			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			Append((byte)value);
			Append((byte)(value >> 8));
			Append((byte)(value >> 16));
			Append((byte)(value >> 24));
		}

		/// <summary>
		///   Writes an 8 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteInt64(long value)
		{
			ValidateCanWrite(8);
			if (_endianess != PlatformInfo.Endianess)
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
		///   Writes an 8 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteUInt64(ulong value)
		{
			ValidateCanWrite(8);
			if (_endianess != PlatformInfo.Endianess)
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
		///   Writes a 4 byte signed fixed-point value with 8 bits for the fractional part.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteFixed8(Fixed8 value)
		{
			WriteInt32(value.RawValue);
		}

		/// <summary>
		///   Writes a 4 byte signed fixed-point value with 16 bits for the fractional part.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteFixed16(Fixed16 value)
		{
			WriteInt32(value.RawValue);
		}

		// ReSharper disable InconsistentNaming

		/// <summary>
		///   Writes a two-component vector of Fixed8.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteVector2f8(Vector2f8 value)
		{
			WriteFixed8(value.X);
			WriteFixed8(value.Y);
		}

		/// <summary>
		///   Writes a two-component vector of Fixed16.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteVector2f16(Vector2f16 value)
		{
			WriteFixed16(value.X);
			WriteFixed16(value.Y);
		}

		/// <summary>
		///   Writes a two-component vector of integers.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteVector2i(Vector2i value)
		{
			WriteInt32(value.X);
			WriteInt32(value.Y);
		}

		// ReSharper restore InconsistentNaming

		/// <summary>
		///   Writes an UTF8 string.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteString(string value)
		{
			Assert.ArgumentNotNull(value, () => value);
			WriteByteArray(Encoding.UTF8.GetBytes(value));
		}

		/// <summary>
		///   Writes a byte array.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteByteArray(byte[] value)
		{
			Assert.ArgumentNotNull(value, () => value);

			WriteInt32(value.Length);
			Copy(value);
		}

		/// <summary>
		///   Copies the given byte array into the buffer.
		/// </summary>
		/// <param name="value">The data that should be copied.</param>
		public void Copy(byte[] value)
		{
			Assert.ArgumentNotNull(value, () => value);

			ValidateCanWrite(value.Length);
			Array.Copy(value, 0, _buffer.Array, _writePosition, value.Length);
			_writePosition += value.Length;
		}

		/// <summary>
		///   Tries to serialize the given object into the buffer. Either, all writes succeed or the buffer remains unmodified
		///   if any writes are out of bounds. Returns true to indicate that all writes have been successful.
		/// </summary>
		/// <typeparam name="T">The type of the object that should be serialized.</typeparam>
		/// <param name="obj">The object that should be serialized into the buffer.</param>
		/// <param name="serializer">The serializer that should be used to atomically modify the buffer.</param>
		public bool TryWrite<T>(T obj, Action<BufferWriter, T> serializer)
		{
			Assert.ArgumentNotNull(serializer, () => serializer);

			var offset = _writePosition;
			try
			{
				serializer(this, obj);
				return true;
			}
			catch (IndexOutOfRangeException)
			{
				_writePosition = offset;
				return false;
			}
		}
	}
}