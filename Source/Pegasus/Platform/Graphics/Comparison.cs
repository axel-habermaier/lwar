namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///   Describes a comparison function that is used for depth testing, for instance.
	/// </summary>
	public enum Comparison
	{
		/// <summary>
		///   Indicates that the comparison never succeeds.
		/// </summary>
		Never = 1201,

		/// <summary>
		///   Indicates that the comparison always succeeds.
		/// </summary>
		Always = 1202,

		/// <summary>
		///   Indicates that the comparison function is the less operation.
		/// </summary>
		Less = 1203,

		/// <summary>
		///   Indicates that the comparison function is the less or equal operation.
		/// </summary>
		LessEqual = 1204,

		/// <summary>
		///   Indicates that the comparison function is the greater operation.
		/// </summary>
		Greater = 1205,

		/// <summary>
		///   Indicates that the comparison function is the greater or equal operation.
		/// </summary>
		GreaterEqual = 1206,

		/// <summary>
		///   Indicates that the comparison function is the equality operation.
		/// </summary>
		Equal = 1207,

		/// <summary>
		///   Indicates that the comparison function is the inequality operation.
		/// </summary>
		NotEqual = 1208
	}
}