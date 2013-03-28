﻿using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	/// <summary>
	///   Represents an expression that is used as a statement.
	/// </summary>
	internal class ExpressionStatement : Statement
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="expression">The expression that the statement should be comprised of.</param>
		public ExpressionStatement(Expression expression)
		{
			Expression = expression;
		}

		/// <summary>
		///   Gets the expression that the statement is comprised of.
		/// </summary>
		public Expression Expression { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitExpressionStatement(this);
		}
	}
}