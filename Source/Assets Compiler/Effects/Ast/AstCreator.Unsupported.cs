using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Creates an abstract syntax tree for the shader code from the syntax tree of a C# method.
	/// </summary>
	internal partial class AstCreator : IAstVisitor<ShaderAstNode>
	{
		public ShaderAstNode VisitPreProcessorDirective(PreProcessorDirective preProcessorDirective)
		{
			UnsupportedCSharpFeature(preProcessorDirective, "preprocessor directive");
			return null;
		}

		public ShaderAstNode VisitCheckedStatement(CheckedStatement checkedStatement)
		{
			UnsupportedCSharpFeature(checkedStatement, "checked");
			return null;
		}

		public ShaderAstNode VisitFixedStatement(FixedStatement fixedStatement)
		{
			UnsupportedCSharpFeature(fixedStatement, "fixed");
			return null;
		}

		public ShaderAstNode VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement)
		{
			UnsupportedCSharpFeature(gotoCaseStatement, "goto");
			return null;
		}

		public ShaderAstNode VisitGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement)
		{
			UnsupportedCSharpFeature(gotoDefaultStatement, "goto");
			return null;
		}

		public ShaderAstNode VisitGotoStatement(GotoStatement gotoStatement)
		{
			UnsupportedCSharpFeature(gotoStatement, "goto");
			return null;
		}

		public ShaderAstNode VisitLabelStatement(LabelStatement labelStatement)
		{
			UnsupportedCSharpFeature(labelStatement, "label");
			return null;
		}

		public ShaderAstNode VisitLockStatement(LockStatement lockStatement)
		{
			UnsupportedCSharpFeature(lockStatement, "lock");
			return null;
		}

		public ShaderAstNode VisitThrowStatement(ThrowStatement throwStatement)
		{
			UnsupportedCSharpFeature(throwStatement, "throw");
			return null;
		}

		public ShaderAstNode VisitTryCatchStatement(TryCatchStatement tryCatchStatement)
		{
			UnsupportedCSharpFeature(tryCatchStatement, "try-catch");
			return null;
		}

		public ShaderAstNode VisitCatchClause(CatchClause catchClause)
		{
			UnsupportedCSharpFeature(catchClause, "catch");
			return null;
		}

		public ShaderAstNode VisitUncheckedStatement(UncheckedStatement uncheckedStatement)
		{
			UnsupportedCSharpFeature(uncheckedStatement, "unchecked");
			return null;
		}

		public ShaderAstNode VisitUnsafeStatement(UnsafeStatement unsafeStatement)
		{
			UnsupportedCSharpFeature(unsafeStatement, "unsafe");
			return null;
		}

		public ShaderAstNode VisitUsingStatement(UsingStatement usingStatement)
		{
			UnsupportedCSharpFeature(usingStatement, "using");
			return null;
		}

		public ShaderAstNode VisitYieldBreakStatement(YieldBreakStatement yieldBreakStatement)
		{
			UnsupportedCSharpFeature(yieldBreakStatement, "yield break");
			return null;
		}

		public ShaderAstNode VisitYieldReturnStatement(YieldReturnStatement yieldReturnStatement)
		{
			UnsupportedCSharpFeature(yieldReturnStatement, "yield return");
			return null;
		}

		public ShaderAstNode VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
		{
			UnsupportedCSharpFeature(anonymousMethodExpression, "anonymous method");
			return null;
		}

		public ShaderAstNode VisitLambdaExpression(LambdaExpression lambdaExpression)
		{
			UnsupportedCSharpFeature(lambdaExpression, "lambda function");
			return null;
		}

		public ShaderAstNode VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression)
		{
			UnsupportedCSharpFeature(baseReferenceExpression, "base");
			return null;
		}

		public ShaderAstNode VisitCheckedExpression(CheckedExpression checkedExpression)
		{
			UnsupportedCSharpFeature(checkedExpression, "checked");
			return null;
		}

		public ShaderAstNode VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression)
		{
			UnsupportedCSharpFeature(nullReferenceExpression, "null");
			return null;
		}

		public ShaderAstNode VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression)
		{
			UnsupportedCSharpFeature(anonymousTypeCreateExpression, "anonymous type");
			return null;
		}

		public ShaderAstNode VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression)
		{
			UnsupportedCSharpFeature(arrayCreateExpression, "dynamic array initialization");
			return null;
		}

		public ShaderAstNode VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression)
		{
			UnsupportedCSharpFeature(pointerReferenceExpression, "pointer");
			return null;
		}

		public ShaderAstNode VisitSizeOfExpression(SizeOfExpression sizeOfExpression)
		{
			UnsupportedCSharpFeature(sizeOfExpression, "sizeof");
			return null;
		}

		public ShaderAstNode VisitStackAllocExpression(StackAllocExpression stackAllocExpression)
		{
			UnsupportedCSharpFeature(stackAllocExpression, "stackalloc");
			return null;
		}

		public ShaderAstNode VisitTypeOfExpression(TypeOfExpression typeOfExpression)
		{
			UnsupportedCSharpFeature(typeOfExpression, "typeof");
			return null;
		}

		public ShaderAstNode VisitUncheckedExpression(UncheckedExpression uncheckedExpression)
		{
			UnsupportedCSharpFeature(uncheckedExpression, "unchecked");
			return null;
		}

		public ShaderAstNode VisitQueryExpression(QueryExpression queryExpression)
		{
			UnsupportedCSharpFeature(queryExpression, "query");
			return null;
		}

		public ShaderAstNode VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause)
		{
			UnsupportedCSharpFeature(queryContinuationClause, "query");
			return null;
		}

		public ShaderAstNode VisitQueryFromClause(QueryFromClause queryFromClause)
		{
			UnsupportedCSharpFeature(queryFromClause, "from");
			return null;
		}

		public ShaderAstNode VisitQueryLetClause(QueryLetClause queryLetClause)
		{
			UnsupportedCSharpFeature(queryLetClause, "let");
			return null;
		}

		public ShaderAstNode VisitQueryWhereClause(QueryWhereClause queryWhereClause)
		{
			UnsupportedCSharpFeature(queryWhereClause, "where");
			return null;
		}

		public ShaderAstNode VisitQueryJoinClause(QueryJoinClause queryJoinClause)
		{
			UnsupportedCSharpFeature(queryJoinClause, "join");
			return null;
		}

		public ShaderAstNode VisitQueryOrderClause(QueryOrderClause queryOrderClause)
		{
			UnsupportedCSharpFeature(queryOrderClause, "orderby");
			return null;
		}

		public ShaderAstNode VisitQueryOrdering(QueryOrdering queryOrdering)
		{
			UnsupportedCSharpFeature(queryOrdering, "ordering");
			return null;
		}

		public ShaderAstNode VisitQuerySelectClause(QuerySelectClause querySelectClause)
		{
			UnsupportedCSharpFeature(querySelectClause, "select");
			return null;
		}

		public ShaderAstNode VisitQueryGroupClause(QueryGroupClause queryGroupClause)
		{
			UnsupportedCSharpFeature(queryGroupClause, "groupby");
			return null;
		}

		public ShaderAstNode VisitAsExpression(AsExpression asExpression)
		{
			UnsupportedCSharpFeature(asExpression, "as");
			return null;
		}

		public ShaderAstNode VisitIsExpression(IsExpression isExpression)
		{
			UnsupportedCSharpFeature(isExpression, "is");
			return null;
		}

		public ShaderAstNode VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression)
		{
			UnsupportedCSharpFeature(defaultValueExpression, "default");
			return null;
		}

		public ShaderAstNode VisitUndocumentedExpression(UndocumentedExpression undocumentedExpression)
		{
			UnsupportedCSharpFeature(undocumentedExpression, "undocumented expression");
			return null;
		}

		public ShaderAstNode VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression)
		{
			UnsupportedCSharpFeature(arrayInitializerExpression, "dynamic array initialization");
			return null;
		}

		public ShaderAstNode VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression)
		{
			UnsupportedCSharpFeature(namedArgumentExpression, "named arguments");
			return null;
		}

		/// <summary>
		///   Reports the node as an unsupported C# feature.
		/// </summary>
		/// <param name="node">The node that should be reported as unsupported.</param>
		/// <param name="description">The description of the unsupported C# feature.</param>
		private void UnsupportedCSharpFeature(AstNode node, string description)
		{
			_context.Error(node, "Unsupported C# feature used: {0}.", description);
		}
	}
}