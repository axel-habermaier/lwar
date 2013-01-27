using System;

namespace Pegasus.Framework.Platform.Assets
{
	using System.IO;
	using System.Text;

	/// <summary>
	///   Reads compiled asset files.
	/// </summary>
	public sealed class AssetReader : DisposableObject
	{
		/// <summary>
		///   The file stream from which the data is read.
		/// </summary>
		private FileStream _stream;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="path">The path of the file that should be read.</param>
		public AssetReader(string path)
		{
			_stream = new FileStream(path + PlatformInfo.AssetExtension, FileMode.Open, FileAccess.Read);
		}

		/// <summary>
		///   Reads a 4 byte signed integer.
		/// </summary>
		public int ReadInt32()
		{
			return _stream.ReadByte() |
				   _stream.ReadByte() << 8 |
				   _stream.ReadByte() << 16 |
				   _stream.ReadByte() << 24;
		}

		/// <summary>
		///   Reads a 4 byte unsigned integer.
		/// </summary>
		public uint ReadUInt32()
		{
			return (uint)(_stream.ReadByte() |
						  _stream.ReadByte() << 8 |
						  _stream.ReadByte() << 16 |
						  _stream.ReadByte() << 24);
		}

		/// <summary>
		///   Reads a 2 byte signed integer.
		/// </summary>
		public short ReadInt16()
		{
			return (short)(_stream.ReadByte() | _stream.ReadByte() << 8);
		}

		/// <summary>
		///   Reads a 2 byte unsigned integer.
		/// </summary>
		public ushort ReadUInt16()
		{
			return (ushort)(_stream.ReadByte() | _stream.ReadByte() << 8);
		}

		/// <summary>
		///   Reads one byte.
		/// </summary>
		public byte ReadByte()
		{
			return (byte)_stream.ReadByte();
		}

		/// <summary>
		///   Reads a byte array.
		/// </summary>
		public byte[] ReadByteArray()
		{
			var length = ReadInt32();
			var buffer = new byte[length];
			ReadBytes(buffer, length);
			return buffer;
		}

		/// <summary>
		///   Reads the given amount of bytes into the buffer.
		/// </summary>
		/// <param name="buffer">The buffer that stores the data.</param>
		/// <param name="count">The number of bytes that should be read.</param>
		public void ReadBytes(byte[] buffer, int count)
		{
			_stream.Read(buffer, 0, count);
		}

		/// <summary>
		///   Reads a string.
		/// </summary>
		public string ReadString()
		{
			var size = ReadInt32();
			var buffer = new byte[size];
			ReadBytes(buffer, size);
			return Encoding.ASCII.GetString(buffer, 0, size);
		}

		/// <summary>
		///   Reads the entire asset file and returns it as a byte array.
		/// </summary>
		public byte[] ReadAll()
		{
			var data = new byte[_stream.Length];
			_stream.Read(data, 0, data.Length);
			return data;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_stream.SafeDispose();
			_stream = null;
		}
	}
}