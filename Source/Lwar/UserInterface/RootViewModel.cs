namespace Lwar.UserInterface
{
	using System;
	using Pegasus.Framework.UserInterface.Controls;

	/// <summary>
	///     The Lwar root view model that represents the non-visible root of the view model stack.
	/// </summary>
	public class RootViewModel : LwarViewModel<UserControl>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public RootViewModel()
		{
			Child = new MainMenuViewModel();
		}
	}
}