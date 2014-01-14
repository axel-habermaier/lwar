﻿namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///   Represents a two-dimensional cube map.
	/// </summary>
	public sealed class CubeMap : Texture
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		internal CubeMap(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}
	}
}