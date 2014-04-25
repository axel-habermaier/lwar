namespace Pegasus.Scripting
{
	using System;
	using Parsing;
	using Platform.Logging;

	/// <summary>
	///     Parses the command line, consisting of a string of cvar set requests. For instance, the
	///     command line "-time_scale 0.01" sets the value of the time scale cvar to 0.01.
	/// </summary>
	internal static class CommandLineParser
	{
		/// <summary>
		///     Parses the command line and executes all successfully parsed cvar set instructions.
		/// </summary>
		/// <param name="arguments">The command line arguments that have been passed to the application.</param>
		public static void Parse(string[] arguments)
		{
			if (arguments.Length == 1)
			{
				Log.Info("No command line arguments have been provided.");
				return;
			}

			Log.Info("Parsing the command line arguments...");

			// Skip the first element of the array, as it only contains the file name of the executing program.
			for (var i = 1; i < arguments.Length; ++i)
			{
				// Only cvar set instructions are supported and we require all cvar names to be prefixed with a dash '-'
				// A cvar is expected now, so if the current argument doesn't start with a dash and is not a valid cvar
				// name, report an error and try again for the next argument
				if (!arguments[i].StartsWith("-"))
				{
					Log.Error("Encountered unexpected token '{0}\\\0' in command line at position {1}. " +
							  " Expected the name of a cvar, prefixed with a dash ('-').", arguments[i], i);
					continue;
				}

				ICvar cvar;
				var cvarName = arguments[i].Substring(1);
				if (!CvarRegistry.TryFind(cvarName, out cvar))
				{
					Log.Error("Reference to unknown cvar '{0}\\\0' in command line at position {1}.", cvarName, i);
					continue;
				}

				// We've found a valid cvar, parse its value; report an error if we've reached the end of the command line already
				if (i + 1 < arguments.Length)
				{
					ParseCvar(cvar, arguments[i + 1]);

					// Skip the next index, as it has already been processed
					++i;
				}
				else
					Log.Error("Unexpected end of command line. Expected a value for cvar '{0}'.", cvar.Name);
			}
		}

		/// <summary>
		///     Parses the given string and updates the cvar's value.
		/// </summary>
		/// <param name="cvar">The cvar that should be updated.</param>
		/// <param name="value">The string value the cvar should be updated to.</param>
		private static void ParseCvar(ICvar cvar, string value)
		{
			Assert.ArgumentNotNull(cvar);
			Assert.ArgumentNotNull(value);

			// If the cvar is of type string, escape all quotes and enclose the given string in quotes to ensure that the string is 
			// correctly parsed
			if (cvar.ValueType == typeof(string))
				value = "\"" + value.Replace("\"", "\\\"") + "\"";

			var parser = TypeRegistry.GetParser(cvar.ValueType);
			var reply = parser.Parse(value);

			if (reply.Status != ReplyStatus.Success)
			{
				Log.Error("Failed to parse the value '{0}\\\0' specified via the command line for cvar '{1}'.", value, cvar.Name);
				Log.Error("Cvar type: {0}", TypeRegistry.GetDescription(cvar.ValueType));
				Log.Error("Examples of valid inputs: {0}, ...", string.Join(", ", TypeRegistry.GetExamples(cvar.ValueType)));
				Log.Error(Help.GetHint(cvar.Name));
			}
			else
				new Instruction(cvar, reply.Result).Execute(false);
		}
	}
}