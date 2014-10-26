namespace Lwar.UserInterface.ViewModels
{
	using System;
	using Pegasus;
	using Pegasus.UserInterface.ViewModels;
	using Scripting;
	using Views;

	/// <summary>
	///     Displays the in-game menu.
	/// </summary>
	public class InGameMenuViewModel : StackedViewModel
	{
		/// <summary>
		///     Indicates whether the menu is top-level.
		/// </summary>
		private bool _isTopLevel = true;

		/// <summary>
		///     Indicates whether the mouse was captured when the in-game menu was opened.
		/// </summary>
		private bool _mouseWasCaptured = Application.Current.Window.MouseCaptured;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public InGameMenuViewModel()
		{
			View = new InGameMenuView();
			Application.Current.Window.MouseCaptured = false;
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
			Application.Current.Window.MouseCaptured = _mouseWasCaptured;
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