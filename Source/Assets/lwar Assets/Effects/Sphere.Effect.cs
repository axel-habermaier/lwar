namespace Lwar.Assets.Effects
{
	using System;
	using Pegasus.AssetsCompiler.Effects;

	[Effect]
	public class SphereEffect : Effect
	{
		public readonly Technique Default = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "FragmentShader"
		};

		[Constant("ShieldConstantBuffer")]
		public readonly Vector3 ImpactPosition;

		public readonly Technique Planet = new Technique
		{
			VertexShader = "VertexShaderPlanet",
			FragmentShader = "FragmentShaderPlanet"
		};

		public readonly Technique Shield = new Technique
		{
			VertexShader = "VertexShaderShield",
			FragmentShader = "FragmentShaderShield"
		};

		public readonly CubeMap SphereTexture;

		[Constant("PlanetConstantBuffer")]
		public readonly Vector3 SunPosition;

		[Constant("ShieldConstantBuffer")]
		public readonly float TimeToLive;

		[Constant]
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

		[VertexShader]
		public void VertexShaderPlanet([Position] Vector4 position,
									   [Normal] Vector3 normal,
									   [Position] out Vector4 outPosition,
									   [TexCoords] out Vector3 worldPosition,
									   [Normal] out Vector3 outNormal)
		{
			outPosition = World * position;
			outPosition = ViewProjection * outPosition;
			outNormal = normal;
			worldPosition = Normalize((World * position).xyz);
		}

		[FragmentShader]
		public void FragmentShaderPlanet([TexCoords] Vector3 worldPosition,
										 [Normal] Vector3 normal,
										 [Color] out Vector4 color)
		{
			var textureColor = SphereTexture.Sample(normal).rgb;

			worldPosition = Normalize(worldPosition);
			var matrix = World;
			matrix[0, 3] = 0.0f;
			matrix[1, 3] = 0.0f;
			matrix[2, 3] = 0.0f;
			matrix[3, 3] = 1.0f;
			normal = Normalize((matrix * new Vector4(normal, 1)).xyz);

			// Diffuse light
			var lightDirection = Normalize(SunPosition - worldPosition);
			var diffuseLight = Max(Dot(lightDirection, normal), 0.0f);

			// Specular light - looks strange at the moment, requires planet-specific specular map
			//var reflection = -lightDirection + 2.0f * diffuseLight * normal;
			//var viewDirection = Normalize(CameraPosition - eyePosition);
			//var specularLight = 10.0f * Max(Pow(Dot(reflection, viewDirection), 64.0f), 0.0f);

			color = new Vector4(textureColor * (diffuseLight + 0.15f /* + specularLight */), 1.0f);
		}

		[VertexShader]
		public void VertexShaderShield([Position] Vector4 position,
									   [Normal] Vector3 normal,
									   [Position] out Vector4 outPosition,
									   [TexCoords] out Vector3 worldPosition,
									   [Normal] out Vector3 outNormal)
		{
			outPosition = World * position;
			outPosition = ViewProjection * outPosition;
			outNormal = normal;
			worldPosition = (World * position).xyz;
		}

		[FragmentShader]
		public void FragmentShaderShield([TexCoords] Vector3 worldPosition,
										 [Normal] Vector3 normal,
										 [Color] out Vector4 color)
		{
			var maxDistance = 70;
			var distance = Min(Distance(worldPosition, ImpactPosition) / maxDistance, 1.0f);

			color = new Vector4(0,0,0,0);// SphereTexture.Sample(normal) * (1 - distance) * TimeToLive;
		}
	}
}