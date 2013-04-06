﻿using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;
	using System.Linq;
	using Parsing;
	using Platform;

	/// <summary>
	///   Represents a configuration file that can be processed by the application, changed, and/or written to disk.
	/// </summary>
	internal class ConfigurationFile
	{
		/// <summary>
		///   The token that starts a single-line comment.
		/// </summary>
		private const string CommentToken = "//";

		/// <summary>
		///   The name of the automatically executed configuration file.
		/// </summary>
		public const string AutoExec = "autoexec.cfg";

		/// <summary>
		///   The application file that is used to read and write the configuration.
		/// </summary>
		private readonly AppFile _file;

		/// <summary>
		///   The parser that is used to parse the file.
		/// </summary>
		private readonly InstructionParser _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="parser"> The parser that should be used to parse the file.</param>
		/// <param name="appName">The name of the application.</param>
		/// <param name="fileName">The name of the configuration file.</param>
		public ConfigurationFile(InstructionParser parser, string appName, string fileName)
		{
			Assert.ArgumentNotNull(parser, () => parser);
			Assert.ArgumentNotNullOrWhitespace(appName, () => appName);

			if (String.IsNullOrWhiteSpace(fileName))
			{
				Log.Error("The file name cannot consist of whitespace only.");
				return;
			}

			_parser = parser;
			_file = new AppFile(appName, fileName);

			if (!_file.IsValid)
				Log.Error("'{0}' is not a valid file name.", _file.FileName);
		}

		/// <summary>
		///   Parses the individual lines of the configuration file.
		/// </summary>
		/// <param name="silent">Indicates whether parser errors should be silently ignored.</param>
		private IEnumerable<Line> ParseLines(bool silent)
		{
			string input;
			if (!_file.Read(out input, e =>
				{
					if (!silent)
						Log.Error("Unable to read '{0}': {1}", _file.FileName, e.Message);
				}))
				yield break;

			var lineNumber = 0;
			foreach (var line in input.Split('\n'))
			{
				++lineNumber;
				if (String.IsNullOrWhiteSpace(line) || line.Trim().StartsWith(CommentToken))
				{
					yield return new Line(line);
					continue;
				}

				yield return ParseLine(line, lineNumber, silent);
			}
		}

		/// <summary>
		///   Parses the given line.
		/// </summary>
		/// <param name="line">The line that should be parsed.</param>
		/// <param name="lineNumber">The number of the line that should be parsed.</param>
		/// <param name="silent">Indicates whether parser errors should be silently ignored.</param>
		private Line ParseLine(string line, int lineNumber, bool silent)
		{
			var inputStream = new InputStream<None>(line, _file.FileName, lineNumber);
			var reply = _parser.Parse(inputStream);

			if (reply.Status == ReplyStatus.Success)
				return new Line(line, reply.Result);
			else
			{
				// Check if there's a comment at the end of the line, try again with the comment removed
				if (inputStream.Skip(CommentToken))
				{
					var endRequest = inputStream.State.Column - CommentToken.Length - 1;
					// Find the first non-whitespace token
					while (endRequest - 1 > 0 && Char.IsWhiteSpace(line[endRequest - 1]))
						--endRequest;

					var parsedLine = ParseLine(line.Substring(0, endRequest), lineNumber, silent);

					return new Line(line, parsedLine.Instruction, endRequest);
				}

				if (!silent)
				{
					reply.GenerateErrorMessage(inputStream);
					Log.Error(reply.Errors.ErrorMessage);
				}

				return new Line(line);
			}
		}

		/// <summary>
		///   Processes all set cvar and command invocation user requests stored in the configuration file.
		/// </summary>
		public void Process()
		{
			if (!_file.IsValid)
				return;

			Log.Info("Processing '{0}'...", _file.FileName);
			foreach (var line in ParseLines(false).Where(line => line.HasInstruction))
				line.Instruction.Execute();
		}

		/// <summary>
		///   Persists the given cvars in the configuration file.
		/// </summary>
		/// <param name="cvars">The cvars that should be persisted in the file.</param>
		public void Persist(IEnumerable<ICvar> cvars)
		{
			Assert.ArgumentNotNull(cvars, () => cvars);

			if (!_file.IsValid)
				return;

			var lines = ParseLines(true).ToList();
			foreach (var cvar in cvars)
			{
				var content = String.Format("{0} {1}", cvar.Name, cvar.StringValue);
				var line = lines.LastOrDefault(l => l.HasInstruction && l.Instruction.HasTarget(cvar));

				if (line == null)
					lines.Add(new Line(content));
				else
					line.Content = content + line.Content.Substring(line.EndOfRequest);
			}

			var fileContent = String.Join(Environment.NewLine, lines.Select(line => line.Content));
			if (_file.Write(fileContent, e => Log.Error("Failed to write '{0}': {1}", _file.FileName, e.Message)))
				Log.Info("'{0}' has been written.", _file.FileName);
		}

		/// <summary>
		///   Represents a line of the configuration file.
		/// </summary>
		private class Line
		{
			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="content">The content of the line.</param>
			public Line(string content)
			{
				Content = content;
				EndOfRequest = Content.Length;
				HasInstruction = false;
			}

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="content">The content of the line.</param>
			/// <param name="instruction">The parsed instruction of the line.</param>
			/// <param name="endOfRequest">The index of the column where the request ended and a comment started.</param>
			public Line(string content, Instruction instruction, int endOfRequest = -1)
			{
				Content = content;
				Instruction = instruction;
				EndOfRequest = endOfRequest == -1 ? Content.Length : endOfRequest;
				HasInstruction = true;
			}

			/// <summary>
			///   Gets or sets the line content.
			/// </summary>
			public string Content { get; set; }

			/// <summary>
			///   Gets parsed instruction of the line.
			/// </summary>
			public Instruction Instruction { get; private set; }

			/// <summary>
			///   Gets a value indicating whether the line has an instruction.
			/// </summary>
			public bool HasInstruction { get; private set; }

			/// <summary>
			///   Gets or sets the index of the column where the instruction ended and a comment started.
			/// </summary>
			public int EndOfRequest { get; private set; }
		}
	}
}