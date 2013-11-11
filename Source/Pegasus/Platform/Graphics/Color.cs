namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a 32-bit color (4 bytes) in the form of RGBA.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Color : IEquatable<Color>
	{
		/// <summary>
		///   The red component of the color.
		/// </summary>
		public byte Red;

		/// <summary>
		///   The green component of the color.
		/// </summary>
		public byte Green;

		/// <summary>
		///   The blue component of the color.
		/// </summary>
		public byte Blue;

		/// <summary>
		///   The alpha component of the color.
		/// </summary>
		public byte Alpha;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="value">The value that will be assigned to all components.</param>
		public Color(byte value)
		{
			Alpha = Red = Green = Blue = value;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="value">The value that will be assigned to all components.</param>
		public Color(float value)
		{
			Alpha = Red = Green = Blue = ToByte(value);
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="red">The red component of the color.</param>
		/// <param name="green">The green component of the color.</param>
		/// <param name="blue">The blue component of the color.</param>
		/// <param name="alpha">The alpha component of the color.</param>
		public Color(byte red, byte green, byte blue, byte alpha)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="red">The red component of the color.</param>
		/// <param name="green">The green component of the color.</param>
		/// <param name="blue">The blue component of the color.</param>
		/// <param name="alpha">The alpha component of the color.</param>
		public Color(float red, float green, float blue, float alpha)
		{
			Red = ToByte(red);
			Green = ToByte(green);
			Blue = ToByte(blue);
			Alpha = ToByte(alpha);
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="argb">A packed unsigned integer containing all four color components.</param>
		public Color(uint argb)
		{
			Alpha = (byte)((argb >> 24) & 255);
			Red = (byte)((argb >> 16) & 255);
			Green = (byte)((argb >> 8) & 255);
			Blue = (byte)(argb & 255);
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="argb">A packed integer containing all four color components.</param>
		public Color(int argb)
		{
			Alpha = (byte)((argb >> 24) & 255);
			Red = (byte)((argb >> 16) & 255);
			Green = (byte)((argb >> 8) & 255);
			Blue = (byte)(argb & 255);
		}

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="red">The red component of the color.</param>
		/// <param name="green">The green component of the color.</param>
		/// <param name="blue">The blue component of the color.</param>
		/// <param name="alpha">The alpha component of the color.</param>
		public static Color FromRgba(byte red, byte green, byte blue, byte alpha)
		{
			return new Color
			{
				Red = red,
				Green = green,
				Blue = blue,
				Alpha = alpha
			};
		}

		/// <summary>
		///   Converts the color into a packed integer.
		/// </summary>
		/// <returns>A packed integer containing all four color components.</returns>
		public int ToArgb()
		{
			int value = Blue;
			value |= Green << 8;
			value |= Red << 16;
			value |= Alpha << 24;

			return value;
		}

		/// <summary>
		///   Tests for equality between two colors.
		/// </summary>
		/// <param name="left">The first color to compare.</param>
		/// <param name="right">The second color to compare.</param>
		public static bool operator ==(Color left, Color right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Tests for inequality between two colors.
		/// </summary>
		/// <param name="left">The first color to compare.</param>
		/// <param name="right">The second color to compare.</param>
		public static bool operator !=(Color left, Color right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///   Returns a string representation of this color.
		/// </summary>
		public override string ToString()
		{
			return string.Format("Red: {0}, Green: {1}, Blue: {2}, Alpha: {3}", Red, Green, Blue, Alpha);
		}

		/// <summary>
		///   Returns a hash code for this color.
		/// </summary>
		public override int GetHashCode()
		{
			return Alpha.GetHashCode() + Red.GetHashCode() + Green.GetHashCode() + Blue.GetHashCode();
		}

		/// <summary>
		///   Determines whether the given color is equal to this color.
		/// </summary>
		/// <param name="other">The other color to compare with this color.</param>
		public bool Equals(Color other)
		{
			return Red == other.Red && Green == other.Green && Blue == other.Blue && Alpha == other.Alpha;
		}

		/// <summary>
		///   Determines whether the specified object is equal to this color.
		/// </summary>
		/// <param name="value">The object to compare with this color.</param>
		public override bool Equals(object value)
		{
			if (value == null)
				return false;

			if (!ReferenceEquals(value.GetType(), typeof(Color)))
				return false;

			return Equals((Color)value);
		}

		/// <summary>
		///   Stores the color information in the given array in RGBA format.
		/// </summary>
		/// <param name="color">The array to which the color information should be copied.</param>
		public void ToFloatArray(float[] color)
		{
			Assert.ArgumentNotNull(color);
			Assert.ArgumentSatisfies(color.Length == 4, "Array has wrong size.");
			color[0] = ToFloat(Red);
			color[1] = ToFloat(Green);
			color[2] = ToFloat(Blue);
			color[3] = ToFloat(Alpha);
		}

		/// <summary>
		///   Converts a byte value to a floating point value.
		/// </summary>
		/// <param name="component">The value that should be converted.</param>
		private static float ToFloat(byte component)
		{
			return component / 255.0f;
		}

		/// <summary>
		///   Converts a floating point value to a byte value.
		/// </summary>
		/// <param name="component">The value that should be converted.</param>
		private static byte ToByte(float component)
		{
			var value = (int)(component * 255.0f);
			return (byte)(value < 0 ? 0 : value > 255 ? 255 : value);
		}

		/// <summary>
		///   Gets a predefined color instance representing 'black'.
		/// </summary>
		public static readonly Color Black = new Color(0, 0, 0, 255);

		/// <summary>
		///   Gets a predefined color instance representing 'white'.
		/// </summary>
		public static readonly Color White = new Color(255, 255, 255, 255);
	}
}