using System;

namespace Lwar.Assets.Shaders
{
	using Pegasus.AssetsCompiler.Effects.Math;
	using Vector4 = Pegasus.AssetsCompiler.Effects.Math.Vector4;

	public static class FullscreenQuad
	{
		public static void ProcessVertex(Vector4 position,
										Vector2 texCoords,
										out Vector4 outPosition,
										out Vector2 outTexCoords)
		{
			outPosition = new Vector4(position.X * -1, position.Z, 1, 1);
			outTexCoords = new Vector2(1 - texCoords.X, texCoords.Y);
		}
	}
}