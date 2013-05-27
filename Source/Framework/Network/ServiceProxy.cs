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
	///   Represents a proxy to a service host that a client can use to remotely invoke service operations on the host.
	/// </summary>
	public abstract class ServiceProxy
	{
		/// <summary>
		///   The packet factory that is used to create incoming and outgoing packets.
		/// </summary>
		private static readonly IPacketFactory PacketFactory = new ServicePacketFactory();

		/// <summary>
		///   The service operation invocations for which no result has been received yet.
		/// </summary>
		private readonly Dictionary<uint, IServiceOperation> _invocations = new Dictionary<uint, IServiceOperation>();

		/// <summary>
		///   The identifier of the service that the remote host hosts.
		/// </summary>
		private readonly ServiceIdentifier _serviceIdentifier;

		/// <summary>
		///   The Tcp-based connection to the host.
		/// </summary>
		private TcpSocket _connection;

		/// <summary>
		///   The number of operation invocation requests sent by the proxy.
		/// </summary>
		private uint _invokeCount;

		/// <summary>
		///   Indicates whether the proxy is currently running.
		/// </summary>
		private bool _isRunning;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="serviceIdentifier">The identifier of the service that the service host hosts.</param>
		protected ServiceProxy(ServiceIdentifier serviceIdentifier)
		{
			_serviceIdentifier = serviceIdentifier;
		}

		/// <summary>
		///   Gets a value indicating whether the proxy is connected to the host.
		/// </summary>
		public bool IsConnected { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the proxy is faulted and can no communicate with the host.
		/// </summary>
		public bool IsFaulted { get; private set; }

		/// <summary>
		///   Initializes the proxy and starts listening for server responses once the proxy is connected.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		public async Task StartAsync(ProcessContext context)
		{
			Assert.That(!_isRunning, "Already running.");

			NetworkLog.ClientInfo("Service proxy is started.");
			_isRunning = true;

			try
			{
				while (!context.IsCanceled)
				{
					if (IsConnected)
					{
						using (var packet = await _connection.ReceiveAsync(context))
							ProcessServerResponse(packet);
					}
					else
						await context.NextFrame();
				}
			}
			catch (SocketOperationException e)
			{
				if (_connection.IsFaulted)
				{
					NetworkLog.ClientError(e.Message);
					IsFaulted = true;
				}
				else
					NetworkLog.ClientInfo(e.Message);

				IsConnected = false;
			}
			finally
			{
				_connection.SafeDispose();
				_isRunning = false;
				NetworkLog.ClientInfo("Service proxy has shut down.");
			}
		}

		/// <summary>
		///   Processes a server response, marking the successful or unsuccessful completion of a service operation.
		/// </summary>
		private void ProcessServerResponse(IncomingPacket packet)
		{
			var header = MessageHeader.Read(packet);

			if (header.MessageType == MessageType.ServiceIdentifierMismatch)
				NetworkLog.ClientError("The host is incompatible: It probably uses a newer or older version of the software.");
			else if (header.MessageType != MessageType.OperationException && header.MessageType != MessageType.OperationResult)
				NetworkLog.ClientDebug("The server sent an unexpected message type: {0}.", header.MessageType);
			else
			{
				var requestIdentifier = packet.Reader.ReadUInt32();
				IServiceOperation operation;

				if (!_invocations.TryGetValue(requestIdentifier, out operation))
					NetworkLog.ClientInfo("Received a response for a service operation after the operation has timed out.");
				else
				{
					switch (header.MessageType)
					{
						case MessageType.OperationResult:
							operation.SetResult(packet);
							break;
						case MessageType.OperationException:
							SetException(operation, packet);
							break;
						default:
							throw new InvalidOperationException("Unexpected message type.");
					}
				}
			}
		}

		/// <summary>
		///   Sets the result of a service operation that has thrown an exception.
		/// </summary>
		/// <param name="operation">The service operation for which the result should be checked.</param>
		/// <param name="packet">The packet that contains the exception type and message.</param>
		private void SetException(IServiceOperation operation, IncomingPacket packet)
		{
			var exceptionType = packet.Reader.ReadString();
			var exceptionMessage = packet.Reader.ReadString();

			try
			{
				var type = Type.GetType(exceptionType);
				operation.SetException(Activator.CreateInstance(type, new object[] { exceptionMessage }) as Exception);
			}
			catch (Exception e)
			{
				NetworkLog.ClientDebug("An exception occurred while trying to deserialize an exception of type '{0}': {1}",
									   exceptionType, e.Message);
				operation.SetException(new Exception(exceptionMessage));
			}
		}

		/// <summary>
		///   Establishes a connection to the host.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="hostEndPoint">The endpoint that the host is listening on for new connections.</param>
		public async Task ConnectAsync(ProcessContext context, IPEndPoint hostEndPoint)
		{
			Assert.ArgumentNotNull(hostEndPoint);
			Assert.That(!IsConnected, "Service proxy is already connected.");

			NetworkLog.ClientInfo("Connecting to service host at {0}.", hostEndPoint);

			_connection.SafeDispose();
			_connection = new TcpSocket(PacketFactory);
			await _connection.ConnectAsync(context, hostEndPoint);

			IsConnected = true;
			NetworkLog.ClientInfo("Established connection to service host at {0}.", hostEndPoint);
		}

		/// <summary>
		///   Invokes a service operation on the host.
		/// </summary>
		/// <typeparam name="TResult">The type of the operation's result.</typeparam>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="operationIdentifier">The unique identifier of the service operation.</param>
		/// <param name="argumentSerializer">Serializes the operation's arguments into the packet.</param>
		/// <param name="resultDeserializer">Deserializes the operation's result value from the packet.</param>
		/// <param name="timeout">
		///   The amount of time in seconds to wait for the completion of the service operation before a
		///   timeout exception should be thrown.
		/// </param>
		protected async Task<TResult> InvokeAsync<TResult>(ProcessContext context,
														   int operationIdentifier,
														   Action<BufferWriter> argumentSerializer,
														   Func<BufferReader, TResult> resultDeserializer,
														   double timeout)
		{
			Assert.ArgumentNotNull(argumentSerializer);
			Assert.ArgumentNotNull(resultDeserializer);
			Assert.ArgumentInRange(operationIdentifier, 0, Byte.MaxValue);
			Assert.That(IsConnected, "The proxy is not connected to the host.");

			var requestIdentifier = ++_invokeCount;
			var packet = PacketFactory.CreateOutgoingPacket();
			var header = new MessageHeader(_serviceIdentifier, MessageType.OperationCall);
			header.Write(packet);
			packet.Writer.WriteByte((byte)operationIdentifier);
			packet.Writer.WriteUInt32(requestIdentifier);

			var operation = ServiceOperation<TResult>.Create(resultDeserializer, timeout);
			try
			{
				_invocations.Add(_invokeCount, operation);
				argumentSerializer(packet.Writer);

				await _connection.SendAsync(context, packet);
				return await context.WaitFor(operation);
			}
			finally
			{
				_invocations.Remove(requestIdentifier);
			}
		}

		/// <summary>
		///   Invokes a service operation on the host.
		/// </summary>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="operationIdentifier">The unique identifier of the service operation.</param>
		/// <param name="argumentSerializer">Serializes the operation's arguments into the packet.</param>
		/// <param name="timeout">
		///   The amount of time in seconds to wait for the completion of the service operation before a
		///   timeout exception should be thrown.
		/// </param>
		protected async Task InvokeAsync(ProcessContext context,
										 int operationIdentifier,
										 Action<BufferWriter> argumentSerializer,
										 double timeout)
		{
			Assert.ArgumentNotNull(argumentSerializer);
			Assert.ArgumentInRange(operationIdentifier, 0, Byte.MaxValue);
			Assert.That(IsConnected, "The proxy is not connected to the host.");

			var requestIdentifier = ++_invokeCount;
			var packet = PacketFactory.CreateOutgoingPacket();
			var header = new MessageHeader(_serviceIdentifier, MessageType.OperationCall);
			header.Write(packet);
			packet.Writer.WriteByte((byte)operationIdentifier);
			packet.Writer.WriteUInt32(requestIdentifier);

			var operation = ServiceOperation.Create(timeout);
			try
			{
				_invocations.Add(_invokeCount, operation);
				argumentSerializer(packet.Writer);

				await _connection.SendAsync(context, packet);
				await context.WaitFor(operation);
			}
			finally
			{
				_invocations.Remove(requestIdentifier);
			}
		}
	}
}