using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Compilation;

	/// <summary>
	///   Represents a single variable declaration, optionally with an initialization expression.
	/// </summary>
	internal class VariableInitializer : IAstNode
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="variable">The declared variable.</param>
		/// <param name="expression">The initialization expression, if any, or null if the variable is not initialized.</param>
		public VariableInitializer(ShaderVariable variable, Expression expression)
		{
			Variable = variable;
			Expression = expression;
		}

		/// <summary>
		///   Gets the declared variable.
		/// </summary>
		public ShaderVariable Variable { get; private set; }

		/// <summary>
		///   Gets the initialization expression, if any, or null if the variable is not initialized.
		/// </summary>
		public Expression Expression { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitVariableInitializer(this);
		}
	}
}