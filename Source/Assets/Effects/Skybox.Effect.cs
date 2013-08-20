using System;

// ReSharper disable once CheckNamespace
namespace Lwar.Assets.Effects.Internal
{
	using Pegasus.AssetsCompiler.Effects;

	[Effect]
	internal class SkyboxEffect : Effect
	{
		public readonly Technique Default = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "FragmentShader"
		};
		   
		public readonly CubeMap Skybox;

		[VertexShader]
		public void VertexShader([Position] Vector4 position,
								 [Position] out Vector4 outPosition,
								 [TexCoords] out Vector3 texCoords)
		{
			var rotationMatrix = View;
			rotationMatrix[0, 3] = 0;
			rotationMatrix[1, 3] = 0;
			rotationMatrix[2, 3] = 0;

			var projection = Projection;
			var f = 1.0f / Tan(0.9f * 0.5f);
			projection[0, 0] = f * ViewportSize.y / ViewportSize.x;
			projection[1, 1] = f;

			outPosition = rotationMatrix * position;
			outPosition = projection * outPosition;
			outPosition.z += 1;

			texCoords = position.xyz;
		}

		[FragmentShader]
		public void FragmentShader([TexCoords] Vector3 texCoords, [Color] out Vector4 color)
		{
			color = Skybox.Sample(texCoords);
		}
	}
}