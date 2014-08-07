namespace Pegasus.Framework.UserInterface.Controls
{
	using System;
	using Math;

	/// <summary>
	///     Represents a base class for controls typically defined in Xaml.
	/// </summary>
	public class UserControl : ContentControl
	{
		/// <summary>
		///     Performs a detailed hit test for the given position. The position is guaranteed to lie within the UI element's 
		///		bounds. This method should be overridden to implement special hit testing logic that is more precise than a 
		///		simple bounding box check.
		/// </summary>
		/// <param name="position">The position that should be checked for a hit.</param>
		/// <returns>Returns true if the UI element is hit; false, otherwise.</returns>
		protected override bool HitTestCore(Vector2d position)
		{
			return false;
		}
	}
}