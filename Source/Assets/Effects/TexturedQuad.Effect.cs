namespace Lwar.Assets.Effects
{
	using System;
	using Pegasus.AssetsCompiler.Effects;

	[Effect]
	public class TexturedQuadEffect : Effect
	{
		public readonly Technique TexturedQuad = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "TexturedQuadFragmentShader"
		};

		public readonly Technique ColoredTexturedQuad = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "ColoredTexturedQuadFragmentShader"
		};

		public readonly Technique FullScreen = new Technique
		{
			VertexShader = "FullScreenVertexShader",
			FragmentShader = "TexturedQuadFragmentShader"
		};

		public readonly Texture2D Texture;

		[Constant]
		public readonly Matrix World;

		[Constant]
		public readonly Vector4 Color;

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
		public void TexturedQuadFragmentShader([TexCoords] Vector2 texCoords, [Color] out Vector4 color)
		{
			color = Texture.Sample(texCoords);
		}

		[FragmentShader]
		public void ColoredTexturedQuadFragmentShader([TexCoords] Vector2 texCoords, [Color] out Vector4 color)
		{
			color = Texture.Sample(texCoords) * Color;
		}
	}
}