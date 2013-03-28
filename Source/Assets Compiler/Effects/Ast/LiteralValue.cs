using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	/// <summary>
	///   Represents a literal value.
	/// </summary>
	internal class LiteralValue : Expression
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="value">The value that the expression represents.</param>
		public LiteralValue(object value)
		{
			Value = value;
		}

		/// <summary>
		///   Gets the value that the expression represents.
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitLiteralValue(this);
		}
	}
}