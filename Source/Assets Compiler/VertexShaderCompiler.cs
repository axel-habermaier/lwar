using System;

namespace Pegasus.AssetsCompiler
{
	using Framework;
	using Framework.Platform.Graphics;
	using SharpDX.D3DCompiler;
	using SharpDX.DXGI;
	using SharpDX.Direct3D11;

	/// <summary>
	///   Compiles vertex shaders.
	/// </summary>
	internal class VertexShaderCompiler : ShaderCompiler
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		public VertexShaderCompiler(string asset)
			: base(asset)
		{
		}

		/// <summary>
		///   Gets a description of the asset type that the compiler supports.
		/// </summary>
		internal override string AssetType
		{
			get { return "Vertex Shaders"; }
		}

		/// <summary>
		///   Compiles the asset.
		/// </summary>
		protected override void CompileCore()
		{
			string glsl, hlsl;
			ExtractShaderCode(Asset, out glsl, out hlsl);

			WriteGlslShader(glsl);
			IfD3DSupported(() =>
				{
					using (var byteCode = CompileHlslShader(Asset, hlsl, "vs_4_0"))
					{
						Buffer.WriteByteArray(byteCode);
						CreateInputLayout(byteCode);
					}
				});
		}

		/// <summary>
		///   Converts a semantic name string into its corresponding VertexDataSemantics value.
		/// </summary>
		private static VertexDataSemantics ConvertSemanticName(string semanticName)
		{
			switch (semanticName.ToUpper())
			{
				case "POSITION":
					return VertexDataSemantics.Position;
				case "TEXCOORD":
					return VertexDataSemantics.TextureCoordinate;
				case "COLOR":
					return VertexDataSemantics.Color;
				case "NORMAL":
					return VertexDataSemantics.Normal;
				default:
					throw new InvalidOperationException(String.Format("Unknown semantic name: '{0}'.", semanticName));
			}
		}

		/// <summary>
		///   Creates the input layout for the vertex shader using shader reflection.
		/// </summary>
		/// <param name="shaderCode">The shader byte code.</param>
		private void CreateInputLayout(byte[] shaderCode)
		{
			using (var reflectionInfo = new ShaderReflection(shaderCode))
			{
				var shaderDesc = reflectionInfo.Description;

				var inputElements = new InputElement[shaderDesc.InputParameters];
				for (var i = 0; i < shaderDesc.InputParameters; i++)
				{
					var paramDesc = reflectionInfo.GetInputParameterDescription(i);

					var semantics = ConvertSemanticName(paramDesc.SemanticName);
					inputElements[i].Slot = (int)semantics;
					inputElements[i].SemanticName = paramDesc.SemanticName;
					inputElements[i].SemanticIndex = paramDesc.SemanticIndex;
					inputElements[i].AlignedByteOffset = 0;
					inputElements[i].Classification = InputClassification.PerVertexData;
					inputElements[i].InstanceDataStepRate = 0;

					var xUsed = paramDesc.UsageMask.HasFlag(RegisterComponentMaskFlags.ComponentX);
					var yUsed = paramDesc.UsageMask.HasFlag(RegisterComponentMaskFlags.ComponentY);
					var zUsed = paramDesc.UsageMask.HasFlag(RegisterComponentMaskFlags.ComponentZ);
					var wUsed = paramDesc.UsageMask.HasFlag(RegisterComponentMaskFlags.ComponentW);

					// The type of color inputs is Vector4; change that to RGBA8 to save 12 bytes per vertex!
					if (semantics == VertexDataSemantics.Color)
					{
						Assert.That(xUsed && yUsed && zUsed && wUsed, "Colors should always have 4 channels.");
						inputElements[i].Format = Format.R8G8B8A8_UNorm;
						continue;
					}

					if (xUsed && yUsed && zUsed && wUsed)
					{
						if (paramDesc.ComponentType == RegisterComponentType.UInt32)
							inputElements[i].Format = Format.R32G32B32A32_UInt;
						else if (paramDesc.ComponentType == RegisterComponentType.Sint32)
							inputElements[i].Format = Format.R32G32B32A32_SInt;
						else if (paramDesc.ComponentType == RegisterComponentType.Float32)
							inputElements[i].Format = Format.R32G32B32A32_Float;
					}
					else if (xUsed && yUsed && zUsed)
					{
						if (paramDesc.ComponentType == RegisterComponentType.UInt32)
							inputElements[i].Format = Format.R32G32B32_UInt;
						else if (paramDesc.ComponentType == RegisterComponentType.Sint32)
							inputElements[i].Format = Format.R32G32B32_SInt;
						else if (paramDesc.ComponentType == RegisterComponentType.Float32)
							inputElements[i].Format = Format.R32G32B32_Float;
					}
					else if (xUsed && yUsed)
					{
						if (paramDesc.ComponentType == RegisterComponentType.UInt32)
							inputElements[i].Format = Format.R32G32_UInt;
						else if (paramDesc.ComponentType == RegisterComponentType.Sint32)
							inputElements[i].Format = Format.R32G32_SInt;
						else if (paramDesc.ComponentType == RegisterComponentType.Float32)
							inputElements[i].Format = Format.R32G32_Float;
					}
					else if (xUsed)
					{
						if (paramDesc.ComponentType == RegisterComponentType.UInt32)
							inputElements[i].Format = Format.R32_UInt;
						else if (paramDesc.ComponentType == RegisterComponentType.Sint32)
							inputElements[i].Format = Format.R32_SInt;
						else if (paramDesc.ComponentType == RegisterComponentType.Float32)
							inputElements[i].Format = Format.R32_Float;
					}
					else
						throw new InvalidOperationException("Unknown usage mask combination.");
				}

				SerializeInputElements(inputElements);
			}
		}

		/// <summary>
		///   Serializes the input elements.
		/// </summary>
		/// <param name="elements">The elements that should be serialized.</param>
		private void SerializeInputElements(InputElement[] elements)
		{
			Buffer.WriteInt32(elements.Length);

			foreach (var element in elements)
			{
				Buffer.WriteInt32(element.AlignedByteOffset);
				Buffer.WriteInt32((int)element.Classification);
				Buffer.WriteInt32((int)element.Format);
				Buffer.WriteInt32(element.InstanceDataStepRate);
				Buffer.WriteInt32(element.SemanticIndex);
				Buffer.WriteString(element.SemanticName);
				Buffer.WriteInt32(element.Slot);
			}
		}
	}
}