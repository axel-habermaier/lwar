using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Compilation;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents the use of an unary operator that is applied to a sub-expression.
	/// </summary>
	internal class UnaryOperatorExpression : Expression
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="expression">The sub-expression the operator is applied to.</param>
		/// <param name="type">The data type of the sub-expression.</param>
		/// <param name="unaryOperator">The unary operator that combines the two expressions.</param>
		public UnaryOperatorExpression(Expression expression, DataType type, UnaryOperatorType unaryOperator)
		{
			Expression = expression;
			Type = type;
			Operator = unaryOperator;
		}

		/// <summary>
		///   Gets the sub-expression the operator is applied to.
		/// </summary>
		public Expression Expression { get; private set; }

		/// <summary>
		///   Gets the data type of the sub-expression.
		/// </summary>
		public DataType Type { get; private set; }

		/// <summary>
		///   Gets the unary operator that combines the two expressions.
		/// </summary>
		public UnaryOperatorType Operator { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitUnaryOperatorStatement(this);
		}
	}
}