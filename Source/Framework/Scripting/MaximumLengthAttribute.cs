using System;

namespace Pegasus.Framework.Scripting
{
	/// <summary>
	///   Ensures that the validated string has a length less than or equal to the maximum allowed length.
	/// </summary>
	public class MaximumLengthAttribute : ValidatorAttribute
	{
		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="maximum">The maximum allowed length of the string value.</param>
		public MaximumLengthAttribute(int maximum)
		{
			Maximum = maximum;
		}

		/// <summary>
		///   Gets a description for the validation performed by the validator.
		/// </summary>
		public override string Description
		{
			get { return String.Format("The given string exceeds the maximum allowed length of {0} characters.", Maximum); }
		}

		/// <summary>
		///   Gets the maximum allowed length of the string value.
		/// </summary>
		public int Maximum { get; private set; }

		/// <summary>
		///   Validates the given value, returning true to indicate that validation succeeded.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		public override bool Validate(object value)
		{
			Assert.ArgumentNotNull(value, () => value);
			Assert.ArgumentSatisfies(value is string, () => value, "The value must be a string.");

			var length = ((string)value).Length;
			return length <= Maximum;
		}
	}
}