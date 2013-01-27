using System;

namespace Pegasus.Framework.Platform
{
	using System.Diagnostics;

	/// <summary>
	///   Wraps a byte buffer, providing methods for reading fundamental data types from the buffer.
	/// </summary>
	public struct BufferReader
	{
		/// <summary>
		///   The buffer from which the data is read.
		/// </summary>
		private readonly ArraySegment<byte> _buffer;

		/// <summary>
		///   The current read position.
		/// </summary>
		private int _readPosition;

		/// <summary>
		///   Initializes a new instance. The valid data of the buffer can be found within the
		///   range [0, buffer.Length).
		/// </summary>
		/// <param name="buffer">The buffer from which the data should be read.</param>
		public BufferReader(byte[] buffer)
			: this(new ArraySegment<byte>(buffer, 0, buffer.Length))
		{
		}

		/// <summary>
		///   Initializes a new instance. The valid data of the buffer can be found within the
		///   range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer from which the data should be read.</param>
		/// <param name="offset">The offset to the first valid byte in the buffer.</param>
		/// <param name="length">The length of the buffer in bytes.</param>
		public BufferReader(byte[] buffer, int offset, int length)
			: this(new ArraySegment<byte>(buffer, offset, length))
		{
		}

		/// <summary>
		///   Initializes a new instance. The valid data of the buffer can be found within the
		///   range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer from which the data should be read.</param>
		public BufferReader(ArraySegment<byte> buffer)
			: this()
		{
			_buffer = buffer;
			Reset();
		}

		/// <summary>
		///   Gets a value indicating whether the end of the buffer has been reached.
		/// </summary>
		public bool EndOfBuffer
		{
			get { return _readPosition - _buffer.Offset >= _buffer.Count; }
		}

		/// <summary>
		///   Resets the read position so that all content can be read again.
		/// </summary>
		public void Reset()
		{
			_readPosition = _buffer.Offset;
		}

		/// <summary>
		///   In debug builds, verifies that reads remain inside the valid data area.
		/// </summary>
		[Conditional("DEBUG"), DebuggerHidden]
		private void ValidateReadPosition()
		{
			Assert.That(!EndOfBuffer, "Read past the end of the valid data area of the buffer.");
		}

		/// <summary>
		///   Reads a Boolean value from the packet.
		/// </summary>
		public bool ReadBoolean()
		{
			ValidateReadPosition();
			var value = _buffer.Array[_readPosition++];
			return value == 1;
		}

		/// <summary>
		///   Reads a signed byte from the packet.
		/// </summary>
		public sbyte ReadSignedByte()
		{
			ValidateReadPosition();
			return (sbyte)_buffer.Array[_readPosition++];
		}

		/// <summary>
		///   Reads an unsigned byte from the packet.
		/// </summary>
		public byte ReadByte()
		{
			ValidateReadPosition();
			return _buffer.Array[_readPosition++];
		}

		/// <summary>
		///   Reads a 2 byte signed integer from the packet.
		/// </summary>
		public short ReadInt16()
		{
			ValidateReadPosition();
			return (short)(_buffer.Array[_readPosition++] | (_buffer.Array[_readPosition++] << 8));
		}

		/// <summary>
		///   Reads a 2 byte unsigned integer from the packet.
		/// </summary>
		public ushort ReadUInt16()
		{
			ValidateReadPosition();
			return (ushort)(_buffer.Array[_readPosition++] | (_buffer.Array[_readPosition++] << 8));
		}

		/// <summary>
		///   Reads an UTF-16 character from the packet.
		/// </summary>
		public char ReadCharacter()
		{
			return (char)ReadUInt16();
		}

		/// <summary>
		///   Reads a 4 byte signed integer from the packet.
		/// </summary>
		public int ReadInt32()
		{
			ValidateReadPosition();
			return _buffer.Array[_readPosition++] |
				   (_buffer.Array[_readPosition++] << 8) |
				   (_buffer.Array[_readPosition++] << 16) |
				   (_buffer.Array[_readPosition++] << 24);
		}

		/// <summary>
		///   Reads a 4 byte unsigned integer from the packet.
		/// </summary>
		public uint ReadUInt32()
		{
			ValidateReadPosition();
			return (uint)(_buffer.Array[_readPosition++] |
						  (_buffer.Array[_readPosition++] << 8) |
						  (_buffer.Array[_readPosition++] << 16) |
						  (_buffer.Array[_readPosition++] << 24));
		}

		/// <summary>
		///   Reads an 8 byte signed integer from the packet.
		/// </summary>
		public long ReadInt64()
		{
			ValidateReadPosition();
			return _buffer.Array[_readPosition++] |
				  ((long)(_buffer.Array[_readPosition++]) << 8) |
				  ((long)(_buffer.Array[_readPosition++]) << 16) |
				  ((long)(_buffer.Array[_readPosition++]) << 24) |
				  ((long)(_buffer.Array[_readPosition++]) << 32) |
				  ((long)(_buffer.Array[_readPosition++]) << 40) |
				  ((long)(_buffer.Array[_readPosition++]) << 48) |
				  ((long)(_buffer.Array[_readPosition++]) << 56);
		}

		/// <summary>
		///   Reads an 8 byte unsigned integer from the packet.
		/// </summary>
		public ulong ReadUInt64()
		{
			ValidateReadPosition();
			return  _buffer.Array[_readPosition++] |
					((ulong)(_buffer.Array[_readPosition++]) << 8) |
					((ulong)(_buffer.Array[_readPosition++]) << 16) |
					((ulong)(_buffer.Array[_readPosition++]) << 24) |
					((ulong)(_buffer.Array[_readPosition++]) << 32) |
					((ulong)(_buffer.Array[_readPosition++]) << 40) |
					((ulong)(_buffer.Array[_readPosition++]) << 48) |
					((ulong)(_buffer.Array[_readPosition++]) << 56);
		}
	}
}