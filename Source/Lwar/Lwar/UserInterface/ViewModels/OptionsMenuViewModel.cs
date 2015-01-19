namespace Lwar.UserInterface.ViewModels
{
	using System;
	using System.Text;
	using Network;
	using Pegasus.UserInterface.ViewModels;
	using Pegasus.Utilities;
	using Scripting;
	using Views;

	/// <summary>
	///     Displays the options menu.
	/// </summary>
	internal class OptionsMenuViewModel : StackedViewModel
	{
		/// <summary>
		///     The player name entered by the user.
		/// </summary>
		private string _playerName = Cvars.PlayerName;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public OptionsMenuViewModel()
		{
			View = new OptionsMenuView();
		}

		/// <summary>
		///     Gets the maximum allowed length of a player name. When characters are entered that require more than one UTF8 code
		///     point, the length exceeded property might be true even before max length characters are entered.
		/// </summary>
		public int MaxPlayerNameLength
		{
			get { return NetworkProtocol.PlayerNameLength; }
		}

		/// <summary>
		///     Gets or sets the player name entered by the user.
		/// </summary>
		public string PlayerName
		{
			get { return _playerName; }
			set
			{
				Assert.ArgumentNotNull(value);

				ChangePropertyValue(ref _playerName, value);
				OnPropertyChanged("PlayerNameLengthExceeded");
				OnPropertyChanged("PlayerNameMissing");
			}
		}

		/// <summary>
		///     Gets a value indicating whether the player name entered by the user exceeds the maximum allowed length.
		/// </summary>
		public bool PlayerNameLengthExceeded
		{
			get { return Encoding.UTF8.GetByteCount(PlayerName) > NetworkProtocol.PlayerNameLength; }
		}

		/// <summary>
		///     Gets a value indicating whether the player name is missing.
		/// </summary>
		public bool PlayerNameMissing
		{
			get { return String.IsNullOrWhiteSpace(PlayerName); }
		}

		/// <summary>
		///     Stores the changes and closes the menu.
		/// </summary>
		public void Ok()
		{
			if (PlayerNameLengthExceeded || PlayerNameMissing)
				return;

			Cvars.PlayerName = PlayerName;
			Parent.ReplaceChild(null);
		}

		/// <summary>
		///     Cancels all changes and closes the menu.
		/// </summary>
		public void Cancel()
		{
			Parent.ReplaceChild(null);
		}
	}
}