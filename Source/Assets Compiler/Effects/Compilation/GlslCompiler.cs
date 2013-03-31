using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using Effects;

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
			Shader.MethodBody.AcceptVisitor(this);
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

		/// <summary>
		///   Gets the token for the given intrinsic function.
		/// </summary>
		/// <param name="intrinsic">The intrinsic function for which the token should be returned.</param>
		protected override string GetToken(Intrinsic intrinsic)
		{
			switch (intrinsic)
			{
				case Intrinsic.InverseSquareRoot:
					return "inversesqrt";
				default:
					return base.GetToken(intrinsic);
			}
		}

		public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			var local = Resolver.Resolve(identifierExpression) as LocalResolveResult;
			if (Shader.Type == ShaderType.VertexShader && local != null && local.IsParameter)
			{
				var parameter = Shader.Parameters.Single(p => p.Name == local.Variable.Name);
				if (parameter.IsOutput && parameter.Semantics == DataSemantics.Position)
				{
					Writer.Append("gl_Position");
					return;
				}
			}

			base.VisitIdentifierExpression(identifierExpression);
		}

		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			var intrinsic = invocationExpression.ResolveIntrinsic(Resolver);
			if (intrinsic != Intrinsic.Sample && intrinsic != Intrinsic.SampleLevel)
			{
				base.VisitInvocationExpression(invocationExpression);
				return;
			}

			if (intrinsic == Intrinsic.Sample)
				Writer.Append("texture(");

			if (intrinsic == Intrinsic.SampleLevel)
				Writer.Append("textureLod(");

			((MemberReferenceExpression)invocationExpression.Target).Target.AcceptVisitor(this);
			if (invocationExpression.Arguments.Count > 0)
			{
				Writer.Append(", ");
				invocationExpression.Arguments.AcceptVisitor(this, ()=>Writer.Append(", "));
			}

			Writer.Append(")");
		}

		/// <summary>
		///   Extracts the column and row indices from the list of indexer arguments.
		/// </summary>
		/// <param name="indexerArguments">The list of indexer arguments.</param>
		/// <param name="first">The expression that should be used as the first index.</param>
		/// <param name="second">The expression that should be used as the second index.</param>
		protected override void GetMatrixIndices(AstNodeCollection<Expression> indexerArguments, out Expression first,
												 out Expression second)
		{
			first = indexerArguments.Skip(1).Single();
			second = indexerArguments.First();
		}
	}
}