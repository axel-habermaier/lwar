using System;

namespace Pegasus.Framework.Scripting
{
	using System.Linq;
	using Platform.Logging;

	/// <summary>
	///   Represents an interpreted instruction that invokes a command, sets a cvar, or displays the value of a cvar.
	/// </summary>
	internal struct Instruction
	{
		/// <summary>
		///   The parameter of the instruction.
		/// </summary>
		private readonly object _parameter;

		/// <summary>
		///   The target of the instruction.
		/// </summary>
		private readonly object _target;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="target">The target of the instruction.</param>
		/// <param name="parameter">The parameter of the instruction.</param>
		public Instruction(object target, object parameter)
		{
			Assert.ArgumentNotNull(target);
			Assert.That(!(target is ICommand) || (parameter is object[] && ((object[])parameter).Length == ((ICommand)target).Parameters.Count()),
						"Incorrect command parameters.");
			Assert.That(!(target is ICvar) || (parameter == null || ((ICvar)target).ValueType.IsInstanceOfType(parameter)),
						"Incorrect cvar parameters.");

			_target = target;
			_parameter = parameter;
		}

		/// <summary>
		///   Executes the instruction.
		/// </summary>
		/// <param name="executedByUser">If true, indicates that the instruction originates from the user (e.g., via the console).</param>
		public void Execute(bool executedByUser)
		{
			var command = _target as ICommand;
			var cvar = _target as ICvar;

			if (command != null)
				command.Invoke((object[])_parameter, executedByUser);

			if (cvar != null && _parameter == null)
			{
				var deferred = String.Empty;
				if (cvar.UpdateMode != UpdateMode.Immediate && cvar.HasDeferredValue)
					deferred = String.Format(", pending update: '{0}'", TypeRegistry.ToString(cvar.DeferredValue, quotedContext: true));

				Log.Info("'{0}' is '{1}', default '{2}'{3}", cvar.Name, TypeRegistry.ToString(cvar.Value, quotedContext: true),
						 TypeRegistry.ToString(cvar.DefaultValue, quotedContext: true), deferred);

				if (cvar.UpdateMode != UpdateMode.Immediate && cvar.HasDeferredValue)
					Log.Warn("{0}", cvar.UpdateMode.ToDisplayString());
			}

			if (cvar != null && _parameter != null)
				cvar.SetValue(_parameter, executedByUser);
		}

		/// <summary>
		///   Indicates whether the instruction has the given object as its target.
		/// </summary>
		/// <param name="target">The object that should be checked.</param>
		public bool HasTarget(object target)
		{
			Assert.ArgumentNotNull(target);
			return _target == target;
		}
	}
}