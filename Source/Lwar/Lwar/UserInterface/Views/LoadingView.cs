namespace Lwar.UserInterface.Views
{
	using System;

	partial class LoadingView
	{
		/// <summary>
		///     Ensures that the keyboard focus is set to the chat message input text box whenever the view becomes visible.
		/// </summary>
		partial void OnLoaded()
		{
			Focus();
		}
	}
}