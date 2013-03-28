using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	/// <summary>
	///   Represents a node in the shader abstract syntax tree.
	/// </summary>
	internal interface IAstNode
	{
		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		void AcceptVisitor(IAstVisitor visitor);
	}
}