using System;

namespace Pegasus.Framework.Rendering.UserInterface
{
	using System.Linq.Expressions;

	/// <summary>
	///   Binds a target dependency object/dependency property pair to a source object and path selector.
	/// </summary>
	/// <typeparam name="T">The type of the value that is bound.</typeparam>
	public class Binding<T>
	{
		/// <summary>
		///   The compiled expression that is used to get the value from the source object.
		/// </summary>
		private Func<object, T> _sourceFunc;

		/// <summary>
		///   The target dependency object that defines the target dependency property.
		/// </summary>
		private DependencyObject _targetObject;

		/// <summary>
		///   The target dependency property whose value is bound.
		/// </summary>
		private DependencyProperty<T> _targetProperty;

		/// <summary>
		///   Gets or sets the expression that is used to get the value from the source object.
		/// </summary>
		public Expression<Func<object, T>> SourceExpression { get; set; }

		/// <summary>
		///   Gets or sets the source object that provides the value to be copied. If the source object is null, the getter is
		///   applied to the target UI element's view model.
		/// </summary>
		public object SourceObject { get; set; }

		/// <summary>
		///   Initializes the binding.
		/// </summary>
		/// <param name="targetObject">The target dependency object that defines the target dependency property.</param>
		/// <param name="targetProperty">The target dependency property whose value should be bound.</param>
		internal void Initialize(DependencyObject targetObject, DependencyProperty<T> targetProperty)
		{
			Assert.ArgumentNotNull(targetObject);
			Assert.ArgumentNotNull(targetProperty);
			Assert.NotNull(SourceExpression, "No getter has been set.");

			_targetObject = targetObject;
			_targetProperty = targetProperty;

			_sourceFunc = SourceExpression.Compile();
			if (SourceObject == null)
			{
				var uiElement = _targetObject as UIElement;
				Assert.NotNull(uiElement, "No source object has been set; this is OK as long as the target object is an UIElement, " +
										  "in which case the UIElement's view model becomes the source object.");

				SourceObject = uiElement.ViewModel;
			}

			_targetObject.SetValue(_targetProperty, _sourceFunc(SourceObject));
			WalkExpression(SourceExpression);
		}

		private void WalkExpression(Expression<Func<object, T>> expression)
		{
			new Visitor().Visit(expression);

			// Idea: consider a.b.c.d
			// Flatten to array as follows:
			// - [0]: object = source, prop = a
			// - [1]: object = a, prop = b
			// - [2]: object = b, prop = c
			// - [3]: object = c, prop = d
			// use reflection to access individual elements (for entire expression, use compiled lambda)
			//    -> getting the value is cheap, property changes of the last element are somewhat cheap, 
			//       property changes somewhere else are expensive (requires reflection, but should not happen that often?)
			// register change handler on each (if invoked, search for object instance and prop name)
			// if, for instance, b.c changes, unregister on c, update entry [3] and register handler again (and set new value on DP)
		}

		class Visitor : ExpressionVisitor
		{
			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.MemberListBinding"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
			{
				return base.VisitMemberListBinding(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.MemberMemberBinding"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
			{
				return base.VisitMemberMemberBinding(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.MemberAssignment"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
			{
				return base.VisitMemberAssignment(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.MemberBinding"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override MemberBinding VisitMemberBinding(MemberBinding node)
			{
				return base.VisitMemberBinding(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.ElementInit"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override ElementInit VisitElementInit(ElementInit node)
			{
				return base.VisitElementInit(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.ListInitExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitListInit(ListInitExpression node)
			{
				return base.VisitListInit(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.MemberInitExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitMemberInit(MemberInitExpression node)
			{
				return base.VisitMemberInit(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitUnary(UnaryExpression node)
			{
				return base.VisitUnary(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.TypeBinaryExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitTypeBinary(TypeBinaryExpression node)
			{
				return base.VisitTypeBinary(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.TryExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitTry(TryExpression node)
			{
				return base.VisitTry(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.CatchBlock"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override CatchBlock VisitCatchBlock(CatchBlock node)
			{
				return base.VisitCatchBlock(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.SwitchExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitSwitch(SwitchExpression node)
			{
				return base.VisitSwitch(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.SwitchCase"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override SwitchCase VisitSwitchCase(SwitchCase node)
			{
				return base.VisitSwitchCase(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.RuntimeVariablesExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
			{
				return base.VisitRuntimeVariables(node);
			}

			/// <summary>
			/// Visits the <see cref="T:System.Linq.Expressions.ParameterExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitParameter(ParameterExpression node)
			{
				return base.VisitParameter(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.NewExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitNew(NewExpression node)
			{
				return base.VisitNew(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.NewArrayExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitNewArray(NewArrayExpression node)
			{
				return base.VisitNewArray(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				return base.VisitMethodCall(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.IndexExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitIndex(IndexExpression node)
			{
				return base.VisitIndex(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitMember(MemberExpression node)
			{
				return base.VisitMember(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.LoopExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitLoop(LoopExpression node)
			{
				return base.VisitLoop(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.Expression`1"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param><typeparam name="T">The type of the delegate.</typeparam>
			protected override Expression VisitLambda<T1>(Expression<T1> node)
			{
				return base.VisitLambda(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.LabelExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitLabel(LabelExpression node)
			{
				return base.VisitLabel(node);
			}

			/// <summary>
			/// Visits the <see cref="T:System.Linq.Expressions.LabelTarget"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override LabelTarget VisitLabelTarget(LabelTarget node)
			{
				return base.VisitLabelTarget(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.InvocationExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitInvocation(InvocationExpression node)
			{
				return base.VisitInvocation(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.GotoExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitGoto(GotoExpression node)
			{
				return base.VisitGoto(node);
			}

			/// <summary>
			/// Visits the children of the extension expression.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitExtension(Expression node)
			{
				return base.VisitExtension(node);
			}

			/// <summary>
			/// Visits the <see cref="T:System.Linq.Expressions.DefaultExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitDefault(DefaultExpression node)
			{
				return base.VisitDefault(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.DynamicExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitDynamic(DynamicExpression node)
			{
				return base.VisitDynamic(node);
			}

			/// <summary>
			/// Visits the <see cref="T:System.Linq.Expressions.DebugInfoExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitDebugInfo(DebugInfoExpression node)
			{
				return base.VisitDebugInfo(node);
			}

			/// <summary>
			/// Visits the <see cref="T:System.Linq.Expressions.ConstantExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitConstant(ConstantExpression node)
			{
				return base.VisitConstant(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.ConditionalExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitConditional(ConditionalExpression node)
			{
				return base.VisitConditional(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.BlockExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitBlock(BlockExpression node)
			{
				return base.VisitBlock(node);
			}

			/// <summary>
			/// Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression"/>.
			/// </summary>
			/// <returns>
			/// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
			/// </returns>
			/// <param name="node">The expression to visit.</param>
			protected override Expression VisitBinary(BinaryExpression node)
			{
				return base.VisitBinary(node);
			}
		}
	}
}