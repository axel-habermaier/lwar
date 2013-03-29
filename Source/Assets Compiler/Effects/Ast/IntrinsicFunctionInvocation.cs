using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Compilation;

	/// <summary>
	///   Represents the invocation of an intrinsic function.
	/// </summary>
	internal class IntrinsicFunctionInvocation : Expression
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="target">The target of the function invocation.</param>
		/// <param name="function">The intrinsic function that should be invoked.</param>
		/// <param name="arguments">The arguments of the function invocation.</param>
		public IntrinsicFunctionInvocation(Expression target, IntrinsicFunction function, Expression[] arguments)
		{
			Target = target;
			Function = function;
			Arguments = arguments;
		}

		/// <summary>
		///   Gets the target of the function invocation.
		/// </summary>
		public Expression Target { get; private set; }

		/// <summary>
		///   Gets the intrinsic function that should be invoked.
		/// </summary>
		public IntrinsicFunction Function { get; private set; }

		/// <summary>
		///   Gets the arguments of the function invocation.
		/// </summary>
		public Expression[] Arguments { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitIntrinsicFunctionInvocation(this);
		}
	}
}