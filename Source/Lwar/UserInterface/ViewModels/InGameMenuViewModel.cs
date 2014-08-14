namespace Lwar.UserInterface.ViewModels
{
	using System;
	using Pegasus.Framework.UserInterface.ViewModels;
	using Scripting;
	using Views;

	/// <summary>
	///     Displays the in-game menu.
	/// </summary>
	public class InGameMenuViewModel : StackedViewModel
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public InGameMenuViewModel()
		{
			View = new InGameMenuView();
		}

		/// <summary>
		///     Hides the in-game menu and lets the player continue playing.
		/// </summary>
		public void Continue()
		{
			Parent.ReplaceChild(null);
		}

		/// <summary>
		///     Leaves the game session.
		/// </summary>
		public void Leave()
		{
			// TODO: Do you really...
			Commands.Disconnect();
		}

		/// <summary>
		///     Exists the game.
		/// </summary>
		public void Exit()
		{
			Commands.Exit();
		}
	}
}