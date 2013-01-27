using System;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
#pragma warning disable 162 // Unreachable code detected

namespace Pegasus.AssetsCompiler
{
    using System.IO;
    using System.Text;
    using Framework;
    using Framework.Platform;

    /// <summary>
    ///   Writes a compiled asset file. The endianess of the compiled file always matches the endianess of the target
    ///   platform, assuming that the compiler is always running on a little endian platform.
    /// </summary>
    public sealed class AssetWriter : DisposableObject
    {
        /// <summary>
        ///   The stream used to write to the asset file.
        /// </summary>
        private readonly FileStream _stream;

        /// <summary>
        ///   Initializes a new instance.
        /// </summary>
        /// <param name="path">The path of the compiled asset.</param>
        public AssetWriter(string path)
        {
            var assetName = path + PlatformInfo.AssetExtension;

            Log.Info("Compiling '{0}'...", assetName);
            _stream = new FileStream(assetName, FileMode.Create);
        }

        protected override void OnDisposing()
        {
            _stream.Dispose();
        }

        /// <summary>
        ///   Writes a 4 byte signed integer into the file.
        /// </summary>
        /// <param name="value">The value that should be written.</param>
        public void WriteInt32(int value)
        {
            if (PlatformInfo.IsBigEndian)
                value = EndianConverter.Convert(value);

            var bytes = BitConverter.GetBytes(value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///   Writes a 4 byte unsigned integer into the file.
        /// </summary>
        /// <param name="value">The value that should be written.</param>
        public void WriteUInt32(uint value)
        {
            if (PlatformInfo.IsBigEndian)
                value = EndianConverter.Convert(value);

            var bytes = BitConverter.GetBytes(value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///   Writes a 2 byte signed integer into the file.
        /// </summary>
        /// <param name="value">The value that should be written.</param>
        public void WriteInt16(short value)
        {
            if (PlatformInfo.IsBigEndian)
                value = EndianConverter.Convert(value);

            var bytes = BitConverter.GetBytes(value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///   Writes a 2 byte unsigned integer into the file.
        /// </summary>
        /// <param name="value">The value that should be written.</param>
        public void WriteUInt16(ushort value)
        {
            if (PlatformInfo.IsBigEndian)
                value = EndianConverter.Convert(value);

            var bytes = BitConverter.GetBytes(value);
            _stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        ///   Writes a byte into the file.
        /// </summary>
        /// <param name="value">The value that should be written.</param>
        public void WriteByte(byte value)
        {
            _stream.WriteByte(value);
        }

        /// <summary>
        ///   Writes a byte array into the file.
        /// </summary>
        /// <param name="value">The value that should be written.</param>
        public void WriteByteArray(byte[] value)
        {
			WriteInt32(value.Length);
            _stream.Write(value, 0, value.Length);
        }

        /// <summary>
        ///   Writes a string into the file.
        /// </summary>
        /// <param name="value">The value that should be written.</param>
        public void WriteString(string value)
        {
            Assert.ArgumentNotNull(value, () => value);
            var bytes = Encoding.ASCII.GetBytes(value);

            WriteInt32(bytes.Length);
            _stream.Write(bytes, 0, bytes.Length);
        }
    }
}

// ReSharper restore ConditionIsAlwaysTrueOrFalse
#pragma warning restore 162