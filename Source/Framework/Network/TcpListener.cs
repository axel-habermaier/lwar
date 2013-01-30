using System;

namespace Pegasus.Framework.Network
{
	using System.Net;
	using System.Net.Sockets;
	using System.Threading.Tasks;
	using Processes;

	/// <summary>
	///   Listens for incoming Tcp connections.
	/// </summary>
	internal class TcpListener
	{
		/// <summary>
		///   The local endpoint that should be used to listen for the new connections.
		/// </summary>
		private readonly IPEndPoint _localEndPoint;

		/// <summary>
		///   Used to create incoming and outgoing packets.
		/// </summary>
		private readonly IPacketFactory _packetFactory;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="packetFactory">The packet factory that should be used to create incoming and outgoing packets.</param>
		/// <param name="localEndPoint">The local endpoint that should be used to listen for the new connections.</param>
		public TcpListener(IPacketFactory packetFactory, IPEndPoint localEndPoint)
		{
			Assert.ArgumentNotNull(packetFactory, () => packetFactory);
			Assert.ArgumentNotNull(localEndPoint, () => localEndPoint);

			_packetFactory = packetFactory;
			_localEndPoint = localEndPoint;
			RetryCount = 3;
		}

		/// <summary>
		///   Gets or sets the retry count that determines the number of times the listener tries to reinitialize itself after
		///   a network-related error before raising the Faulted event and giving up.
		/// </summary>
		public int RetryCount { get; set; }

		/// <summary>
		///   Gets a value indicating whether the listener is faulted and can no longer accept new incoming connections.
		/// </summary>
		public bool IsFaulted { get; private set; }

		/// <summary>
		///   Raised when a new connection has been accepted.
		/// </summary>
		public event Action<TcpSocket> Connected;

		/// <summary>
		///   Starts to listen for new incoming connections. After a new connection has been accepted, the Connected event is
		///   raised. In case of an error, several attempts are made to start listening again before giving up and entering the
		///   faulted state.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		public async Task ListenAsync(ProcessContext context)
		{
			var retryCount = RetryCount;
			try
			{
				do
				{
					try
					{
						using (var socket = TcpSocket.Create())
						{
							socket.Bind(_localEndPoint);
							socket.Listen(Int32.MaxValue);
							NetworkLog.ServerDebug("Ready to accept client connections on {0}.", _localEndPoint);

							while (!context.IsCanceled)
							{
								var acceptedSocket = await socket.AcceptAsync(context);
								NetworkLog.ServerDebug("Accepted connection from {0}.", acceptedSocket.RemoteEndPoint);

								retryCount = RetryCount;
								if (Connected != null)
									Connected(new TcpSocket(_packetFactory, acceptedSocket));
								else
									acceptedSocket.SafeDispose();
							}
						}
					}
					catch (SocketException e)
					{
						NetworkLog.ServerError("Failed to listen on {0}: {1}.", _localEndPoint, e.Message);
						--retryCount;
					}
				} while (retryCount > 0 && !context.IsCanceled);
			}
			finally
			{
				if (retryCount == 0)
					IsFaulted = true;
			}
		}
	}
}