namespace Lwar
{
	using System;
	using Network;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;

	/// <summary>
	///     Sends server discovery messages.
	/// </summary>
	public class ServerDiscovery : DisposableObject
	{
		/// <summary>
		///     The number of times the socket should be recreated after a socket fault before giving up.
		/// </summary>
		private const int RetryCount = 10;

		/// <summary>
		///     A cached buffer that is used to hold the contents of the discovery messages.
		/// </summary>
		private readonly byte[] _buffer = new byte[7];

		/// <summary>
		///     The port that the server is using to communicate with its clients.
		/// </summary>
		private readonly ushort _serverPort;

		/// <summary>
		///     The number of times the socket has faulted and has been recreated.
		/// </summary>
		private int _faultCount;

		/// <summary>
		///     The number of seconds that have passed since the last discovery message has been sent.
		/// </summary>
		private double _secondsSinceLastDiscoveryMessage = Double.MaxValue;

		/// <summary>
		///     The UDP socket that is used to send server discovery messages.
		/// </summary>
		private UdpSocket _socket = new UdpSocket();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverPort">The port that the server is using to communicate with its clients.</param>
		public ServerDiscovery(ushort serverPort)
		{
			_serverPort = serverPort;
		}

		/// <summary>
		///     Periodically sends a discovery message for the server.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have passed since the last update.</param>
		public void SendDiscoveryMessage(double elapsedSeconds)
		{
			if (_socket == null)
				return;

			_secondsSinceLastDiscoveryMessage += elapsedSeconds;
			if (_secondsSinceLastDiscoveryMessage < 1 / NetworkProtocol.DiscoveryFrequency)
				return;

			try
			{
				using (var writer = BufferWriter.Create(_buffer, Endianess.Big))
				{
					writer.WriteUInt32(NetworkProtocol.AppIdentifier);
					writer.WriteByte(NetworkProtocol.Revision);
					writer.WriteUInt16(_serverPort);

					_socket.Send(_buffer, writer.Count, NetworkProtocol.MulticastGroup);
				}

				_secondsSinceLastDiscoveryMessage = 0;
			}
			catch (NetworkException e)
			{
				if (_faultCount >= RetryCount)
				{
					Log.Error("Server discovery has been disabled. {0}", e.Message);

					_socket.SafeDispose();
					_socket = null;
					return;
				}

				++_faultCount;
				_socket.SafeDispose();
				_socket = new UdpSocket();
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_socket.SafeDispose();
		}
	}
}