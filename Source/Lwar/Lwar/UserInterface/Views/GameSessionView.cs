namespace Lwar.UserInterface.Views
{
	using System;
	using Pegasus.Framework.UserInterface.Controls;

	partial class GameSessionView
	{
		/// <summary>
		///     Invoked once the UI element and all of its children have been fully loaded.
		/// </summary>
		partial void OnLoaded()
		{
			var overlayPanel = new ShipOverlayPanel();
			_layoutRoot.Add(overlayPanel);

			// Ensure everything else lies above the overlay panel
			Panel.SetZIndex(overlayPanel, -1000);
		}
	}
}