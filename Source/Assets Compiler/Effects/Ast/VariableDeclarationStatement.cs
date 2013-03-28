using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	/// <summary>
	///   Represents a declaration of one or more variables.
	/// </summary>
	internal class VariableDeclarationStatement : Statement
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="variables">The declared variables.</param>
		public VariableDeclarationStatement(VariableInitializer[] variables)
		{
			Variables = variables;
		}

		/// <summary>
		///   Gets the declared variables.
		/// </summary>
		public VariableInitializer[] Variables { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitVariableDeclarationStatement(this);
		}
	}
}