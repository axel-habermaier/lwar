using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using Framework;
	using Framework.Platform.Graphics;
	using Semantics;

	/// <summary>
	///   Cross-compiles a C# shader method to GLSL.
	/// </summary>
	internal sealed class GlslCompiler : CrossCompiler
	{
		/// <summary>
		///   Generates the shader code for shader literals.
		/// </summary>
		/// <param name="literal">The shader literal that should be generated.</param>
		protected override void GenerateLiteral(ShaderLiteral literal)
		{
			Writer.Append("const {0} {1}", ToShaderType(literal.Type), literal.Name);

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
						Writer.AppendLine("{0} {1};", ToShaderType(constant.Type), constant.Name);
				});
			Writer.Newline();
		}

		/// <summary>
		///   Generates the shader code for texture objects.
		/// </summary>
		/// <param name="texture">The shader texture that should be generated.</param>
		protected override void GenerateTextureObject(ShaderTexture texture)
		{
			Writer.AppendLine("layout(binding = {0}) uniform {1} {2};", texture.Slot, ToShaderType(texture.Type), texture.Name);
		}

		/// <summary>
		///   Generates the shader inputs if the shader is a vertex shader.
		/// </summary>
		/// <param name="inputs">The shader inputs that should be generated.</param>
		protected override void GenerateVertexShaderInputs(IEnumerable<ShaderParameter> inputs)
		{
			foreach (var input in inputs)
				Writer.AppendLine("layout(location = {0}) in {1} {2};", (int)input.Semantics, ToShaderType(input.Type), input.Name);
		}

		/// <summary>
		///   Generates the shader outputs if the shader is a vertex shader.
		/// </summary>
		/// <param name="outputs">The shader outputs that should be generated.</param>
		protected override void GenerateVertexShaderOutputs(IEnumerable<ShaderParameter> outputs)
		{
			foreach (var output in outputs)
			{
				if (output.Semantics == DataSemantics.Position)
				{
					Writer.AppendLine("out gl_PerVertex");
					Writer.AppendBlockStatement(() => Writer.AppendLine("vec4 gl_Position;"));
				}
				else
					Writer.AppendLine("out {0} {1};", ToShaderType(output.Type), output.Name);
			}
		}

		/// <summary>
		///   Generates the shader inputs if the shader is a fragment shader.
		/// </summary>
		/// <param name="inputs">The shader inputs that should be generated.</param>
		protected override void GenerateFragmentShaderInputs(IEnumerable<ShaderParameter> inputs)
		{
			foreach (var input in inputs)
				Writer.AppendLine("in {0} {1};", ToShaderType(input.Type), input.Name);
		}

		/// <summary>
		///   Generates the shader outputs if the shader is a fragment shader.
		/// </summary>
		/// <param name="outputs">The shader outputs that should be generated.</param>
		protected override void GenerateFragmentShaderOutputs(IEnumerable<ShaderParameter> outputs)
		{
			foreach (var output in outputs)
			{
				var slot = output.Semantics - DataSemantics.Color0;
				Assert.InRange(slot, 0, SemanticsAttribute.MaximumIndex);

				Writer.AppendLine("layout(location = {2}) out {0} {1};", ToShaderType(output.Type), output.Name, slot);
			}
		}

		/// <summary>
		///   Generates the shader entry point.
		/// </summary>
		protected override void GenerateMainMethod()
		{
			Writer.AppendLine("void main()");
			Writer.AppendBlockStatement(() => Shader.MethodBody.Statements.AcceptVisitor(this));
		}

		/// <summary>
		///   Gets the corresponding GLSL type.
		/// </summary>
		/// <param name="type">The data type that should be converted.</param>
		protected override string ToShaderType(DataType type)
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
					throw new NotSupportedException("Unsupported data type.");
			}
		}
	}
}