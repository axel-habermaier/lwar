using System;

namespace Lwar.Assets.Effects
{
	using Pegasus.AssetsCompiler.Effects;

	/// <summary>
	///   Applies a Gaussian blur filter to a texture.
	/// </summary>
	[Effect]
	public class BlurEffect : Effect
	{
		private static readonly float[] Offsets = new[] { 0.0f, 1.3846153846f, 3.2307692308f };
		private static readonly float[] Weights = new[] { 0.2270270270f, 0.3162162162f, 0.0702702703f };

		/// <summary> 
		///   Applies a horizontal Gaussian blur filter.
		/// </summary>
		public readonly Technique BlurHorizontally = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "HorizontalBlur"
		};

		/// <summary>
		///   Applies a vertical Gaussian blur filter.
		/// </summary>
		public readonly Technique BlurVertically = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "VerticalBlur"
		};

		/// <summary>
		///   The texture that is blurred.
		/// </summary>
		public readonly Texture2D Texture;

		[VertexShader]
		public void VertexShader([Position] Vector4 position,
								 [TexCoords] Vector2 texCoords,
								 [Position] out Vector4 outPosition,
								 [TexCoords] out Vector2 outTexCoords)
		{
			outPosition = new Vector4(position.x * -1, position.z, 1, 1);
			outTexCoords = new Vector2(1 - texCoords.x, texCoords.y);
		}

		[FragmentShader]
		public void HorizontalBlur([TexCoords(0)] Vector2 texCoords, [Color] out Vector4 color)
		{
			color = Texture.Sample(texCoords) * Weights[0];

			for (var i = 1; i < 3; ++i)
			{
				var coordinates = new Vector2(texCoords.x, texCoords.x) * ViewportSize.x;
				coordinates += new Vector2(Offsets[i], -Offsets[i]);
				coordinates /= ViewportSize.x;

				color += Texture.Sample(new Vector2(coordinates.x, texCoords.y)) * Weights[i];
				color += Texture.Sample(new Vector2(coordinates.y, texCoords.y)) * Weights[i];
			}
		}

		[FragmentShader]
		public void VerticalBlur([TexCoords(0)] Vector2 texCoords, [Color] out Vector4 color)
		{
			color = Texture.Sample(texCoords) * Weights[0];

			for (var i = 1; i < 3; ++i)
			{
				var coordinates = new Vector2(texCoords.y, texCoords.y) * ViewportSize.y;
				coordinates += new Vector2(Offsets[i], -Offsets[i]);
				coordinates /= ViewportSize.y;

				color += Texture.Sample(new Vector2(texCoords.x, coordinates.x)) * Weights[i];
				color += Texture.Sample(new Vector2(texCoords.x, coordinates.y)) * Weights[i];
			}
		}
	}
}