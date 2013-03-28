using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	/// <summary>
	///   Cross-compiles a C# shader method to HLSL.
	/// </summary>
	internal class HlslCrossCompiler : CrossCompiler
	{
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
	}
}