using System;

namespace Pegasus.AssetsCompiler.Effects.Compilation
{
	using System.IO;
	using System.Text;
	using Framework;

	/// <summary>
	///   Writes code with a C-like syntax to an in-memory buffer.
	/// </summary>
	public class CodeWriter
	{
		/// <summary>
		///   The buffer that contains the written code.
		/// </summary>
		private readonly StringBuilder _buffer = new StringBuilder(4096);

		/// <summary>
		///   Indicates whether the writer is currently at the beginning of a new line.
		/// </summary>
		private bool _atBeginningOfLine;

		/// <summary>
		///   The number of tabs that are placed at the beginning of the next line.
		/// </summary>
		private int _indent;

		/// <summary>
		///   Appends the given format string to the current line.
		/// </summary>
		/// <param name="format">The format string that should be appended.</param>
		/// <param name="arguments">The arguments that should be copied into the format string.</param>
		[StringFormatMethod("format")]
		public void Append(string format, params object[] arguments)
		{
			AddIndentation();
			_buffer.AppendFormat(format, arguments);
		}

		/// <summary>
		///   Appends the given format string to the current line and starts a new line.
		/// </summary>
		/// <param name="format">The format string that should be appended.</param>
		/// <param name="arguments">The arguments that should be copied into the format string.</param>
		[StringFormatMethod("format")]
		public void AppendLine(string format, params object[] arguments)
		{
			AddIndentation();
			_buffer.AppendFormat(format, arguments);
			Newline();
		}

		/// <summary>
		///   Appends a new line to the buffer.
		/// </summary>
		public void Newline()
		{
			_buffer.AppendLine();
			_atBeginningOfLine = true;
		}

		/// <summary>
		///   Ensures that the subsequent write operation is performed on a new line.
		/// </summary>
		public void EnsureNewLine()
		{
			if (!_atBeginningOfLine)
				Newline();
		}

		/// <summary>
		///   If the writer is currently at the beginning of a new line, adds the necessary number of tabs to the current line in
		///   order to get the desired indentation level.
		/// </summary>
		private void AddIndentation()
		{
			if (!_atBeginningOfLine)
				return;

			_atBeginningOfLine = false;
			for (var i = 0; i < _indent; ++i)
				_buffer.Append("\t");
		}

		/// <summary>
		///   Increases the indent.
		/// </summary>
		public void IncreaseIndent()
		{
			++_indent;
		}

		/// <summary>
		///   Decreases the indent.
		/// </summary>
		public void DecreaseIndent()
		{
			--_indent;
		}

		/// <summary>
		///   Appends a block statement to the buffer.
		/// </summary>
		/// <param name="content">
		///   Generates the content that should be placed within the block statement by calling Append methods
		///   of this code writer instance.
		/// </param>
		/// <param name="terminateWithSemicolon">Indicates whether the closing brace should be followed by a semicolon.</param>
		public void AppendBlockStatement(Action content, bool terminateWithSemicolon = false)
		{
			Assert.ArgumentNotNull(content, () => content);

			EnsureNewLine();
			AppendLine("{{");
			IncreaseIndent();

			content();

			EnsureNewLine();
			DecreaseIndent();
			Append("}}");

			if (terminateWithSemicolon)
				Append(";");

			Newline();
		}

		/// <summary>
		///   Writes the contents of the buffer to the given file.
		/// </summary>
		/// <param name="file">The file the buffer should be written to.</param>
		public void WriteToFile(string file)
		{
			Assert.ArgumentNotNullOrWhitespace(file, () => file);
			File.WriteAllText(file, _buffer.ToString());
		}
	}
}