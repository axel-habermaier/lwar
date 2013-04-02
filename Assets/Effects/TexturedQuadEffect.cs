using System;

namespace Lwar.Assets.Effects
{
	using Pegasus.AssetsCompiler.Effects;

	[Effect]
	public class TexturedQuadEffect : Effect
	{
		public readonly Technique Default = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "FragmentShader"
		};

		public readonly Technique FullScreen = new Technique
		{
			VertexShader = "FullScreenVertexShader",
			FragmentShader = "FragmentShader"
		};

		public readonly Texture2D Texture;

		[Constant]
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
		}

		[VertexShader]
		public void FullScreenVertexShader([Position] Vector4 position,
								 [TexCoords] Vector2 texCoords,
								 [Position] out Vector4 outPosition,
								 [TexCoords] out Vector2 outTexCoords)
		{
			outPosition = position;
			outTexCoords = texCoords;
		}

		[FragmentShader]
		public void FragmentShader([TexCoords] Vector2 texCoords, [Color] out Vector4 color)
		{
			color = Texture.Sample(texCoords);
		}
	}
}