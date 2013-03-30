using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using System.Linq;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;

	/// <summary>
	///   Cross-compiles a C# shader method.
	/// </summary>
	internal abstract partial class CrossCompiler : IAstVisitor
	{
		/// <summary>
		///   Gets the code writer the generated code should be written to.
		/// </summary>
		protected CodeWriter Writer { get; private set; }

		/// <summary>
		///   Gets the shader that is compiled.
		/// </summary>
		protected ShaderMethod Shader { get; private set; }

		/// <summary>
		///   Gets the resolver that should be used to resolve type information.
		/// </summary>
		protected CSharpAstResolver Resolver { get; private set; }

		/// <summary>
		///   Cross-compiles the C# shader method.
		/// </summary>
		/// <param name="effect">The effect class the shader method belongs to.</param>
		/// <param name="shader">The C# shader method that should be cross-compiled.</param>
		/// <param name="writer">The code writer the generated code should be written to.</param>
		/// <param name="resolver">The C# resolver that should be used to resolve type information.</param>
		public void Compile(EffectClass effect, ShaderMethod shader, CodeWriter writer, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(effect, () => effect);
			Assert.ArgumentNotNull(shader, () => shader);
			Assert.ArgumentNotNull(writer, () => writer);
			Assert.ArgumentNotNull(resolver, () => resolver);

			Writer = writer;
			Resolver = resolver;
			Shader = shader;

			if (effect.Literals.Any())
			{
				foreach (var literal in effect.Literals)
					GenerateLiteral(literal);

				Writer.Newline();
			}

			foreach (var constantBuffer in effect.ConstantBuffers)
				GenerateConstantBuffer(constantBuffer);

			if (effect.Textures.Any())
			{
				foreach (var texture in effect.Textures)
					GenerateTextureObject(texture);
			}

			if (shader.Type == ShaderType.VertexShader)
			{
				GenerateVertexShaderInputs(shader.Inputs);
				Writer.Newline();

				GenerateVertexShaderOutputs(shader.Outputs);
				Writer.Newline();
			}
			else
			{
				GenerateFragmentShaderInputs(shader.Inputs);
				Writer.Newline();

				GenerateFragmentShaderOutputs(shader.Outputs);
				Writer.Newline();
			}

			GenerateMainMethod();
		}

		/// <summary>
		///   Generates the shader code for shader literals.
		/// </summary>
		/// <param name="literal">The shader literal that should be generated.</param>
		protected abstract void GenerateLiteral(ShaderLiteral literal);

		/// <summary>
		///   Generates the shader code for shader constant buffers.
		/// </summary>
		/// <param name="constantBuffer">The constant buffer that should be generated.</param>
		protected abstract void GenerateConstantBuffer(ConstantBuffer constantBuffer);

		/// <summary>
		///   Generates the shader code for texture objects.
		/// </summary>
		/// <param name="texture">The shader texture that should be generated.</param>
		protected abstract void GenerateTextureObject(ShaderTexture texture);

		/// <summary>
		///   Generates the shader inputs if the shader is a vertex shader.
		/// </summary>
		/// <param name="inputs">The shader inputs that should be generated.</param>
		protected abstract void GenerateVertexShaderInputs(IEnumerable<ShaderParameter> inputs);

		/// <summary>
		///   Generates the shader outputs if the shader is a vertex shader.
		/// </summary>
		/// <param name="outputs">The shader outputs that should be generated.</param>
		protected abstract void GenerateVertexShaderOutputs(IEnumerable<ShaderParameter> outputs);

		/// <summary>
		///   Generates the shader inputs if the shader is a fragment shader.
		/// </summary>
		/// <param name="inputs">The shader inputs that should be generated.</param>
		protected abstract void GenerateFragmentShaderInputs(IEnumerable<ShaderParameter> inputs);

		/// <summary>
		///   Generates the shader outputs if the shader is a fragment shader.
		/// </summary>
		/// <param name="outputs">The shader outputs that should be generated.</param>
		protected abstract void GenerateFragmentShaderOutputs(IEnumerable<ShaderParameter> outputs);

		/// <summary>
		///   Generates the shader entry point.
		/// </summary>
		protected abstract void GenerateMainMethod();

		/// <summary>
		///   Gets the corresponding shader type.
		/// </summary>
		/// <param name="type">The data type that should be converted.</param>
		protected abstract string ToShaderType(DataType type);

		/// <summary>
		///   Extracts the column and row indices from the list of indexer arguments.
		/// </summary>
		/// <param name="indexerArguments">The list of indexer arguments.</param>
		/// <param name="first">The expression that should be used as the first index.</param>
		/// <param name="second">The expression that should be used as the second index.</param>
		protected abstract void GetMatrixIndices(AstNodeCollection<Expression> indexerArguments, out Expression first,
												 out Expression second);

		/// <summary>
		///   Gets the token for the given assignment operator.
		/// </summary>
		/// <param name="assignmentOperator">The assignment operator for which the token should be returned.</param>
		private string GetToken(AssignmentOperatorType assignmentOperator)
		{
			switch (assignmentOperator)
			{
				case AssignmentOperatorType.Assign:
					return "=";
				case AssignmentOperatorType.Add:
					return "+=";
				case AssignmentOperatorType.Subtract:
					return "-=";
				case AssignmentOperatorType.Multiply:
					return "*=";
				case AssignmentOperatorType.Divide:
					return "/=";
				case AssignmentOperatorType.Modulus:
					return "%=";
				case AssignmentOperatorType.ShiftLeft:
					return "<<=";
				case AssignmentOperatorType.ShiftRight:
					return ">>=";
				case AssignmentOperatorType.BitwiseAnd:
					return "&=";
				case AssignmentOperatorType.BitwiseOr:
					return "|=";
				case AssignmentOperatorType.ExclusiveOr:
					return "^=";
				default:
					throw new NotSupportedException("Unsupported assignment operator type.");
			}
		}

		/// <summary>
		///   Gets the token for the given unary operator.
		/// </summary>
		/// <param name="unaryOperator">The unary operator for which the token should be returned.</param>
		private string GetToken(UnaryOperatorType unaryOperator)
		{
			switch (unaryOperator)
			{
				case UnaryOperatorType.Not:
					return "!";
				case UnaryOperatorType.BitNot:
					return "~";
				case UnaryOperatorType.Minus:
					return "-";
				case UnaryOperatorType.Plus:
					return "+";
				case UnaryOperatorType.PostIncrement:
				case UnaryOperatorType.Increment:
					return "++";
				case UnaryOperatorType.PostDecrement:
				case UnaryOperatorType.Decrement:
					return "--";
				default:
					throw new NotSupportedException("Unsupported unary operator type.");
			}
		}

		/// <summary>
		///   Gets the token for the given binary operator.
		/// </summary>
		/// <param name="binaryOperator">The binary operator for which the token should be returned.</param>
		private string GetToken(BinaryOperatorType binaryOperator)
		{
			switch (binaryOperator)
			{
				case BinaryOperatorType.BitwiseAnd:
					return "&";
				case BinaryOperatorType.BitwiseOr:
					return "|";
				case BinaryOperatorType.ConditionalAnd:
					return "&&";
				case BinaryOperatorType.ConditionalOr:
					return "||";
				case BinaryOperatorType.ExclusiveOr:
					return "^";
				case BinaryOperatorType.GreaterThan:
					return ">";
				case BinaryOperatorType.GreaterThanOrEqual:
					return ">=";
				case BinaryOperatorType.Equality:
					return "==";
				case BinaryOperatorType.InEquality:
					return "!=";
				case BinaryOperatorType.LessThan:
					return "<";
				case BinaryOperatorType.LessThanOrEqual:
					return "<=";
				case BinaryOperatorType.Add:
					return "+";
				case BinaryOperatorType.Subtract:
					return "-";
				case BinaryOperatorType.Multiply:
					return "*";
				case BinaryOperatorType.Divide:
					return "/";
				case BinaryOperatorType.Modulus:
					return "%";
				case BinaryOperatorType.ShiftLeft:
					return "<<";
				case BinaryOperatorType.ShiftRight:
					return ">>";
				default:
					throw new NotSupportedException("Unsupported binary operator type.");
			}
		}
		/// <summary>
		///   Gets the token for the given intrinsic function.
		/// </summary>
		/// <param name="intrinsic">The intrinsic function for which the token should be returned.</param>
		private string GetToken(Intrinsic intrinsic)
		{
			switch (intrinsic)
			{
				case Intrinsic.Cosinus:
					return "cos";
				default:
					throw new NotSupportedException("Unsupported intrinsic function.");
			}
		}

		/// <summary>
		///   Visits the given statement, treating it as a single-line block statement if it is not actually a block statement.
		/// </summary>
		/// <param name="statement">The statement that should be visited.</param>
		private void VisitStatementBlock(Statement statement)
		{
			if (statement is BlockStatement)
				statement.AcceptVisitor(this);
			else
				Writer.AppendBlockStatement(() =>
					{
						statement.AcceptVisitor(this);
						if (!OmitTerminatingSemicolon(statement))
							Writer.Append(";");
					});
		}

		/// <summary>
		///   Checks whether the terminating semicolon can be safely omitted from the statement.
		/// </summary>
		/// <param name="node">The node that should be checked.</param>
		private bool OmitTerminatingSemicolon(AstNode node)
		{
			return node is BlockStatement || node is ForStatement || node is WhileStatement || node is IfElseStatement;
		}
	}
}