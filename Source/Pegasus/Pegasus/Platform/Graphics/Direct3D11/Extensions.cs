namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using Bindings;
	using Logging;
	using Rendering;

	internal static class Extensions
	{
		[DllImport("Kernel32.dll", EntryPoint = "FormatMessageW", SetLastError = true)]
		private static extern unsafe int FormatMessage(uint flags, IntPtr source, D3D11Result error, int language, void* buffer,
													   int size, IntPtr[] arguments);

		[DebuggerHidden]
		public static void CheckSuccess(this D3D11Result result, string message)
		{
			if (!result.Success)
				Log.Die("{0}", GetErrorMessage(result, message));
		}

		public static unsafe string GetErrorMessage(this D3D11Result result, string message)
		{
			const uint ignoreInserts = 0x00000200;
			const uint fromSystem = 0x00001000;
			const int bufferSize = 2048;
			byte* buffer = stackalloc byte[bufferSize];
			var length = FormatMessage(fromSystem | ignoreInserts, IntPtr.Zero, result, 0, buffer, bufferSize, null);

			if (length > 0)
				return String.Format("{0} {1}", message, Marshal.PtrToStringAuto(new IntPtr(buffer)).Trim());

			return String.Format("{0} (unknown error code)", message);
		}

		public static int GetSemanticIndex(this DataSemantics semantics)
		{
			switch (semantics)
			{
				case DataSemantics.Position:
					return 0;
				case DataSemantics.Color0:
					return 0;
				case DataSemantics.Color1:
					return 1;
				case DataSemantics.Color2:
					return 2;
				case DataSemantics.Color3:
					return 3;
				case DataSemantics.TexCoords0:
					return 0;
				case DataSemantics.TexCoords1:
					return 1;
				case DataSemantics.TexCoords2:
					return 2;
				case DataSemantics.TexCoords3:
					return 3;
				case DataSemantics.Normal:
					return 0;
				default:
					throw new InvalidOperationException("Unsupported data semantics.");
			}
		}

		public static unsafe D3D11Color Map(this Color color)
		{
			float* components = stackalloc float[4];
			color.ToFloatArray(components);

			return new D3D11Color
			{
				Red = components[0],
				Green = components[1],
				Blue = components[2],
				Alpha = components[3],
			};
		}

		public static string GetSemanticName(this DataSemantics semantics)
		{
			switch (semantics)
			{
				case DataSemantics.Position:
					return "POSITION";
				case DataSemantics.Color0:
					return "COLOR";
				case DataSemantics.Color1:
					return "COLOR";
				case DataSemantics.Color2:
					return "COLOR";
				case DataSemantics.Color3:
					return "COLOR";
				case DataSemantics.TexCoords0:
					return "TEXCOORD";
				case DataSemantics.TexCoords1:
					return "TEXCOORD";
				case DataSemantics.TexCoords2:
					return "TEXCOORD";
				case DataSemantics.TexCoords3:
					return "TEXCOORD";
				case DataSemantics.Normal:
					return "NORMAL";
				default:
					throw new InvalidOperationException("Unsupported data semantics.");
			}
		}

		public static D3D11BlendOperation Map(this BlendOperation blendOperation)
		{
			switch (blendOperation)
			{
				case BlendOperation.Add:
					return D3D11BlendOperation.Add;
				case BlendOperation.Subtract:
					return D3D11BlendOperation.Subtract;
				case BlendOperation.ReverseSubtract:
					return D3D11BlendOperation.ReverseSubtract;
				case BlendOperation.Minimum:
					return D3D11BlendOperation.Minimum;
				case BlendOperation.Maximum:
					return D3D11BlendOperation.Maximum;
				default:
					throw new InvalidOperationException("Unsupported blend operation.");
			}
		}

		public static D3D11BlendOption Map(this BlendOption blendOption)
		{
			switch (blendOption)
			{
				case BlendOption.Zero:
					return D3D11BlendOption.Zero;
				case BlendOption.One:
					return D3D11BlendOption.One;
				case BlendOption.SourceColor:
					return D3D11BlendOption.SourceColor;
				case BlendOption.InverseSourceColor:
					return D3D11BlendOption.InverseSourceColor;
				case BlendOption.SourceAlpha:
					return D3D11BlendOption.SourceAlpha;
				case BlendOption.InverseSourceAlpha:
					return D3D11BlendOption.InverseSourceAlpha;
				case BlendOption.DestinationAlpha:
					return D3D11BlendOption.DestinationAlpha;
				case BlendOption.InverseDestinationAlpha:
					return D3D11BlendOption.InverseDestinationAlpha;
				case BlendOption.DestinationColor:
					return D3D11BlendOption.DestinationColor;
				case BlendOption.InverseDestinationColor:
					return D3D11BlendOption.InverseDestinationColor;
				case BlendOption.SourceAlphaSaturate:
					return D3D11BlendOption.SourceAlphaSaturate;
				case BlendOption.BlendFactor:
					return D3D11BlendOption.BlendFactor;
				case BlendOption.InverseBlendFactor:
					return D3D11BlendOption.InverseBlendFactor;
				default:
					throw new InvalidOperationException("Unsupported blend option.");
			}
		}

		public static D3D11Comparison Map(this Comparison comparison)
		{
			switch (comparison)
			{
				case Comparison.Always:
					return D3D11Comparison.Always;
				case Comparison.Equal:
					return D3D11Comparison.Equal;
				case Comparison.Greater:
					return D3D11Comparison.Greater;
				case Comparison.Less:
					return D3D11Comparison.Less;
				case Comparison.GreaterEqual:
					return D3D11Comparison.GreaterEqual;
				case Comparison.LessEqual:
					return D3D11Comparison.LessEqual;
				case Comparison.Never:
					return D3D11Comparison.Never;
				case Comparison.NotEqual:
					return D3D11Comparison.NotEqual;
				default:
					throw new InvalidOperationException("Unsupported comparison.");
			}
		}

		public static D3D11ColorWriteMaskFlags Map(this ColorWriteChannels channels)
		{
			var mask = (D3D11ColorWriteMaskFlags)0;

			if ((channels & ColorWriteChannels.Red) == ColorWriteChannels.Red)
				mask |= D3D11ColorWriteMaskFlags.Red;
			if ((channels & ColorWriteChannels.Green) == ColorWriteChannels.Green)
				mask |= D3D11ColorWriteMaskFlags.Green;
			if ((channels & ColorWriteChannels.Blue) == ColorWriteChannels.Blue)
				mask |= D3D11ColorWriteMaskFlags.Blue;
			if ((channels & ColorWriteChannels.Alpha) == ColorWriteChannels.Alpha)
				mask |= D3D11ColorWriteMaskFlags.Alpha;

			return mask;
		}

		public static D3D11CullMode Map(this CullMode cullMode)
		{
			switch (cullMode)
			{
				case CullMode.None:
					return D3D11CullMode.None;
				case CullMode.Front:
					return D3D11CullMode.Front;
				case CullMode.Back:
					return D3D11CullMode.Back;
				default:
					throw new InvalidOperationException("Unsupported cull mode.");
			}
		}

		public static D3D11FillMode Map(this FillMode fillMode)
		{
			switch (fillMode)
			{
				case FillMode.Wireframe:
					return D3D11FillMode.Wireframe;
				case FillMode.Solid:
					return D3D11FillMode.Solid;
				default:
					throw new InvalidOperationException("Unsupported fill mode.");
			}
		}

		public static DXGIFormat Map(this IndexSize indexSize)
		{
			switch (indexSize)
			{
				case IndexSize.SixteenBits:
					return DXGIFormat.R16_UInt;
				case IndexSize.ThirtyTwoBits:
					return DXGIFormat.R32_UInt;
				default:
					throw new InvalidOperationException("Unsupported index size.");
			}
		}

		public static D3D11MapMode Map(this MapMode mapMode)
		{
			switch (mapMode)
			{
				case MapMode.Read:
					return D3D11MapMode.Read;
				case MapMode.Write:
					return D3D11MapMode.Write;
				case MapMode.WriteDiscard:
					return D3D11MapMode.WriteDiscard;
				case MapMode.WriteNoOverwrite:
					return D3D11MapMode.WriteNoOverwrite;
				case MapMode.ReadWrite:
					return D3D11MapMode.ReadWrite;
				default:
					throw new InvalidOperationException("Unsupported map mode.");
			}
		}

		public static D3D11PrimitiveTopology Map(this PrimitiveType primitiveType)
		{
			switch (primitiveType)
			{
				case PrimitiveType.LineList:
					return D3D11PrimitiveTopology.LineList;
				case PrimitiveType.LineStrip:
					return D3D11PrimitiveTopology.LineStrip;
				case PrimitiveType.TriangleList:
					return D3D11PrimitiveTopology.TriangleList;
				case PrimitiveType.TriangleStrip:
					return D3D11PrimitiveTopology.TriangleStrip;
				default:
					throw new InvalidOperationException("Unsupported primitive type.");
			}
		}

		public static D3D11ResourceUsage Map(this ResourceUsage resourceUsage)
		{
			switch (resourceUsage)
			{
				case ResourceUsage.Staging:
					return D3D11ResourceUsage.Staging;
				case ResourceUsage.Static:
					return D3D11ResourceUsage.Immutable;
				case ResourceUsage.Default:
					return D3D11ResourceUsage.Default;
				case ResourceUsage.Dynamic:
					return D3D11ResourceUsage.Dynamic;
				default:
					throw new InvalidOperationException("Unsupported resource usage.");
			}
		}

		public static D3D11StencilOperation Map(this StencilOperation stencilOperation)
		{
			switch (stencilOperation)
			{
				case StencilOperation.Keep:
					return D3D11StencilOperation.Keep;
				case StencilOperation.Zero:
					return D3D11StencilOperation.Zero;
				case StencilOperation.Replace:
					return D3D11StencilOperation.Replace;
				case StencilOperation.IncrementAndClamp:
					return D3D11StencilOperation.IncrementAndClamp;
				case StencilOperation.DecrementAndClamp:
					return D3D11StencilOperation.DecrementAndClamp;
				case StencilOperation.Invert:
					return D3D11StencilOperation.Invert;
				case StencilOperation.Increment:
					return D3D11StencilOperation.Increment;
				case StencilOperation.Decrement:
					return D3D11StencilOperation.Decrement;
				default:
					throw new InvalidOperationException("Unsupported stencil operation.");
			}
		}

		public static DXGIFormat Map(this SurfaceFormat surfaceFormat)
		{
			switch (surfaceFormat)
			{
				case SurfaceFormat.R8:
					return DXGIFormat.R8_UNorm;
				case SurfaceFormat.Rgba8:
					return DXGIFormat.R8G8B8A8_UNorm;
				case SurfaceFormat.Depth24Stencil8:
					return DXGIFormat.D24_UNorm_S8_UInt;
				case SurfaceFormat.Bc1:
					return DXGIFormat.BC1_UNorm;
				case SurfaceFormat.Bc2:
					return DXGIFormat.BC2_UNorm;
				case SurfaceFormat.Bc3:
					return DXGIFormat.BC3_UNorm;
				case SurfaceFormat.Bc4:
					return DXGIFormat.BC4_UNorm;
				case SurfaceFormat.Bc5:
					return DXGIFormat.BC5_UNorm;
				case SurfaceFormat.Rgba16F:
					return DXGIFormat.R16G16B16A16_Float;
				default:
					throw new InvalidOperationException("Unsupported surface format.");
			}
		}

		public static D3D11TextureAddressMode Map(this TextureAddressMode addressMode)
		{
			switch (addressMode)
			{
				case TextureAddressMode.Wrap:
					return D3D11TextureAddressMode.Wrap;
				case TextureAddressMode.Clamp:
					return D3D11TextureAddressMode.Clamp;
				case TextureAddressMode.Border:
					return D3D11TextureAddressMode.Border;
				default:
					throw new InvalidOperationException("Unsupported texture address mode.");
			}
		}

		public static D3D11Filter Map(this TextureFilter textureFilter)
		{
			switch (textureFilter)
			{
				case TextureFilter.NearestNoMipmaps:
				case TextureFilter.Nearest:
					return D3D11Filter.MinMagMipPoint;
				case TextureFilter.BilinearNoMipmaps:
				case TextureFilter.Bilinear:
					return D3D11Filter.MinMagLinearMipPoint;
				case TextureFilter.Trilinear:
					return D3D11Filter.MinMagMipLinear;
				case TextureFilter.Anisotropic:
					return D3D11Filter.Anisotropic;
				default:
					throw new InvalidOperationException("Unsupported texture filter.");
			}
		}

		public static DXGIFormat Map(this VertexDataFormat format)
		{
			switch (format)
			{
				case VertexDataFormat.Single:
					return DXGIFormat.R32_Float;
				case VertexDataFormat.Vector2:
					return DXGIFormat.R32G32_Float;
				case VertexDataFormat.Vector3:
					return DXGIFormat.R32G32B32_Float;
				case VertexDataFormat.Vector4:
					return DXGIFormat.R32G32B32A32_Float;
				case VertexDataFormat.Color:
					return DXGIFormat.R8G8B8A8_UNorm;
				default:
					throw new InvalidOperationException("Unsupported vertex data format.");
			}
		}
	}
}