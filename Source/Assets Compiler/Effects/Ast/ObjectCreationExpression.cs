using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Compilation;

	/// <summary>
	///   Represents an object creation expression.
	/// </summary>
	internal class ObjectCreationExpression : Expression
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="type">The type of the created object.</param>
		/// <param name="arguments">The arguments that are passed to the constructor.</param>
		public ObjectCreationExpression(DataType type, Expression[] arguments)
		{
			Type = type;
			Arguments = arguments;
		}

		/// <summary>
		///   Gets the arguments that are passed to the constructor.
		/// </summary>
		public Expression[] Arguments { get; private set; }

		/// <summary>
		///   Gets the type of the created object.
		/// </summary>
		public DataType Type { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitObjectCreationExpression(this);
		}
	}
}