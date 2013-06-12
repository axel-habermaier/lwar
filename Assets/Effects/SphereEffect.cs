using System;

namespace Lwar.Assets.Effects
{
	using Pegasus.AssetsCompiler.Effects;

	[Effect]
	public class SphereEffect : Effect
	{
		public readonly Technique Default = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "FragmentShader"
		};

		public readonly Technique Planet = new Technique
		{
			VertexShader = "VertexShaderPlanet",
			FragmentShader = "FragmentShaderPlanet"
		};

		public readonly CubeMap SphereTexture;

		[Constant]
		public readonly Matrix World;

		[Constant]
		public readonly Vector3 SunPosition;

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
									   [TexCoords] out Vector3 eyePosition,
									   [Normal] out Vector3 outNormal)
		{
			outPosition = World * position;
			outPosition = ViewProjection * outPosition;
			outNormal = normal;
			eyePosition = Normalize((World * position).xyz);
		}

		[FragmentShader]
		public void FragmentShaderPlanet([TexCoords] Vector3 eyePosition, [Normal] Vector3 normal, [Color] out Vector4 color)
		{
			var textureColor = SphereTexture.Sample(normal).rgb;

			eyePosition = Normalize(eyePosition);
			var matrix = World;
			matrix[0, 3] = 0.0f;
			matrix[1, 3] = 0.0f;
			matrix[2, 3] = 0.0f;
			matrix[3, 3] = 1.0f;
			normal = Normalize((matrix * new Vector4(normal, 1)).xyz);

			// Diffuse light
			var lightDirection = Normalize(SunPosition - eyePosition);
			var diffuseLight = Max(Dot(lightDirection, normal), 0.0f);

			// Specular light - looks strange at the moment, requires planet-specific specular map
			//var reflection = -lightDirection + 2.0f * diffuseLight * normal;
			//var viewDirection = Normalize(CameraPosition - eyePosition);
			//var specularLight = 10.0f * Max(Pow(Dot(reflection, viewDirection), 64.0f), 0.0f);

			color = new Vector4(textureColor * (diffuseLight + 0.15f /* + specularLight */), 1.0f);
		}
	}
}