using System;

namespace Pegasus.Framework.Scripting.Requests
{
	/// <summary>
	///   A user request that instructs the system to invoke a command with the parameters supplied by the user.
	/// </summary>
	internal sealed class InvokeCommand : CommandRequest
	{
		/// <summary>
		///   The parameters that should be passed to the command.
		/// </summary>
		private readonly object[] _parameters;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="command">The the command that should be invoked.</param>
		/// <param name="parameters">The parameters that should be passed to the command.</param>
		public InvokeCommand(ICommand command, object[] parameters)
			: base(command)
		{
			Assert.ArgumentNotNull(parameters, () => parameters);
			_parameters = parameters;
		}

		/// <summary>
		///   Executes the user command.
		/// </summary>
		public override void Execute()
		{
			Command.Invoke(_parameters);
		}
	}
}