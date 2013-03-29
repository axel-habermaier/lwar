using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Compilation;

	/// <summary>
	///   Implementation of the visitor pattern in order to visit all syntax nodes within a shader syntax tree.
	/// </summary>
	internal interface IAstVisitor
	{
		void VisitAssignmentExpression(AssignmentExpression assignmentExpression);
		void VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression);
		void VisitBlockStatement(BlockStatement blockStatement);
		void VisitBreakStatement(BreakStatement breakStatement);
		void VisitContinueStatement(ContinueStatement continueStatement);
		void VisitDoWhileStatement(DoWhileStatement doWhileStatement);
		void VisitExpressionStatement(ExpressionStatement expressionStatement);
		void VisitForStatement(ForStatement forStatement);
		void VisitIfElseStatement(IfElseStatement ifElseStatement);
		void VisitIndexerExpression(IndexerExpression indexerExpression);
		void VisitIntrinsicFunctionInvocation(IntrinsicFunctionInvocation intrinsicFunctionInvocation);
		void VisitLiteralValue(LiteralValue literalValue);
		void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression);
		void VisitObjectCreationExpression(ObjectCreationExpression objectCreationExpression);
		void VisitReturnStatement(ReturnStatement returnStatement);
		void VisitUnaryOperatorStatement(UnaryOperatorExpression unaryOperatorExpression);
		void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement);
		void VisitVariableInitializer(VariableInitializer variableInitializer);

		void VisitVariableReference<T>(VariableReference<T> variableReference)
			where T : IShaderDataObject;

		void VisitWhileStatement(WhileStatement whileStatement);
	}
}