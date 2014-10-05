namespace Pegasus.Scripting.Validators
{
	using System;
	using Framework.UserInterface;
	using Math;

	/// <summary>
	///     Ensures that the validated value lies within the acceptable bounds of a window position.
	/// </summary>
	public class WindowPositionAttribute : ValidatorAttribute
	{
		/// <summary>
		///     Gets an error message that describes a validation error.
		/// </summary>
		public override string ErrorMessage
		{
			get
			{
				return String.Format("Only screen positions between ({0},{1}) and ({2},{3}) are supported.",
					-NativeWindow.MaximumSize.Width, -NativeWindow.MaximumSize.Height,
					NativeWindow.MaximumSize.Width, NativeWindow.MaximumSize.Height);
			}
		}

		/// <summary>
		///     Gets a description of the validation performed by the validator.
		/// </summary>
		public override string Description
		{
			get
			{
				return String.Format("must lie within ({0},{1}) and ({2},{3})",
					-NativeWindow.MaximumSize.Width, -NativeWindow.MaximumSize.Height,
					NativeWindow.MaximumSize.Width, NativeWindow.MaximumSize.Height);
			}
		}

		/// <summary>
		///     Validates the given value, returning true to indicate that validation succeeded.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		public override bool Validate(object value)
		{
			Assert.ArgumentNotNull(value);
			Assert.ArgumentSatisfies(value is Vector2, "The value is not of type 'Vector2'.");

			var position = (Vector2)value;
			return -NativeWindow.MaximumSize.Width <= position.X && -NativeWindow.MaximumSize.Height <= position.Y &&
				   NativeWindow.MaximumSize.Width >= position.X && NativeWindow.MaximumSize.Height >= position.Y;
		}
	}
}