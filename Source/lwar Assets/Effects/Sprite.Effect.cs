namespace Lwar.Assets.Effects
{
	using System;
	using Pegasus.AssetsCompiler.Effects;

	[Effect]
	public class SpriteEffect : Effect
	{
		public readonly Technique Default = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "FragmentShader"
		};

		public readonly Texture2D Sprite;

		[Constant]
		public readonly Matrix World;

		[VertexShader]
		public void VertexShader([Position] Vector2 position,
								 [TexCoords] Vector2 texCoords,
								 [Color] Vector4 color,
								 [Position] out Vector4 outPosition,
								 [TexCoords] out Vector2 outTexCoords,
								 [Color] out Vector4 outColor)
		{
			outPosition = World * new Vector4(position.x, position.y, 0, 1);
			outPosition = ViewProjection * outPosition;
			outColor = color;
			outTexCoords = texCoords;
		}

		[FragmentShader]
		public void FragmentShader([TexCoords] Vector2 texCoords,
								   [Color] Vector4 color,
								   [Color] out Vector4 outColor)
		{
			outColor = Sprite.Sample(texCoords) * color;
		}
	}
}