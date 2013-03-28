using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Compilation;
	using ICSharpCode.NRefactory.CSharp;

	/// <summary>
	///   Represents the use of a binary operator that combines two sub-expressions.
	/// </summary>
	internal class BinaryOperatorExpression : Expression
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="left">The sub-expression on the left-hand side of the operator.</param>
		/// <param name="leftType">The data type of the sub-expression on the left-hand side of the operator.</param>
		/// <param name="binaryOperator">The binary operator that combines the two expressions.</param>
		/// <param name="right">The sub-expression on the right-hand side of the operator.</param>
		/// <param name="rightType">The data type of the sub-expression on the right-hand side of the operator.</param>
		public BinaryOperatorExpression(Expression left, DataType leftType, BinaryOperatorType binaryOperator, Expression right,
										DataType rightType)
		{
			Left = left;
			LeftType = leftType;
			Operator = binaryOperator;
			Right = right;
			RightType = rightType;
		}

		/// <summary>
		///   Gets the sub-expression on the left-hand side of the operator.
		/// </summary>
		public Expression Left { get; private set; }

		/// <summary>
		///   Gets the sub-expression on the right-hand side of the operator.
		/// </summary>
		public Expression Right { get; private set; }

		/// <summary>
		///   Gets the data type of the sub-expression on the left-hand side of the operator.
		/// </summary>
		public DataType LeftType { get; private set; }

		/// <summary>
		///   Gets the data type of the sub-expression on the right-hand side of the operator.
		/// </summary>
		public DataType RightType { get; private set; }

		/// <summary>
		///   Gets the binary operator that combines the two expressions.
		/// </summary>
		public BinaryOperatorType Operator { get; private set; }
	}
}