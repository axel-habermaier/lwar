using System;

namespace Lwar.Assets.Shaders
{
	using System.Runtime.InteropServices;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Assets;
	using Pegasus.Framework.Platform.Graphics;

	public partial class BlurEffect : Effect
	{
		private static readonly float[] Offsets = new[] { 0.0f, 1.3846153846f, 3.2307692308f };
		private static readonly float[] Weights = new[] { 0.2270270270f, 0.3162162162f, 0.0702702703f };

		public Texture2D Texture;

		[ConstantBuffer]
		[StructLayout(LayoutKind.Sequential)]
		private struct Constants
		{
			public int Size;
			public int Mipmap;
		}

		[FragmentShader]
		private void HorizontalBlur([TexCoords(0)] Vector2 texCoords,
									[Color] out Vector4 color)
		{
			color = Blur(texCoords, true);
		}

		[FragmentShader]
		private void VerticalBlur([TexCoords(0)] Vector2 texCoords,
								  [Color] out Vector4 color)
		{
			color = Blur(texCoords, false);
		}

		[VertexShader]
		private void VertexShader([Position] Vector4 position,
								  [TexCoords] Vector2 texCoords,
								  [Position] out Vector4 outPosition,
								  [TexCoords] out Vector2 outTexCoords)
		{
			FullscreenQuadVertexShader(position, texCoords, out outPosition, out outTexCoords);
		}

		private Vector4 Blur(Vector2 texCoords, bool horizontal)
		{
			var color = Texture.Sample(texCoords, Mipmap) * Weights[0];

			for (var i = 1; i < 3; ++i)
			{
				Vector2 offset;
				if (horizontal)
					offset = new Vector2(Offsets[i], 0);
				else
					offset = new Vector2(0, Offsets[i]);

				color += Texture.Sample((texCoords * Size + offset) / Size, Mipmap) * Weights[i];
				color += Texture.Sample((texCoords * Size - offset) / Size, Mipmap) * Weights[i];
			}

			return color;
		}

		public void VerticalBlur()
		{
			Bind(VertexShader, VerticalBlur);
		}

		public void HorizontalBlur()
		{
			Bind(VertexShader, HorizontalBlur);
		}
	}

	public delegate void VS(Vector4 v, Vector2 t, out Vector4 op, out Vector2 ot);

	public delegate void FS(Vector2 t, out Vector4 c);

	public partial class BlurEffect
	{
		public int Size;
		public int Mipmap;

		private void SilenceWarnings()
		{
			Size = 2;
			Mipmap = 3;
			var c = new Constants { Mipmap = Mipmap, Size = Size };
			Texture = null;
		}
	}

	public class BlurEffectGenerated
	{
		private readonly ConstantBuffer<Constants> _constantBuffer;
		private readonly FragmentShader _horizontalBlur;
		private readonly VertexShader _vertexShader;
		private readonly FragmentShader _verticalBlur;

		public Texture2D Texture;

		public unsafe BlurEffectGenerated(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(assets, () => assets);

			_vertexShader = assets.LoadVertexShader("Shaders/Lwar.Assets.Shaders.BlurEffect.VertexShader");
			_horizontalBlur = assets.LoadFragmentShader("Shaders/Lwar.Assets.Shaders.BlurEffect.HorizontalBlur");
			_verticalBlur = assets.LoadFragmentShader("Shaders/Lwar.Assets.Shaders.BlurEffect.VerticalBlur");

			_constantBuffer = new ConstantBuffer<Constants>(graphicsDevice, (buffer, data) => buffer.Copy(&data));
		}

		private void Bind(VertexShader vs, FragmentShader fs)
		{
			Texture.Bind(0);
			//Texture.Sampler.Bind(0);

			var changed = false;

			if (_constantBuffer.Data.Mipmap != Mipmap)
			{
				changed = true;
				_constantBuffer.Data.Mipmap = Mipmap;
			}

			if (_constantBuffer.Data.Size != Size)
			{
				changed = true;
				_constantBuffer.Data.Size = Size;
			}

			if (changed)
				_constantBuffer.Update();

			_constantBuffer.Bind(2);
			vs.Bind();
			fs.Bind();
		}

