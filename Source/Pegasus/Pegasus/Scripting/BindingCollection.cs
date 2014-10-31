namespace Pegasus.Scripting
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Parsing;
	using Platform.Logging;
	using Platform.Memory;
	using UserInterface.Input;
	using Utilities;

	/// <summary>
	///     Manages all registered instruction bindings.
	/// </summary>
	internal class BindingCollection : DisposableObject
	{
		/// <summary>
		///     The registered instruction bindings.
		/// </summary>
		private readonly List<Binding> _bindings = new List<Binding>();

		/// <summary>
		///     The logical input device that is used to determine whether the logical inputs are triggered.
		/// </summary>
		private readonly LogicalInputDevice _device;

		/// <summary>
		///     The parser that is used to parse the instructions.
		/// </summary>
		private readonly Parser<Instruction> _parser;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="device">The logical input device that is used to determine whether the logical inputs are triggered.</param>
		public BindingCollection(LogicalInputDevice device)
		{
			Assert.ArgumentNotNull(device);

			_device = device;
			_parser = new InstructionParser();

			Commands.OnBind += OnBind;
			Commands.OnUnbind += OnUnbind;
			Commands.OnUnbindAll += OnUnbindAll;
			Commands.OnListBindings += OnListBindings;
		}

		/// <summary>
		///     Executes all instructions for which the binding's trigger has been triggered.
		/// </summary>
		public void Update()
		{
			if (_device.TextInputEnabled || Application.Current.IsConsoleOpen)
				return;

			foreach (var binding in _bindings)
				binding.ExecuteIfTriggered();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.OnBind -= OnBind;
			Commands.OnUnbind -= OnUnbind;
			Commands.OnUnbindAll -= OnUnbindAll;
			Commands.OnListBindings -= OnListBindings;
		}

		/// <summary>
		///     Lists all active bindings.
		/// </summary>
		private void OnListBindings()
		{
			StringBuilder builder;
			using (StringBuilderPool.Allocate(out builder))
			{
				var bindingGroups = (from binding in _bindings
									 group binding by binding.Command
									 into bindingGroup
									 orderby bindingGroup.Key
									 select new { Command = bindingGroup.Key, Bindings = bindingGroup.ToArray() }).ToArray();

				builder.Append("\n");
				foreach (var group in bindingGroups)
				{
					builder.AppendFormat("'\\lightgrey{0}\\\0' on ", group.Command);
					builder.Append(String.Join(", ",
						group.Bindings.Select(binding => String.Format("\\lightgrey{0}\\\0", TypeRegistry.ToString(binding.Input.Trigger)))));
					builder.Append("\n");
				}

				if (bindingGroups.Length == 0)
				{
					Log.Warn("There are no registered bindings.");
					return;
				}

				Log.Info("{0}", builder);
			}
		}

		/// <summary>
		///     Removes all bindings.
		/// </summary>
		private void OnUnbindAll()
		{
			_bindings.Clear();
			Log.Info("All command bindings have been removed.");
		}

		/// <summary>
		///     Invoked when the unbind command is used.
		/// </summary>
		/// <param name="trigger">The trigger that should be unbound.</param>
		private void OnUnbind(InputTrigger trigger)
		{
			var removed = 0;
			for (var i = 0; i < _bindings.Count; ++i)
			{
				if (!_bindings[i].Input.Trigger.Equals(trigger))
					continue;

				++removed;
				_bindings.RemoveAt(i);
				--i;
			}

			if (removed == 1)
				Log.Info("The command binding for '{0}' has been removed.", TypeRegistry.ToString(trigger));
			else if (removed != 0)
				Log.Info("{0} command bindings for '{1}' have been removed.", removed, TypeRegistry.ToString(trigger));
			else
				Log.Error("No binding could be found with trigger '{0}'. Note that the order of operands is important for combined triggers " +
						  "(i.e., [Key(A,Pressed)|Key(B,Pressed)] is considered to be different from [Key(B,Pressed)|Key(A,Pressed)]). " +
						  "Use the 'list_bindings' command to view all active bindings.", TypeRegistry.ToString(trigger));
		}

		/// <summary>
		///     Invoked when the bind command is used.
		/// </summary>
		/// <param name="trigger">The trigger that should be bound.</param>
		/// <param name="command">The instruction that should be bound.</param>
		private void OnBind(InputTrigger trigger, string command)
		{
			var reply = _parser.Parse(command);

			var input = new LogicalInput(trigger);
			_device.Add(input);

			if (reply.Status == ReplyStatus.Success)
				_bindings.Add(new Binding(input, command, reply.Result));
			else
				_bindings.Add(item: new Binding(input, command, reply.Errors.ErrorMessage));
		}
	}
}