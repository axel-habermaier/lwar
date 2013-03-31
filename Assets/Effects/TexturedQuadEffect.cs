using System;

namespace Lwar.Assets.Effects
{
	using Pegasus.AssetsCompiler.Effects;

	[Effect]
	public class TexturedQuadEffect : Effect
	{
		public readonly Texture2D Texture;

		[ShaderConstant(ChangeFrequency.PerDrawCall)]
		public readonly Matrix World;

		[VertexShader]
		public void VertexShader([Position] Vector4 position,
								 [TexCoords] Vector2 texCoords,
								 [Position] out Vector4 outPosition,
								 [TexCoords] out Vector2 outTexCoords)
		{
			outPosition = World * position;
			outPosition = ViewProjection * outPosition;
			outTexCoords = texCoords;
			var f = position.Length;
			var q = Normalize(position).x;
		}

		[FragmentShader]
		public void FragmentShader([TexCoords] Vector2 texCoords, [Color] out Vector4 color)
		{
			color = Texture.Sample(texCoords);
		}
	}
}