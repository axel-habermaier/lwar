using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Compilation;

	/// <summary>
	///   Represents a reference to a shader variable, parameter, texture object, constant or literal.
	/// </summary>
	internal class VariableReference<T> : Expression
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
			if (typeof(T) == typeof(ShaderConstant))
				visitor.VisitVariableReference((VariableReference<ShaderConstant>)(object)this);
			else if (typeof(T) == typeof(ShaderLiteral))
				visitor.VisitVariableReference((VariableReference<ShaderLiteral>)(object)this);
			else if (typeof(T) == typeof(ShaderTexture))
				visitor.VisitVariableReference((VariableReference<ShaderTexture>)(object)this);
			else if (typeof(T) == typeof(ShaderVariable))
				visitor.VisitVariableReference((VariableReference<ShaderVariable>)(object)this);
			else
				throw new InvalidOperationException("Unknown variable reference type.");
		}
	}
}