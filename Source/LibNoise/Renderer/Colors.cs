using System;

namespace LibNoise.Renderer
{
	/// <summary>
	///   Well-known colors.
	/// </summary>
	public static class Colors
	{
		/// <summary>
		///   Create a black color.
		/// </summary>
		public static Color Black
		{
			get { return new Color(0, 0, 0, 255); }
		}

		/// <summary>
		///   Create a white color.
		/// </summary>
		public static Color White
		{
			get { return new Color(255, 255, 255, 255); }
		}

		/// <summary>
		///   Create a solid red color.
		/// </summary>
		public static Color Red
		{
			get { return new Color(255, 0, 0, 255); }
		}

		/// <summary>
		///   Create a solid green color.
		/// </summary>
		public static Color Green
		{
			get { return new Color(0, 255, 0, 255); }
		}

		/// <summary>
		///   Create a solid blue color.
		/// </summary>
		public static Color Blue
		{
			get { return new Color(0, 0, 255, 255); }
		}

		/// <summary>
		///   Create a transparent color.
		/// </summary>
		public static Color Transparent
		{
			get { return new Color(0, 0, 0, 0); }
		}
	}
}