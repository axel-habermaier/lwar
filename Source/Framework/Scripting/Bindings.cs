using System;

namespace Pegasus.Framework.Scripting
{
	using System.Collections.Generic;
	using Parsing;
	using Platform.Input;
	using Platform.Memory;

	/// <summary>
	///   Manages all registered instruction bindings.
	/// </summary>
	internal class Bindings : DisposableObject
	{
		/// <summary>
		///   The registered instruction bindings.
		/// </summary>
		private readonly List<Binding> _bindings = new List<Binding>();

		/// <summary>
		///   The command registry that is used to look up commands.
		/// </summary>
		private readonly CommandRegistry _commands;

		/// <summary>
		///   The logical input device that is used to determine whether the logical inputs are triggered.
		/// </summary>
		private readonly LogicalInputDevice _device;

		/// <summary>
		///   The parser that is used to parse the instructions.
		/// </summary>
		private readonly Parser<Instruction, None> _parser;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="device">The logical input device that is used to determine whether the logical inputs are triggered.</param>
		/// <param name="commands">The command registry that should be used to look up commands.</param>
		/// <param name="cvars">The cvar registry that should be used to look up cvars.</param>
		public Bindings(LogicalInputDevice device, CommandRegistry commands, CvarRegistry cvars)
		{
			Assert.ArgumentNotNull(device, () => device);
			Assert.ArgumentNotNull(cvars, () => cvars);
			Assert.ArgumentNotNull(commands, () => commands);

			_device = device;
			_commands = commands;
			_parser = new InstructionParser(_commands, cvars);

			commands.OnBind += OnBindCommand;
		}

		/// <summary>
		///   Registers a new binding.
		/// </summary>
		/// <param name="binding">The binding that should be registered.</param>
		private void Register(Binding binding)
		{
			_bindings.Add(binding);
		}

		/// <summary>
		///   Executes all instructions for which the binding's trigger has been triggered.
		/// </summary>
		public void Update()
		{
			foreach (var binding in _bindings)
				binding.ExecuteIfTriggered();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_commands.OnBind -= OnBindCommand;
		}

		/// <summary>
		///   Invoked when the bind command is used.
		/// </summary>
		/// <param name="trigger">The trigger that should be bound.</param>
		/// <param name="command">The instruction that should be bound.</param>
		private void OnBindCommand(InputTrigger trigger, string command)
		{
			if (String.IsNullOrWhiteSpace(command))
			{
				Log.Error("The second parameter of the bind command must not be empty.");
				return;
			}

			var reply = _parser.Parse(command);
			if (reply.Status == ReplyStatus.Success)
			{
				var input = new LogicalInput(trigger, InputModes.Debug | InputModes.Menu | InputModes.Game);
				_device.Register(input);
				Register(new Binding(input, reply.Result));
			}
			else
				Log.Error("Error while parsing the second parameter of the bind command: {0}", reply.Errors.ErrorMessage);
		}
	}
}