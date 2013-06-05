using System;

namespace Pegasus.Framework.Scripting.Validators
{
	using Math;
	using Platform;

	/// <summary>
	///   Ensures that the validated value lies within the acceptable bounds of a window position.
	/// </summary>
	public class WindowPositionAttribute : ValidatorAttribute
	{
		/// <summary>
		///   Gets a description for the validation performed by the validator.
		/// </summary>
		public override string Description
		{
			get
			{
				return String.Format("Only screen positions between ({0},{1}) and ({2},{3}) are supported.",
									 -Window.MaximumSize.Width, -Window.MaximumSize.Height,
									 Window.MaximumSize.Width, Window.MaximumSize.Height);
			}
		}

		/// <summary>
		///   Validates the given value, returning true to indicate that validation succeeded.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		public override bool Validate(object value)
		{
			Assert.ArgumentNotNull(value);
			Assert.ArgumentSatisfies(value is Vector2i, "The value is not of type 'Vector2i'.");

			var position = (Vector2i)value;
			return -Window.MaximumSize.Width <= position.X && -Window.MaximumSize.Height <= position.Y &&
				   Window.MaximumSize.Width >= position.X && Window.MaximumSize.Height >= position.Y;
		}
	}
}