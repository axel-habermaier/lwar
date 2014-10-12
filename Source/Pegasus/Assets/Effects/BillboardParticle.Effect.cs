namespace Pegasus.Assets.Effects
{
	using System;
	using AssetsCompiler.Effects;

	[Effect]
	public class BillboardParticleEffect : Effect
	{
		public readonly Technique Default = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "FragmentShader"
		};

		public readonly Texture2D Texture;

		[Constant]
		public readonly Matrix World;

		[VertexShader]
		public void VertexShader([Position] Vector2 vertexPosition,
								 [TexCoords] Vector3 particlePosition,
								 [Color] Vector4 color,
								 [TexCoords(1)] float scale,
								 [Position] out Vector4 outPosition,
								 [TexCoords] out Vector2 outTexCoords,
								 [Color] out Vector4 outColor)
		{
			var position = World * new Vector4(particlePosition, 1);
			position = View * position;
			position.xy += scale * vertexPosition;

			outPosition = Projection * position;
			outTexCoords = vertexPosition + new Vector2(0.5f, 0.5f);
			outColor = color;
		}

		[FragmentShader]
		public void FragmentShader([TexCoords] Vector2 texCoords, [Color] Vector4 color, [Color] out Vector4 outColor)
		{
			outColor = new Vector4(Texture.Sample(texCoords).rgb * color.rgb * color.a, 1);
		}
	}
}