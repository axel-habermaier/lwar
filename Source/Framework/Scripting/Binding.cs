using System;

namespace Pegasus.Framework.Scripting
{
	using Platform.Input;

	/// <summary>
	///   Binds an instruction to a logical input. Whenever the input is triggered, the instruction is executed.
	/// </summary>
	internal struct Binding
	{
		/// <summary>
		///   The input that triggers the execution of the instruction.
		/// </summary>
		private readonly LogicalInput _input;

		/// <summary>
		///   The instruction that is executed when the input is triggered.
		/// </summary>
		private readonly Instruction _instruction;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="input">The input that should trigger the execution of the instruction.</param>
		/// <param name="instruction">The instruction that should be executed when the input is triggered.</param>
		public Binding(LogicalInput input, Instruction instruction)
		{
			Assert.ArgumentNotNull(input);

			_input = input;
			_instruction = instruction;
		}

		/// <summary>
		///   Executes the user request if the input has been triggered.
		/// </summary>
		public void ExecuteIfTriggered()
		{
			if (_input.IsTriggered)
				_instruction.Execute();
		}
	}
}