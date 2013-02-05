using System;

namespace Pegasus.AssetsCompiler
{
	using System.IO;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;
	using SharpDX.D3DCompiler;
	using SharpDX.DXGI;
	using SharpDX.Direct3D11;

	/// <summary>
	///   Processes HLSL shaders.
	/// </summary>
	public sealed class HlslShaderProcessor : AssetProcessor
	{
		/// <summary>
		///   Gets a value indicating whether the file extension should be removed from the compiled asset name.
		/// </summary>
		public override bool RemoveExtension
		{
			get { return false; }
		}

		/// <summary>
		///   Returns true if the processor can process a file with the given extension.
		/// </summary>
		/// <param name="extension">The extension of the file that should be processed.</param>
		public override bool CanProcess(string extension)
		{
			return extension == ".hlsl";
		}

		/// <summary>
		///   Processes the given file, writing the compiled output to the given target destination.
		/// </summary>
		/// <param name="source">The source file that should be processed.</param>
		/// <param name="sourceRelative">The path to the source file relative to the Assets root directory.</param>
		/// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		public override void Process(string source, string sourceRelative, BufferWriter writer)
		{
			Assert.ArgumentNotNullOrWhitespace(source, () => source);
			Assert.ArgumentNotNull(writer, () => writer);

			try
			{
				var shaderCode = File.ReadAllText(source);
				var firstLineEnd = shaderCode.IndexOf("\r\n");
				if (firstLineEnd == -1)
					throw new InvalidOperationException("Shader must define profile in first line. No line break found.");

				var profile = shaderCode.Substring(1, firstLineEnd - 1);
				if (profile != "vs_4_0" && profile != "ps_4_0")
					throw new InvalidOperationException(String.Format("Unknown shader profile '{0}'", profile));

				var flags = ShaderFlags.EnableStrictness;
#if DEBUG
				flags |= ShaderFlags.Debug;
#endif
				var result = ShaderBytecode.Compile(shaderCode.Substring(firstLineEnd), "Main", profile, flags, EffectFlags.None, source);
				if (result.HasErrors)
					throw new InvalidOperationException(result.Message);

				writer.WriteByteArray(result.Bytecode);

				if (profile.StartsWith("vs_"))
					CreateInputLayout(result.Bytecode, writer);
			}
			catch (DllNotFoundException)
			{
				Log.Warn("Ignoring HLSL shader '{0}' because Direct3D is not supported.", source);
			}
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
		/// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		private void CreateInputLayout(byte[] shaderCode, BufferWriter writer)
		{
			var reflectionInfo = new ShaderReflection(shaderCode);
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

			reflectionInfo.Dispose();
			SerializeInputElements(inputElements, writer);
		}

		/// <summary>
		///   Serializes the input elements.
		/// </summary>
		/// <param name="elements">The elements that should be serialized.</param>
		/// <param name="writer">The writer that should be used to write the compiled asset file.</param>
		private void SerializeInputElements(InputElement[] elements, BufferWriter writer)
		{
			writer.WriteInt32(elements.Length);

			foreach (var element in elements)
			{
				writer.WriteInt32(element.AlignedByteOffset);
				writer.WriteInt32((int)element.Classification);
				writer.WriteInt32((int)element.Format);
				writer.WriteInt32(element.InstanceDataStepRate);
				writer.WriteInt32(element.SemanticIndex);
				writer.WriteString(element.SemanticName);
				writer.WriteInt32(element.Slot);
			}
		}
	}
}