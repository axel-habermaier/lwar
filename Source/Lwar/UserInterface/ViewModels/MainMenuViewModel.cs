namespace Lwar.UserInterface.ViewModels
{
	using System;
	using Pegasus.Framework.UserInterface.ViewModels;
	using Pegasus.Platform.Network;
	using Scripting;
	using Views;

	/// <summary>
	///     Represents the main menu that is shown when no game is active.
	/// </summary>
	public class MainMenuViewModel : StackedViewModel
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public MainMenuViewModel()
		{
			View = new MainMenuView();
		}

		/// <summary>
		///     Starts a single player game.
		/// </summary>
		public void PlaySingle()
		{
			Commands.StartServer();
			Commands.Connect(IPAddress.LocalHost);
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