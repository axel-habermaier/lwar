using System;

namespace Pegasus.AssetsCompiler.CodeGeneration
{
	using Framework.Platform;
	using Framework.Platform.Logging;
	using ICSharpCode.NRefactory;

	/// <summary>
	///   Provides methods to report errors and warnings.
	/// </summary>
	public interface IErrorReporter
	{
		/// <summary>
		///   Reports an error or a warning.
		/// </summary>
		/// <param name="type">The type of the reported message.</param>
		/// <param name="file">The name of the file for which the message should be reported.</param>
		/// <param name="message">The message that should be reported.</param>
		/// <param name="begin">The beginning of the message location in the source file.</param>
		/// <param name="end">The end of the message location in the source file.</param>
		void Report(LogType type, string file, string message, TextLocation begin, TextLocation end);
	}
}