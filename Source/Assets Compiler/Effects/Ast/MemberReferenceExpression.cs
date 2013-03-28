using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Compilation;

	/// <summary>
	///   Represents a member reference expression.
	/// </summary>
	internal class MemberReferenceExpression : Expression
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="target">The expression for the target object.</param>
		/// <param name="type">The type of the target.</param>
		/// <param name="member">The name of the member that is accessed.</param>
		public MemberReferenceExpression(Expression target, DataType type, string member)
		{
			Target = target;
			Type = type;
			Member = member;
		}

		/// <summary>
		///   Gets the expression for the target object.
		/// </summary>
		public Expression Target { get; private set; }

		/// <summary>
		///   Gets the type of the target.
		/// </summary>
		public DataType Type { get; private set; }

		/// <summary>
		///   Gets the name of the member that is accessed.
		/// </summary>
		public string Member { get; private set; }

		/// <summary>
		///   Accepts a visitor, calling the appropriate Visit method on the visitor.
		/// </summary>
		/// <param name="visitor">The visitor whose Visit method should be called.</param>
		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitMemberReferenceExpression(this);
		}
	}
}