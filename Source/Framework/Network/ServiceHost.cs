using System;

namespace Pegasus.Framework.Network
{
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;
	using Platform;
	using Platform.Memory;
	using Processes;

	/// <summary>
	///   Hosts a service that clients can connect to in order to remotely invoke the service operations.
	/// </summary>
	/// <typeparam name="TService">The type of the service that is hosted.</typeparam>
	public abstract class ServiceHost<TService>
		where TService : class
	{
		/// <summary>
		///   The packet factory that is used to create incoming and outgoing packets.
		/// </summary>
		private static readonly IPacketFactory PacketFactory = new ServicePacketFactory();

		/// <summary>
		///   The processes that handle incoming service operation requests from clients.
		/// </summary>
		private readonly List<IProcess> _clientProcesses = new List<IProcess>();

		/// <summary>
		///   The process scheduler that the host uses to scheduler asynchronous processes.
		/// </summary>
		private readonly ProcessScheduler _scheduler = new ProcessScheduler();

		/// <summary>
		///   The identifier of the service that the service host hosts.
		/// </summary>
		private readonly ServiceIdentifier _serviceIdentifier;

		/// <summary>
		///   Indicates whether the host is currently running.
		/// </summary>
		private bool _isRunning;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="service">The instance of the service whose operations should be invoked on a client request.</param>
		/// <param name="serviceIdentifier">The identifier of the service that the service host hosts.</param>
		protected ServiceHost(TService service, ServiceIdentifier serviceIdentifier)
		{
			Assert.ArgumentNotNull(service, () => service);

			Service = service;
			_serviceIdentifier = serviceIdentifier;
		}

		/// <summary>
		///   Gets the instance of the service whose operations should be invoked on a client request.
		/// </summary>
		protected TService Service { get; private set; }

		/// <summary>
		///   Initializes the host and starts listening for new client connections as well as responding to operation invocation
		///   requests from clients. The given endpoint represents the IP address and port that clients have to use to connect to
		///   the host.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="endPoint">The endpoint on which the server should listen for new client connections.</param>
		public async Task StartAsync(ProcessContext context, IPEndPoint endPoint)
		{
			Assert.ArgumentNotNull(endPoint, () => endPoint);
			Assert.That(!_isRunning, "Already running.");

			NetworkLog.ServerInfo("Service host for service '{0}' is started.", typeof(TService).FullName);
			_isRunning = true;

			var listener = new TcpListener(PacketFactory, endPoint);
			listener.Connected += OnClientConnected;

			try
			{
				using (_scheduler.CreateProcess(listener.ListenAsync))
				{
					while (!context.IsCanceled)
					{
						if (listener.IsFaulted)
						{
							NetworkLog.ServerError("The host is no longer able to respond to new client connections.");
							break;
						}

						_scheduler.RunProcesses();
						await context.NextFrame();
					}
				}
			}
			finally
			{
				_isRunning = false;

				foreach (var clientProcess in _clientProcesses)
					clientProcess.Dispose();

				_scheduler.SafeDispose();

				NetworkLog.ServerInfo("Service host for service '{0}' has shut down.", typeof(TService).FullName);
			}
		}

		/// <summary>
		///   Invoked when a new connection has been established.
		/// </summary>
		/// <param name="connection">The new connection that has been established.</param>
		private void OnClientConnected(TcpSocket connection)
		{
			Assert.ArgumentNotNull(connection, () => connection);
			_clientProcesses.Add(_scheduler.CreateProcess(ctx => ProcessClientRequests(ctx, connection)));
		}

		/// <summary>
		///   Processes all incoming requests for the client that established the given connection.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="connection">The connection that should be used to communicate with the client.</param>
		private async Task ProcessClientRequests(ProcessContext context, TcpSocket connection)
		{
			Assert.ArgumentNotNull(connection, () => connection);

			try
			{
				while (!context.IsCanceled && _isRunning)
				{
					using (var packet = await connection.ReceiveAsync(context))
					{
						var header = MessageHeader.Read(packet);

						if (!await CheckServiceIdentifiers(context, connection, header.ServiceIdentifier))
							break;

						if (!await CheckMessageType(context, connection, header.MessageType))
							break;

						await InvokeOperation(context, connection, packet);
					}
				}
			}
			catch (SocketOperationException e)
			{
				if (connection.IsFaulted)
					NetworkLog.ServerError(e.Message);
				else
					NetworkLog.ServerInfo(e.Message);
			}
			finally
			{
				connection.SafeDispose();
			}
		}

		/// <summary>
		///   Updates the host's internal state. Must be called periodically or the host will not receive any client requests.
		/// </summary>
		public void Update()
		{
			_scheduler.RunProcesses();
		}

		/// <summary>
		///   Checks whether the request used the correct service identifier. If not, informs the client about the mismatch and
		///   returns false.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="connection">The connection that should be used to communicate with the client.</param>
		/// <param name="identifier">The service identifier sent by the client.</param>
		private async Task<bool> CheckServiceIdentifiers(ProcessContext context, TcpSocket connection,
														 ServiceIdentifier identifier)
		{
			if (identifier == _serviceIdentifier)
				return true;

			var packet = PacketFactory.CreateOutgoingPacket();
			var header = new MessageHeader(_serviceIdentifier, MessageType.ServiceIdentifierMismatch);
			header.Write(packet);

			NetworkLog.ServerWarn("Rejected request from {0} because of a service identifier mismatch.", connection.RemoteEndPoint);
			await connection.SendAsync(context, packet);
			return false;
		}

		/// <summary>
		///   Checks whether the client requested an operation invication. If not, informs the client about the invalid message
		///   type and returns false.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="connection">The connection that should be used to communicate with the client.</param>
		/// <param name="messageType">The type of the message sent by the client.</param>
		private async Task<bool> CheckMessageType(ProcessContext context, TcpSocket connection, MessageType messageType)
		{
			if (messageType == MessageType.OperationCall)
				return true;

			var packet = PacketFactory.CreateOutgoingPacket();
			var header = new MessageHeader(_serviceIdentifier, MessageType.InvalidMessageType);
			header.Write(packet);

			NetworkLog.ServerWarn("Rejected request from {0} because of the message type was invalid.", connection.RemoteEndPoint);
			await connection.SendAsync(context, packet);
			return true;
		}

		/// <summary>
		///   Executes the service operation requested by the client and sends either the result of the operation or the exception
		///   thrown by the operation back to the client.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="connection">The connection that should be used to send the result of the operation to the client.</param>
		/// <param name="packet">The packet that should be used to deserialize the operation and operation arguments.</param>
		private async Task InvokeOperation(ProcessContext context, TcpSocket connection, IncomingPacket packet)
		{
			var response = PacketFactory.CreateOutgoingPacket();
			var operation = packet.Reader.ReadByte();
			var requestId = packet.Reader.ReadUInt32();

			try
			{
				var header = new MessageHeader(_serviceIdentifier, MessageType.OperationResult);
				header.Write(response);
				response.Writer.WriteUInt32(requestId); // Send back request id so that client knows which call the result belongs to

				InvokeOperation(packet.Reader, response.Writer, operation);
			}
			catch (Exception e)
			{
				response.Writer.Reset();

				var header = new MessageHeader(_serviceIdentifier, MessageType.OperationException);
				header.Write(response);

				response.Writer.WriteUInt32(requestId); // Send back request id so that client knows which call the exception belongs to
				response.Writer.WriteString(e.GetType().FullName);
				response.Writer.WriteString(e.Message);
			}

			await connection.SendAsync(context, response);
		}

		/// <summary>
		///   Deserializes the data stored in the incoming packet, invokes the operation identified by the packet and returns the
		///   result of the operation in the outgoing packet that can subsequently be sent back to the client.
		/// </summary>
		/// <param name="incomingPacket">The packet that should be used to deserialize the operation and operation arguments.</param>
		/// <param name="outgoingPacket">The packet that should contain the result of the service operation.</param>
		/// <param name="operation">Identifies the operation that the client requested.</param>
		protected abstract void InvokeOperation(BufferReader incomingPacket, BufferWriter outgoingPacket, int operation);
	}
}