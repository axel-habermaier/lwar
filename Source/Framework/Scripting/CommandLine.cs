using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;
	using Parsing;
	using Requests;

	/// <summary>
	///   Stores the command line arguments that have been passed to the application.
	/// </summary>
	internal class CommandLine
	{
		/// <summary>
		///   Indicates whether the compiled assets should be cleaned.
		/// </summary>
		public bool CleanAssets;

		/// <summary>
		///   Indicates whether the all assets should be compiled.
		/// </summary>
		public bool CompileAssets;

		/// <summary>
		///   Indicates whether the all assets should be recompiled.
		/// </summary>
		public bool RecompileAssets;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public CommandLine()
		{
			SetCvars = new List<SetCvar>();
		}

		/// <summary>
		///   A list of cvar set requests.
		/// </summary>
		public List<SetCvar> SetCvars { get; private set; }

		/// <summary>
		///   Parses the command line.
		/// </summary>
		public static CommandLine Parse()
		{
			var reply = new CommandLineParser().Parse(Environment.CommandLine);
			if (reply.Status != ReplyStatus.Success)
				Log.Die(reply.Errors.ErrorMessage);
			else
				return reply.Result;

			return new CommandLine();
		}
	}
}