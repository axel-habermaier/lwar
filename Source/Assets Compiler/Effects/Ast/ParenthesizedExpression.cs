using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	/// <summary>
	///   Represents an expression, enclosed in parentheses.
	/// </summary>
	internal class ParenthesizedExpression : Expression
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="expression">The expression that is enclosed in parentheses.</param>
		public ParenthesizedExpression(Expression expression)
		{
			Expression = expression;
		}

		/// <summary>
		///   Gets the expression that is enclosed in parentheses.
		/// </summary>
		public Expression Expression { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitParenthesizedExpression(this);
		}
	}
}