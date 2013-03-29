//using System;

//namespace Pegasus.AssetsCompiler.Effects.Compilation
//{
//	using Framework;
//	using Framework.Platform.Graphics;

//	/// <summary>
//	///   Cross-compiles a C# shader method to GLSL.
//	/// </summary>
//	internal sealed class GlslCrossCompiler : CrossCompiler
//	{
//		/// <summary>
//		///   Generates the shader code for shader literals.
//		/// </summary>
//		/// <param name="literal">The shader literal that should be generated.</param>
//		protected override void GenerateLiteral(ShaderLiteral literal)
//		{
//			Writer.Append("const {0} {1}", ToShaderType(literal.Type), literal.Name);

//			if (literal.IsArray)
//				Writer.Append("[]");

//			Writer.Append(" = ");

//			if (literal.IsArray)
//			{
//				Writer.Append("( ");
//				Writer.Append(String.Join(", ", (object[])literal.Value));
//				Writer.Append(" )");
//			}
//			else
//				Writer.Append(literal.Value.ToString().ToLower());

//			Writer.AppendLine(";");
//		}

//		/// <summary>
//		///   Generates the shader code for shader constant buffers.
//		/// </summary>
//		/// <param name="constantBuffer">The constant buffer that should be generated.</param>
//		protected override void GenerateConstantBuffer(ConstantBuffer constantBuffer)
//		{
//			Writer.AppendLine("layout(std140, binding = {0}) uniform {1}", constantBuffer.Slot, constantBuffer.Name);
//			Writer.AppendBlockStatement(() =>
//				{
//					foreach (var constant in constantBuffer.Constants)
//						Writer.AppendLine("{0} {1};", ToShaderType(constant.Type), constant.Name);
//				});
//			Writer.Newline();
//		}

//		/// <summary>
//		///   Generates the shader code for texture objects.
//		/// </summary>
//		/// <param name="texture">The shader texture that should be generated.</param>
//		protected override void GenerateTextureObject(ShaderTexture texture)
//		{
//			Writer.AppendLine("layout(binding = {0}) uniform {1} {2};", texture.Slot, ToShaderType(texture.Type), texture.Name);
//		}

//		/// <summary>
//		///   Generates the shader inputs.
//		/// </summary>
//		protected override void GenerateInputs()
//		{
//			foreach (var input in Shader.Inputs)
//			{
//				switch (Shader.Type)
//				{
//					case ShaderType.VertexShader:
//						var slot = ToVertexDataSlot(input.Semantics);
//						Writer.AppendLine("layout(location = {0}) in {1} {2};", slot, ToShaderType(input.Type), input.Name);
//						break;
//					case ShaderType.FragmentShader:
//						Writer.AppendLine("in {0} {1};", ToShaderType(input.Type), input.Name);
//						break;
//					default:
//						throw new InvalidOperationException("Unsupported shader type.");
//				}
//			}
//		}

//		/// <summary>
//		///   Generates the shader outputs.
//		/// </summary>
//		protected override void GenerateOutputs()
//		{
//			foreach (var output in Shader.Outputs)
//			{
//				if (Shader.Type == ShaderType.VertexShader && output.Semantics == DataSemantics.Position)
//				{
//					Writer.AppendLine("out gl_PerVertex");
//					Writer.AppendBlockStatement(() => Writer.AppendLine("vec4 gl_Position;"));
//				}
//				else if (Shader.Type == ShaderType.FragmentShader)
//					Writer.AppendLine("layout(location = {2}) out {0} {1};", ToShaderType(output.Type), output.Name,
//									  output.Semantics - DataSemantics.Color0);
//				else
//					Writer.AppendLine("out {0} {1};", ToShaderType(output.Type), output.Name);
//			}
//		}

//		/// <summary>
//		///   Generates the shader entry point.
//		/// </summary>
//		protected override void GenerateMainMethod()
//		{
//			Writer.AppendLine("void main()");
//			Writer.AppendBlockStatement(GenerateShaderCode);
//		}

//		/// <summary>
//		///   Gets the corresponding GLSL type.
//		/// </summary>
//		/// <param name="type">The data type that should be converted.</param>
//		protected override string ToShaderType(DataType type)
//		{
//			switch (type)
//			{
//				case DataType.Boolean:
//					return "bool";
//				case DataType.Integer:
//					return "int";
//				case DataType.Float:
//					return "float";
//				case DataType.Vector2:
//					return "vec2";
//				case DataType.Vector3:
//					return "vec3";
//				case DataType.Vector4:
//					return "vec4";
//				case DataType.Matrix:
//					return "mat4";
//				case DataType.Texture2D:
//					return "sampler2D";
//				case DataType.CubeMap:
//					return "samplerCube";
//				default:
//					return "unknown-type";
//			}
//		}

//		/// <summary>
//		///   Converts the data semantics to a vertex data slot.
//		/// </summary>
//		/// <param name="semantics">The semantics that should be converted.</param>
//		private int ToVertexDataSlot(DataSemantics semantics)
//		{
//			DataSemantics vertexSemantics;

//			switch (semantics)
//			{
//				case DataSemantics.Position:
//					vertexSemantics = DataSemantics.Position;
//					break;
//				case DataSemantics.Normal:
//					vertexSemantics = DataSemantics.Normal;
//					break;
//				case DataSemantics.TexCoords0:
//					vertexSemantics = DataSemantics.TexCoords0;
//					break;
//				case DataSemantics.Color0:
//					vertexSemantics = DataSemantics.Color0;
//					break;
//				default:
//					Context.Error(Shader.ShaderCode,
//								  "Vertex shader '{0}' uses an unsupported input semantics '{1}'.", Shader.Name, semantics);
//					vertexSemantics = DataSemantics.Position;
//					break;
//			}

//			var slot = (int)vertexSemantics - (int)DataSemantics.Position;
//			Assert.InRange(slot, 0, 16);
//			return slot;
//		}
//	}
//}