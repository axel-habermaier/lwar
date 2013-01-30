using System;

namespace Pegasus.Framework.Platform
{
	using System.Text;
	using Math;

	/// <summary>
	///   Wraps a byte buffer, providing methods for reading fundamental data types from the buffer.
	/// </summary>
	public class BufferReader : PooledObject<BufferReader>
	{
		/// <summary>
		///   The buffer from which the data is read.
		/// </summary>
		private ArraySegment<byte> _buffer;

		/// <summary>
		///   Indicates the which endian encoding the buffer uses.
		/// </summary>
		private Endianess _endianess;

		/// <summary>
		///   The current read position.
		/// </summary>
		private int _readPosition;

		/// <summary>
		///   Gets a value indicating whether the end of the buffer has been reached.
		/// </summary>
		public bool EndOfBuffer
		{
			get { return _readPosition - _buffer.Offset >= _buffer.Count; }
		}

		/// <summary>
		///   Gets the number of bytes that have been read from the buffer.
		/// </summary>
		public int Count
		{
			get { return _readPosition - _buffer.Offset; }
		}

		/// <summary>
		///   Creates a new instance. The valid data of the buffer can be found within the
		///   range [0, buffer.Length).
		/// </summary>
		/// <param name="buffer">The buffer from which the data should be read.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public static BufferReader Create(byte[] buffer, Endianess endianess = Endianess.Little)
		{
			return Create(new ArraySegment<byte>(buffer, 0, buffer.Length), endianess);
		}

		/// <summary>
		///   Creates a new instance. The valid data of the buffer can be found within the
		///   range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer from which the data should be read.</param>
		/// <param name="offset">The offset to the first valid byte in the buffer.</param>
		/// <param name="length">The length of the buffer in bytes.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public static BufferReader Create(byte[] buffer, int offset, int length, Endianess endianess = Endianess.Little)
		{
			return Create(new ArraySegment<byte>(buffer, offset, length), endianess);
		}

		/// <summary>
		///   Creates a new instance. The valid data of the buffer can be found within the
		///   range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer from which the data should be read.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public static BufferReader Create(ArraySegment<byte> buffer, Endianess endianess = Endianess.Little)
		{
			var reader = GetInstance();
			reader._endianess = endianess;
			reader._buffer = buffer;
			reader.Reset();
			return reader;
		}

		/// <summary>
		///   Resets the read position so that all content can be read again.
		/// </summary>
		public void Reset()
		{
			_readPosition = _buffer.Offset;
		}

		/// <summary>
		///   Checks whether the given number of bytes can be read from the buffer.
		/// </summary>
		/// <param name="size">The number of bytes that should be checked.</param>
		public bool CanRead(int size)
		{
			return _readPosition + size < _buffer.Offset + _buffer.Count;
		}

		/// <summary>
		///   Reads a Boolean value.
		/// </summary>
		public bool ReadBoolean()
		{
			return ReadByte() == 1;
		}

		/// <summary>
		///   Reads a signed byte.
		/// </summary>
		public sbyte ReadSignedByte()
		{
			return (sbyte)ReadByte();
		}

		/// <summary>
		///   Reads an unsigned byte.
		/// </summary>
		public byte ReadByte()
		{
			Assert.That(!EndOfBuffer, "Read past the end of the valid data area of the buffer.");
			return _buffer.Array[_readPosition++];
		}

		/// <summary>
		///   Reads a 2 byte signed integer.
		/// </summary>
		public short ReadInt16()
		{
			var value = (short)(ReadByte() | (ReadByte() << 8));

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///   Reads a 2 byte unsigned integer.
		/// </summary>
		public ushort ReadUInt16()
		{
			var value = (ushort)(ReadByte() | (ReadByte() << 8));

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///   Reads an UTF-16 character.
		/// </summary>
		public char ReadCharacter()
		{
			return (char)ReadUInt16();
		}

		/// <summary>
		///   Reads a 4 byte signed integer.
		/// </summary>
		public int ReadInt32()
		{
			var value = ReadByte() | (ReadByte() << 8) | (ReadByte() << 16) | (ReadByte() << 24);

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///   Reads a 4 byte unsigned integer.
		/// </summary>
		public uint ReadUInt32()
		{
			var value = (uint)(ReadByte() | (ReadByte() << 8) | (ReadByte() << 16) | (ReadByte() << 24));

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///   Reads an 8 byte signed integer.
		/// </summary>
		public long ReadInt64()
		{
			var value = ReadByte() |
						((long)(ReadByte()) << 8) |
						((long)(ReadByte()) << 16) |
						((long)(ReadByte()) << 24) |
						((long)(ReadByte()) << 32) |
						((long)(ReadByte()) << 40) |
						((long)(ReadByte()) << 48) |
						((long)(ReadByte()) << 56);

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///   Reads an 8 byte unsigned integer.
		/// </summary>
		public ulong ReadUInt64()
		{
			var value = ReadByte() |
						((ulong)(ReadByte()) << 8) |
						((ulong)(ReadByte()) << 16) |
						((ulong)(ReadByte()) << 24) |
						((ulong)(ReadByte()) << 32) |
						((ulong)(ReadByte()) << 40) |
						((ulong)(ReadByte()) << 48) |
						((ulong)(ReadByte()) << 56);

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///   Reads a 4 byte signed fixed-point value with 8 bits for the fractional part.
		/// </summary>
		public Fixed8 ReadFixed8()
		{
			return new Fixed8 { RawValue = ReadInt32() };
		}

		/// <summary>
		///   Reads a 4 byte signed fixed-point value with 16 bits for the fractional part.
		/// </summary>
		public Fixed16 ReadFixed16()
		{
			return new Fixed16 { RawValue = ReadInt32() };
		}

		// ReSharper disable InconsistentNaming

		/// <summary>
		///   Reads a two-component vector of Fixed8.
		/// </summary>
		public Vector2f8 ReadVector2f8()
		{
			return new Vector2f8(ReadFixed8(), ReadFixed8());
		}

		/// <summary>
		///   Reads a two-component vector of Fixed16.
		/// </summary>
		public Vector2f16 ReadVector2f16()
		{
			return new Vector2f16(ReadFixed16(), ReadFixed16());
		}

		/// <summary>
		///   Reads a two-component vector of integers.
		/// </summary>
		public Vector2i ReadVector2i()
		{
			return new Vector2i(ReadInt32(), ReadInt32());
		}

		// ReSharper restore InconsistentNaming

		/// <summary>
		///   Reads a string.
		/// </summary>
		public string ReadString()
		{
			return Encoding.UTF8.GetString(ReadByteArray());
		}

		/// <summary>
		///   Reads a byte array.
		/// </summary>
		public byte[] ReadByteArray()
		{
			var length = ReadInt32();
			var byteArray = new byte[length];

			Array.Copy(_buffer.Array, _readPosition, byteArray, 0, length);
			_readPosition += length;

			return byteArray;
		}
	}
}