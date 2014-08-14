namespace Lwar.UserInterface.Views
{
	using System;
	using Pegasus.Math;

	partial class InGameMenuView
	{
		/// <summary>
		///     Invoked once the UI element and all of its children have been fully loaded.
		/// </summary>
		partial void OnLoaded()
		{
			Focus();
		}

		/// <summary>
		///     Performs a detailed hit test for the given position. The position is guaranteed to lie within the UI element's
		///     bounds. This method should be overridden to implement special hit testing logic that is more precise than a
		///     simple bounding box check.
		/// </summary>
		/// <param name="position">The position that should be checked for a hit.</param>
		/// <returns>Returns true if the UI element is hit; false, otherwise.</returns>
		protected override bool HitTestCore(Vector2d position)
		{
			// The in-game menu lies above everything else.
			return true;
		}
	}
}