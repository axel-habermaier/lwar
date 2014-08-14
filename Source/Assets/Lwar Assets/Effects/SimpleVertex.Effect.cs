namespace Lwar.Assets.Effects
{
	using System;
	using Pegasus.AssetsCompiler.Effects;

	[Effect]
	public class SimpleVertexEffect : Effect
	{
		[Constant]
		public readonly Vector4 Color;

		public readonly Technique Default = new Technique
		{
			VertexShader = "VertexShader",
			FragmentShader = "FragmentShader"
		};

		[VertexShader]
		public void VertexShader([Position] Vector3 position,
								 [Position] out Vector4 outPosition)
		{
			outPosition = ViewProjection * new Vector4(position, 1);
		}

		[FragmentShader]
		public void FragmentShader([Color] out Vector4 color)
		{
			color = Color;
		}
	}
}