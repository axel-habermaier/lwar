namespace Pegasus.Scripting.Validators
{
	using System;
	using Platform;

	/// <summary>
	///   Ensures that the validated string is a file name.
	/// </summary>
	public class FileNameAttribute : ValidatorAttribute
	{
		/// <summary>
		///   Gets an error message that describes a validation error.
		/// </summary>
		public override string ErrorMessage
		{
			get
			{
				return "The given string is not a valid file name. It either contains a path specifier such as '/', or it contains illegal characters.";
			}
		}

		/// <summary>
		///   Gets a description of the validation performed by the validator.
		/// </summary>
		public override string Description
		{
			get { return "must be a valid file name"; }
		}

		/// <summary>
		///   Validates the given value, returning true to indicate that validation succeeded.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		public override bool Validate(object value)
		{
			Assert.ArgumentNotNull(value);
			Assert.ArgumentSatisfies(value is string, "The value must be a string.");

			var file = new AppFile("ignored", (string)value);
			return file.IsValid;
		}
	}
}