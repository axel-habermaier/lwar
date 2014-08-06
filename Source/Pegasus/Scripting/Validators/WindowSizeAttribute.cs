namespace Pegasus.Scripting.Validators
{
	using System;
	using Framework.UserInterface;
	using Math;
	using Platform;

	/// <summary>
	///     Ensures that the validated value lies within the acceptable bounds of a window size.
	/// </summary>
	public class WindowSizeAttribute : ValidatorAttribute
	{
		/// <summary>
		///     Gets an error message that describes a validation error.
		/// </summary>
		public override string ErrorMessage
		{
			get
			{
				return String.Format("Only resolutions between {0}x{1} and {2}x{3} are supported.",
									 NativeWindow.MinimumSize.Width, NativeWindow.MinimumSize.Height,
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
				return String.Format("must lie within {0}x{1} and {2}x{3}",
									 NativeWindow.MinimumSize.Width, NativeWindow.MinimumSize.Height,
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
			Assert.ArgumentSatisfies(value is Size, "The value is not of type 'Size'.");

			var size = (Size)value;
			return NativeWindow.MinimumSize.Width <= size.Width && NativeWindow.MinimumSize.Height <= size.Height &&
				   NativeWindow.MaximumSize.Width >= size.Width && NativeWindow.MaximumSize.Height >= size.Height;
		}
	}
}