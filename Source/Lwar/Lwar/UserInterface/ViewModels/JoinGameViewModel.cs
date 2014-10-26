namespace Lwar.UserInterface.ViewModels
{
	using System;
	using System.Globalization;
	using System.Linq;
	using Network;
	using Pegasus.Platform;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.UserInterface;
	using Pegasus.UserInterface.ViewModels;
	using Scripting;
	using Views;

	/// <summary>
	///     Lets the user select a server to connect to.
	/// </summary>
	public class JoinGameViewModel : StackedViewModel
	{
		/// <summary>
		///     The buffer that is used to receive the multi cast data.
		/// </summary>
		private readonly byte[] _buffer = new byte[NetworkProtocol.MaxPacketSize];

		/// <summary>
		///     The socket that is used to receive discovery messages.
		/// </summary>
		private readonly UdpSocket _multicastSocket = new UdpSocket();

		/// <summary>
		///     The IP address of the server to connect to.
		/// </summary>
		private string _ipAddress;

		/// <summary>
		///     Indicates whether the IP address is invalid.
		/// </summary>
		private bool _ipAddressIsInvalid;

		/// <summary>
		///     Indicates whether the initialization of the service failed.
		/// </summary>
		private bool _isFaulted;

		/// <summary>
		///     The IP address of the server to connect to.
		/// </summary>
		private string _port;

		/// <summary>
		///     Indicates whether the port is invalid.
		/// </summary>
		private bool _portIsInvalid;

		/// <summary>
		///     Initializes the instance.
		/// </summary>
		public JoinGameViewModel()
		{
			IPAddress = "::1";
			Port = NetworkProtocol.DefaultServerPort.ToString(CultureInfo.InvariantCulture);
			View = new JoinGameView();
			DiscoveredServers = new ObservableCollection<ServerInfo>();
		}

		/// <summary>
		///     Gets the list of known servers that have been discovered.
		/// </summary>
		public ObservableCollection<ServerInfo> DiscoveredServers { get; private set; }

		/// <summary>
		///     Gets or sets the IP address of the server to connect to.
		/// </summary>
		public string IPAddress
		{
			get { return _ipAddress; }
			set
			{
				ChangePropertyValue(ref _ipAddress, value);

				IPAddress address;
				IPAddressIsInvalid = !Pegasus.Platform.Network.IPAddress.TryParse(value, out address);
			}
		}

		/// <summary>
		///     Gets or sets the IP address of the server to connect to.
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
		public bool IPAddressIsInvalid
		{
			get { return _ipAddressIsInvalid; }
			private set { ChangePropertyValue(ref _ipAddressIsInvalid, value); }
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
		///     Sets the selected server.
		/// </summary>
		public ServerInfo SelectedServer
		{
			set
			{
				if (value == null)
					return;

				Commands.Connect(value.EndPoint.Address, value.EndPoint.Port);
			}
		}

		/// <summary>
		///     Connects to the server at the given IP address and port.
		/// </summary>
		public void Connect()
		{
			if (PortIsInvalid || IPAddressIsInvalid)
				return;

			Commands.Connect(Pegasus.Platform.Network.IPAddress.Parse(IPAddress), UInt16.Parse(Port));
		}

		/// <summary>
		///     Returns to the main menu.
		/// </summary>
		public void ReturnToMainMenu()
		{
			Parent.ReplaceChild(null);
		}

		/// <summary>
		///     Activates the view model, presenting its content and view on the UI.
		/// </summary>
		protected override void OnActivated()
		{
			try
			{
				_multicastSocket.BindMulticast(NetworkProtocol.MulticastGroup);
				Log.Info("Server discovery initialized.");
			}
			catch (NetworkException e)
			{
				_isFaulted = true;
				Log.Error("Failed to initialize server discovery: {0}", e.Message);
			}
		}

		/// <summary>
		///     Updates the view model's state.
		/// </summary>
		protected override void OnUpdate()
		{
			// Remove all servers that have timed out
			for (var i = 0; i < DiscoveredServers.Count; ++i)
			{
				if (!DiscoveredServers[i].HasTimedOut)
					continue;

				DiscoveredServers.RemoveAt(i);
				--i;
			}

			if (_isFaulted)
				return;

			try
			{
				// Check for incoming discovery messages
				int size;
				IPEndPoint endPoint;
				while (_multicastSocket.TryReceive(_buffer, out endPoint, out size))
				{
					using (var reader = BufferReader.Create(_buffer, 0, size, Endianess.Big))
						HandleDiscoveryMessage(reader, endPoint);
				}
			}
			catch (NetworkException e)
			{
				_isFaulted = true;
				Log.Error("Server discovery service failure: {0}", e.Message);
			}
		}

		/// <summary>
		///     Handles the discovery message that has just been received.
		/// </summary>
		/// <param name="reader">The reader that should be used to read the contents of the discovery message.</param>
		/// <param name="endPoint">The endpoint of the sender of the discovery message.</param>
		private void HandleDiscoveryMessage(BufferReader reader, IPEndPoint endPoint)
		{
			if (!reader.CanRead(sizeof(uint) + sizeof(byte) + sizeof(ushort)))
			{
				Log.Debug("Ignored invalid discovery message from {0}.", endPoint);
				return;
			}

			var applicationIdentifier = reader.ReadUInt32();
			var revision = reader.ReadByte();

			var port = reader.ReadUInt16();
			var name = reader.ReadString(NetworkProtocol.ServerNameLength);

			if (applicationIdentifier != NetworkProtocol.AppIdentifier || revision != NetworkProtocol.Revision)
			{
				Log.Debug("Ignored invalid discovery message from {0}.", endPoint);
				return;
			}

			endPoint = new IPEndPoint(endPoint.Address, port);

			// Check if we already know this server; if not add it, otherwise update the server's discovery time
			var server = DiscoveredServers.SingleOrDefault(s => s.EndPoint == endPoint && s.Name == name);
			if (server == null)
			{
				server = new ServerInfo { EndPoint = endPoint, Name = name, DiscoveryTime = Clock.SystemTime };
				DiscoveredServers.Add(server);
			}
			else
				server.DiscoveryTime = Clock.SystemTime;
		}

		/// <summary>
		///     Deactivates the view model, removing its content and view from the UI.
		/// </summary>
		protected override void OnDeactivated()
		{
			_multicastSocket.SafeDispose();
		}

		/// <summary>
		///     Stores information about a discovered server.
		/// </summary>
		public class ServerInfo
		{
			/// <summary>
			///     Gets or sets the last time a discovery message has been received from the server.
			/// </summary>
			public double DiscoveryTime { get; set; }

			/// <summary>
			///     Gets or sets the end point of the server.
			/// </summary>
			public IPEndPoint EndPoint { get; set; }

			/// <summary>
			///     Gets or sets the name of the server.
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			///     Gets a value indicating whether the server has timed out and is presumably no longer running.
			/// </summary>
			public bool HasTimedOut
			{
				get { return (Clock.SystemTime - DiscoveryTime) > NetworkProtocol.DiscoveryTimeout; }
			}
		}
	}
}