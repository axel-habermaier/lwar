namespace Pegasus.UserInterface.Controls
{
	using System;

	/// <summary>
	///     Determines the resolution source of a render output panel.
	/// </summary>
	public enum ResolutionSource
	{
		/// <summary>
		///     Indicates that the resolution of the render output always matches the actual size of the render output panel.
		/// </summary>
		Layout,

		/// <summary>
		///     Indicates that the render output panel uses the resolution stored in the application's resolution cvar.
		/// </summary>
		Application,

		/// <summary>
		///     Indicates that the render output panel uses the resolution set in its resolution dependency property.
		/// </summary>
		Explicit
	}
}