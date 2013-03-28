﻿using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	/// <summary>
	///   Represents a return statement.
	/// </summary>
	internal class ReturnStatement : Statement
	{
		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitReturnStatement(this);
		}
	}
}