using System;

namespace Lwar.Assets.Shaders
{
	using Pegasus.AssetsCompiler.Effects;
	using Pegasus.AssetsCompiler.Effects.Math;
	using Pegasus.AssetsCompiler.Effects.Semantics;

	[Effect]
	public class SunHeat : Effect
	{
		public readonly CubeMap CubeMap;
		public readonly Texture2D HeatMap;

		[ShaderConstant(ChangeFrequency.PerDrawCall)]
		public readonly Matrix Rotation1;

		[ShaderConstant(ChangeFrequency.PerDrawCall)]
		public readonly Matrix Rotation2;

		[ShaderConstant(ChangeFrequency.PerDrawCall)]
		public readonly Matrix World;

		[VertexShader]
		public void VertexShader([Position] Vector4 position,
								 [Normal] Vector4 normal,
								 [Position] out Vector4 outPosition,
								 [TexCoords(0)] out Vector3 texCoords1,
								 [TexCoords(1)] out Vector3 texCoords2)
		{
			outPosition = World * ViewProjection * position;
			texCoords1 = (Rotation1 * normal).Xyz;
			texCoords2 = (Rotation2 * normal).Xyz;
		}

		[FragmentShader]
		public void FragmentShader([TexCoords(0)] Vector3 texCoords0,
								   [TexCoords(1)] Vector3 texCoords1,
								   [Color] out Vector4 color)
		{
			var sample1 = CubeMap.Sample(texCoords0).R;
			var sample2 = CubeMap.Sample(texCoords1).R;

			var result = sample1 + sample2;
			var blend = result / 2.0f;

			color = HeatMap.Sample(new Vector2(blend, 0));
			color = new Vector4(color.Rgb * blend, result / 4.0f);
		}
	}
}