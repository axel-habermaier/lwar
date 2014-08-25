namespace Pegasus.Scripting.Validators
{
	using System;

	/// <summary>
	///     Ensures that the validated value lies within the given bounds.
	/// </summary>
	public class RangeAttribute : ValidatorAttribute
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="lowerBound">The lower bound of the range.</param>
		/// <param name="upperBound">The upper bound of the range.</param>
		public RangeAttribute(object lowerBound, object upperBound)
		{
			Assert.ArgumentNotNull(lowerBound);
			Assert.ArgumentNotNull(upperBound);
			Assert.That(lowerBound.GetType() == upperBound.GetType(), "The types of the lower and upper bounds do not match.");
			Assert.ArgumentSatisfies(lowerBound is IComparable,
				"The types of the lower and upper bounds must implement IComparable.");

			LowerBound = (IComparable)lowerBound;
			UpperBound = (IComparable)upperBound;
		}

		/// <summary>
		///     Gets the lower bound of the range.
		/// </summary>
		public IComparable LowerBound { get; private set; }

		/// <summary>
		///     Gets the upper bound of the range.
		/// </summary>
		public IComparable UpperBound { get; private set; }

		/// <summary>
		///     Gets an error message that describes a validation error.
		/// </summary>
		public override string ErrorMessage
		{
			get
			{
				return String.Format("The given value does not lie within {0} and {1}.",
					TypeRegistry.ToString(LowerBound), TypeRegistry.ToString(UpperBound));
			}
		}

		/// <summary>
		///     Gets a description of the validation performed by the validator.
		/// </summary>
		public override string Description
		{
			get { return String.Format("must lie within {0} and {1}", TypeRegistry.ToString(LowerBound), TypeRegistry.ToString(UpperBound)); }
		}

		/// <summary>
		///     Validates the given value, returning true to indicate that validation succeeded.
		/// </summary>
		/// <param name="value">The value that should be validated.</param>
		public override bool Validate(object value)
		{
			Assert.ArgumentNotNull(value);
			Assert.ArgumentSatisfies(value.GetType() == LowerBound.GetType(), "The value does not have the same type as the bounds.");

			return LowerBound.CompareTo(value) <= 0 && UpperBound.CompareTo(value) >= 0;
		}
	}
}