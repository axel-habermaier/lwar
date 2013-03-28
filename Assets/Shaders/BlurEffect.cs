using System;

namespace Lwar.Assets.Shaders
{
	using Pegasus.AssetsCompiler.Effects;
	using Pegasus.AssetsCompiler.Effects.Math;
	using Pegasus.AssetsCompiler.Effects.Semantics;

	[Effect]
	public class BlurEffect : Effect
	{
		private static readonly float[] Offsets = new[] { 0.0f, 1.3846153846f, 3.2307692308f };
		private static readonly float[] Weights = new[] { 0.2270270270f, 0.3162162162f, 0.0702702703f };
		
		public readonly Texture2D Texture;

		[FragmentShader]
		public void HorizontalBlur([TexCoords(0)] Vector2 texCoords, [Color] out Vector4 color)
		{
			color = Texture.Sample(texCoords) * Weights[0];

			for (var i = 1; i < 3; ++i)
			{
				var coordinates = new Vector2(texCoords.X, texCoords.X) * ViewportSize.X;
				coordinates += new Vector2(Offsets[i], -Offsets[i]);
				coordinates /= ViewportSize.X;

				color += Texture.Sample(new Vector2(coordinates.X, texCoords.Y)) * Weights[i];
				color += Texture.Sample(new Vector2(coordinates.Y, texCoords.Y)) * Weights[i];
			}
		}

		[FragmentShader]
		public void VerticalBlur([TexCoords(0)] Vector2 texCoords, [Color] out Vector4 color)
		{
			color = Texture.Sample(texCoords) * Weights[0];

			for (var i = 1; i < 3; ++i)
			{
				var coordinates = new Vector2(texCoords.Y, texCoords.Y) * ViewportSize.Y;
				coordinates += new Vector2(Offsets[i], -Offsets[i]);
				coordinates /= ViewportSize.Y;

				color += Texture.Sample(new Vector2(texCoords.X, coordinates.X)) * Weights[i];
				color += Texture.Sample(new Vector2(texCoords.X, coordinates.Y)) * Weights[i];
			}
		}

		[VertexShader]
		public void VertexShader([Position] Vector4 position,
								 [TexCoords] Vector2 texCoords,
								 [Position] out Vector4 outPosition,
								 [TexCoords] out Vector2 outTexCoords)
		{
			FullscreenQuad.ProcessVertex(position, texCoords, out outPosition, out outTexCoords);
		}
	}

	//public class BlurEffectGenerated
	//{
	//	private readonly ConstantBuffer<Constants> _constantBuffer;
	//	private readonly Pegasus.Framework.Platform.Graphics.FragmentShader _horizontalBlur;
	//	private readonly VertexShader _vertexShader;
	//	private readonly Pegasus.Framework.Platform.Graphics.FragmentShader _verticalBlur;
	//	public int Mipmap;
	//	public int Size;

	//	public Texture2D Texture;

	//	public unsafe BlurEffectGenerated(GraphicsDevice graphicsDevice, AssetsManager assets)
	//	{
	//		Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
	//		Assert.ArgumentNotNull(assets, () => assets);

	//		_vertexShader = assets.LoadVertexShader("Shaders/Lwar.Assets.Shaders.BlurEffect.VertexShader");
	//		_horizontalBlur = assets.LoadFragmentShader("Shaders/Lwar.Assets.Shaders.BlurEffect.HorizontalBlur");
	//		_verticalBlur = assets.LoadFragmentShader("Shaders/Lwar.Assets.Shaders.BlurEffect.VerticalBlur");

	//		_constantBuffer = new ConstantBuffer<Constants>(graphicsDevice, (buffer, data) => buffer.Copy(&data));
	//	}

	//	public void Bind(FragmentShader fs)
	//	{
	//	}

	//	private void Bind(VertexShader vs, FragmentShader fs)
	//	{
	//		//Texture.Bind(0);
	//		//Texture.Sampler.Bind(0);

	//		var changed = false;

	//		if (_constantBuffer.Data.Mipmap != Mipmap)
	//		{
	//			changed = true;
	//			_constantBuffer.Data.Mipmap = Mipmap;
	//		}

	//		if (_constantBuffer.Data.Size != Size)
	//		{
	//			changed = true;
	//			_constantBuffer.Data.Size = Size;
	//		}

	//		if (changed)
	//			_constantBuffer.Update();

	//		_constantBuffer.Bind(2);
	//		vs.Bind();
	//		fs.Bind();
	//	}

	//	[StructLayout(LayoutKind.Sequential, Size = 16)]
	//	private struct Constants
	//	{
	//		public int Size;
	//		public int Mipmap;
	//	}
	//}
}