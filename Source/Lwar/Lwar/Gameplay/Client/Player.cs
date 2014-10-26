namespace Lwar.Gameplay.Client
{
	using System;
	using Entities;
	using Network;
	using Pegasus.Entities;
	using Pegasus.Platform.Logging;
	using Pegasus.UserInterface;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a player.
	/// </summary>
	public class Player : PooledNotifyPropertyChanged
	{
		/// <summary>
		///     The number of deaths.
		/// </summary>
		private int _deaths;

		/// <summary>
		///     A value indicating whether the name of the player is unique.
		/// </summary>
		private bool _hasUniqueName;

		/// <summary>
		///     The number of kills that the player has scored.
		/// </summary>
		private int _kills;

		/// <summary>
		///     The sequence number of the last remote update of the player.
		/// </summary>
		private uint _lastUpdateSequenceNumber;

		/// <summary>
		///     The name of the player
		/// </summary>
		private string _name;

		/// <summary>
		///     The player's ping.
		/// </summary>
		private int _ping;

		/// <summary>
		///     Gets or sets a value indicating whether the name of the player is unique.
		/// </summary>
		public bool HasUniqueName
		{
			get { return _hasUniqueName; }
			set
			{
				if (_hasUniqueName == value)
					return;

				_hasUniqueName = value;
				OnPropertyChanged("DisplayName");
			}
		}

		/// <summary>
		///     Gets or sets the name of the player.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				ChangePropertyValue(ref _name, value);
				OnPropertyChanged("DisplayName");
			}
		}

		/// <summary>
		///     Gets the display name of the player, with an optional numeric suffix to make the player name unique.
		/// </summary>
		public string DisplayName
		{
			get
			{
				if (!HasUniqueName)
					return String.Format("{0}\\\0 ({1})", Name, Identity.Identifier);

				return Name;
			}
		}

		/// <summary>
		///     Gets or sets the player's ping.
		/// </summary>
		public int Ping
		{
			get { return _ping; }
			set { ChangePropertyValue(ref _ping, value); }
		}

		/// <summary>
		///     Gets or sets the number of kills that the player has scored.
		/// </summary>
		public int Kills
		{
			get { return _kills; }
			set { ChangePropertyValue(ref _kills, value); }
		}

		/// <summary>
		///     Gets or sets the number of deaths.
		/// </summary>
		public int Deaths
		{
			get { return _deaths; }
			set { ChangePropertyValue(ref _deaths, value); }
		}

		/// <summary>
		///     Gets or sets the player's ship.
		/// </summary>
		public ShipEntity Ship { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether this player is the local one.
		/// </summary>
		public bool IsLocalPlayer { get; set; }

		/// <summary>
		///     Gets the player's identity.
		/// </summary>
		public Identity Identity { get; private set; }

		/// <summary>
		///     Checks whether the player accepts an update with the given sequence number. All following player updates are only
		///     accepted when their sequence number exceeds the given one.
		/// </summary>
		/// <param name="sequenceNumber">The sequence number that should be checked.</param>
		public bool AcceptUpdate(uint sequenceNumber)
		{
			if (_lastUpdateSequenceNumber >= sequenceNumber)
			{
				Log.Debug("Player rejected outdated update.");
				return false;
			}

			_lastUpdateSequenceNumber = sequenceNumber;
			return true;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="gameSession">The game session the instance should be created for.</param>
		/// <param name="id">The identity of the player.</param>
		/// <param name="name">The name of the player.</param>
		public static Player Create(ClientGameSession gameSession, Identity id, string name)
		{
			Assert.ArgumentNotNull(gameSession);
			Assert.ArgumentNotNullOrWhitespace(name);

			var player = gameSession.Allocate<Player>();
			player.Identity = id;
			player.Ship = null;
			player.Name = name;
			player.HasUniqueName = true;
			player._lastUpdateSequenceNumber = 0;
			return player;
		}

		/// <summary>
		///     Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return String.Format("'{0}' ({1})", DisplayName, Identity);
		}
	}
}