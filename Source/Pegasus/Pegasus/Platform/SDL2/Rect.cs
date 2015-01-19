namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;
	using Math;

	/// <summary>
	///     Represents an SDL rectangle.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct Rect
	{
		/// <summary>
		///     The X position of the rectangle.
		/// </summary>
		public readonly int X;

		/// <summary>
		///     The Y position of the rectangle.
		/// </summary>
		public readonly int Y;

		/// <summary>
		///     The width of the rectangle.
		/// </summary>
		public readonly int Width;

		/// <summary>
		///     The height of the rectangle.
		/// </summary>
		public readonly int Height;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="rectangle">The rectangle the instance should be initialized to.</param>
		public Rect(Rectangle rectangle)
		{
			X = MathUtils.RoundIntegral(rectangle.Left);
			Y = MathUtils.RoundIntegral(rectangle.Top);
			Width = MathUtils.RoundIntegral(rectangle.Width);
			Height = MathUtils.RoundIntegral(rectangle.Height);
		}
	}
}