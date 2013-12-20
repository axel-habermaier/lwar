namespace Pegasus.Platform.Graphics
{
	using System;

	/// <summary>
	///   Indicates which faces should be culled.
	/// </summary>
	public enum CullMode
	{
		/// <summary>
		///   Indicates that no faces should be culled.
		/// </summary>
		None = 1301,

		/// <summary>
		///   Indicates that front-facing faces should be culled.
		/// </summary>
		Front = 1302,

		/// <summary>
		///   Indicates that back-facing faces should be culled.
		/// </summary>
		Back = 1303
	}
}