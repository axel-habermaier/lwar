namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Indicates which operation should be used for blending.
	/// </summary>
	public enum BlendOperation
	{
		/// <summary>
		///     Indicates that the blend operation adds the values.
		/// </summary>
		Add,

		/// <summary>
		///     Indicates that the blend operation subtracts the values.
		/// </summary>
		Subtract,

		/// <summary>
		///     Indicates that the blend operation subtracts the first value from the second.
		/// </summary>
		ReverseSubtract,

		/// <summary>
		///     Indicates that the blend operation finds the minimum of both values.
		/// </summary>
		Minimum,

		/// <summary>
		///     Indicates that the blend operation finds the maximum of both values.
		/// </summary>
		Maximum
	}
}