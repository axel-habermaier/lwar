namespace Pegasus.Platform.Memory
{
	using System;
	using System.Diagnostics;
	using System.Text;
	using Logging;
	using Utilities;

	/// <summary>
	///     Wraps a byte buffer, providing methods for reading fundamental data types from the buffer.
	/// </summary>
	public struct BufferReader : IDisposable
	{
		/// <summary>
		///    Represents a deserialization function.
		/// </summary>
		/// <typeparam name="T">The type of the object that should be deserialized.</typeparam>
		/// <param name="reader">The reader that is used to read the object that should be deserialized.</param>
		public delegate T Deserializer<out T>(ref BufferReader reader);

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
		///     Reads from the given buffer. The valid data of the buffer can be found within the
		///     range [0, buffer.Length).
		/// </summary>
		/// <param name="buffer">The buffer from which the data should be read.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public BufferReader(byte[] buffer, Endianess endianess)
			: this(new ArraySegment<byte>(buffer, 0, buffer.Length), endianess)
		{
		}

		/// <summary>
		///     Reads from the given buffer. The valid data of the buffer can be found within the
		///     range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer from which the data should be read.</param>
		/// <param name="offset">The offset to the first valid byte in the buffer.</param>
		/// <param name="length">The length of the buffer in bytes.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public BufferReader(byte[] buffer, int offset, int length, Endianess endianess)
			: this(new ArraySegment<byte>(buffer, offset, length), endianess)
		{
		}

		/// <summary>
		///     Reads from the given buffer. The valid data of the buffer can be found within the
		///     range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer from which the data should be read.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public BufferReader(ArraySegment<byte> buffer, Endianess endianess)
			: this()
		{
			Assert.ArgumentNotNull(buffer.Array);

			_endianess = endianess;
			_buffer = buffer;

			Reset();
		}

		/// <summary>
		///     Gets the buffer that is read from.
		/// </summary>
		public byte[] Buffer
		{
			get { return _buffer.Array; }
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
				if (!_pointer.IsInitialized)
					_pointer = new BufferPointer(_buffer.Array, _buffer.Offset, _buffer.Count);

				return _pointer.Pointer + _readPosition;
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_pointer.Dispose();
			_pointer = new BufferPointer();
			_buffer = new ArraySegment<byte>();
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
			Assert.NotNull(_buffer.Array, "No buffer has been set for reading.");
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

		/// <summary>
		///     Reads an UTF8-encoded string of unbounded length from the buffer.
		/// </summary>
		public string ReadString()
		{
			var bytes = ReadByteArray();
			return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}

		/// <summary>
		///     Reads an UTF8-encoded string of the given length from the buffer.
		/// </summary>
		/// <param name="maxLength">The maximum length of the string.</param>
		public string ReadString(byte maxLength)
		{
			var actualLength = ReadByte();
			return ReadString(actualLength, maxLength);
		}

		/// <summary>
		///     Reads an UTF8-encoded string of the given length from the buffer.
		/// </summary>
		/// <param name="maxLength">The maximum length of the string.</param>
		public string ReadString(ushort maxLength)
		{
			var actualLength = ReadUInt16();
			return ReadString(actualLength, maxLength);
		}

		/// <summary>
		///     Reads an UTF8-encoded string of the given length from the buffer.
		/// </summary>
		/// <param name="maxLength">The maximum length of the string.</param>
		public string ReadString(int maxLength)
		{
			var actualLength = ReadInt32();
			return ReadString(actualLength, maxLength);
		}

		/// <summary>
		///     Reads an UTF8-encoded string of the given length from the buffer.
		/// </summary>
		/// <param name="actualLength">The actual length of the string.</param>
		/// <param name="maxLength">The maximum length of the string.</param>
		private string ReadString(int actualLength, int maxLength)
		{
			Assert.ArgumentSatisfies(maxLength > 0, "Invalid maximum length.");

			var skipBytes = 0;
			if (actualLength > maxLength)
			{
				Log.Warn("Reading a string that exceeds the maximum allowed length. String truncated.");
				skipBytes = actualLength - maxLength;
				actualLength = maxLength;
			}

			var bytes = new byte[actualLength];
			Copy(bytes);

			// Skip the remaining bytes if the string is too long
			for (var i = 0; i < skipBytes; ++i)
				ReadByte();

			return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}

		/// <summary>
		///     Reads a byte array of the given length.
		/// </summary>
		/// <param name="length">The length of the byte array.</param>
		public byte[] ReadByteArray(int length)
		{
			Assert.ArgumentSatisfies(length > 0, "Invalid length.");
			ValidateCanRead(length);

			var byteArray = new byte[length];
			Array.Copy(_buffer.Array, _readPosition, byteArray, 0, length);
			_readPosition += length;

			return byteArray;
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

		/// <summary>
		///     Tries to deserialize an object of the given type from the buffer. Either, all reads succeed or the read position of
		///     the buffer remains unmodified if any reads are out of bounds. Returns true to indicate that the object has been
		///     successfully deserialized.
		/// </summary>
		/// <typeparam name="T">The type of the object that should be deserialized.</typeparam>
		/// <param name="obj">The object that the deserialized values should be written to.</param>
		/// <param name="deserializer">The deserializer that should be used to deserialize the object.</param>
		public bool TryRead<T>(out T obj, Deserializer<T> deserializer)
		{
			Assert.ArgumentNotNull(deserializer);

			var offset = _readPosition;
			try
			{
				obj = deserializer(ref this);
				return true;
			}
			catch (IndexOutOfRangeException)
			{
				_readPosition = offset;
				obj = default(T);
				return false;
			}
		}
	}
}