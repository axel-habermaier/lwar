using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	using Framework;

	/// <summary>
	///   Represents a set of statements that are executed sequentially.
	/// </summary>
	internal class BlockStatement : Statement
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="statements"> The statements that the statement block should consist of.</param>
		public BlockStatement(Statement[] statements)
		{
			Assert.ArgumentNotNull(statements, () => statements);
			Statements = statements;
		}

		/// <summary>
		///   Gets the statements that the statement block consists of.
		/// </summary>
		public Statement[] Statements { get; private set; }
	}
}