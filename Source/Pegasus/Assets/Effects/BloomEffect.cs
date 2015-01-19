namespace Pegasus.Assets.Effects
{
	using System;
	using AssetsCompiler.Effects;

	/// <summary>
	///     A post-processing bloom effect.
	/// </summary>
	/// <remarks>Adopted from the XNA bloom example.</remarks>
	[Effect]
	public class BloomEffect : Effect
	{
		private const float BloomThreshold = 0.25f;
		private const int BlurSampleCount = 15;
		private const float BloomIntensity = 2.5f;
		private const float BaseIntensity = 1;
		private const float BloomSaturation = 1f;
		private const float BaseSaturation = 1.1f;
		public readonly Texture2D BloomTexture;

		public readonly Technique Combine = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "CombineFragmentShader"
		};

		public readonly Technique Extract = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "ExtractFragmentShader"
		};

		public readonly Technique GaussianBlur = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "BlurFragmentShader"
		};

		[Constant, ArrayLength(BlurSampleCount)]
		public readonly Vector2[] SampleOffsets;

		[Constant, ArrayLength(BlurSampleCount)]
		public readonly float[] SampleWeights;

		public readonly Texture2D Texture;

		[FragmentShader]
		public void ExtractFragmentShader([TexCoords] Vector2 texCoords, [Color] out Vector4 outColor)
		{
			var color = Texture.Sample(texCoords);
			var subtract = new Vector4(BloomThreshold);

			// Subtract the bloom threshold from each color component, then scale it back up so that the
			// maximum value is 1 and clamp to the range [0,1].
			outColor = Saturate((color - subtract) / (1 - BloomThreshold));
		}

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
		public void BlurFragmentShader([TexCoords] Vector2 texCoords, [Color] out Vector4 color)
		{
			color = new Vector4(0);

			for (var i = 0; i < BlurSampleCount; i++)
				color += Texture.Sample(texCoords + SampleOffsets[i]) * SampleWeights[i];
		}

		[FragmentShader]
		public void CombineFragmentShader([TexCoords] Vector2 texCoords, [Color] out Vector4 color)
		{
			// Look up the bloom and original base image colors
			var bloom = BloomTexture.Sample(texCoords);
			var texture = Texture.Sample(texCoords);

			// Adjust color saturation and intensity
			//bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
			texture = AdjustSaturation(texture, BaseSaturation) * BaseIntensity;

			// Darken down the base image in areas where there is a lot of bloom,
			// to prevent things looking excessively burned-out
			texture *= new Vector4(1) - Saturate(bloom);

			// Combine the two images
			color = texture + bloom;
		}

		private Vector4 AdjustSaturation(Vector4 color, float saturation)
		{
			// The constants 0.3, 0.59, and 0.11 are chosen because the
			// human eye is more sensitive to green light, and less to blue
			var grey = Dot(color.rgb, new Vector3(0.3f, 0.59f, 0.11f));
			return Lerp(new Vector4(grey), color, new Vector4(saturation));
		}
	}
}