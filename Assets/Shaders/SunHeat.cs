using System;

namespace Lwar.Assets.Shaders
{
	using Pegasus.Framework.Math;

	public class SunHeat : VertexShader
	{
		[Slot(0)]
		private PerFrameConstants _perFrame = new PerFrameConstants();

		[Slot(1)]
		private PerObjectConstants _perObject = new PerObjectConstants();

		public void Main([Position] Vector4 position,
						 [Normal] Vector4 normal,
						 [TexCoords(0)] out Vector3 texCoords1,
						 [TexCoords(1)] out Vector3 texCoords2)
		{
			//Position = _perObject.World * _perFrame.ViewProjection * position;
			//texCoords1 = _perObject.Rotation1 * normal;
			//texCoords2 = _perObject.Rotation2 * normal;
			texCoords1 = new Vector3();
			texCoords2=new Vector3();
		}
	}
}