using System;

namespace Lwar.Assets.Effects
{
	using Pegasus.AssetsCompiler.Effects;

	[Effect]
	public class ParallaxEffect : Effect
	{
		public readonly Technique Default = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "FragmentShader"
		};

		[Constant]
		public readonly Vector2 CameraPosition;

		public readonly Texture2D TextureAtlas;

		[VertexShader]
		public void VertexShader([Position] Vector4 position,
								 [Normal] Vector3 texCoordsAndDistance,
								 [Position] out Vector4 outPosition,
								 [TexCoords] out Vector2 outTexCoords)
		{
			position.x += CameraPosition.x * texCoordsAndDistance.z;
			position.z += CameraPosition.y * texCoordsAndDistance.z;

			outPosition = ViewProjection * position;
			outTexCoords = texCoordsAndDistance.xy;
		}

		[FragmentShader]
		public void FragmentShader([TexCoords] Vector2 texCoords, [Color] out Vector4 color)
		{
			color = TextureAtlas.Sample(texCoords);
		}
	}
}