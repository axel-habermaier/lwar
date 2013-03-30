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

		public virtual void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
		{
			assignmentExpression.Left.AcceptVisitor(this);
			Writer.Append(" {0} ", GetToken(assignmentExpression.Operator));
			assignmentExpression.Right.AcceptVisitor(this);
		}

		public virtual void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
		{
			Writer.Append("(");
			binaryOperatorExpression.Left.AcceptVisitor(this);
			Writer.Append(" {0} ", GetToken(binaryOperatorExpression.Operator));
			binaryOperatorExpression.Right.AcceptVisitor(this);
			Writer.Append(")");
		}

		public virtual void VisitBlockStatement(BlockStatement blockStatement)
		{
			Writer.AppendBlockStatement(() => blockStatement.Statements.AcceptVisitor(this, node =>
				{
					if (!OmitTerminatingSemicolon(node))
						Writer.AppendLine(";");
				}, true));
		}

		public virtual void VisitBreakStatement(BreakStatement breakStatement)
		{
			Writer.Append("break");
		}

		public virtual void VisitContinueStatement(ContinueStatement continueStatement)
		{
			Writer.Append("continue");
		}

		public virtual void VisitDoWhileStatement(DoWhileStatement doWhileStatement)
		{
			Writer.AppendLine("do");
			VisitStatementBlock(doWhileStatement.EmbeddedStatement);
			Writer.Append("while (");
			doWhileStatement.Condition.AcceptVisitor(this);
			Writer.Append(")");
		}

		public void VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression)
		{
			Writer.Append("(");
			parenthesizedExpression.Expression.AcceptVisitor(this);
			Writer.Append(")");
		}

		public virtual void VisitExpressionStatement(ExpressionStatement expressionStatement)
		{
			expressionStatement.Expression.AcceptVisitor(this);
		}

		public virtual void VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			Writer.Append(identifierExpression.Identifier);
		}

		public virtual void VisitForStatement(ForStatement forStatement)
		{
			Writer.Append("for (");

			forStatement.Initializers.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append("; ");

			forStatement.Condition.AcceptVisitor(this);
			Writer.Append("; ");

			forStatement.Iterators.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append(")");

			VisitStatementBlock(forStatement.EmbeddedStatement);
		}

		public virtual void VisitIfElseStatement(IfElseStatement ifElseStatement)
		{
			Writer.Append("if (");
			ifElseStatement.Condition.AcceptVisitor(this);
			Writer.AppendLine(")");
			VisitStatementBlock(ifElseStatement.TrueStatement);

			if (ifElseStatement.FalseStatement.IsNull)
				return;

			Writer.Append("else ");
			VisitStatementBlock(ifElseStatement.FalseStatement);
		}

		public virtual void VisitConditionalExpression(ConditionalExpression conditionalExpression)
		{
			conditionalExpression.Condition.AcceptVisitor(this);
			Writer.Append(" ? ");
			conditionalExpression.TrueExpression.AcceptVisitor(this);
			Writer.Append(" : ");
			conditionalExpression.FalseExpression.AcceptVisitor(this);
		}

		public virtual void VisitIndexerExpression(IndexerExpression indexerExpression)
		{
		}

		public void VisitEmptyStatement(EmptyStatement emptyStatement)
		{
		}

		public void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
		}

		public virtual void VisitPrimitiveExpression(PrimitiveExpression primitiveExpression)
		{
			Writer.Append(primitiveExpression.Value.ToString().ToLower());
		}

		public virtual void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
		}

		public virtual void VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
		{
			var resolved = Resolver.Resolve(objectCreateExpression);

			Writer.Append("{0}(", ToShaderType(resolved.Type.ToDataType()));
			objectCreateExpression.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append(")");
		}

		public virtual void VisitReturnStatement(ReturnStatement returnStatement)
		{
			Writer.Append("return");
		}

		public virtual void VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression)
		{
			Writer.Append("(");
			if (unaryOperatorExpression.Operator == UnaryOperatorType.PostDecrement ||
				unaryOperatorExpression.Operator == UnaryOperatorType.PostIncrement)
			{
				unaryOperatorExpression.Expression.AcceptVisitor(this);
				Writer.Append(GetToken(unaryOperatorExpression.Operator));
			}
			else
			{
				Writer.Append(GetToken(unaryOperatorExpression.Operator));
				unaryOperatorExpression.Expression.AcceptVisitor(this);
			}
			Writer.Append(")");
		}

		public virtual void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
		{
			var resolved = Resolver.Resolve(variableDeclarationStatement.Variables.First());
			Writer.Append("{0} ", ToShaderType(resolved.Type.ToDataType()));

			variableDeclarationStatement.Variables.AcceptVisitor(this, () => Writer.Append(", "));
		}

		public virtual void VisitVariableInitializer(VariableInitializer variableInitializer)
		{
			Writer.Append(variableInitializer.Name);

			if (variableInitializer.Initializer.IsNull)
				return;

			Writer.Append(" = ");
			variableInitializer.Initializer.AcceptVisitor(this);
		}

		public virtual void VisitWhileStatement(WhileStatement whileStatement)
		{
			Writer.Append("while (");
			whileStatement.Condition.AcceptVisitor(this);
			Writer.AppendLine(")");
			VisitStatementBlock(whileStatement.EmbeddedStatement);
		}

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