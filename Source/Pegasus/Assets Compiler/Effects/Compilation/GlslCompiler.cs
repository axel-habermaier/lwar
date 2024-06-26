﻿namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;
	using Utilities;

	/// <summary>
	///     Cross-compiles a C# shader method to GLSL.
	/// </summary>
	internal sealed class GlslCompiler : CrossCompiler
	{
		/// <summary>
		///     Generates the shader code for shader literals.
		/// </summary>
		/// <param name="literal">The shader literal that should be generated.</param>
		protected override void GenerateLiteral(ShaderLiteral literal)
		{
			Writer.Append("const {0} {1}", ToShaderType(literal.Type), Escape(literal.Name));

			if (literal.IsArray)
				Writer.Append("[{0}]", literal.Value.GetConstantValues(Resolver).Length);

			Writer.Append(" = ");

			if (literal.IsArray)
			{
				Writer.Append("{0}[] ( ", ToShaderType(literal.Type));
				Writer.Append(String.Join(", ", literal.Value.GetConstantValues(Resolver)));
				Writer.Append(" )");
			}
			else
				literal.Value.AcceptVisitor(this);

			Writer.AppendLine(";");
		}

		/// <summary>
		///     Generates the shader code for shader constant buffers.
		/// </summary>
		/// <param name="constantBuffer">The constant buffer that should be generated.</param>
		protected override void GenerateConstantBuffer(ConstantBuffer constantBuffer)
		{
			Writer.AppendLine("layout(std140, binding = {0}) uniform {2}{1}", constantBuffer.Slot, constantBuffer.Name,
				CompilationContext.ReservedInternalIdentifierPrefix);
			Writer.AppendBlockStatement(() =>
			{
				foreach (var constant in constantBuffer.Constants)
				{
					Writer.Append("{0} {1}", ToShaderType(constant.Type), Escape(constant.Name));
					if (constant.IsArray)
						Writer.Append("[{0}]", constant.ArrayLength);
					Writer.AppendLine(";");
				}
			}, true);
			Writer.NewLine();
		}

		/// <summary>
		///     Generates the shader code for texture objects.
		/// </summary>
		/// <param name="texture">The shader texture that should be generated.</param>
		protected override void GenerateTextureObject(ShaderTexture texture)
		{
			Writer.AppendLine("layout(binding = {0}) uniform {1} {2};", texture.Slot, ToShaderType(texture.Type), Escape(texture.Name));
		}

		/// <summary>
		///     Generates the shader inputs if the shader is a vertex shader.
		/// </summary>
		/// <param name="inputs">The shader inputs that should be generated.</param>
		protected override void GenerateVertexShaderInputs(IEnumerable<ShaderParameter> inputs)
		{
			foreach (var input in inputs)
				Writer.AppendLine("layout(location = {0}) in {1} {2};", (int)input.Semantics, ToShaderType(input.Type),
					Escape(input.Name));
		}

		/// <summary>
		///     Generates the shader outputs if the shader is a vertex shader.
		/// </summary>
		/// <param name="outputs">The shader outputs that should be generated.</param>
		protected override void GenerateVertexShaderOutputs(IEnumerable<ShaderParameter> outputs)
		{
			foreach (var output in outputs)
				Writer.AppendLine("layout(location = {2}) out {0} {1};", ToShaderType(output.Type), Escape(output.Name), (int)output.Semantics);
		}

		/// <summary>
		///     Generates the shader inputs if the shader is a fragment shader.
		/// </summary>
		/// <param name="inputs">The shader inputs that should be generated.</param>
		protected override void GenerateFragmentShaderInputs(IEnumerable<ShaderParameter> inputs)
		{
			foreach (var input in inputs)
				Writer.AppendLine("layout(location = {2}) in {0} {1};", ToShaderType(input.Type), Escape(input.Name), (int)input.Semantics);
		}

		/// <summary>
		///     Generates the shader outputs if the shader is a fragment shader.
		/// </summary>
		/// <param name="outputs">The shader outputs that should be generated.</param>
		protected override void GenerateFragmentShaderOutputs(IEnumerable<ShaderParameter> outputs)
		{
			foreach (var output in outputs)
			{
				var slot = output.Semantics - DataSemantics.Color0;
				Assert.InRange(slot, 0, SemanticsAttribute.MaximumIndex);

				Writer.AppendLine("layout(location = {2}) out {0} {1};", ToShaderType(output.Type), Escape(output.Name), slot);
			}
		}

		/// <summary>
		///     Generates the shader entry point.
		/// </summary>
		protected override void GenerateMainMethod()
		{
			Writer.AppendLine("void main()");
			Writer.AppendBlockStatement(() =>
			{
				foreach (var input in Shader.Inputs)
					Writer.AppendLine("{0} {1}{2} = {3};", ToShaderType(input.Type),
						CompilationContext.ReservedInternalIdentifierPrefix, input.Name, Escape(input.Name));

				Writer.NewLine();
				Shader.MethodBody.AcceptVisitor(this);
			});
		}

		/// <summary>
		///     Gets the corresponding GLSL type.
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
		///     Gets the token for the given intrinsic function.
		/// </summary>
		/// <param name="intrinsic">The intrinsic function for which the token should be returned.</param>
		protected override string GetToken(Intrinsic intrinsic)
		{
			switch (intrinsic)
			{
				case Intrinsic.InverseSquareRoot:
					return "inversesqrt";
				case Intrinsic.Lerp:
					return "mix";
				default:
					return base.GetToken(intrinsic);
			}
		}

		public override void VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			var local = Resolver.Resolve(identifierExpression) as LocalResolveResult;
			if (local != null && local.IsParameter && GeneratingMainMethod)
			{
				var parameter = Shader.Parameters.Single(p => p.Name == local.Variable.Name);
				if (Shader.Type == ShaderType.VertexShader && parameter.IsOutput && parameter.Semantics == DataSemantics.Position)
				{
					Writer.Append("gl_Position");
					return;
				}

				if (!parameter.IsOutput)
				{
					Writer.Append("{0}{1}", CompilationContext.ReservedInternalIdentifierPrefix, identifierExpression.Identifier);
					return;
				}
			}

			base.VisitIdentifierExpression(identifierExpression);
		}

		protected override void VisitIntrinsicExpression(InvocationExpression invocationExpression)
		{
			var intrinsic = invocationExpression.ResolveIntrinsic(Resolver);
			if (intrinsic != Intrinsic.Sample && intrinsic != Intrinsic.SampleLevel && intrinsic != Intrinsic.Saturate)
			{
				base.VisitIntrinsicExpression(invocationExpression);
				return;
			}

			if (intrinsic == Intrinsic.Saturate)
			{
				Writer.Append("clamp(");
				invocationExpression.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
				Writer.Append(", 0.0f, 1.0f)");
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
				invocationExpression.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
			}

			Writer.Append(")");
		}

		/// <summary>
		///     Extracts the column and row indices from the list of indexer arguments.
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