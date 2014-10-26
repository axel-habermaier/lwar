﻿namespace Pegasus.Platform.Memory
{
	using System;
	using System.Diagnostics;
	using System.Text;
	using Utilities;

	/// <summary>
	///     Wraps a byte buffer, providing methods for writing fundamental data types to the buffer.
	/// </summary>
	public sealed class BufferWriter : UniquePooledObject
	{
		/// <summary>
		///     The default pool for buffer writer instances.
		/// </summary>
		private static readonly ObjectPool<BufferWriter> DefaultPool =
			new ObjectPool<BufferWriter>(() => new BufferWriter(), hasGlobalLifetime: true);

		/// <summary>
		///     The buffer to which the data is written.
		/// </summary>
		private ArraySegment<byte> _buffer;

		/// <summary>
		///     Indicates the which endian encoding the buffer uses.
		/// </summary>
		private Endianess _endianess;

		/// <summary>
		///     The maximum position that was written by the writer.
		/// </summary>
		private int _maxWritePosition;

		/// <summary>
		///     The current write position.
		/// </summary>
		private int _writePosition;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private BufferWriter()
		{
		}

		/// <summary>
		///     Gets or sets the zero-based write position where the next write operation will be performed.
		/// </summary>
		public int WritePosition
		{
			get { return _writePosition - _buffer.Offset; }
			set
			{
				Assert.InRange(value, 0, _maxWritePosition - _buffer.Offset);
				_writePosition = value + _buffer.Offset;
			}
		}

		/// <summary>
		///     Gets the buffer that is written to.
		/// </summary>
		public byte[] Buffer
		{
			get { return _buffer.Array; }
		}

		/// <summary>
		///     Gets the number of bytes that have been written to the buffer.
		/// </summary>
		public int Count
		{
			get { return _maxWritePosition - _buffer.Offset; }
		}

		/// <summary>
		///     Writes to the given buffer. Data is written to the buffer within the range [0, buffer.Length).
		/// </summary>
		/// <param name="buffer">The buffer to which the data should be written.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public static BufferWriter Create(byte[] buffer, Endianess endianess = Endianess.Little)
		{
			return Create(buffer, 0, buffer.Length, endianess);
		}

		/// <summary>
		///     Writes to the given buffer. Data is written to the buffer within the range [offset, offset + length).
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
		///     Writes to the given buffer. Data is written to the buffer within the range [offset, offset + length).
		/// </summary>
		/// <param name="buffer">The buffer to which the data should be written.</param>
		/// <param name="endianess">Specifies the endianess of the buffer.</param>
		public static BufferWriter Create(ArraySegment<byte> buffer, Endianess endianess = Endianess.Little)
		{
			Assert.ArgumentNotNull(buffer.Array);

			var bufferWriter = DefaultPool.Allocate();
			bufferWriter._endianess = endianess;
			bufferWriter._buffer = buffer;
			bufferWriter.Reset();

			return bufferWriter;
		}

		/// <summary>
		///     Resets the write position so that all content can be overwritten.
		/// </summary>
		public void Reset()
		{
			_writePosition = _buffer.Offset;
			_maxWritePosition = _buffer.Offset;
		}

		/// <summary>
		///     Checks whether the given number of bytes can be written to the buffer.
		/// </summary>
		/// <param name="size">The number of bytes that should be checked.</param>
		public bool CanWrite(int size)
		{
			return _writePosition + size <= _buffer.Offset + _buffer.Count;
		}

		/// <summary>
		///     Skips writing the given number of bytes.
		/// </summary>
		/// <param name="byteCount">The number of bytes that should be skipped.</param>
		public void SkipBytes(int byteCount)
		{
			Assert.ArgumentInRange(byteCount, 0, Int32.MaxValue);
			ValidateCanWrite(byteCount);

			AdvanceWritePosition(byteCount);
		}

		/// <summary>
		///     Advances the write position by the given number of bytes.
		/// </summary>
		/// <param name="bytes">The number of bytes the write position should be advanced.</param>
		private void AdvanceWritePosition(int bytes)
		{
			_writePosition += bytes;
			_maxWritePosition = Math.Max(_maxWritePosition, _writePosition);
		}

		/// <summary>
		///     Checks whether the given number of bytes can be written to the buffer and throws an exception if not.
		/// </summary>
		/// <param name="size">The number of bytes that should be checked.</param>
		[DebuggerHidden]
		private void ValidateCanWrite(int size)
		{
			Assert.NotNull(_buffer.Array, "No buffer has been set for writing.");
			if (!CanWrite(size))
				throw new IndexOutOfRangeException("Attempted to write past the end of the buffer.");
		}

		/// <summary>
		///     Appends the given byte value to the end of the payload.
		/// </summary>
		/// <param name="value">The value that should be appended.</param>
		private void Append(byte value)
		{
			_buffer.Array[_writePosition] = value;
			AdvanceWritePosition(1);
		}

		/// <summary>
		///     Writes a Boolean value.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteBoolean(bool value)
		{
			ValidateCanWrite(1);
			Append((byte)(value ? 1 : 0));
		}

		/// <summary>
		///     Writes a signed byte.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteSByte(sbyte value)
		{
			ValidateCanWrite(1);
			Append((byte)value);
		}

		/// <summary>
		///     Writes an unsigned byte.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void WriteByte(byte value)
		{
			ValidateCanWrite(1);
			Append(value);
		}

		/// <summary>
		///     Writes a 2 byte signed integer.
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
		///     Writes a 2 byte unsigned integer.
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
		///     Writes an UTF-16 character.
		/// </summary>
		/// <param name="character">The value that should be written.</param>
		public void WriteCharacter(char character)
		{
			ValidateCanWrite(2);
			WriteUInt16(character);
		}

		/// <summary>
		///     Writes a 4 byte signed integer.
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
		///     Writes a 4 byte unsigned integer.
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
		///     Writes an 8 byte signed integer.
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
		///     Writes an 8 byte unsigned integer.
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
		/// <param name="value">The data that should be copied.</param>
		public void Copy(byte[] value)
		{
			Assert.ArgumentNotNull(value);

			ValidateCanWrite(value.Length);
			Array.Copy(value, 0, _buffer.Array, _writePosition, value.Length);
			AdvanceWritePosition(value.Length);
		}

		/// <summary>
		///     Tries to serialize the given object into the buffer. Either, all writes succeed or the buffer remains unmodified
		///     if any writes are out of bounds. Returns true to indicate that all writes have been successful.
		/// </summary>
		/// <typeparam name="T">The type of the object that should be serialized.</typeparam>
		/// <param name="obj">The object that should be serialized into the buffer.</param>
		/// <param name="serializer">The serializer that should be used to atomically modify the buffer.</param>
		public bool TryWrite<T>(T obj, Action<BufferWriter, T> serializer)
		{
			Assert.ArgumentNotNull(serializer);

			var previousWritePosition = _writePosition;
			var previousMaxWritePosition = _maxWritePosition;

			try
			{
				serializer(this, obj);
				return true;
			}
			catch (IndexOutOfRangeException)
			{
				_writePosition = previousWritePosition;
				_maxWritePosition = previousMaxWritePosition;
				return false;
			}
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			_buffer = new ArraySegment<byte>();
		}
	}
}