using System;

namespace Pegasus.Framework.Scripting.Requests
{
	/// <summary>
	///   A user request that instructs the system to set a cvar to the value provided by the user.
	/// </summary>
	internal sealed class SetCvar : CvarRequest
	{
		/// <summary>
		///   The value that should be set.
		/// </summary>
		private readonly object _value;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="cvar">The cvar that should be set.</param>
		/// <param name="value">The value that should be set.</param>
		public SetCvar(ICvar cvar, object value)
			: base(cvar)
		{
			Assert.ArgumentNotNull(value, () => value);
			_value = value;
		}

		/// <summary>
		///   Executes the user command.
		/// </summary>
		public override void Execute()
		{
			Cvar.SetValue(_value);
		}
	}
}