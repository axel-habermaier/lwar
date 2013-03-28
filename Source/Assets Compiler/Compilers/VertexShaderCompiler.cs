using System;

namespace Pegasus.AssetsCompiler.Compilers
{
	using Assets;
	using Framework;
	using Framework.Platform;
	using Framework.Platform.Graphics;
	using SharpDX.D3DCompiler;

	/// <summary>
	///   Compiles vertex shaders.
	/// </summary>
	internal class VertexShaderCompiler : ShaderCompiler<VertexShaderAsset>
	{
		/// <summary>
		///   Compiles the asset.
		/// </summary>
		/// <param name="asset">The asset that should be compiled.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		protected override void CompileCore(VertexShaderAsset asset, BufferWriter buffer)
		{
			string glsl, hlsl;
			ExtractShaderCode(asset, out glsl, out hlsl);

			ShaderBytecode byteCode = null;
			if (Configuration.CompileHlsl)
			{
				byteCode = CompileHlslShader(asset, hlsl, "vs_4_0");
				CreateInputLayout(buffer, byteCode);
			}
			else
				buffer.WriteByte(0);

			WriteGlslShader(buffer, glsl);
			if (Configuration.CompileHlsl)
				buffer.Copy(byteCode);

			byteCode.SafeDispose();
		}

		/// <summary>
		///   Converts a semantic name string into its corresponding DataSemantics value.
		/// </summary>
		private static DataSemantics ConvertSemanticName(string semanticName)
		{
			switch (semanticName.ToUpper())
			{
				case "POSITION":
					return DataSemantics.Position;
				case "TEXCOORD":
					return DataSemantics.TexCoords0;
				case "COLOR":
					return DataSemantics.Color0;
				case "NORMAL":
					return DataSemantics.Normal;
				default:
					throw new InvalidOperationException(String.Format("Unknown semantic name: '{0}'.", semanticName));
			}
		}

		/// <summary>
		///   Creates the input layout for the vertex shader using shader reflection.
		/// </summary>
		/// <param name="shaderCode">The shader byte code.</param>
		/// <param name="buffer">The buffer the compilation output should be appended to.</param>
		private static void CreateInputLayout(BufferWriter buffer, byte[] shaderCode)
		{
			using (var reflectionInfo = new ShaderReflection(shaderCode))
			{
				var shaderDesc = reflectionInfo.Description;
				Assert.InRange(shaderDesc.InputParameters, 0, Byte.MaxValue);
				buffer.WriteByte((byte)shaderDesc.InputParameters);

				for (var i = 0; i < shaderDesc.InputParameters; i++)
				{
					var paramDesc = reflectionInfo.GetInputParameterDescription(i);

					var semantics = ConvertSemanticName(paramDesc.SemanticName);
					buffer.WriteByte((byte)semantics);

					if (paramDesc.ComponentType == RegisterComponentType.UInt32 || paramDesc.ComponentType == RegisterComponentType.SInt32)
						Log.Die("Unsupported shader input parameter type.");

					VertexDataFormat format;
					var xUsed = paramDesc.UsageMask.HasFlag(RegisterComponentMaskFlags.ComponentX);
					var yUsed = paramDesc.UsageMask.HasFlag(RegisterComponentMaskFlags.ComponentY);
					var zUsed = paramDesc.UsageMask.HasFlag(RegisterComponentMaskFlags.ComponentZ);
					var wUsed = paramDesc.UsageMask.HasFlag(RegisterComponentMaskFlags.ComponentW);

					// The type of color inputs is Vector4; change that to RGBA8 to save 12 bytes per vertex
					if (semantics == DataSemantics.Color0)
					{
						Assert.That(xUsed && yUsed && zUsed && wUsed, "Colors should always have 4 channels.");
						format = VertexDataFormat.Color;
					}
					else if (xUsed && yUsed && zUsed && wUsed)
						format = VertexDataFormat.Vector4;
					else if (xUsed && yUsed && zUsed)
						format = VertexDataFormat.Vector3;
					else if (xUsed && yUsed)
						format = VertexDataFormat.Vector2;
					else if (xUsed)
						format = VertexDataFormat.Single;
					else
						throw new InvalidOperationException("Unknown usage mask combination.");

					buffer.WriteByte((byte)format);
				}
			}
		}
	}
}