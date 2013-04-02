using System;

namespace Pegasus.AssetsCompiler.Effects
{
	/// <summary>
	///   Represents a 2-dimensional texture object from which a shader can retrieve texel data.
	/// </summary>
	public class Texture2D
	{
		/// <summary>
		///   Forbid initialization of new texture instances.
		/// </summary>
		private Texture2D()
		{
		}

		/// <summary>
		///   Samples the texel data at the given coordinates.
		/// </summary>
		/// <param name="coordinates">The coordinates at which the texel data should be sampled.</param>
		[MapsTo(Intrinsic.Sample)]
		public Vector4 Sample(Vector2 coordinates)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Samples the texel data at the given coordinates and mipmap level.
		/// </summary>
		/// <param name="coordinates">The coordinates at which the texel data should be sampled.</param>
		/// <param name="mipmap">The mipmap level at which the texel data should be sampled.</param>
		[MapsTo(Intrinsic.SampleLevel)]
		public Vector4 Sample(Vector2 coordinates, int mipmap)
		{
			throw new NotImplementedException();
		}
	}
}