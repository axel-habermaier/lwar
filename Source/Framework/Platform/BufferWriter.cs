using System;

namespace Pegasus.Framework.Platform
{
	/// <summary>
	///   Wraps a byte buffer, providing methods for writing fundamental data types to the buffer.
	/// </summary>
	public struct BufferWriter
	{
		/// <summary>
		///   The buffer to which the data is written.
		/// </summary>
		private readonly ArraySegment<byte> _buffer;

		/// <summary>
		///   The current write position.
		/// </summary>
		private int _writePosition;

		/// <summary>
		///   Initializes a new instance. Data is therefore written to the buffer within the range [0, buffer.Length).
		/// </summary>
		/// <param name="buffer">The buffer to which the data should be written.</param>
		public BufferWriter(byte[] buffer)
			: this(buffer, 0, buffer.Length)
		{
		}

		/// <summary>
		///   Initializes a new instance. Data is therefore written to the buffer within the range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer to which the data should be written.</param>
		/// <param name="offset"> The offset to the first byte of the buffer that should be written.</param>
		/// <param name="length">The length of the buffer in bytes.</param>
		public BufferWriter(byte[] buffer, int offset, int length)
			: this(new ArraySegment<byte>(buffer, offset, length))
		{
		}

		/// <summary>
		///   Initializes a new instance. Data is therefore written to the buffer within the range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer to which the data should be written.</param>
		public BufferWriter(ArraySegment<byte> buffer)
			: this()
		{
			_buffer = buffer;
			Reset();
		}

		/// <summary>
		///   Gets the number of bytes that have been written to the buffer starting at the offset. If the buffer writer is reset,
		///   then the number of written bytes is reset as well.
		/// </summary>
		public int WrittenBytes
		{
			get { return _writePosition - _buffer.Offset; }
		}

		/// <summary>
		///   Resets the write position so that all content can be overwritten.
		/// </summary>
		public void Reset()
		{
			_writePosition = _buffer.Offset;
		}

		/// <summary>
		///   Appends the given byte value to the end of the payload.
		/// </summary>
		/// <param name="value">The value that should be appended.</param>
		private void Append(byte value)
		{
			Assert.That(_writePosition < _buffer.Offset + _buffer.Count,
						"Attempted to write outside the valid data area of the buffer.");
			_buffer.Array[_writePosition++] = value;
		}

		/// <summary>
		///   Writes a Boolean value into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(bool value)
		{
			Append((byte)(value ? 1 : 0));
		}

		/// <summary>
		///   Writes a signed byte into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(sbyte value)
		{
			Append((byte)value);
		}

		/// <summary>
		///   Writes an unsigned byte into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(byte value)
		{
			Append(value);
		}

		/// <summary>
		///   Writes a 2 byte signed integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(short value)
		{
			Append((byte)value);
			Append((byte)(value >> 8));
		}

		/// <summary>
		///   Writes a 2 byte unsigned integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(ushort value)
		{
			Append((byte)value);
			Append((byte)(value >> 8));
		}

		/// <summary>
		///   Writes an UTF-16 character into the packet.
		/// </summary>
		/// <param name="character">The value that should be written.</param>
		public void Write(char character)
		{
			Write((ushort)character);
		}

		/// <summary>
		///   Writes a 4 byte signed integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(int value)
		{
			Append((byte)value);
			Append((byte)(value >> 8));
			Append((byte)(value >> 16));
			Append((byte)(value >> 24));
		}

		/// <summary>
		///   Writes a 4 byte unsigned integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(uint value)
		{
			Append((byte)value);
			Append((byte)(value >> 8));
			Append((byte)(value >> 16));
			Append((byte)(value >> 24));
		}

		/// <summary>
		///   Writes an 8 byte signed integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(long value)
		{
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
		///   Writes an 8 byte unsigned integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(ulong value)
		{
			Append((byte)value);
			Append((byte)(value >> 8));
			Append((byte)(value >> 16));
			Append((byte)(value >> 24));
			Append((byte)(value >> 32));
			Append((byte)(value >> 40));
			Append((byte)(value >> 48));
			Append((byte)(value >> 56));
		}
	}
}