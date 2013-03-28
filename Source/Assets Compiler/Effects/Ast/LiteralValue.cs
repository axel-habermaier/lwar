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
	}
}