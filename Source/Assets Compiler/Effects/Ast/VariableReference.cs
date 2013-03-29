using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Compilation;

	/// <summary>
	///   Represents a reference to a shader variable, parameter, texture object, constant or literal.
	/// </summary>
	internal class VariableReference<T> : Expression
		where T : IShaderDataObject
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="variable">The referenced variable.</param>
		public VariableReference(T variable)
		{
			Variable = variable;
		}

		/// <summary>
		///   Gets the referenced variable.
		/// </summary>
		public T Variable { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitVariableReference(this);
		}
	}
}