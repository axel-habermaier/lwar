namespace Lwar.Rendering
{
	using System;

	/// <summary>
	///     Defines how the camera handles the current zoom value.
	/// </summary>
	public enum ZoomMode
	{
		/// <summary>
		///     Indicates that the camera uses the default zoom mode.
		/// </summary>
		Default,

		/// <summary>
		///     Indicates that the camera uses the zoom mode for rendering the starfield. When rendering the parallax scrolling
		///     starfield, the effect of changing the zoom is less obvious to simulate distant stars.
		/// </summary>
		Starfield
	}
}