		public int Size;
		public int Mipmap;

		[StructLayout(LayoutKind.Sequential, Size = 16)]
		private struct Constants
		{
			public int Size;
			public int Mipmap;
		}
	}

	
	public struct Vector4
	{
		public float W;
		public float X, Y, Z;

		public Vector4(float f, float f1, int i, int i1)
		{
			throw new NotImplementedException();
		}

		public static Vector4 operator *(Vector4 v, float f)
		{
			return new Vector4();
		}

		public static Vector4 operator /(Vector4 v, float f)
		{
			return new Vector4();
		}

		public static Vector4 operator +(Vector4 v, Vector4 q)
		{
			return new Vector4();
		}
	}

	public struct Vector2
	{
		public float X, Y;

		public Vector2(float x, float y)
			: this()
		{
		}

		public static Vector2 operator *(Vector2 v, float f)
		{
			return new Vector2();
		}

		public static Vector2 operator /(Vector2 v, float f)
		{
			return new Vector2();
		}

		public static Vector2 operator +(Vector2 v, Vector2 q)
		{
			return new Vector2();
		}

		public static Vector2 operator -(Vector2 v, Vector2 q)
		{
			return new Vector2();
		}
	}

	public struct Vector3
	{
		public float X, Y, Z;

		public Vector3(float x, float y)
			: this()
		{
		}

		public static implicit operator Vector3(Vector4 v)
		{
			return new Vector3();
		}

		public static Vector3 operator *(Vector3 v, float f)
		{
			return new Vector3();
		}

		public static Vector3 operator /(Vector3 v, float f)
		{
			return new Vector3();
		}

		public static Vector3 operator +(Vector3 v, Vector3 q)
		{
			return new Vector3();
		}

		public static Vector3 operator -(Vector3 v, Vector3 q)
		{
			return new Vector3();
		}
	}

	public struct Matrix
	{
		public static Vector4 operator *(Matrix m, Vector4 v)
		{
			return new Vector4();
		}

		public static Matrix operator *(Matrix m, Matrix v)
		{
			return new Matrix();
		}

		public static Vector4 operator *(Matrix m, Vector3 v)
		{
			return new Vector4();
		}
	}

	public class VertexShaderAttribute : Attribute
	{
	}

	public class FragmentShaderAttribute : Attribute
	{
	}

	#region Infrastructure

	public static class Texture2DExtensions
	{
		public static Vector4 Sample(this Texture2D texture, Vector2 texCoord)
		{
			return new Vector4();
		}

		public static Vector4 Sample(this Texture2D texture, Vector2 texCoord, int mipmap)
		{
			return new Vector4();
		}
	}

	public class Effect
	{
		protected Matrix Projection { get; private set; }
		protected Matrix View { get; private set; }
		protected Matrix ViewProjection { get; private set; }
		protected Matrix World { get; private set; }

		protected void FullscreenQuadVertexShader(Vector4 position,
												  Vector2 texCoords,
												  out Vector4 outPosition,
												  out Vector2 outTexCoords)
		{
			outPosition = new Vector4(position.X * -1, position.Z, 1, 1);
			outTexCoords = new Vector2(1 - texCoords.X, texCoords.Y);
		}

		protected void Bind(VS vs, FS fs)
		{
		}

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
		public TexCoordsAttribute()
		{
		}

		public TexCoordsAttribute(int slot)
		{
			Slot = slot;
		}

		public int Slot { get; private set; }
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	public class ColorAttribute : Attribute
	{
		public ColorAttribute()
		{
		}

		public ColorAttribute(int slot)
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

	#endregion
}