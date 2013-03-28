using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	/// <summary>
	///   Represents a code statements.
	/// </summary>
	internal abstract class Statement : IAstNode
	{
		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public abstract void AcceptVisitor(IAstVisitor visitor);
	}
}