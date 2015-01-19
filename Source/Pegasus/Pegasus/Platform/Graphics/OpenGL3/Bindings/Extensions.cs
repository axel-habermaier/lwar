namespace Pegasus.Platform.Graphics.OpenGL3.Bindings
{
	using System;

	internal static class Extensions
	{
		public static uint GetInternalFormat(this SurfaceFormat format)
		{
			switch (format)
			{
				case SurfaceFormat.Rgba8:
					return GL.Rgba;
				case SurfaceFormat.R8:
					return GL.R8;
				case SurfaceFormat.Bc1:
					return GL.CompressedRgbDxt1;
				case SurfaceFormat.Bc2:
					return GL.CompressedRgbaDxt3;
				case SurfaceFormat.Bc3:
					return GL.CompressedRgbaDxt5;
				case SurfaceFormat.Bc4:
					return GL.CompressedRedRgtc1;
				case SurfaceFormat.Bc5:
					return GL.CompressedRgRgtc2;
				case SurfaceFormat.Rgba16F:
					return GL.Rgba;
				case SurfaceFormat.Depth24Stencil8:
					return GL.DepthStencil;
				default:
					throw new InvalidOperationException("Unsupported surface format.");
			}
		}

		public static int GetIndexSizeInBytes(this IndexSize indexSize)
		{
			switch (indexSize)
			{
				case IndexSize.SixteenBits:
					return 2;
				case IndexSize.ThirtyTwoBits:
					return 4;
				default:
					throw new InvalidOperationException("Unsupported index size.");
			}
		}

		public static uint Map(this IndexSize indexSize)
		{
			switch (indexSize)
			{
				case IndexSize.SixteenBits:
					return GL.UnsignedShort;
				case IndexSize.ThirtyTwoBits:
					return GL.UnsignedInt;
				default:
					throw new InvalidOperationException("Unsupported index size.");
			}
		}

		public static uint GetVertexDataType(this VertexDataFormat format)
		{
			switch (format)
			{
				case VertexDataFormat.Single:
				case VertexDataFormat.Vector2:
				case VertexDataFormat.Vector3:
				case VertexDataFormat.Vector4:
					return GL.Float;
				case VertexDataFormat.Color:
					// The type of color inputs in the shader is Vector4; however, we only send RGBA8 values and let the shader
					// convert the data to save 12 bytes per vertex
					return GL.UnsignedByte;
				default:
					throw new InvalidOperationException("Unsupported vertex data format.");
			}
		}

		public static int GetVertexDataComponentCount(this VertexDataFormat format)
		{
			switch (format)
			{
				case VertexDataFormat.Single:
					return 1;
				case VertexDataFormat.Vector2:
					return 2;
				case VertexDataFormat.Vector3:
					return 3;
				case VertexDataFormat.Vector4:
					return 4;
				case VertexDataFormat.Color:
					// The type of color inputs in the shader is Vector4; however, we only send RGBA8 values and let the shader
					// convert the data to save 12 bytes per vertex
					return 4;
				default:
					throw new InvalidOperationException("Unsupported vertex data format.");
			}
		}

		public static uint GetMinFilter(this TextureFilter filter)
		{
			switch (filter)
			{
				case TextureFilter.Nearest:
					return GL.NearestMipmapNearest;
				case TextureFilter.NearestNoMipmaps:
					return GL.Nearest;
				case TextureFilter.Bilinear:
					return GL.LinearMipmapNearest;
				case TextureFilter.BilinearNoMipmaps:
					return GL.Linear;
				case TextureFilter.Trilinear:
					return GL.LinearMipmapLinear;
				case TextureFilter.Anisotropic:
					return GL.LinearMipmapLinear;
				default:
					throw new InvalidOperationException("Unsupported texture filter.");
			}
		}

		public static uint GetMagFilter(this TextureFilter filter)
		{
			switch (filter)
			{
				case TextureFilter.Nearest:
					return GL.Nearest;
				case TextureFilter.NearestNoMipmaps:
					return GL.Nearest;
				case TextureFilter.Bilinear:
					return GL.Linear;
				case TextureFilter.BilinearNoMipmaps:
					return GL.Linear;
				case TextureFilter.Trilinear:
					return GL.Linear;
				case TextureFilter.Anisotropic:
					return GL.Linear;
				default:
					throw new InvalidOperationException("Unsupported texture filter.");
			}
		}

		public static uint Map(this BlendOperation blendOperation)
		{
			switch (blendOperation)
			{
				case BlendOperation.Add:
					return GL.FuncAdd;
				case BlendOperation.Subtract:
					return GL.FuncSubtract;
				case BlendOperation.ReverseSubtract:
					return GL.FuncReverseSubtract;
				case BlendOperation.Minimum:
					return GL.Min;
				case BlendOperation.Maximum:
					return GL.Max;
				default:
					throw new InvalidOperationException("Unsupported blend operation.");
			}
		}

		public static uint Map(this BlendOption blendOption)
		{
			switch (blendOption)
			{
				case BlendOption.Zero:
					return GL.Zero;
				case BlendOption.One:
					return GL.One;
				case BlendOption.SourceColor:
					return GL.SrcColor;
				case BlendOption.InverseSourceColor:
					return GL.OneMinusSrcColor;
				case BlendOption.SourceAlpha:
					return GL.SrcAlpha;
				case BlendOption.InverseSourceAlpha:
					return GL.OneMinusSrcAlpha;
				case BlendOption.DestinationAlpha:
					return GL.DstAlpha;
				case BlendOption.InverseDestinationAlpha:
					return GL.OneMinusDstAlpha;
				case BlendOption.DestinationColor:
					return GL.DstColor;
				case BlendOption.InverseDestinationColor:
					return GL.OneMinusDstColor;
				case BlendOption.SourceAlphaSaturate:
					return GL.SrcAlphaSaturate;
				case BlendOption.BlendFactor:
					return GL.ConstantColor;
				case BlendOption.InverseBlendFactor:
					return GL.OneMinusConstantColor;
				default:
					throw new InvalidOperationException("Unsupported blend option.");
			}
		}

		public static uint Map(this Comparison comparison)
		{
			switch (comparison)
			{
				case Comparison.Always:
					return GL.Always;
				case Comparison.Equal:
					return GL.Equal;
				case Comparison.Greater:
					return GL.Greater;
				case Comparison.Less:
					return GL.Less;
				case Comparison.GreaterEqual:
					return GL.Gequal;
				case Comparison.LessEqual:
					return GL.Lequal;
				case Comparison.Never:
					return GL.Never;
				case Comparison.NotEqual:
					return GL.Notequal;
				default:
					throw new InvalidOperationException("Unsupported comparison function.");
			}
		}

		public static uint Map(this FillMode fillMode)
		{
			switch (fillMode)
			{
				case FillMode.Wireframe:
					return GL.Line;
				case FillMode.Solid:
					return GL.Fill;
				default:
					throw new InvalidOperationException("Unsupported fill mode.");
			}
		}

		public static uint Map(this MapMode mapMode)
		{
			switch (mapMode)
			{
				case MapMode.Read:
					return GL.MapReadBit;
				case MapMode.Write:
					return GL.MapWriteBit;
				case MapMode.WriteDiscard:
				case MapMode.WriteNoOverwrite:
					return GL.MapWriteBit | GL.MapInvalidateRangeBit | GL.MapUnsynchronizedBit;
				case MapMode.ReadWrite:
					return GL.MapReadBit | GL.MapWriteBit;
				default:
					throw new InvalidOperationException("Unsupported map mode.");
			}
		}

		public static uint Map(this PrimitiveType primitiveType)
		{
			switch (primitiveType)
			{
				case PrimitiveType.LineList:
					return GL.Lines;
				case PrimitiveType.LineStrip:
					return GL.LineStrip;
				case PrimitiveType.TriangleList:
					return GL.Triangles;
				case PrimitiveType.TriangleStrip:
					return GL.TriangleStrip;
				default:
					throw new InvalidOperationException("Unsupported primitive type.");
			}
		}

		public static uint Map(this ResourceUsage resourceUsage)
		{
			switch (resourceUsage)
			{
				case ResourceUsage.Staging:
					return GL.DynamicRead;
				case ResourceUsage.Static:
					return GL.StaticDraw;
				case ResourceUsage.Default:
					return GL.StreamDraw;
				case ResourceUsage.Dynamic:
					return GL.DynamicDraw;
				default:
					throw new InvalidOperationException("Unsupported resource usage.");
			}
		}

		public static uint Map(this StencilOperation stencilOperation)
		{
			switch (stencilOperation)
			{
				case StencilOperation.Keep:
					return GL.Keep;
				case StencilOperation.Zero:
					return GL.Zero;
				case StencilOperation.Replace:
					return GL.Replace;
				case StencilOperation.IncrementAndClamp:
					return GL.Incr;
				case StencilOperation.DecrementAndClamp:
					return GL.Decr;
				case StencilOperation.Invert:
					return GL.Invert;
				case StencilOperation.Increment:
					return GL.IncrWrap;
				case StencilOperation.Decrement:
					return GL.DecrWrap;
				default:
					throw new InvalidOperationException("Unsupported stencil operation.");
			}
		}

		public static uint Map(this SurfaceFormat surfaceFormat)
		{
			switch (surfaceFormat)
			{
				case SurfaceFormat.Rgba8:
					return GL.Rgba;
				case SurfaceFormat.R8:
					return GL.R8;
				case SurfaceFormat.Bc1:
					return GL.CompressedRgbDxt1;
				case SurfaceFormat.Bc2:
					return GL.CompressedRgbaDxt3;
				case SurfaceFormat.Bc3:
					return GL.CompressedRgbaDxt5;
				case SurfaceFormat.Bc4:
					return GL.CompressedRedRgtc1;
				case SurfaceFormat.Bc5:
					return GL.CompressedRgRgtc2;
				case SurfaceFormat.Rgba16F:
					return GL.Rgba;
				case SurfaceFormat.Depth24Stencil8:
					return GL.DepthStencil;
				default:
					throw new InvalidOperationException("Unsupported surface format.");
			}
		}

		public static uint Map(this TextureAddressMode addressMode)
		{
			switch (addressMode)
			{
				case TextureAddressMode.Wrap:
					return GL.Repeat;
				case TextureAddressMode.Clamp:
					return GL.ClampToEdge;
				case TextureAddressMode.Border:
					return GL.ClampToBorder;
				default:
					throw new InvalidOperationException("Unsupported address mode.");
			}
		}
	}
}