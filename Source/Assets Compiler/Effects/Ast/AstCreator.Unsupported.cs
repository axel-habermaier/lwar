//using System;

//namespace Pegasus.AssetsCompiler.Effects.Ast
//{
//	using ICSharpCode.NRefactory.CSharp;

//	/// <summary>
//	///   Creates an abstract syntax tree for the shader code from the syntax tree of a C# method.
//	/// </summary>
//	internal partial class AstCreator
//	{
//		public IAstNode VisitPreProcessorDirective(PreProcessorDirective preProcessorDirective)
//		{
//			UnsupportedCSharpFeature(preProcessorDirective, "preprocessor directive");
//			return null;
//		}

//		public IAstNode VisitCheckedStatement(CheckedStatement checkedStatement)
//		{
//			UnsupportedCSharpFeature(checkedStatement, "checked");
//			return null;
//		}

//		public IAstNode VisitFixedStatement(FixedStatement fixedStatement)
//		{
//			UnsupportedCSharpFeature(fixedStatement, "fixed");
//			return null;
//		}

//		public IAstNode VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement)
//		{
//			UnsupportedCSharpFeature(gotoCaseStatement, "goto");
//			return null;
//		}

//		public IAstNode VisitForeachStatement(ForeachStatement foreachStatement)
//		{
//			UnsupportedCSharpFeature(foreachStatement, "foreach");
//			return null;
//		}

//		public IAstNode VisitGotoDefaultStatement(GotoDefaultStatement gotoDefaultStatement)
//		{
//			UnsupportedCSharpFeature(gotoDefaultStatement, "goto");
//			return null;
//		}

//		public IAstNode VisitGotoStatement(GotoStatement gotoStatement)
//		{
//			UnsupportedCSharpFeature(gotoStatement, "goto");
//			return null;
//		}

//		public IAstNode VisitLabelStatement(LabelStatement labelStatement)
//		{
//			UnsupportedCSharpFeature(labelStatement, "label");
//			return null;
//		}

//		public IAstNode VisitLockStatement(LockStatement lockStatement)
//		{
//			UnsupportedCSharpFeature(lockStatement, "lock");
//			return null;
//		}

//		public IAstNode VisitThrowStatement(ThrowStatement throwStatement)
//		{
//			UnsupportedCSharpFeature(throwStatement, "throw");
//			return null;
//		}

//		public IAstNode VisitTryCatchStatement(TryCatchStatement tryCatchStatement)
//		{
//			UnsupportedCSharpFeature(tryCatchStatement, "try-catch");
//			return null;
//		}

//		public IAstNode VisitCatchClause(CatchClause catchClause)
//		{
//			UnsupportedCSharpFeature(catchClause, "catch");
//			return null;
//		}

//		public IAstNode VisitUncheckedStatement(UncheckedStatement uncheckedStatement)
//		{
//			UnsupportedCSharpFeature(uncheckedStatement, "unchecked");
//			return null;
//		}

//		public IAstNode VisitUnsafeStatement(UnsafeStatement unsafeStatement)
//		{
//			UnsupportedCSharpFeature(unsafeStatement, "unsafe");
//			return null;
//		}

//		public IAstNode VisitUsingStatement(UsingStatement usingStatement)
//		{
//			UnsupportedCSharpFeature(usingStatement, "using");
//			return null;
//		}

//		public IAstNode VisitYieldBreakStatement(YieldBreakStatement yieldBreakStatement)
//		{
//			UnsupportedCSharpFeature(yieldBreakStatement, "yield break");
//			return null;
//		}

//		public IAstNode VisitYieldReturnStatement(YieldReturnStatement yieldReturnStatement)
//		{
//			UnsupportedCSharpFeature(yieldReturnStatement, "yield return");
//			return null;
//		}

//		public IAstNode VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
//		{
//			UnsupportedCSharpFeature(anonymousMethodExpression, "anonymous method");
//			return null;
//		}

//		public IAstNode VisitLambdaExpression(LambdaExpression lambdaExpression)
//		{
//			UnsupportedCSharpFeature(lambdaExpression, "lambda function");
//			return null;
//		}

//		public IAstNode VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression)
//		{
//			UnsupportedCSharpFeature(baseReferenceExpression, "base");
//			return null;
//		}

//		public IAstNode VisitCheckedExpression(CheckedExpression checkedExpression)
//		{
//			UnsupportedCSharpFeature(checkedExpression, "checked");
//			return null;
//		}

//		public IAstNode VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression)
//		{
//			UnsupportedCSharpFeature(nullReferenceExpression, "null");
//			return null;
//		}

//		public IAstNode VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression)
//		{
//			UnsupportedCSharpFeature(anonymousTypeCreateExpression, "anonymous type");
//			return null;
//		}

