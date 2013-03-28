using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
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
	}
}