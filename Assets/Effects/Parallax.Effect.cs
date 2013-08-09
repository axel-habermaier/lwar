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

		public readonly Texture2D TextureAtlas;

		// Must be in sync with ParallaxRenderer's distance range
		private const float DistanceMin = 0.1f;
		private const float DistanceMax = 0.8f;
		private const float SpeedMin = 0.85f;
		private const float SpeedMax = 0.9999f;

		[VertexShader]
		public void VertexShader([Position] Vector4 position,
								 [Normal] Vector3 texCoordsAndDistance,
								 [Position] out Vector4 outPosition,
								 [TexCoords] out Vector2 outTexCoords)
		{
			// Transform the distance range into the speed range
			const float distanceRange = DistanceMax - DistanceMin;
			const float speedRange = SpeedMax - SpeedMin;
			var speed = (((texCoordsAndDistance.z - DistanceMin) * speedRange) / distanceRange) + SpeedMin;

			// Offset the star's position by the current camera position and the speed in order to simulate
			// slower star movement when the star is further away from the camera
			position.x += CameraPosition.x * speed;
			position.z += CameraPosition.z * speed;

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