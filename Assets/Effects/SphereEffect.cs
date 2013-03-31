using System;

namespace Lwar.Assets.Effects
{
	using Pegasus.AssetsCompiler.Effects;
	using Pegasus.AssetsCompiler.Effects.Math;
	using Pegasus.AssetsCompiler.Effects.Semantics;

	[Effect]
	public class SphereEffect : Effect
	{
		public readonly CubeMap SphereTexture;

		[ShaderConstant(ChangeFrequency.PerDrawCall)]
		public readonly Matrix World;

		[VertexShader]
		public void VertexShader([Position] Vector4 position,
								 [Normal] Vector3 normal,
								 [Position] out Vector4 outPosition,
								 [Normal] out Vector3 outNormal)
		{
			outPosition = World * position;
			outPosition = ViewProjection * outPosition;
			outNormal = normal;
		}

		[FragmentShader]
		public void FragmentShader([Normal] Vector3 normal, [Color] out Vector4 color)
		{
			color = SphereTexture.Sample(normal);
		}
	}
}