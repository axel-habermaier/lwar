﻿using System;

namespace Pegasus.Framework.Platform
{
	using System.Diagnostics;
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
			return _readPosition + size <= _buffer.Offset + _buffer.Count;
		}

		/// <summary>
		///   Checks whether the given number of bytes can be read from the buffer and throws an exception if not.
		/// </summary>
		/// <param name="size">The number of bytes that should be checked.</param>
		[DebuggerHidden]
		private void ValidateCanRead(int size)
		{
			if (!CanRead(size))
				throw new IndexOutOfRangeException("Attempted to read past the end of the buffer.");
		}

		/// <summary>
		///   Reads an unsigned byte.
		/// </summary>
		private byte Next()
		{
			return _buffer.Array[_readPosition++];
		}

		/// <summary>
		///   Reads a Boolean value.
		/// </summary>
		public bool ReadBoolean()
		{
			ValidateCanRead(1);
			return Next() == 1;
		}

		/// <summary>
		///   Reads a signed byte.
		/// </summary>
		public sbyte ReadSignedByte()
		{
			ValidateCanRead(1);
			return (sbyte)Next();
		}

		/// <summary>
		///   Reads an unsigned byte.
		/// </summary>
		public byte ReadByte()
		{
			ValidateCanRead(1);
			return Next();
		}

		/// <summary>
		///   Reads a 2 byte signed integer.
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
		///   Reads a 2 byte unsigned integer.
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
			ValidateCanRead(4);
			var value = Next() | (Next() << 8) | (Next() << 16) | (Next() << 24);

			if (_endianess != PlatformInfo.Endianess)
				value = EndianConverter.Convert(value);

			return value;
		}

		/// <summary>
		///   Reads a 4 byte unsigned integer.
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
		///   Reads an 8 byte signed integer.
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
		///   Reads an 8 byte unsigned integer.
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
		///   Reads an UTF8 string.
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
			ValidateCanRead(4);
			var length = ReadInt32();
			ValidateCanRead(length);

			var byteArray = new byte[length];
			Array.Copy(_buffer.Array, _readPosition, byteArray, 0, length);
			_readPosition += length;

			return byteArray;
		}

		/// <summary>
		/// Returns all remaining bytes.
		/// </summary>
		public byte[] ReadToEnd()
		{
			var data = new byte[_buffer.Count - _readPosition];
			Array.Copy(_buffer.Array, _readPosition, data, 0, data.Length);
			_readPosition += data.Length;
			return data;
		}

		/// <summary>
		///   Tries to deserialize an object of the given type from the buffer. Either, all reads succeed or the read position of
		///   the buffer remains unmodified if any reads are out of bounds. Returns true to indicate that the object has been
		///   successfully deserialized.
		/// </summary>
		/// <typeparam name="T">The type of the object that should be deserialized.</typeparam>
		/// <param name="obj">The object that the deserialized values should be written to.</param>
		/// <param name="deserializer">The deserializer that should be used to deserialize the object.</param>
		public bool TryRead<T>(T obj, Action<BufferReader, T> deserializer)
		{
			Assert.ArgumentNotNull(deserializer, () => deserializer);

			var offset = _readPosition;
			try
			{
				deserializer(this, obj);
				return true;
			}
			catch (IndexOutOfRangeException)
			{
				_readPosition = offset;
				return false;
			}
		}
	}
}