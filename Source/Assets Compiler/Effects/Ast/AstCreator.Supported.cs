using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Creates an abstract syntax tree for the shader code from the syntax tree of a C# method.
	/// </summary>
	internal partial class AstCreator : IAstVisitor<ShaderAstNode>
	{
		public ShaderAstNode VisitBlockStatement(BlockStatement blockStatement)
		{
			return null;
		}

		public ShaderAstNode VisitEmptyStatement(EmptyStatement emptyStatement)
		{
			return null;
		}

		public ShaderAstNode VisitExpressionStatement(ExpressionStatement expressionStatement)
		{
			return null;
		}

		public ShaderAstNode VisitBreakStatement(BreakStatement breakStatement)
		{
			return null;
		}

		public ShaderAstNode VisitContinueStatement(ContinueStatement continueStatement)
		{
			return null;
		}

		public ShaderAstNode VisitDoWhileStatement(DoWhileStatement doWhileStatement)
		{
			return null;
		}

		public ShaderAstNode VisitForeachStatement(ForeachStatement foreachStatement)
		{
			return null;
		}

		public ShaderAstNode VisitForStatement(ForStatement forStatement)
		{
			return null;
		}

		public ShaderAstNode VisitIfElseStatement(IfElseStatement ifElseStatement)
		{
			return null;
		}

		public ShaderAstNode VisitReturnStatement(ReturnStatement returnStatement)
		{
			return null;
		}

		public ShaderAstNode VisitSwitchStatement(SwitchStatement switchStatement)
		{
			return null;
		}

		public ShaderAstNode VisitSwitchSection(SwitchSection switchSection)
		{
			return null;
		}

		public ShaderAstNode VisitCaseLabel(CaseLabel caseLabel)
		{
			return null;
		}

		public ShaderAstNode VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
		{
			return null;
		}

		public ShaderAstNode VisitWhileStatement(WhileStatement whileStatement)
		{
			return null;
		}

		public ShaderAstNode VisitAssignmentExpression(AssignmentExpression assignmentExpression)
		{
			return null;
		}

		public ShaderAstNode VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression)
		{
			return null;
		}

		public ShaderAstNode VisitCastExpression(CastExpression castExpression)
		{
			return null;
		}

		public ShaderAstNode VisitConditionalExpression(ConditionalExpression conditionalExpression)
		{
			return null;
		}

		public ShaderAstNode VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			return null;
		}

		public ShaderAstNode VisitIndexerExpression(IndexerExpression indexerExpression)
		{
			return null;
		}

		public ShaderAstNode VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			return null;
		}

		public ShaderAstNode VisitDirectionExpression(DirectionExpression directionExpression)
		{
			return null;
		}

		public ShaderAstNode VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			return null;
		}

		public ShaderAstNode VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
		{
			return null;
		}

		public ShaderAstNode VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression)
		{
			return null;
		}

		public ShaderAstNode VisitPrimitiveExpression(PrimitiveExpression primitiveExpression)
		{
			return null;
		}

		public ShaderAstNode VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression)
		{
			return null;
		}

		public ShaderAstNode VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression)
		{
			return null;
		}

		public ShaderAstNode VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression)
		{
			return null;
		}

		public ShaderAstNode VisitArraySpecifier(ArraySpecifier arraySpecifier)
		{
			return null;
		}

		public ShaderAstNode VisitNamedExpression(NamedExpression namedExpression)
		{
			return null;
		}

		public ShaderAstNode VisitEmptyExpression(EmptyExpression emptyExpression)
		{
			return null;
		}
	}
}