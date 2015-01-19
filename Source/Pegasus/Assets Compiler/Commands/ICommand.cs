namespace Pegasus.AssetsCompiler.Commands
{
	using System;

	/// <summary>
	///     Represents a command that can be invoked via the command line.
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		///     Executes the command.
		/// </summary>
		void Execute();
	}
}