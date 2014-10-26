namespace Lwar.UserInterface.ViewModels
{
	using System;
	using System.Globalization;
	using Network;
	using Pegasus.Platform.Network;
	using Pegasus.UserInterface.ViewModels;
	using Scripting;
	using Views;

	/// <summary>
	///     Lets the user start a local game server.
	/// </summary>
	public class StartGameViewModel : StackedViewModel
	{
		/// <summary>
		///     The IP address of the server to connect to.
		/// </summary>
		private string _port;

		/// <summary>
		///     Indicates whether the port is invalid.
		/// </summary>
		private bool _portIsInvalid;

		/// <summary>
		///     The name of the server.
		/// </summary>
		private string _serverName;

		/// <summary>
		///     Indicates whether the name is invalid.
		/// </summary>
		private bool _serverNameIsInvalid;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public StartGameViewModel()
		{
			_serverName = "Unnamed Server";
			_port = NetworkProtocol.DefaultServerPort.ToString(CultureInfo.InvariantCulture);

			View = new StartGameView();
		}

		/// <summary>
		///     The maximum allowed server name length.
		/// </summary>
		public int MaxServerNameLength
		{
			get { return NetworkProtocol.ServerNameLength; }
		}

		/// <summary>
		///     Gets or sets the name of the server.
		/// </summary>
		public string ServerName
		{
			get { return _serverName; }
			set
			{
				ChangePropertyValue(ref _serverName, value);
				ServerNameIsInvalid = _serverName == null || _serverName.Length >= NetworkProtocol.ServerNameLength;
			}
		}

		/// <summary>
		///     Gets or sets the IP address of the server.
		/// </summary>
		public string Port
		{
			get { return _port; }
			set
			{
				ChangePropertyValue(ref _port, value);

				ushort port;
				PortIsInvalid = !UInt16.TryParse(value, out port);
			}
		}

		/// <summary>
		///     Gets a value indicating whether the IP address is invalid.
		/// </summary>
		public bool ServerNameIsInvalid
		{
			get { return _serverNameIsInvalid; }
			private set { ChangePropertyValue(ref _serverNameIsInvalid, value); }
		}

		/// <summary>
		///     Gets a value indicating whether the port is invalid.
		/// </summary>
		public bool PortIsInvalid
		{
			get { return _portIsInvalid; }
			private set { ChangePropertyValue(ref _portIsInvalid, value); }
		}

		/// <summary>
		///     Starts a local game server.
		/// </summary>
		public void StartGame()
		{
			if (ServerNameIsInvalid || PortIsInvalid)
				return;

			var port = UInt16.Parse(_port);
			if (Server.TryStart(_serverName, port))
				Commands.Connect(IPAddress.LocalHost, port);
		}

		/// <summary>
		///     Returns to the main menu.
		/// </summary>
		public void ReturnToMainMenu()
		{
			Parent.ReplaceChild(null);
		}
	}
}