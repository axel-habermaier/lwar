namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///     Indicates which faces should be culled.
	/// </summary>
	public enum CullMode
	{
		/// <summary>
		///     Indicates that no faces should be culled.
		/// </summary>
		None,

		/// <summary>
		///     Indicates that front-facing faces should be culled.
		/// </summary>
		Front,

		/// <summary>
		///     Indicates that back-facing faces should be culled.
		/// </summary>
		Back
	}
}