using System;

namespace Pegasus.Framework.Network
{
	using Math;
	using Platform;

	/// <summary>
	///   Represents an outgoing packet.
	/// </summary>
	public class OutgoingPacket : Packet<OutgoingPacket>
	{
		/// <summary>
		///   The writer that is used to write the payload to the data buffer.
		/// </summary>
		private BufferWriter _writer;

		/// <summary>
		///   Gets the packet data buffer.
		/// </summary>
		internal ArraySegment<byte> DataBuffer
		{
			get
			{
				Assert.That(_writer.WrittenBytes < UInt16.MaxValue, "Packet is too long.");

				var writer = new BufferWriter(Buffer, 0, Packet.SizeByteCount);
				writer.Write(BigEndianConverter.Convert((ushort)_writer.WrittenBytes));

				return new ArraySegment<byte>(Buffer, 0, _writer.WrittenBytes + Packet.SizeByteCount);
			}
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		internal static OutgoingPacket Create()
		{
			var packet = GetInstance();
			packet._writer = new BufferWriter(packet.Buffer, Packet.SizeByteCount, Packet.MaxSize - Packet.SizeByteCount);
			return packet;
		}

		/// <summary>
		///   Resets the write position of the packet so that all content can be overwritten.
		/// </summary>
		public void Reset()
		{
			_writer.Reset();
		}

		/// <summary>
		///   Writes a Boolean value into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(bool value)
		{
			_writer.Write(value);
		}

		/// <summary>
		///   Writes a signed byte into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(sbyte value)
		{
			_writer.Write(value);
		}

		/// <summary>
		///   Writes an unsigned byte into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(byte value)
		{
			_writer.Write(value);
		}

		/// <summary>
		///   Writes a 2 byte signed integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(short value)
		{
			_writer.Write(BigEndianConverter.Convert(value));
		}

		/// <summary>
		///   Writes a 2 byte unsigned integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(ushort value)
		{
			_writer.Write(BigEndianConverter.Convert(value));
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
			_writer.Write(BigEndianConverter.Convert(value));
		}

		/// <summary>
		///   Writes a 4 byte unsigned integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(uint value)
		{
			_writer.Write(BigEndianConverter.Convert(value));
		}

		/// <summary>
		///   Writes an 8 byte signed integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(long value)
		{
			_writer.Write(BigEndianConverter.Convert(value));
		}

		/// <summary>
		///   Writes an 8 byte unsigned integer into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(ulong value)
		{
			_writer.Write(BigEndianConverter.Convert(value));
		}

		/// <summary>
		///   Writes a 4 byte signed fixed-point value with 8 bits for the fractional part into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(Fixed8 value)
		{
			Write(value.RawValue);
		}

		/// <summary>
		///   Writes a 4 byte signed fixed-point value with 16 bits for the fractional part into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(Fixed16 value)
		{
			Write(value.RawValue);
		}

		/// <summary>
		///   Writes a two-component vector into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(Vector2f8 value)
		{
			Write(value.X);
			Write(value.Y);
		}

		/// <summary>
		///   Writes a two-component vector into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(Vector2f16 value)
		{
			Write(value.X);
			Write(value.Y);
		}

		/// <summary>
		///   Writes a string into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(string value)
		{
			Assert.ArgumentNotNull(value, () => value);
			Assert.InRange(value.Length, 0, Packet.MaxStringLength);

			Write((byte)value.Length);
			foreach (var character in value)
				Write(character);
		}

		/// <summary>
		///   Writes a byte array into the packet.
		/// </summary>
		/// <param name="value">The value that should be written.</param>
		public void Write(byte[] value)
		{
			Assert.ArgumentNotNull(value, () => value);
			Assert.InRange(value.Length, 0, UInt16.MaxValue);

			Write((ushort)value.Length);
			foreach (var b in value)
				Write(b);
		}
	}
}