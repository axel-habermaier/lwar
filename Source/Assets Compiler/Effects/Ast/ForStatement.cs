using System;

namespace Pegasus.AssetsCompiler.Effects.Ast
{
	/// <summary>
	///   Represents a for-loop.
	/// </summary>
	internal class ForStatement : Statement
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="initializers">The initializer statements that are executed when the loop is entered.</param>
		/// <param name="condition">The termination condition.</param>
		/// <param name="actions">The actions that are executed after each invocation of the loop.</param>
		/// <param name="body">The statement comprising the loop's body.</param>
		public ForStatement(Statement[] initializers, Expression condition, Statement[] actions, Statement body)
		{
			Initializers = initializers;
			Condition = condition;
			Actions = actions;
			Body = body;
		}

		/// <summary>
		///   Gets the initializer statements that are executed when the loop is entered.
		/// </summary>
		public Statement[] Initializers { get; private set; }

		/// <summary>
		///   Gets the termination condition.
		/// </summary>
		public Expression Condition { get; private set; }

		/// <summary>
		///   Gets the actions that are executed after each invocation of the loop.
		/// </summary>
		public Statement[] Actions { get; private set; }

		/// <summary>
		///   Gets the statement comprising the loop's body.
		/// </summary>
		public Statement Body { get; private set; }
	}
}