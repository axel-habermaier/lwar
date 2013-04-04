using System;

namespace Pegasus.Framework.Scripting.Requests
{
	/// <summary>
	///   A user request that instructs the system to show the affected command's help description.
	/// </summary>
	internal sealed class DescribeCommand : CommandRequest
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="command">The command that should be described.</param>
		public DescribeCommand(ICommand command)
			: base(command)
		{
		}

		/// <summary>
		///   Executes the user command.
		/// </summary>
		public override void Execute()
		{
			Log.Info("'{0}': {1}", Command.Signature, Command.Description);
		}
	}
}