namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using CSharp;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using Utilities;

	/// <summary>
	///     Cross-compiles a C# shader method.
	/// </summary>
	internal abstract partial class CrossCompiler : IAstVisitor
	{
		/// <summary>
		///     Gets the code writer the generated code should be written to.
		/// </summary>
		protected CodeWriter Writer { get; private set; }

		/// <summary>
		///     Gets the shader that is compiled.
		/// </summary>
		protected ShaderMethod Shader { get; private set; }

		/// <summary>
		///     Gets the resolver that should be used to resolve type information.
		/// </summary>
		protected CSharpAstResolver Resolver { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the shader main method is being compiled.
		/// </summary>
		protected bool GeneratingMainMethod { get; private set; }

		/// <summary>
		///     Cross-compiles the C# shader method.
		/// </summary>
		/// <param name="effect">The effect class the shader method belongs to.</param>
		/// <param name="shader">The C# shader method that should be cross-compiled.</param>
		/// <param name="writer">The code writer the generated code should be written to.</param>
		/// <param name="resolver">The C# resolver that should be used to resolve type information.</param>
		public void Compile(EffectClass effect, ShaderMethod shader, CodeWriter writer, CSharpAstResolver resolver)
		{
			Assert.ArgumentNotNull(effect);
			Assert.ArgumentNotNull(shader);
			Assert.ArgumentNotNull(writer);
			Assert.ArgumentNotNull(resolver);

			Writer = writer;
			Resolver = resolver;
			Shader = shader;

			var literals = effect.Literals.Where(shader.Uses).ToArray();
			if (literals.Length != 0)
			{
				foreach (var literal in literals)
					GenerateLiteral(literal);

				Writer.NewLine();
			}

			foreach (var constantBuffer in effect.ConstantBuffers.Where(shader.Uses))
				GenerateConstantBuffer(constantBuffer);

			foreach (var texture in effect.Textures.Where(shader.Uses))
				GenerateTextureObject(texture);

			if (shader.Type == ShaderType.VertexShader)
			{
				GenerateVertexShaderInputs(shader.Inputs);
				Writer.NewLine();

				GenerateVertexShaderOutputs(shader.Outputs);
				Writer.NewLine();
			}
			else
			{
				GenerateFragmentShaderInputs(shader.Inputs);
				Writer.NewLine();

				GenerateFragmentShaderOutputs(shader.Outputs);
				Writer.NewLine();
			}

			foreach (var method in effect.HelperMethods.Where(shader.Uses))
				GenerateMethod(method);

			GeneratingMainMethod = true;
			GenerateMainMethod();
		}

		/// <summary>
		///     Generates the shader code for shader literals.
		/// </summary>
		/// <param name="literal">The shader literal that should be generated.</param>
		protected abstract void GenerateLiteral(ShaderLiteral literal);

		/// <summary>
		///     Generates the shader code for shader constant buffers.
		/// </summary>
		/// <param name="constantBuffer">The constant buffer that should be generated.</param>
		protected abstract void GenerateConstantBuffer(ConstantBuffer constantBuffer);

		/// <summary>
		///     Generates the shader code for texture objects.
		/// </summary>
		/// <param name="texture">The shader texture that should be generated.</param>
		protected abstract void GenerateTextureObject(ShaderTexture texture);

		/// <summary>
		///     Generates the shader inputs if the shader is a vertex shader.
		/// </summary>
		/// <param name="inputs">The shader inputs that should be generated.</param>
		protected abstract void GenerateVertexShaderInputs(IEnumerable<ShaderParameter> inputs);

		/// <summary>
		///     Generates the shader outputs if the shader is a vertex shader.
		/// </summary>
		/// <param name="outputs">The shader outputs that should be generated.</param>
		protected abstract void GenerateVertexShaderOutputs(IEnumerable<ShaderParameter> outputs);

		/// <summary>
		///     Generates the shader inputs if the shader is a fragment shader.
		/// </summary>
		/// <param name="inputs">The shader inputs that should be generated.</param>
		protected abstract void GenerateFragmentShaderInputs(IEnumerable<ShaderParameter> inputs);

		/// <summary>
		///     Generates the shader outputs if the shader is a fragment shader.
		/// </summary>
		/// <param name="outputs">The shader outputs that should be generated.</param>
		protected abstract void GenerateFragmentShaderOutputs(IEnumerable<ShaderParameter> outputs);

		/// <summary>
		///     Generates the shader entry point.
		/// </summary>
		protected abstract void GenerateMainMethod();

		/// <summary>
		///     Generates the shader entry point.
		/// </summary>
		/// <param name="method">The C# method the shader code should be generated for.</param>
		protected virtual void GenerateMethod(ShaderMethod method)
		{
			Writer.Append("{0} {1}(", ToShaderType(method.ReturnType), Escape(method.Name));
			Writer.AppendSeparated(method.Parameters, ", ", p => Writer.Append("{0} {1}", ToShaderType(p.Type), Escape(p.Name)));
			Writer.AppendLine(")");
			Writer.AppendBlockStatement(() => method.MethodBody.AcceptVisitor(this));
		}

		/// <summary>
		///     Gets the corresponding shader type.
		/// </summary>
		/// <param name="type">The data type that should be converted.</param>
		protected abstract string ToShaderType(DataType type);

		/// <summary>
		///     Extracts the column and row indices from the list of indexer arguments.
		/// </summary>
		/// <param name="indexerArguments">The list of indexer arguments.</param>
		/// <param name="first">The expression that should be used as the first index.</param>
		/// <param name="second">The expression that should be used as the second index.</param>
		protected abstract void GetMatrixIndices(AstNodeCollection<Expression> indexerArguments, out Expression first,
												 out Expression second);

		/// <summary>
		///     Gets the token for the given assignment operator.
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
		///     Gets the token for the given unary operator.
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
		///     Gets the token for the given binary operator.
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
		///     Gets the token for the given intrinsic function.
		/// </summary>
		/// <param name="intrinsic">The intrinsic function for which the token should be returned.</param>
		protected virtual string GetToken(Intrinsic intrinsic)
		{
			switch (intrinsic)
			{
				case Intrinsic.Sine:
					return "sin";
				case Intrinsic.Cosine:
					return "cos";
				case Intrinsic.Tangent:
					return "tan";
				case Intrinsic.ArcSine:
					return "asin";
				case Intrinsic.ArcCosine:
					return "acos";
				case Intrinsic.ArcTangent:
					return "atan";
				case Intrinsic.Ceil:
					return "ceil";
				case Intrinsic.Floor:
					return "floor";
				case Intrinsic.Clamp:
					return "clamp";
				case Intrinsic.SquareRoot:
					return "sqrt";
				case Intrinsic.Exponential:
					return "exp";
				case Intrinsic.Power:
					return "pow";
				case Intrinsic.Absolute:
					return "abs";
				case Intrinsic.Round:
					return "round";
				case Intrinsic.Max:
					return "max";
				case Intrinsic.Min:
					return "min";
				case Intrinsic.Cross:
					return "cross";
				case Intrinsic.Dot:
					return "dot";
				case Intrinsic.Distance:
					return "distance";
				case Intrinsic.Normalize:
					return "normalize";
				case Intrinsic.Saturate:
					return "saturate";
				case Intrinsic.Lerp:
					return "lerp";
				default:
					throw new NotSupportedException("Unsupported intrinsic function.");
			}
		}

		/// <summary>
		///     Visits the given statement, treating it as a single-line block statement if it is not actually a block statement.
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
		///     Checks whether the terminating semicolon can be safely omitted from the statement.
		/// </summary>
		/// <param name="node">The node that should be checked.</param>
		private static bool OmitTerminatingSemicolon(AstNode node)
		{
			Assert.ArgumentNotNull(node);
			return node is BlockStatement || node is ForStatement || node is WhileStatement || node is IfElseStatement;
		}

		/// <summary>
		///     Returns an escaped version of the name that is guaranteed not to clash with any shader built-in variables, keywords
		///     or intrinsics.
		/// </summary>
		/// <param name="name">The name that should be escaped.</param>
		protected static string Escape(string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);
			return String.Format("{0}{1}", CompilationContext.ReservedIdentifierPrefix, name);
		}
	}
}