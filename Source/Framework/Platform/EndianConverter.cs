using System;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
#pragma warning disable 162 // Unreachable code detected

namespace Pegasus.Framework.Platform
{
	/// <summary>
	///   Converts little endian to big endian or vice versa.
	/// </summary>
	public static class EndianConverter
	{
		/// <summary>
		///   Converts an 8 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public static long Convert(long value)
		{
			return ((long)(byte)(value)) << 56 |
				   ((long)(byte)(value >> 8)) << 48 |
				   ((long)(byte)(value >> 16)) << 40 |
				   ((long)(byte)(value >> 24)) << 32 |
				   ((long)(byte)(value >> 32)) << 24 |
				   ((long)(byte)(value >> 40)) << 16 |
				   ((long)(byte)(value >> 48)) << 8 |
				   (byte)(value >> 56);
		}

		/// <summary>
		///   Converts an 8 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public static ulong Convert(ulong value)
		{
			return ((ulong)((byte)(value)) << 56 |
					((ulong)(byte)(value >> 8)) << 48 |
					((ulong)(byte)(value >> 16)) << 40 |
					((ulong)(byte)(value >> 24)) << 32 |
					((ulong)(byte)(value >> 32)) << 24 |
					((ulong)(byte)(value >> 40)) << 16 |
					((ulong)(byte)(value >> 48)) << 8 |
					(byte)(value >> 56));
		}

		/// <summary>
		///   Converts a 4 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public static int Convert(int value)
		{
			return ((byte)(value)) << 24 |
				   ((byte)(value >> 8)) << 16 |
				   ((byte)(value >> 16)) << 8 |
				   ((byte)(value >> 24));
		}

		/// <summary>
		///   Converts a 4 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public static uint Convert(uint value)
		{
			return (uint)(((byte)(value)) << 24 |
						  ((byte)(value >> 8)) << 16 |
						  ((byte)(value >> 16)) << 8 |
						  ((byte)(value >> 24)));
		}

		/// <summary>
		///   Converts a 2 byte signed integer.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public static short Convert(short value)
		{
			return (short)(((byte)(value)) << 8 | ((byte)(value >> 8)));
		}

		/// <summary>
		///   Converts a 2 byte unsigned integer.
		/// </summary>
		/// <param name="value">The value that should be converted.</param>
		public static ushort Convert(ushort value)
		{
			return (ushort)(((byte)(value)) << 8 | ((byte)(value >> 8)));
		}
	}
}

// ReSharper restore ConditionIsAlwaysTrueOrFalse
#pragma warning restore 162