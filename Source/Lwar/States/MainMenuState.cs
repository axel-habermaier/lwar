namespace Lwar.States
{
	using System;
	using Pegasus.Framework;
	using UserInterface;

	/// <summary>
	///     Represents the main menu that is shown when no game is active.
	/// </summary>
	public class MainMenuState : State
	{
		private readonly MainMenuView _view = new MainMenuView();

		/// <summary>
		///     Invoked when the state has been entered.
		/// </summary>
		public override void OnEntered()
		{
			Application.Current.Window.LayoutRoot.Children.Add(_view);
		}

		/// <summary>
		///     Invoked when the state has been left.
		/// </summary>
		public override void OnLeft()
		{
			Application.Current.Window.LayoutRoot.Children.Remove(_view);
		}
	}
}