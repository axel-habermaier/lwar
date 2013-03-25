using System;

namespace Lwar.Assets.Shaders
{
	using System.Runtime.InteropServices;
	using Pegasus.Framework.Math;

	public class BlurShader : FragmentShader
	{
		private static readonly float[] Offsets = new[] { 0.0f, 1.3846153846f, 3.2307692308f };
		private static readonly float[] Weights = new[] { 0.2270270270f, 0.3162162162f, 0.0702702703f };

		[Slot(3)]
		private BlurConstants _blurConstants = new BlurConstants();

		[Slot(0)]
		private Texture _texture = new Texture();

		#region BlurConstants

		private readonly int Mipmap;
		private readonly int Size;

		#endregion

		public void HorizontalBlur([TexCoords(0)] Vector2 texCoords)
		{
			Blur(texCoords, true);
		}

		public void VerticalBlur([TexCoords(0)] Vector2 texCoords)
		{
			Blur(texCoords, false);
		}

		private void Blur(Vector2 texCoords, bool horizontal)
		{
			var size = _blurConstants.Size;
			var mipmap = _blurConstants.Mipmap;

			var color = _texture.Sample(texCoords, mipmap) * Weights[0];

			for (var i = 1; i < 3; ++i)
			{
				Vector2 offset;
				if (horizontal)
					offset = new Vector2(Offsets[i], 0);
				else
					offset = new Vector2(0, Offsets[i]);

				color += _texture.Sample((texCoords * size + offset) / size, Mipmap) * Weights[i];
				color += _texture.Sample((texCoords * size - offset) / size, Mipmap) * Weights[i];
			}

			Output = color;
		}
	}

	public class VertexShader
	{
		public Vector4 Position { private get; set; }
	}

	[ConstantBuffer]
	[StructLayout(LayoutKind.Sequential)]
	public struct PerFrameConstants
	{
		public Matrix View;
		public Matrix Projection;
		public Matrix ViewProjection;
	}

	[ConstantBuffer]
	[StructLayout(LayoutKind.Sequential)]
	public struct PerObjectConstants
	{
		public Matrix World;
		public Matrix Rotation1;
		public Matrix Rotation2;
	}

	#region Infrastructure

	public struct Texture
	{
		public Vector4 Sample(Vector2 texCoord)
		{
			return new Vector4();
		}

		public Vector4 Sample(Vector2 texCoord, int mipmap)
		{
			return new Vector4();
		}
	}

	public class FragmentShader
	{
		public Vector4 Output { private get; set; }
	}

	[AttributeUsage(AttributeTargets.Struct)]
	public class ConstantBufferAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	public class PositionAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	public class NormalAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	public class TexCoordsAttribute : Attribute
	{
		public TexCoordsAttribute(int slot)
		{
			Slot = slot;
		}

		public int Slot { get; private set; }
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class SlotAttribute : Attribute
	{
		public SlotAttribute(int slot)
		{
			Slot = slot;
		}

		public int Slot { get; private set; }
	}

	[ConstantBuffer]
	[StructLayout(LayoutKind.Sequential)]
	public struct BlurConstants
	{
		public int Size;
		public int Mipmap;
	}

	#endregion
}