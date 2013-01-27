using System;

namespace Pegasus.Framework.Network
{
	using System.Text;
	using Math;
	using Platform;

	/// <summary>
	///   Represents an incoming packet.
	/// </summary>
	public class IncomingPacket : Packet<IncomingPacket>
	{
		/// <summary>
		///   The reader that is used to read the payload.
		/// </summary>
		private BufferReader _reader;

		/// <summary>
		///   Gets the amount of bytes contained in the packet.
		/// </summary>
		public int Size
		{
			get { return BigEndianConverter.Convert(new BufferReader(DataBuffer).ReadUInt16()); }
		}

		/// <summary>
		///   Gets the packet data buffer.
		/// </summary>
		internal ArraySegment<byte> DataBuffer
		{
			get { return new ArraySegment<byte>(Buffer, 0, Packet.MaxSize); }
		}

		/// <summary>
		///   Gets a value indicating whether the end of the packet has been reached.
		/// </summary>
		public bool EndOfPacket
		{
			get { return _reader.EndOfBuffer; }
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		internal static IncomingPacket Create()
		{
			return GetInstance();
		}

		/// <summary>
		///   Resets the read position of the packet so that all content can be read again.
		/// </summary>
		public void Reset()
		{
			_reader = new BufferReader(Buffer, Packet.SizeByteCount, Size);
		}

		/// <summary>
		///   Reads a Boolean value from the packet.
		/// </summary>
		public bool ReadBoolean()
		{
			return _reader.ReadBoolean();
		}

		/// <summary>
		///   Reads a signed byte from the packet.
		/// </summary>
		public sbyte ReadSignedByte()
		{
			return _reader.ReadSignedByte();
		}

		/// <summary>
		///   Reads an unsigned byte from the packet.
		/// </summary>
		public byte ReadByte()
		{
			return _reader.ReadByte();
		}

		/// <summary>
		///   Reads a 2 byte signed integer from the packet.
		/// </summary>
		public short ReadInt16()
		{
			return BigEndianConverter.Convert(_reader.ReadInt16());
		}

		/// <summary>
		///   Reads a 2 byte unsigned integer from the packet.
		/// </summary>
		public ushort ReadUInt16()
		{
			return BigEndianConverter.Convert(_reader.ReadUInt16());
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
			return BigEndianConverter.Convert(_reader.ReadInt32());
		}

		/// <summary>
		///   Reads a 4 byte unsigned integer from the packet.
		/// </summary>
		public uint ReadUInt32()
		{
			return BigEndianConverter.Convert(_reader.ReadUInt32());
		}

		/// <summary>
		///   Reads an 8 byte signed integer from the packet.
		/// </summary>
		public long ReadInt64()
		{
			return BigEndianConverter.Convert(_reader.ReadInt64());
		}

		/// <summary>
		///   Reads an 8 byte unsigned integer from the packet.
		/// </summary>
		public ulong ReadUInt64()
		{
			return BigEndianConverter.Convert(_reader.ReadUInt64());
		}

		/// <summary>
		///   Reads a 4 byte signed fixed-point value with 8 bits for the fractional part from the packet.
		/// </summary>
		public Fixed8 ReadFixed8()
		{
			return new Fixed8 { RawValue = ReadInt32() };
		}

		/// <summary>
		///   Reads a 4 byte signed fixed-point value with 16 bits for the fractional part from the packet.
		/// </summary>
		public Fixed16 ReadFixed16()
		{
			return new Fixed16 { RawValue = ReadInt32() };
		}

		// ReSharper disable InconsistentNaming

		/// <summary>
		///   Reads a two-component vector from the packet.
		/// </summary>
		public Vector2f8 ReadVector2f8()
		{
			return new Vector2f8(ReadFixed8(), ReadFixed8());
		}

		/// <summary>
		///   Reads a two-component vector from the packet.
		/// </summary>
		public Vector2f16 ReadVector2f16()
		{
			return new Vector2f16(ReadFixed16(), ReadFixed16());
		}

		// ReSharper restore InconsistentNaming

		/// <summary>
		///   Reads a string from the packet.
		/// </summary>
		public string ReadString()
		{
			var length = ReadByte();
			Assert.InRange(length, 0, Packet.MaxStringLength);

			if (length == 0)
				return String.Empty;

			var builder = new StringBuilder();
			for (var i = 0; i < length; ++i)
				builder.Append(ReadCharacter());
			return builder.ToString();
		}

		/// <summary>
		///   Reads a byte array from the packet.
		/// </summary>
		public byte[] ReadByteArray()
		{
			var length = ReadUInt16();
			var byteArray = new byte[length];

			for (var i = 0; i < length; ++i)
				byteArray[i] = ReadByte();

			return byteArray;
		}
	}
}