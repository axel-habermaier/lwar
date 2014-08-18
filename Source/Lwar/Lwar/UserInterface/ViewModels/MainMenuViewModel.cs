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
		///     Indicates whether the menu is top-level.
		/// </summary>
		private bool _isTopLevel = true;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public MainMenuViewModel()
		{
			View = new MainMenuView();
		}

		/// <summary>
		///     Gets a value indicating whether the menu is top-level.
		/// </summary>
		public bool IsTopLevel
		{
			get { return _isTopLevel; }
			private set { ChangePropertyValue(ref _isTopLevel, value); }
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

		/// <summary>
		///     Opens the options menu.
		/// </summary>
		public void Options()
		{
			ReplaceChild(new OptionsMenuViewModel());
		}

		/// <summary>
		///     Invoked when the child of the view model has been changed.
		/// </summary>
		protected override void OnChildChanged()
		{
			IsTopLevel = Child == null;
		}
	}
}