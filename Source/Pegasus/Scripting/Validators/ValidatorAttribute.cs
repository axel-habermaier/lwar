namespace Pegasus.Scripting.Validators
{
	using System;

	/// <summary>
	///     When applied to a cvar or command parameter, performs additional validation before setting the cvar's value or
	///     invoking the command.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
	public abstract class ValidatorAttribute : Attribute
	{
		/// <summary>
		///     Gets an error message that describes a validation error.
		/// </summary>
		public abstract string ErrorMessage { get; }

		/// <summary>
		///     Gets a description of the validation performed by the validator.
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		///     Validates the given value, returning true to indicate that validation succeeded.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		public abstract bool Validate(object value);
	}
}