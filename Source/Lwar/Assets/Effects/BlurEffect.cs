﻿namespace Lwar.Assets.Effects
{
	using System;
	using Pegasus.AssetsCompiler.Effects;

	/// <summary>
	///     Applies a Gaussian blur filter to a texture.
	/// </summary>
	[Effect]
	public class BlurEffect : Effect
	{
		/// <summary>
		///     Applies a single-pass Gaussian blur filter.
		/// </summary>
		public readonly Technique Gaussian = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "GaussianBlur"
		};

		/// <summary>
		///     The texture that is blurred.
		/// </summary>
		public readonly Texture2D Texture;

		[Constant]
		public readonly Vector2 ViewportSize;

		[VertexShader]
		public void VertexShader([Position] Vector4 position,
								 [TexCoords] Vector2 texCoords,
								 [Position] out Vector4 outPosition,
								 [TexCoords] out Vector2 outTexCoords)
		{
			outPosition = position;
			outTexCoords = texCoords;
		}

		[FragmentShader]
		public void GaussianBlur([TexCoords(0)] Vector2 texCoords, [Color] out Vector4 color)
		{
			var offset = new Vector2(1.5f, 1.5f) / ViewportSize;

			color = Texture.Sample(texCoords + new Vector2(offset.x, offset.y));
			color += Texture.Sample(texCoords + new Vector2(-offset.x, offset.y));
			color += Texture.Sample(texCoords + new Vector2(offset.x, -offset.y));
			color += Texture.Sample(texCoords + new Vector2(-offset.x, -offset.y));
			color /= 4;
		}
	}
}