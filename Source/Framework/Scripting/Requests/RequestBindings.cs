using System;

namespace Pegasus.Framework.Scripting.Requests
{
	using System.Collections.Generic;
	using Parsing;
	using Platform.Input;

	/// <summary>
	///   Manages all registered command bindings.
	/// </summary>
	internal class RequestBindings : DisposableObject
	{
		/// <summary>
		///   The registered command bindings.
		/// </summary>
		private readonly List<RequestBinding> _bindings = new List<RequestBinding>();

		/// <summary>
		///   The logical input device that is used to determine whether the logical inputs are triggered.
		/// </summary>
		private readonly LogicalInputDevice _device;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="device">The logical input device that is used to determine whether the logical inputs are triggered.</param>
		public RequestBindings(LogicalInputDevice device)
		{
			Assert.ArgumentNotNull(device, () => device);

			_device = device;
			Commands.Bind.Invoked += OnBindCommand;
		}

		/// <summary>
		///   Registers a new command binding.
		/// </summary>
		/// <param name="binding">The binding that should be registered.</param>
		private void Register(RequestBinding binding)
		{
			_bindings.Add(binding);
		}

		/// <summary>
		///   Invokes all commands for which the binding's trigger has been triggered.
		/// </summary>
		public void InvokeTriggeredBindings()
		{
			foreach (var binding in _bindings)
				binding.ExecuteIfTriggered();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Commands.Bind.Invoked -= OnBindCommand;
		}

		/// <summary>
		///   Invoked when the bind command is used.
		/// </summary>
		/// <param name="trigger">The trigger that should be bound.</param>
		/// <param name="command">The command invocation that should be bound.</param>
		private void OnBindCommand(InputTrigger trigger, string command)
		{
			if (String.IsNullOrWhiteSpace(command))
			{
				Log.Error("The second parameter of the bind command must not be empty.");
				return;
			}

			var reply = new RequestParser().Parse(command);
			if (reply.Status == ReplyStatus.Success)
			{
				var input = new LogicalInput(trigger, InputModes.Debug | InputModes.Menu | InputModes.Game);
				_device.Register(input);
				Register(new RequestBinding(input, reply.Result));
			}
			else
				Log.Error("Error while parsing the second parameter of the bind command: {0}", reply.Errors.ErrorMessage);
		}
	}
}