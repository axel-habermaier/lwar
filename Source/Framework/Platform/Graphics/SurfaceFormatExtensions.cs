using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Provides extension methods for the surface format enumeration.
	/// </summary>
	public static class SurfaceFormatExtensions
	{
		/// <summary>
		///   Gets the number of components from the surface format.
		/// </summary>
		/// <param name="format">The surface format.</param>
		public static int ComponentCount(this SurfaceFormat format)
		{
			switch (format)
			{
				case SurfaceFormat.Color:
					return 4;
				default:
					throw new InvalidOperationException("Unsupported surface format.");
			}
		}
	}
}