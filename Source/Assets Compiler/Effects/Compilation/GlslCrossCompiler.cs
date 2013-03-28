using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	/// <summary>
	///   Cross-compiles a C# shader method to GLSL.
	/// </summary>
	internal class GlslCrossCompiler : CrossCompiler
	{
		/// <summary>
		///   Generates the shader code for shader literals.
		/// </summary>
		/// <param name="literal">The shader literal that should be generated.</param>
		protected override void GenerateLiteral(ShaderLiteral literal)
		{
			Writer.Append("const {0} {1}", ToGlsl(literal.Type), literal.Name);

			if (literal.IsArray)
				Writer.Append("[]");

			Writer.Append(" = ");

			if (literal.IsArray)
			{
				Writer.Append("( ");
				Writer.Append(String.Join(", ", (object[])literal.Value));
				Writer.Append(" )");
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
			Writer.AppendLine("layout(std140, binding = {0}) uniform {1}", constantBuffer.Slot, constantBuffer.Name);
			Writer.AppendBlockStatement(() =>
				{
					foreach (var constant in constantBuffer.Constants)
						Writer.AppendLine("{0} {1};", ToGlsl(constant.Type), constant.Name);
				});
			Writer.Newline();
		}

		/// <summary>
		///   Generates the shader code for texture objects.
		/// </summary>
		/// <param name="texture">The shader texture that should be generated.</param>
		protected override void GenerateTextureObject(ShaderTexture texture)
		{
			Writer.AppendLine("layout(binding = {0}) uniform {1} {2};", texture.Slot, ToGlsl(texture.Type), texture.Name);
		}

		/// <summary>
		///   Gets the corresponding GLSL type.
		/// </summary>
		/// <param name="type">The data type that should be converted.</param>
		private static string ToGlsl(DataType type)
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
					return "vec2";
				case DataType.Vector3:
					return "vec3";
				case DataType.Vector4:
					return "vec4";
				case DataType.Matrix:
					return "mat4";
				case DataType.Texture2D:
					return "sampler2D";
				case DataType.CubeMap:
					return "samplerCube";
				default:
					return "unknown-type";
			}
		}
	}
}