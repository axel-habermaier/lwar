using System;

namespace Pegasus.Framework.Scripting.Validators
{
	using Math;
	using Platform;

	/// <summary>
	///   Ensures that the validated value lies within the acceptable bounds of a window size.
	/// </summary>
	public class WindowSizeAttribute : ValidatorAttribute
	{
		/// <summary>
		///   Gets a description for the validation performed by the validator.
		/// </summary>
		public override string Description
		{
			get
			{
				return String.Format("Only resolutions between {0}x{1} and {2}x{3} are supported.",
									 Window.MinimumSize.Width, Window.MinimumSize.Height,
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
			Assert.ArgumentSatisfies(value is Size, "The value is not of type 'Size'.");

			var size = (Size)value;
			return Window.MinimumSize.Width <= size.Width && Window.MinimumSize.Height <= size.Height &&
				   Window.MaximumSize.Width >= size.Width && Window.MaximumSize.Height >= size.Height;
		}
	}
}