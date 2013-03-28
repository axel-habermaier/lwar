﻿using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Linq;
	using Framework.Platform.Graphics;

	/// <summary>
	///   Cross-compiles a C# shader method to HLSL.
	/// </summary>
	internal class HlslCrossCompiler : CrossCompiler
	{
		/// <summary>
		///   The name of the shader output structure.
		/// </summary>
		private const string OutputStructName = "__OUTPUT";

		/// <summary>
		///   The name of the shader input structure.
		/// </summary>
		private const string InputStructName = "__INPUT";

		/// <summary>
		///   Generates the shader code for shader literals.
		/// </summary>
		/// <param name="literal">The shader literal that should be generated.</param>
		protected override void GenerateLiteral(ShaderLiteral literal)
		{
			Writer.Append("static const {0} {1}", ToHlsl(literal.Type), literal.Name);

			if (literal.IsArray)
				Writer.Append("[]");

			Writer.Append(" = ");

			if (literal.IsArray)
			{
				Writer.Append("{{ ");
				Writer.Append(String.Join(", ", (object[])literal.Value));
				Writer.Append(" }}");
			}
			else
				Writer.Append(literal.Value.ToString().ToLower());

			Writer.AppendLine(";");
		}

		/// <summary>
		///   Generates the shader code for shader constant buffers.
		/// </summary>
		/// <param name="constantBuffer">The constant buffer that should be generated.</param>
		protected override void GenerateConstantBuffer(ConstantBuffer constantBuffer)
		{
			Writer.Append("cbuffer {0} : register(b{1})", constantBuffer.Name, constantBuffer.Slot);
			Writer.AppendBlockStatement(() =>
				{
					foreach (var constant in constantBuffer.Constants)
					{
						if (constant.Type == DataType.Matrix)
							Writer.Append("column_major ");

						Writer.AppendLine("{0} {1};", ToHlsl(constant.Type), constant.Name);
					}
				});
			Writer.Newline();
		}

		/// <summary>
		///   Generates the shader code for texture objects.
		/// </summary>
		/// <param name="texture">The shader texture that should be generated.</param>
		protected override void GenerateTextureObject(ShaderTexture texture)
		{
			Writer.AppendLine("{0} {1} : register(t{2});", ToHlsl(texture.Type), texture.Name, texture.Slot);
			Writer.AppendLine("SamplerState {0}Sampler : register(s{1});", texture.Name, texture.Slot);
			Writer.Newline();
		}

		/// <summary>
		///   Generates the shader inputs.
		/// </summary>
		protected override void GenerateInputs()
		{
			Writer.AppendLine("struct {0}", InputStructName);
			Writer.AppendBlockStatement(() =>
				{
					foreach (var input in Shader.Inputs)
						Writer.AppendLine("{0} {1} : {2};", ToHlsl(input.Type), input.Name, ToHlsl(input.Semantics));
				}, true);
		}

		/// <summary>
		///   Generates the shader outputs.
		/// </summary>
		protected override void GenerateOutputs()
		{
			Writer.AppendLine("struct {0}", OutputStructName);
			Writer.AppendBlockStatement(() =>
				{
					foreach (var output in Shader.Outputs)
					{
						if (Shader.Type == ShaderType.VertexShader && output.Semantics == DataSemantics.Position)
						{
							Writer.AppendLine("{0} {1} : SV_Position;", ToHlsl(output.Type), output.Name);
							continue;
						}

						var semantics = ToHlsl(output.Semantics);
						if (Shader.Type == ShaderType.FragmentShader && output.Semantics == DataSemantics.Color0)
							semantics = "SV_Target0";
						if (Shader.Type == ShaderType.FragmentShader && output.Semantics == DataSemantics.Color1)
							semantics = "SV_Target1";
						if (Shader.Type == ShaderType.FragmentShader && output.Semantics == DataSemantics.Color2)
							semantics = "SV_Target2";
						if (Shader.Type == ShaderType.FragmentShader && output.Semantics == DataSemantics.Color3)
							semantics = "SV_Target3";

						Writer.AppendLine("{0} {1} : {2};", ToHlsl(output.Type), output.Name, semantics);
					}
				}, true);
		}

		/// <summary>
		///   Gets the corresponding HLSL type.
		/// </summary>
		/// <param name="type">The data type that should be converted.</param>
		private static string ToHlsl(DataType type)
		{
			switch (type)
			{
				case DataType.Boolean:
					return "bool";
				case DataType.Integer:
					return "int";
				case DataType.Float:
					return "float";
				case DataType.Vector2:
					return "float2";
				case DataType.Vector3:
					return "float3";
				case DataType.Vector4:
					return "float4";
				case DataType.Matrix:
					return "float4x4";
				case DataType.Texture2D:
					return "Texture2D";
				case DataType.CubeMap:
					return "TextureCube";
				default:
					return "unknown-type";
			}
		}

		/// <summary>
		///   Gets the corresponding HLSL semantics.
		/// </summary>
		/// <param name="semantics">The data semantics that should be converted.</param>
		private static string ToHlsl(DataSemantics semantics)
		{
			switch (semantics)
			{
				case DataSemantics.Position:
					return "POSITION";
				case DataSemantics.Color0:
					return "COLOR0";
				case DataSemantics.Color1:
					return "COLOR1";
				case DataSemantics.Color2:
					return "COLOR2";
				case DataSemantics.Color3:
					return "COLOR3";
				case DataSemantics.Normal:
					return "NORMAL";
				case DataSemantics.TexCoords0:
					return "TEXCOORD0";
				case DataSemantics.TexCoords1:
					return "TEXCOORD1";
				case DataSemantics.TexCoords2:
					return "TEXCOORD2";
				case DataSemantics.TexCoords3:
					return "TEXCOORD3";
				default:
					return "unknown-semantics";
			}
		}
	}
}