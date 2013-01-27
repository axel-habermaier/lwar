using System;

namespace Pegasus.Framework.Scripting.Requests
{
	/// <summary>
	///   An interface for user requests. User requests are generally entered via the console by the user
	///   and created by the parser.
	/// </summary>
	internal interface IRequest
	{
		/// <summary>
		///   Executes the user command.
		/// </summary>
		void Execute();
	}
}