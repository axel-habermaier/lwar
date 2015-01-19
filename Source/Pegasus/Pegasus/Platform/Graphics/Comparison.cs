namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Describes a comparison function that is used for depth testing, for instance.
	/// </summary>
	public enum Comparison
	{
		/// <summary>
		///     Indicates that the comparison never succeeds.
		/// </summary>
		Never,

		/// <summary>
		///     Indicates that the comparison function is the less operation.
		/// </summary>
		Less,

		/// <summary>
		///     Indicates that the comparison function is the equality operation.
		/// </summary>
		Equal,

		/// <summary>
		///     Indicates that the comparison function is the less or equal operation.
		/// </summary>
		LessEqual,

		/// <summary>
		///     Indicates that the comparison function is the greater operation.
		/// </summary>
		Greater,

		/// <summary>
		///     Indicates that the comparison function is the inequality operation.
		/// </summary>
		NotEqual,

		/// <summary>
		///     Indicates that the comparison function is the greater or equal operation.
		/// </summary>
		GreaterEqual,

		/// <summary>
		///     Indicates that the comparison always succeeds.
		/// </summary>
		Always
	}
}