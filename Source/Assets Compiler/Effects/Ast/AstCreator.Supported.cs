using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using System.Linq;
	using Compilation;
	using Framework;
	using ICSharpCode.NRefactory.CSharp;
	using ICSharpCode.NRefactory.Semantics;

	/// <summary>
	///   Creates an abstract syntax tree for the shader code from the syntax tree of a C# method.
	/// </summary>
	internal partial class AstCreator
	{
		public IAstNode VisitBlockStatement(ICSharpCode.NRefactory.CSharp.BlockStatement blockStatement)
		{
			var statements = blockStatement.Statements.Visit<Statement>(this);
			return new BlockStatement(statements);
		}

		public IAstNode VisitEmptyStatement(EmptyStatement emptyStatement)
		{
			return null;
		}

		public IAstNode VisitExpressionStatement(ICSharpCode.NRefactory.CSharp.ExpressionStatement expressionStatement)
		{
			return new ExpressionStatement((Expression)expressionStatement.Expression.AcceptVisitor(this));
		}

		public IAstNode VisitForStatement(ICSharpCode.NRefactory.CSharp.ForStatement forStatement)
		{
			var initializers = forStatement.Initializers.Visit<Statement>(this);
			var actions = forStatement.Iterators.Visit<Statement>(this);

			var condition = forStatement.Condition.Visit<Expression>(this);
			var body = forStatement.EmbeddedStatement.Visit<Statement>(this);

			return new ForStatement(initializers, condition, actions, body);
		}

		public IAstNode VisitVariableDeclarationStatement(
			ICSharpCode.NRefactory.CSharp.VariableDeclarationStatement variableDeclarationStatement)
		{
			var initializers = variableDeclarationStatement.Variables.Visit<VariableInitializer>(this);
			return new VariableDeclarationStatement(initializers);
		}

		public IAstNode VisitWhileStatement(ICSharpCode.NRefactory.CSharp.WhileStatement whileStatement)
		{
			return null;
		}

		public IAstNode VisitAssignmentExpression(ICSharpCode.NRefactory.CSharp.AssignmentExpression assignmentExpression)
		{
			var left = assignmentExpression.Left.Visit<Expression>(this);
			var right = assignmentExpression.Right.Visit<Expression>(this);

			return new AssignmentExpression(left, assignmentExpression.Operator, right);
		}

		public IAstNode VisitBinaryOperatorExpression(
			ICSharpCode.NRefactory.CSharp.BinaryOperatorExpression binaryOperatorExpression)
		{
			var left = binaryOperatorExpression.Left.Visit<Expression>(this);
			var right = binaryOperatorExpression.Right.Visit<Expression>(this);

			var leftType = _context.Resolve(binaryOperatorExpression.Left).Type.ToDataType();
			var rightType = _context.Resolve(binaryOperatorExpression.Right).Type.ToDataType();

			return new BinaryOperatorExpression(left, leftType, binaryOperatorExpression.Operator, right, rightType);
		}

		public IAstNode VisitVariableInitializer(ICSharpCode.NRefactory.CSharp.VariableInitializer variableInitializer)
		{
			var resolved = _context.Resolve<LocalResolveResult>(variableInitializer);
			var expression = variableInitializer.Initializer.Visit<Expression>(this);
			var variable = _shader.Variables.Single(v => v.IsSame(resolved.Variable));

			return new VariableInitializer(variable, expression);
		}

		public IAstNode VisitCastExpression(CastExpression castExpression)
		{
			return null;
		}

		public IAstNode VisitConditionalExpression(ConditionalExpression conditionalExpression)
		{
			return null;
		}

		public IAstNode VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			var local = _context.Resolve<LocalResolveResult>(identifierExpression);
			if (local != null)
			{
				if (local.IsParameter)
				{
					var parameter = _shader.Parameters.Single(p => p.Name == local.Variable.Name);
					return new VariableReference<ShaderParameter>(parameter);
				}

				var variable = _shader.Variables.Single(v => v.IsSame(local.Variable));
				return new VariableReference<ShaderVariable>(variable);
			}

			var member = _context.Resolve<MemberResolveResult>(identifierExpression);
			if (member != null)
			{
				var constant = _effect.Constants.SingleOrDefault(v => v.Name == member.Member.Name);
				if (constant != null)
					return new VariableReference<ShaderConstant>(constant);

				var literal = _effect.Literals.SingleOrDefault(v => v.Name == member.Member.Name);
				if (literal != null)
					return new VariableReference<ShaderLiteral>(literal);

				var texture = _effect.Textures.SingleOrDefault(v => v.Name == member.Member.Name);
				if (texture != null)
					return new VariableReference<ShaderTexture>(texture);
			}

			return null;
		}

		public IAstNode VisitIndexerExpression(ICSharpCode.NRefactory.CSharp.IndexerExpression indexerExpression)
		{
			var target = indexerExpression.Target.Visit<Expression>(this);
			var arguments = indexerExpression.Arguments.Visit<Expression>(this);

			return new IndexerExpression(target, arguments);
		}

		public IAstNode VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			var arguments = invocationExpression.Arguments.Visit<Expression>(this);

			var method = _context.Resolve<MemberResolveResult>(invocationExpression).Member;
			var dataType = method.DeclaringType.ToDataType();
			if (dataType == DataType.Texture2D || dataType == DataType.CubeMap)
			{
				if (method.Name == "Sample" && arguments.Length == 1)
					return new IntrinsicFunctionInvocation(IntrinsicFunction.Sample, arguments);

				if (method.Name == "Sample" && arguments.Length == 2)
					return new IntrinsicFunctionInvocation(IntrinsicFunction.SampleLevel, arguments);

				Assert.That(false, "Unsupported texture function.");
			}

			throw new InvalidOperationException("Unsupported function invocation.");
		}

		public IAstNode VisitDirectionExpression(DirectionExpression directionExpression)
		{
			return null;
		}

		public IAstNode VisitMemberReferenceExpression(
			ICSharpCode.NRefactory.CSharp.MemberReferenceExpression memberReferenceExpression)
		{
			var target = memberReferenceExpression.Target.Visit<Expression>(this);
			var resolved = _context.Resolve<MemberResolveResult>(memberReferenceExpression);

			return new MemberReferenceExpression(target, resolved.TargetResult.Type.ToDataType(), memberReferenceExpression.MemberName);
		}

		public IAstNode VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
		{
			var resolved = _context.Resolve(objectCreateExpression);
			var arguments = objectCreateExpression.Arguments.Visit<Expression>(this);

			return new ObjectCreationExpression(resolved.Type.ToDataType(), arguments);
		}

		public IAstNode VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression)
		{
			return null;
		}

		public IAstNode VisitPrimitiveExpression(PrimitiveExpression primitiveExpression)
		{
			return new LiteralValue(primitiveExpression.Value);
		}

		public IAstNode VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression)
		{
			return null;
		}

		public IAstNode VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression)
		{
			return null;
		}

		public IAstNode VisitUnaryOperatorExpression(ICSharpCode.NRefactory.CSharp.UnaryOperatorExpression unaryOperatorExpression)
		{
			var expression = unaryOperatorExpression.Expression.Visit<Expression>(this);
			var type = _context.Resolve(unaryOperatorExpression.Expression).Type.ToDataType();

			return new UnaryOperatorExpression(expression, type, unaryOperatorExpression.Operator);
		}

		public IAstNode VisitArraySpecifier(ArraySpecifier arraySpecifier)
		{
			return null;
		}

		public IAstNode VisitNamedExpression(NamedExpression namedExpression)
		{
			return null;
		}

		public IAstNode VisitEmptyExpression(EmptyExpression emptyExpression)
		{
			return null;
		}

		public IAstNode VisitBreakStatement(ICSharpCode.NRefactory.CSharp.BreakStatement breakStatement)
		{
			return null;
		}

		public IAstNode VisitContinueStatement(ICSharpCode.NRefactory.CSharp.ContinueStatement continueStatement)
		{
			return null;
		}

		public IAstNode VisitDoWhileStatement(ICSharpCode.NRefactory.CSharp.DoWhileStatement doWhileStatement)
		{
			return null;
		}

		public IAstNode VisitIfElseStatement(ICSharpCode.NRefactory.CSharp.IfElseStatement ifElseStatement)
		{
			return null;
		}

		public IAstNode VisitReturnStatement(ICSharpCode.NRefactory.CSharp.ReturnStatement returnStatement)
		{
			return null;
		}
	}
}