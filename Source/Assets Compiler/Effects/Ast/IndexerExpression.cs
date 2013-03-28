using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	/// <summary>
	///   Represents an array indexing expression with multiple arguments.
	/// </summary>
	internal class IndexerExpression : Expression
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="target">The expression that represents the target of the indexing operation.</param>
		/// <param name="arguments">The arguments of the indexing operation.</param>
		public IndexerExpression(Expression target, Expression[] arguments)
		{
			Target = target;
			Arguments = arguments;
		}

		/// <summary>
		///   Gets the expression that represents the target of the indexing operation.
		/// </summary>
		public Expression Target { get; private set; }

		/// <summary>
		///   Gets the arguments of the indexing operation.
		/// </summary>
		public Expression[] Arguments { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitIndexerExpression(this);
		}
	}
}