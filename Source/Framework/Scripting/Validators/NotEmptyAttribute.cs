using System;

namespace Pegasus.Framework.Scripting.Validators
{
	/// <summary>
	///   Ensures that the validated string does not consist of whitespaces only.
	/// </summary>
	public class NotEmptyAttribute : ValidatorAttribute
	{
		/// <summary>
		///   Gets a description for the validation performed by the validator.
		/// </summary>
		public override string Description
		{
			get { return "The given string cannot consist of whitespaces only."; }
		}

		/// <summary>
		///   Validates the given value, returning true to indicate that validation succeeded.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		public override bool Validate(object value)
		{
			Assert.ArgumentNotNull(value);
			Assert.ArgumentSatisfies(value is string, "The value must be a string.");

			return !String.IsNullOrWhiteSpace((string)value);
		}
	}
}