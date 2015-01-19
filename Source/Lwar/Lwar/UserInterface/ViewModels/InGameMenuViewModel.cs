namespace Lwar.UserInterface.ViewModels
{
	using System;
	using Pegasus;
	using Pegasus.UserInterface.Input;
	using Pegasus.UserInterface.ViewModels;
	using Scripting;
	using Views;

	/// <summary>
	///     Displays the in-game menu.
	/// </summary>
	internal class InGameMenuViewModel : StackedViewModel
	{
		/// <summary>
		///     Indicates whether the menu is top-level.
		/// </summary>
		private bool _isTopLevel = true;

		/// <summary>
		///     Indicates whether relative mouse mode was enabled when the in-game menu was opened.
		/// </summary>
		private bool _relativeMouseMode = Mouse.RelativeMouseMode;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public InGameMenuViewModel()
		{
			View = new InGameMenuView();
			Mouse.RelativeMouseMode = false;
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
		///     Hides the in-game menu and lets the player continue playing.
		/// </summary>
		public void Continue()
		{
			Parent.ReplaceChild(null);
			Mouse.RelativeMouseMode = _relativeMouseMode;
		}

		/// <summary>
		///     Hides the in-game menu and lets the player continue playing when the in-game menu is closed implicitly.
		/// </summary>
		public void ContinueImplicit()
		{
			if (IsTopLevel)
				Continue();
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