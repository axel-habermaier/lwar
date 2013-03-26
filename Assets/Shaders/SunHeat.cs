using System;

namespace Lwar.Assets.Shaders
{
	public class SunHeat : Effect
	{
		public Matrix Rotation1 { get; set; }
		public Matrix Rotation2 { get; set; }

		[VertexShader]
		private void Main([Position] Vector4 position,
						  [Normal] Vector4 normal,
						  [Position] out Vector4 outPosition,
						  [TexCoords(0)] out Vector3 texCoords1,
						  [TexCoords(1)] out Vector3 texCoords2)
		{
			outPosition = World * ViewProjection * position;
			texCoords1 = Rotation1 * normal;
			texCoords2 = Rotation2 * normal;
		}
	}
}