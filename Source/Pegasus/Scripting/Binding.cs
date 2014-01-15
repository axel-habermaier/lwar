namespace Pegasus.Scripting
{
	using System;
	using Platform.Input;

	/// <summary>
	///     Binds an instruction to a logical input. Whenever the input is triggered, the instruction is executed.
	/// </summary>
	internal struct Binding
	{
		/// <summary>
		///     The instruction that is executed when the input is triggered.
		/// </summary>
		private readonly Instruction _instruction;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="input">The input that should trigger the execution of the instruction.</param>
		/// <param name="command">The unparsed instruction.</param>
		/// <param name="instruction">The instruction that should be executed when the input is triggered.</param>
		public Binding(LogicalInput input, string command, Instruction instruction)
			: this()
		{
			Assert.ArgumentNotNull(input);
			Assert.ArgumentNotNullOrWhitespace(command);

			Input = input;
			Command = command;
			_instruction = instruction;
		}

		/// <summary>
		///     The unparsed instruction.
		/// </summary>
		public string Command { get; private set; }

		/// <summary>
		///     The input that triggers the execution of the instruction.
		/// </summary>
		public LogicalInput Input { get; private set; }

		/// <summary>
		///     Executes the user request if the input has been triggered.
		/// </summary>
		public void ExecuteIfTriggered()
		{
			if (Input.IsTriggered)
				_instruction.Execute(true);
		}
	}
}