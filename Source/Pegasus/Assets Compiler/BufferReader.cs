namespace Pegasus.AssetsCompiler
{
	using System;
	using System.Diagnostics;
	using System.Text;
	using Platform;
	using Platform.Memory;

	/// <summary>
	///     Wraps a byte buffer, providing methods for reading fundamental data types from the buffer.
	/// </summary>
	public class BufferReader
	{
		/// <summary>
		///     Indicates the which endian encoding the buffer uses.
		/// </summary>
		private readonly Endianess _endianess;

		/// <summary>
		///     The buffer from which the data is read.
		/// </summary>
		private ArraySegment<byte> _buffer;

		/// <summary>
		///     A pointer to the first byte of the buffer.
		/// </summary>
		private BufferPointer _pointer;

		/// <summary>
		///     The current read position.
		/// </summary>
		private int _readPosition;

		/// <summary>
		///     Creates a new instance. The valid data of the buffer can be found within the
		///     range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer from which the data should be read.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public BufferReader(byte[] buffer, Endianess endianess = Endianess.Little)
		{
			_endianess = endianess;
			_buffer = new ArraySegment<byte>(buffer);
			Reset();
		}

		/// <summary>
		///     Gets a value indicating whether the end of the buffer has been reached.
		/// </summary>
		public bool EndOfBuffer
		{
			get { return _readPosition - _buffer.Offset >= _buffer.Count; }
		}

		/// <summary>
		///     Gets the number of bytes that have been read from the buffer.
		/// </summary>
		public int Count
		{
			get { return _readPosition - _buffer.Offset; }
		}

		/// <summary>
		///     Gets the size of the entire buffer in bytes.
		/// </summary>
		public int BufferSize
		{
			get { return _buffer.Count; }
		}

		/// <summary>
		///     Gets a pointer to the next byte of the buffer that should be read.
		/// </summary>
		public unsafe byte* Pointer
		{
			get
			{
				if (_pointer == null)
					_pointer = new BufferPointer(_buffer.Array, _buffer.Offset, _buffer.Count);

				return _pointer.Pointer + _readPosition;
			}
		}

		/// <summary>
		///     Resets the read position so that all content can be read again.
		/// </summary>
		public void Reset()
		{
			_readPosition = _buffer.Offset;
		}

		/// <summary>
		///     Checks whether the given number of bytes can be read from the buffer.
		/// </summary>
		/// <param name="size">The number of bytes that should be checked.</param>
		public bool CanRead(int size)
		{
			return _readPosition + size <= _buffer.Offset + _buffer.Count;
		}

		/// <summary>
		///     Skips the given number of bytes.
		/// </summary>
		/// <param name="count">The number of bytes that should be skipped.</param>
		public void Skip(int count)
		{
			Assert.ArgumentInRange(count, 0, Int32.MaxValue);
			ValidateCanRead(count);
			_readPosition += count;
		}

		/// <summary>
		///     Checks whether the given number of bytes can be read from the buffer and throws an exception if not.
		/// </summary>
		/// <param name="size">The number of bytes that should be checked.</param>
		[DebuggerHidden]
		private void ValidateCanRead(int size)
		{
			if (!CanRead(size))
				throw new IndexOutOfRangeException("Attempted to read past the end of the buffer.");
		}

		/// <summary>
		///     Reads an unsigned byte.
		/// </summary>
		private byte Next()
		{
			return _buffer.Array[_readPosition++];
		}

		/// <summary>
		///     Reads a Boolean value.
		/// </summary>
		public bool ReadBoolean()
		{
			ValidateCanRead(1);
			return Next() == 1;
		}

		/// <summary>
		///     Reads a signed byte.
		/// </summary>
		public sbyte ReadSignedByte()
		{
			ValidateCanRead(1);
			return (sbyte)Next();
		}

		/// <summary>
		///     Reads an unsigned byte.
		/// </summary>
		public byte ReadByte()
		{
			ValidateCanRead(1);
			return Next();
		}

		/// <summary>
		///     Reads a 2 byte signed integer.
		/// </summary>
		public short ReadInt16()
		{
			ValidateCanRead(2);
			var value = (short)(Next() | (Next() << 8));

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///     Reads a 2 byte unsigned integer.
		/// </summary>
		public ushort ReadUInt16()
		{
			ValidateCanRead(2);
			var value = (ushort)(Next() | (Next() << 8));

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///     Reads an UTF-16 character.
		/// </summary>
		public char ReadCharacter()
		{
			return (char)ReadUInt16();
		}

		/// <summary>
		///     Reads a 4 byte signed integer.
		/// </summary>
		public int ReadInt32()
		{
			ValidateCanRead(4);
			var value = Next() | (Next() << 8) | (Next() << 16) | (Next() << 24);

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///     Reads a 4 byte unsigned integer.
		/// </summary>
		public uint ReadUInt32()
		{
			ValidateCanRead(4);
			var value = (uint)(Next() | (Next() << 8) | (Next() << 16) | (Next() << 24));

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///     Reads an 8 byte signed integer.
		/// </summary>
		public long ReadInt64()
		{
			ValidateCanRead(8);
			var value = Next() |
						((long)(Next()) << 8) |
						((long)(Next()) << 16) |
						((long)(Next()) << 24) |
						((long)(Next()) << 32) |
						((long)(Next()) << 40) |
						((long)(Next()) << 48) |
						((long)(Next()) << 56);

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///     Reads an 8 byte unsigned integer.
		/// </summary>
		public ulong ReadUInt64()
		{
			ValidateCanRead(8);
			var value = Next() |
						((ulong)(Next()) << 8) |
						((ulong)(Next()) << 16) |
						((ulong)(Next()) << 24) |
						((ulong)(Next()) << 32) |
						((ulong)(Next()) << 40) |
						((ulong)(Next()) << 48) |
						((ulong)(Next()) << 56);

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		// ReSharper restore InconsistentNaming

		/// <summary>
		///     Reads an UTF8 string.
		/// </summary>
		public string ReadString()
		{
			return Encoding.UTF8.GetString(ReadByteArray());
		}

		/// <summary>
		///     Reads a byte array.
		/// </summary>
		public byte[] ReadByteArray()
		{
			ValidateCanRead(4);
			var length = ReadInt32();
			ValidateCanRead(length);

			var byteArray = new byte[length];
			Array.Copy(_buffer.Array, _readPosition, byteArray, 0, length);
			_readPosition += length;

			return byteArray;
		}

		/// <summary>
		///     Copies the requested number of bytes into the buffer, starting at the given offset.
		/// </summary>
		/// <param name="buffer">The buffer into which the data should be copied.</param>
		public void Copy(byte[] buffer)
		{
			Copy(buffer, 0, buffer.Length);
		}

		/// <summary>
		///     Copies the requested number of bytes into the buffer, starting at the given offset.
		/// </summary>
		/// <param name="buffer">The buffer into which the data should be copied.</param>
		/// <param name="offset">The first byte in the buffer that should be written.</param>
		/// <param name="length">The number of bytes that should be copied.</param>
		public void Copy(byte[] buffer, int offset, int length)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.That(offset + length <= buffer.Length, "Out of bounds.");

			ValidateCanRead(length);
			Array.Copy(_buffer.Array, _readPosition, buffer, offset, length);
			_readPosition += length;
		}
	}
}