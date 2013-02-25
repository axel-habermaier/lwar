using System;

namespace Pegasus.Framework.Platform
{
	using System.Diagnostics;

	/// <summary>
	///   Starts and manages an external process.
	/// </summary>
	public static class ExternalProcess
	{
		/// <summary>
		///   Runs an external tool process.
		/// </summary>
		/// <param name="fileName">The file name of the external tool executable.</param>
		/// <param name="commandLine">The command line arguments that should be passed to the tool.</param>
		/// <param name="arguments">The arguments that should be copied into the command line.</param>
		public static void Run(string fileName, string commandLine, params object[] arguments)
		{
			var process = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = new ProcessStartInfo(fileName, String.Format(commandLine, arguments))
				{
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardInput = true,
					RedirectStandardOutput = true
				}
			};

			process.OutputDataReceived += (o, e) =>
				{
					if (!String.IsNullOrWhiteSpace(e.Data))
						Log.Info("{0}: {1}", fileName, e.Data);
				};
			process.ErrorDataReceived += (o, e) =>
				{
					if (!String.IsNullOrWhiteSpace(e.Data))
						Log.Die("{0}: {1}", fileName, e.Data);
				};

			process.Start();

			process.BeginErrorReadLine();
			process.BeginOutputReadLine();
			process.WaitForExit();
		}
	}
}