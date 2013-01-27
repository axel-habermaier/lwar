using System;

namespace Pegasus.Framework.Scripting.Requests
{
	/// <summary>
	///   A user request that affects a command.
	/// </summary>
	internal abstract class CommandRequest : IRequest
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="command">The command that should be affected by the request.</param>
		protected CommandRequest(ICommand command)
		{
			Assert.ArgumentNotNull(command, () => command);
			Command = command;
		}

		/// <summary>
		///   The command that is affected by the request.
		/// </summary>
		protected ICommand Command { get; private set; }

		/// <summary>
		///   Executes the user command.
		/// </summary>
		public abstract void Execute();
	}
}