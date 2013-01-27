using System;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
#pragma warning disable 162 // Unreachable code detected

namespace Pegasus.Framework.Platform
{
    /// <summary>
    ///   Converts to big endian if necessary.
    /// </summary>
    public static class BigEndianConverter
    {
		/// <summary>
		///   Converts an 8 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public static long Convert(long value)
		{
			if (PlatformInfo.IsBigEndian)
				return value;
			return EndianConverter.Convert(value);
		}

		/// <summary>
		///   Converts an 8 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public static ulong Convert(ulong value)
		{
			if (PlatformInfo.IsBigEndian)
				return value;
			return EndianConverter.Convert(value);
		}

        /// <summary>
        ///   Converts a 4 byte signed integer.
        /// </summary>
        /// <param name="value">The value that should be converted.</param>
        public static int Convert(int value)
        {
            if (PlatformInfo.IsBigEndian)
                return value;
            return EndianConverter.Convert(value);
        }

        /// <summary>
        ///   Converts a 4 byte unsigned integer.
        /// </summary>
        /// <param name="value">The value that should be converted.</param>
        public static uint Convert(uint value)
        {
            if (PlatformInfo.IsBigEndian)
                return value;
            return EndianConverter.Convert(value);
        }

        /// <summary>
        ///   Converts a 2 byte signed integer.
        /// </summary>
        /// <param name="value">The value that should be converted.</param>
        public static short Convert(short value)
        {
            if (PlatformInfo.IsBigEndian)
                return value;
            return EndianConverter.Convert(value);
        }

        /// <summary>
        ///   Converts a 2 byte unsigned integer.
        /// </summary>
        /// <param name="value">The value that should be converted.</param>
        public static ushort Convert(ushort value)
        {
            if (PlatformInfo.IsBigEndian)
                return value;
            return EndianConverter.Convert(value);
        }
    }
}

// ReSharper restore ConditionIsAlwaysTrueOrFalse
#pragma warning restore 162