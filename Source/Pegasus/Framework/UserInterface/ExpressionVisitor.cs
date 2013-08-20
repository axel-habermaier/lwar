using System;

namespace Pegasus.Framework.UserInterface
{
	using System.Linq.Expressions;

	/// <summary>
	///   A base class for expression tree visitors that check member accesses. All non-supported expression types cause a
	///   run-time failure in debug builds.
	/// </summary>
	internal abstract class MemberAccessExpressionVisitor : ExpressionVisitor
	{
		protected override sealed Expression VisitLambda<T1>(Expression<T1> node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitLambda(node);
		}

		protected override sealed Expression VisitBinary(BinaryExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitBinary(node);
		}

		protected override sealed Expression VisitDebugInfo(DebugInfoExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitDebugInfo(node);
		}

		protected override sealed MemberListBinding VisitMemberListBinding(MemberListBinding node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitMemberListBinding(node);
		}

		protected override sealed MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitMemberMemberBinding(node);
		}

		protected override sealed MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitMemberAssignment(node);
		}

		protected override sealed MemberBinding VisitMemberBinding(MemberBinding node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitMemberBinding(node);
		}

		protected override sealed ElementInit VisitElementInit(ElementInit node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitElementInit(node);
		}

		protected override sealed Expression VisitListInit(ListInitExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitListInit(node);
		}

		protected override sealed Expression VisitMemberInit(MemberInitExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitMemberInit(node);
		}

		protected override sealed Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitTypeBinary(node);
		}

		protected override sealed Expression VisitTry(TryExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitTry(node);
		}

		protected override sealed CatchBlock VisitCatchBlock(CatchBlock node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitCatchBlock(node);
		}

		protected override sealed Expression VisitSwitch(SwitchExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitSwitch(node);
		}

		protected override sealed SwitchCase VisitSwitchCase(SwitchCase node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitSwitchCase(node);
		}

		protected override sealed Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitRuntimeVariables(node);
		}

		protected override sealed Expression VisitNew(NewExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitNew(node);
		}

		protected override sealed Expression VisitNewArray(NewArrayExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitNewArray(node);
		}

		protected override sealed Expression VisitMethodCall(MethodCallExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitMethodCall(node);
		}

		protected override sealed Expression VisitIndex(IndexExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitIndex(node);
		}

		protected override sealed Expression VisitLoop(LoopExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitLoop(node);
		}

		protected override sealed Expression VisitLabel(LabelExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitLabel(node);
		}

		protected override sealed LabelTarget VisitLabelTarget(LabelTarget node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitLabelTarget(node);
		}

		protected override sealed Expression VisitInvocation(InvocationExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitInvocation(node);
		}

		protected override sealed Expression VisitGoto(GotoExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitGoto(node);
		}

		protected override sealed Expression VisitExtension(Expression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitExtension(node);
		}

		protected override sealed Expression VisitDefault(DefaultExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitDefault(node);
		}

		protected override sealed Expression VisitDynamic(DynamicExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitDynamic(node);
		}

		protected override sealed Expression VisitConstant(ConstantExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitConstant(node);
		}

		protected override sealed Expression VisitConditional(ConditionalExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitConditional(node);
		}

		protected override sealed Expression VisitBlock(BlockExpression node)
		{
			Assert.That(false, "Unsupported expression element.");
			return base.VisitBlock(node);
		}
	}
}