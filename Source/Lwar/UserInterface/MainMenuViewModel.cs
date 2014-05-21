namespace Lwar.UserInterface
{
	using System;
	using Pegasus.Platform.Network;
	using Scripting;

	/// <summary>
	///     Represents the main menu that is shown when no game is active.
	/// </summary>
	public class MainMenuViewModel : LwarViewModel<MainMenuView>
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public MainMenuViewModel()
		{
			View = new MainMenuView();
		}

		/// <summary>
		///     Activates the view model, presenting its content and view on the UI.
		/// </summary>
		protected override void OnActivated()
		{
			Commands.OnConnect += Connect;
		}

		/// <summary>
		///     Deactivates the view model, removing its content and view from the UI.
		/// </summary>
		protected override void OnDeactivated()
		{
			Commands.OnConnect -= Connect;
		}

		/// <summary>
		///     Connects to the server at the given end point and joins the game session.
		/// </summary>
		/// <param name="address">The IP address of the server.</param>
		/// <param name="port">The port of the server.</param>
		private void Connect(IPAddress address, ushort port)
		{
			Parent.Child = new InGameViewModel(new IPEndPoint(address, port));
		}
	}
}