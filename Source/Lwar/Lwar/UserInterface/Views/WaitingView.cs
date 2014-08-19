namespace Lwar.UserInterface.Views
{
	using System;

	partial class WaitingView
	{
		/// <summary>
		///     Invoked once the UI element and all of its children have been fully loaded.
		/// </summary>
		partial void OnLoaded()
		{
			Focus();
		}
	}
}