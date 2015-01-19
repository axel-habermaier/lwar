namespace Lwar.Assets.Effects
{
	using System;
	using Pegasus.AssetsCompiler.Effects;

	[Effect]
	public class SunEffect : Effect
	{
		public readonly CubeMap CubeMap;

		public readonly Technique Default = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "FragmentShader"
		};

		public readonly Texture2D HeatMap;

		[Constant]
		public readonly Matrix Rotation1;

		[Constant]
		public readonly Matrix Rotation2;

		[Constant]
		public readonly Matrix World;

		[VertexShader]
		public void VertexShader([Position] Vector4 position,
								 [Normal] Vector3 normal,
								 [Position] out Vector4 outPosition,
								 [TexCoords(0)] out Vector3 texCoords1,
								 [TexCoords(1)] out Vector3 texCoords2)
		{
			outPosition = World * position;
			outPosition = ViewProjection * outPosition;
			texCoords1 = (Rotation1 * new Vector4(normal, 1)).xyz;
			texCoords2 = (Rotation2 * new Vector4(normal, 1)).xyz;
		}

		[FragmentShader]
		public void FragmentShader([TexCoords(0)] Vector3 texCoords0,
								   [TexCoords(1)] Vector3 texCoords1,
								   [Color] out Vector4 color)
		{
			var sample1 = CubeMap.Sample(texCoords0).r;
			var sample2 = CubeMap.Sample(texCoords1).r;

			var result = sample1 + sample2;
			var blend = result / 2.0f;

			color = HeatMap.Sample(new Vector2(blend, 0));
			color = new Vector4(color.rgb * blend, result / 4.0f);
		}
	}
}