using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents an assignment of a value to a variable.
	/// </summary>
	internal class AssignmentExpression : Expression
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="left">The expression on the left-hand side of the assignment operator.</param>
		/// <param name="assignmentOperator">The assignment operator.</param>
		/// <param name="right">The expression on the right-hand side of the assignment operator.</param>
		public AssignmentExpression(Expression left, AssignmentOperatorType assignmentOperator, Expression right)
		{
			Left = left;
			Operator = assignmentOperator;
			Right = right;
		}

		/// <summary>
		///   Gets the expression on the left-hand side of the assignment operator.
		/// </summary>
		public Expression Left { get; private set; }

		/// <summary>
		///   Gets the expression on the right-hand side of the assignment operator.
		/// </summary>
		public Expression Right { get; private set; }

		/// <summary>
		///   Gets the assignment operator.
		/// </summary>
		public AssignmentOperatorType Operator { get; private set; }

		/// Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitAssignmentExpression(this);
		}
	}
}