using System;

namespace Pegasus.Scripting
{
	/// <summary>
	///   Represents a line of a configuration file.
	/// </summary>
	internal class ConfigurationLine
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="content">The content of the line.</param>
		public ConfigurationLine(string content)
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
		public ConfigurationLine(string content, Instruction instruction, int endOfRequest = -1)
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