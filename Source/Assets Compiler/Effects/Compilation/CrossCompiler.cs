using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using Ast;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using AssignmentExpression = Ast.AssignmentExpression;
	using BinaryOperatorExpression = Ast.BinaryOperatorExpression;
	using BlockStatement = Ast.BlockStatement;
	using BreakStatement = Ast.BreakStatement;
	using ContinueStatement = Ast.ContinueStatement;
	using DoWhileStatement = Ast.DoWhileStatement;
	using ExpressionStatement = Ast.ExpressionStatement;
	using ForStatement = Ast.ForStatement;
	using IAstVisitor = Ast.IAstVisitor;
	using IfElseStatement = Ast.IfElseStatement;
	using IndexerExpression = Ast.IndexerExpression;
	using MemberReferenceExpression = Ast.MemberReferenceExpression;
	using ParenthesizedExpression = Ast.ParenthesizedExpression;
	using ReturnStatement = Ast.ReturnStatement;
	using UnaryOperatorExpression = Ast.UnaryOperatorExpression;
	using VariableDeclarationStatement = Ast.VariableDeclarationStatement;
	using VariableInitializer = Ast.VariableInitializer;
	using WhileStatement = Ast.WhileStatement;

	/// <summary>
	///   Cross-compiles a C# shader method.
	/// </summary>
	internal abstract class CrossCompiler : IAstVisitor
	{
		/// <summary>
		///   The C# shader method that is cross-compiled.
		/// </summary>
		protected ShaderMethod Shader { get; private set; }

		/// <summary>
		///   The context of the compilation.
		/// </summary>
		//protected CompilationContext Context { get; private set; }

		/// <summary>
		///   The effect class the shader method belongs to.
		/// </summary>
		protected EffectClass Effect { get; private set; }

		/// <summary>
		///   The code writer the generated code should be written to.
		/// </summary>
		protected CodeWriter Writer { get; private set; }

		public virtual void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
		{
			assignmentExpression.Left.AcceptVisitor(this);
			Writer.Append(" {0} ", GetToken(assignmentExpression.Operator));
			assignmentExpression.Right.AcceptVisitor(this);
		}

		public virtual void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
		{
			binaryOperatorExpression.Left.AcceptVisitor(this);
			Writer.Append(" {0} ", GetToken(binaryOperatorExpression.Operator));
			binaryOperatorExpression.Right.AcceptVisitor(this);
		}

		public virtual void VisitBlockStatement(BlockStatement blockStatement)
		{
			Writer.AppendBlockStatement(() => blockStatement.Statements.AcceptVisitor(this));
		}

		public virtual void VisitBreakStatement(BreakStatement breakStatement)
		{
			Writer.AppendLine("break;");
		}

		public virtual void VisitContinueStatement(ContinueStatement continueStatement)
		{
			Writer.AppendLine("continue;");
		}

		public virtual void VisitDoWhileStatement(DoWhileStatement doWhileStatement)
		{
			throw new NotImplementedException();
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
			Writer.AppendLine(";");
		}

		public virtual void VisitForStatement(ForStatement forStatement)
		{
			Writer.Append("for (");
			Writer.Append(")");
			forStatement.Body.AcceptVisitor(this);
		}

		public virtual void VisitIfElseStatement(IfElseStatement ifElseStatement)
		{
			throw new NotImplementedException();
		}

		public virtual void VisitIndexerExpression(IndexerExpression indexerExpression)
		{
			indexerExpression.Target.AcceptVisitor(this);
			Writer.Append("[");
			indexerExpression.Arguments.AcceptVisitor(this);
			Writer.Append("]");
		}

		public virtual void VisitIntrinsicFunctionInvocation(IntrinsicFunctionInvocation intrinsicFunctionInvocation)
		{
			switch (intrinsicFunctionInvocation.Function)
			{
				case IntrinsicFunction.Sample:
					intrinsicFunctionInvocation.Target.AcceptVisitor(this);
					break;
				case IntrinsicFunction.SampleLevel:
					intrinsicFunctionInvocation.Target.AcceptVisitor(this);
					break;
				default:
					throw new InvalidOperationException("Unsupported intrinsic function.");
			}

			Writer.Append("(");
			intrinsicFunctionInvocation.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append(")");
		}

		public virtual void VisitLiteralValue(LiteralValue literalValue)
		{
			Writer.Append(literalValue.Value.ToString());
		}

		public virtual void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			memberReferenceExpression.Target.AcceptVisitor(this);
			var member = memberReferenceExpression.Member;
			var type = memberReferenceExpression.Type;

			if (type == DataType.Vector2 || type == DataType.Vector3 || type == DataType.Vector4 || type == DataType.Matrix)
				member = member.ToLower();

			Writer.Append(".{0}", member);
		}

		public virtual void VisitObjectCreationExpression(ObjectCreationExpression objectCreationExpression)
		{
			Writer.Append("{0}(", ToShaderType(objectCreationExpression.Type));
			objectCreationExpression.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append(")");
		}

		public virtual void VisitReturnStatement(ReturnStatement returnStatement)
		{
			Writer.AppendLine("return;");
		}

		public virtual void VisitUnaryOperatorStatement(UnaryOperatorExpression unaryOperatorExpression)
		{
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
		}

		public virtual void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
		{
			variableDeclarationStatement.Variables.AcceptVisitor(this, () => Writer.AppendLine(";"));
			Writer.AppendLine(";");
		}

		public virtual void VisitVariableInitializer(VariableInitializer variableInitializer)
		{
			Writer.Append("{0} {1}", ToShaderType(variableInitializer.Variable.Type), variableInitializer.Variable.Name);
			if (variableInitializer.Expression != null)
			{
				Writer.Append(" = ");
				variableInitializer.Expression.AcceptVisitor(this);
			}
		}

		public virtual void VisitVariableReference<T>(VariableReference<T> variableReference)
			where T : IShaderDataObject
		{
			Writer.Append(variableReference.Variable.Name);
		}

		public virtual void VisitWhileStatement(WhileStatement whileStatement)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Generates the shader code.
		/// </summary>
		protected void GenerateShaderCode()
		{
			var blockStatement = (BlockStatement)Shader.SyntaxTree;
			blockStatement.Statements.AcceptVisitor(this);
		}

		/// <summary>
		///   Cross-compiles the C# shader method.
		/// </summary>
		/// <param name="context">The context of the compilation.</param>
		/// <param name="effect">The effect class the shader method belongs to.</param>
		/// <param name="shader">The C# shader method that should be cross-compiled.</param>
		/// <param name="writer">The code writer the generated code should be written to.</param>
		//public void GenerateCode(CompilationContext context, EffectClass effect, ShaderMethod shader, CodeWriter writer)
		//{
		//	Assert.ArgumentNotNull(context, () => context);
		//	Assert.ArgumentNotNull(effect, () => effect);
		//	Assert.ArgumentNotNull(shader, () => shader);
		//	Assert.ArgumentNotNull(writer, () => writer);

		//	Context = context;
		//	Effect = effect;
		//	Shader = shader;
		//	Writer = writer;

		//	foreach (var literal in effect.Literals)
		//		GenerateLiteral(literal);

		//	Writer.Newline();
		//	foreach (var constantBuffer in effect.ConstantBuffers)
		//		GenerateConstantBuffer(constantBuffer);

		//	foreach (var texture in effect.Textures)
		//		GenerateTextureObject(texture);

		//	GenerateInputs();
		//	Writer.Newline();

		//	GenerateOutputs();
		//	Writer.Newline();

		//	GenerateMainMethod();
		//}

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
		///   Generates the shader inputs.
		/// </summary>
		protected abstract void GenerateInputs();

		/// <summary>
		///   Generates the shader outputs.
		/// </summary>
		protected abstract void GenerateOutputs();

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
					throw new InvalidOperationException("Unsupported assignment operator type.");
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
					return "";
				case UnaryOperatorType.BitNot:
					return "";
				case UnaryOperatorType.Minus:
					return "";
				case UnaryOperatorType.Plus:
					return "";
				case UnaryOperatorType.Increment:
					return "";
				case UnaryOperatorType.Decrement:
					return "";
				case UnaryOperatorType.PostIncrement:
					return "";
				case UnaryOperatorType.PostDecrement:
					return "";
				default:
					throw new InvalidOperationException("Unsupported unary operator type.");
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
					throw new InvalidOperationException("Unsupported binary operator type.");
			}
		}
	}
}