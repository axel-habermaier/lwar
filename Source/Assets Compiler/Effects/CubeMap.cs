using System;

namespace Pegasus.AssetsCompiler.Effects
{
	using Math;

	/// <summary>
	///   Represents a 2-dimensional cubemap object from which a shader can retrieve texel data.
	/// </summary>
	public class CubeMap
	{
		/// <summary>
		///   Forbid initialization of new cubemap instances.
		/// </summary>
		private CubeMap()
		{
		}

		/// <summary>
		///   Samples the texel data at the given coordinates.
		/// </summary>
		/// <param name="coordinates">The coordinates at which the texel data should be sampled.</param>
		[MapsTo(Intrinsic.Sample)]
		public Vector4 Sample(Vector3 coordinates)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Samples the texel data at the given coordinates and mipmap level.
		/// </summary>
		/// <param name="coordinates">The coordinates at which the texel data should be sampled.</param>
		/// <param name="mipmap">The mipmap level at which the texel data should be sampled.</param>
		[MapsTo(Intrinsic.SampleLevel)]
		public Vector4 Sample(Vector3 coordinates, int mipmap)
		{
			throw new NotImplementedException();
		}
	}
}