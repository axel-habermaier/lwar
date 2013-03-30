using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.Collections.Generic;
	using Ast;
	using Framework;
	using Framework.Platform.Graphics;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.CSharp.Resolver;
	using AssignmentExpression = ICSharpCode.NRefactory.CSharp.AssignmentExpression;
	using ExpressionStatement = ICSharpCode.NRefactory.CSharp.ExpressionStatement;
	using IAstVisitor = ICSharpCode.NRefactory.CSharp.IAstVisitor;

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
		/// Gets the shader that is compiled.
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

		//public virtual void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
		//{
		//	binaryOperatorExpression.Left.AcceptVisitor(this);
		//	Writer.Append(" {0} ", GetToken(binaryOperatorExpression.Operator));
		//	binaryOperatorExpression.Right.AcceptVisitor(this);
		//}

		//public virtual void VisitBlockStatement(BlockStatement blockStatement)
		//{
		//	Writer.AppendBlockStatement(() => blockStatement.Statements.AcceptVisitor(this));
		//}

		//public virtual void VisitBreakStatement(BreakStatement breakStatement)
		//{
		//	Writer.AppendLine("break;");
		//}

		//public virtual void VisitContinueStatement(ContinueStatement continueStatement)
		//{
		//	Writer.AppendLine("continue;");
		//}

		//public virtual void VisitDoWhileStatement(DoWhileStatement doWhileStatement)
		//{
		//	throw new NotImplementedException();
		//}

		//public void VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression)
		//{
		//	Writer.Append("(");
		//	parenthesizedExpression.Expression.AcceptVisitor(this);
		//	Writer.Append(")");
		//}

		public virtual void VisitExpressionStatement(ExpressionStatement expressionStatement)
		{
			expressionStatement.Expression.AcceptVisitor(this);
			Writer.AppendLine(";");
		}

		public virtual void VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			Writer.Append(identifierExpression.Identifier);
		}

		//public virtual void VisitForStatement(ForStatement forStatement)
		//{
		//	Writer.Append("for (");
		//	Writer.Append(")");
		//	forStatement.Body.AcceptVisitor(this);
		//}

		//public virtual void VisitIfElseStatement(IfElseStatement ifElseStatement)
		//{
		//	throw new NotImplementedException();
		//}

		//public virtual void VisitIndexerExpression(IndexerExpression indexerExpression)
		//{
		//	indexerExpression.Target.AcceptVisitor(this);
		//	Writer.Append("[");
		//	indexerExpression.Arguments.AcceptVisitor(this);
		//	Writer.Append("]");
		//}

		//public virtual void VisitIntrinsicFunctionInvocation(IntrinsicFunctionInvocation intrinsicFunctionInvocation)
		//{
		//	switch (intrinsicFunctionInvocation.Function)
		//	{
		//		case IntrinsicFunction.Sample:
		//			intrinsicFunctionInvocation.Target.AcceptVisitor(this);
		//			break;
		//		case IntrinsicFunction.SampleLevel:
		//			intrinsicFunctionInvocation.Target.AcceptVisitor(this);
		//			break;
		//		default:
		//			throw new InvalidOperationException("Unsupported intrinsic function.");
		//	}

		//	Writer.Append("(");
		//	intrinsicFunctionInvocation.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
		//	Writer.Append(")");
		//}

		public virtual void VisitPrimitiveExpression(PrimitiveExpression primitiveExpression)
		{
			Writer.Append(primitiveExpression.Value.ToString());
		}

		//public virtual void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		//{
		//	memberReferenceExpression.Target.AcceptVisitor(this);
		//	var member = memberReferenceExpression.Member;
		//	var type = memberReferenceExpression.Type;

		//	if (type == DataType.Vector2 || type == DataType.Vector3 || type == DataType.Vector4 || type == DataType.Matrix)
		//		member = member.ToLower();

		//	Writer.Append(".{0}", member);
		//}

		public virtual void VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
		{
			var resolved = Resolver.Resolve(objectCreateExpression);

			Writer.Append("{0}(", ToShaderType(resolved.Type.ToDataType()));
			objectCreateExpression.Arguments.AcceptVisitor(this, () => Writer.Append(", "));
			Writer.Append(")");
		}

		//public virtual void VisitReturnStatement(ReturnStatement returnStatement)
		//{
		//	Writer.AppendLine("return;");
		//}

		//public virtual void VisitUnaryOperatorStatement(UnaryOperatorExpression unaryOperatorExpression)
		//{
		//	if (unaryOperatorExpression.Operator == UnaryOperatorType.PostDecrement ||
		//		unaryOperatorExpression.Operator == UnaryOperatorType.PostIncrement)
		//	{
		//		unaryOperatorExpression.Expression.AcceptVisitor(this);
		//		Writer.Append(GetToken(unaryOperatorExpression.Operator));
		//	}
		//	else
		//	{
		//		Writer.Append(GetToken(unaryOperatorExpression.Operator));
		//		unaryOperatorExpression.Expression.AcceptVisitor(this);
		//	}
		//}

		//public virtual void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
		//{
		//	variableDeclarationStatement.Variables.AcceptVisitor(this, () => Writer.AppendLine(";"));
		//	Writer.AppendLine(";");
		//}

		//public virtual void VisitVariableInitializer(VariableInitializer variableInitializer)
		//{
		//	Writer.Append("{0} {1}", ToShaderType(variableInitializer.Variable.Type), variableInitializer.Variable.Name);
		//	if (variableInitializer.Expression != null)
		//	{
		//		Writer.Append(" = ");
		//		variableInitializer.Expression.AcceptVisitor(this);
		//	}
		//}

		//public virtual void VisitWhileStatement(WhileStatement whileStatement)
		//{
		//	throw new NotImplementedException();
		//}

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

			foreach (var literal in effect.Literals)
				GenerateLiteral(literal);

			Writer.Newline();
			foreach (var constantBuffer in effect.ConstantBuffers)
				GenerateConstantBuffer(constantBuffer);

			foreach (var texture in effect.Textures)
				GenerateTextureObject(texture);

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
	}
}