//		public IAstNode VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression)
//		{
//			UnsupportedCSharpFeature(arrayCreateExpression, "dynamic array initialization");
//			return null;
//		}

//		public IAstNode VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression)
//		{
//			UnsupportedCSharpFeature(pointerReferenceExpression, "pointer");
//			return null;
//		}

//		public IAstNode VisitSizeOfExpression(SizeOfExpression sizeOfExpression)
//		{
//			UnsupportedCSharpFeature(sizeOfExpression, "sizeof");
//			return null;
//		}

//		public IAstNode VisitStackAllocExpression(StackAllocExpression stackAllocExpression)
//		{
//			UnsupportedCSharpFeature(stackAllocExpression, "stackalloc");
//			return null;
//		}

//		public IAstNode VisitTypeOfExpression(TypeOfExpression typeOfExpression)
//		{
//			UnsupportedCSharpFeature(typeOfExpression, "typeof");
//			return null;
//		}

//		public IAstNode VisitUncheckedExpression(UncheckedExpression uncheckedExpression)
//		{
//			UnsupportedCSharpFeature(uncheckedExpression, "unchecked");
//			return null;
//		}

//		public IAstNode VisitQueryExpression(QueryExpression queryExpression)
//		{
//			UnsupportedCSharpFeature(queryExpression, "query");
//			return null;
//		}

//		public IAstNode VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause)
//		{
//			UnsupportedCSharpFeature(queryContinuationClause, "query");
//			return null;
//		}

//		public IAstNode VisitQueryFromClause(QueryFromClause queryFromClause)
//		{
//			UnsupportedCSharpFeature(queryFromClause, "from");
//			return null;
//		}

//		public IAstNode VisitQueryLetClause(QueryLetClause queryLetClause)
//		{
//			UnsupportedCSharpFeature(queryLetClause, "let");
//			return null;
//		}

//		public IAstNode VisitQueryWhereClause(QueryWhereClause queryWhereClause)
//		{
//			UnsupportedCSharpFeature(queryWhereClause, "where");
//			return null;
//		}

//		public IAstNode VisitQueryJoinClause(QueryJoinClause queryJoinClause)
//		{
//			UnsupportedCSharpFeature(queryJoinClause, "join");
//			return null;
//		}

//		public IAstNode VisitQueryOrderClause(QueryOrderClause queryOrderClause)
//		{
//			UnsupportedCSharpFeature(queryOrderClause, "orderby");
//			return null;
//		}

//		public IAstNode VisitQueryOrdering(QueryOrdering queryOrdering)
//		{
//			UnsupportedCSharpFeature(queryOrdering, "ordering");
//			return null;
//		}

//		public IAstNode VisitQuerySelectClause(QuerySelectClause querySelectClause)
//		{
//			UnsupportedCSharpFeature(querySelectClause, "select");
//			return null;
//		}

//		public IAstNode VisitQueryGroupClause(QueryGroupClause queryGroupClause)
//		{
//			UnsupportedCSharpFeature(queryGroupClause, "groupby");
//			return null;
//		}

//		public IAstNode VisitAsExpression(AsExpression asExpression)
//		{
//			UnsupportedCSharpFeature(asExpression, "as");
//			return null;
//		}

//		public IAstNode VisitIsExpression(IsExpression isExpression)
//		{
//			UnsupportedCSharpFeature(isExpression, "is");
//			return null;
//		}

//		public IAstNode VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression)
//		{
//			UnsupportedCSharpFeature(defaultValueExpression, "default");
//			return null;
//		}

//		public IAstNode VisitUndocumentedExpression(UndocumentedExpression undocumentedExpression)
//		{
//			UnsupportedCSharpFeature(undocumentedExpression, "undocumented expression");
//			return null;
//		}

//		public IAstNode VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression)
//		{
//			UnsupportedCSharpFeature(arrayInitializerExpression, "dynamic array initialization");
//			return null;
//		}

//		public IAstNode VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression)
//		{
//			UnsupportedCSharpFeature(namedArgumentExpression, "named arguments");
//			return null;
//		}

//		public IAstNode VisitSwitchStatement(SwitchStatement switchStatement)
//		{
//			UnsupportedCSharpFeature(switchStatement, "switch statement");
//			return null;
//		}

//		public IAstNode VisitSwitchSection(SwitchSection switchSection)
//		{
//			UnsupportedCSharpFeature(switchSection, "switch statement");
//			return null;
//		}

//		public IAstNode VisitCaseLabel(CaseLabel caseLabel)
//		{
//			UnsupportedCSharpFeature(caseLabel, "case label");
//			return null;
//		}


//	}
//}