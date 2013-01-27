using System;

namespace Pegasus.Framework.Network
{
	using System.Net;
	using System.Net.Sockets;
	using System.Threading.Tasks;
	using Processes;

	/// <summary>
	///   Provides extension methods for awaitable socket operations.
	/// </summary>
	internal static class SocketExtensions
	{
		/// <summary>
		///   Reinitializes a socket by disposing the given socket instance if it is not null or already disposed and returning a
		///   new socket instance.
		/// </summary>
		public static Socket Reinitialize(this Socket socket)
		{
			socket.SafeDispose();
			return TcpSocket.Create();
		}

		/// <summary>
		///   Delays the execution of the process until the connect operation completes.
		/// </summary>
		/// <param name="socket">The socket that should establish the connection.</param>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="endPoint">The remote endpoint of the connection.</param>
		public static Awaiter<bool> ConnectAsync(this Socket socket, ProcessContext context, IPEndPoint endPoint)
		{
			Assert.ArgumentNotNull(socket, () => socket);
			Assert.ArgumentNotNull(endPoint, () => endPoint);

			var eventArgs = new SocketAsyncEventArgs { RemoteEndPoint = endPoint };
			return ExecuteAsync(context, eventArgs, socket.ConnectAsync, e => e.SocketError == SocketError.Success);
		}

		/// <summary>
		///   Delays the execution of the process until the accept operation completes.
		/// </summary>
		/// <param name="socket">The socket that should be used to accept new connections.</param>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		public static Awaiter<Socket> AcceptAsync(this Socket socket, ProcessContext context)
		{
			Assert.ArgumentNotNull(socket, () => socket);
			return ExecuteAsync(context, new SocketAsyncEventArgs(), socket.AcceptAsync, e => e.AcceptSocket);
		}

		/// <summary>
		///   Delays the execution of the process until data has been received.
		/// </summary>
		/// <param name="socket">The socket that should be used to receive the data.</param>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="buffer">The buffer into which the received data should be copied.</param>
		public static Awaiter<int> ReceiveAsync(this Socket socket, ProcessContext context, ArraySegment<byte> buffer)
		{
			Assert.ArgumentNotNull(socket, () => socket);

			var eventArgs = new SocketAsyncEventArgs { SocketFlags = SocketFlags.None };
			eventArgs.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);
			return ExecuteAsync(context, eventArgs, socket.ReceiveAsync, e => e.BytesTransferred);
		}

		/// <summary>
		///   Delays the execution of the process until data has been received.
		/// </summary>
		/// <param name="socket">The socket that should be used to receive the data.</param>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="buffer">The buffer into which the received data should be copied.</param>
		/// <param name="remoteEndPoint">After the method completes, contains the endpoint of the peer that sent the packet.</param>
		public static async Task<int> ReceiveFromAsync(this Socket socket, ProcessContext context, ArraySegment<byte> buffer,
													   IPEndPoint remoteEndPoint)
		{
			Assert.ArgumentNotNull(socket, () => socket);

			var eventArgs = new SocketAsyncEventArgs { SocketFlags = SocketFlags.None };
			eventArgs.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);
			await ExecuteAsync<object>(context, eventArgs, socket.ReceiveAsync, e => null);

			var endPoint = (IPEndPoint)eventArgs.RemoteEndPoint;
			remoteEndPoint.Address = endPoint.Address;
			remoteEndPoint.Port = endPoint.Port;

			Assert.That(eventArgs.Offset == buffer.Offset, "Unexpected offset.");
			return eventArgs.Count;
		}

		/// <summary>
		///   Delays the execution of the process until data has been sent.
		/// </summary>
		/// <param name="socket">The socket that should be used to send the data.</param>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="buffer">The buffer from which the sent data should be copied.</param>
		public static Awaiter<object> SendAsync(this Socket socket, ProcessContext context, ArraySegment<byte> buffer)
		{
			Assert.ArgumentNotNull(socket, () => socket);

			var eventArgs = new SocketAsyncEventArgs { SocketFlags = SocketFlags.None };
			eventArgs.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);
			return ExecuteAsync<object>(context, eventArgs, socket.SendAsync, e => null);
		}

		/// <summary>
		///   Delays the execution of the process until data has been sent.
		/// </summary>
		/// <param name="socket">The socket that should be used to send the data.</param>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="buffer">The buffer from which the sent data should be copied.</param>
		/// <param name="remoteEndPoint">The endpoint of the peer the packet should be sent to.</param>
		public static Awaiter<object> SendToAsync(this Socket socket, ProcessContext context, ArraySegment<byte> buffer,
												  IPEndPoint remoteEndPoint)
		{
			Assert.ArgumentNotNull(socket, () => socket);
			Assert.ArgumentNotNull(remoteEndPoint, () => remoteEndPoint);

			var eventArgs = new SocketAsyncEventArgs { SocketFlags = SocketFlags.None, RemoteEndPoint = remoteEndPoint };
			eventArgs.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);
			return ExecuteAsync<object>(context, eventArgs, socket.SendToAsync, e => null);
		}

		/// <summary>
		///   Executes an asynchronous socket operation.
		/// </summary>
		/// <typeparam name="TResult">The type of the operation's result.</typeparam>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="eventArgs">The event args that report the completion of the socket operation.</param>
		/// <param name="asyncOperation">
		///   A function that executes the asynchronous socket operation and returns false if the
		///   operation completed synchronously.
		/// </param>
		/// <param name="getResult">Gets the result of the operation from the event args.</param>
		private static Awaiter<TResult> ExecuteAsync<TResult>(ProcessContext context,
															  SocketAsyncEventArgs eventArgs,
															  Func<SocketAsyncEventArgs, bool> asyncOperation,
															  Func<SocketAsyncEventArgs, TResult> getResult)
		{
			Assert.ArgumentNotNull(eventArgs, () => eventArgs);
			Assert.ArgumentNotNull(asyncOperation, () => asyncOperation);
			Assert.ArgumentNotNull(getResult, () => getResult);

			eventArgs.UserToken = getResult;
			var process = SocketOperation<TResult>.Create(eventArgs);

			var willRaiseEvent = asyncOperation(eventArgs);
			if (!willRaiseEvent)
				process.OnSynchronouslyCompleted(eventArgs);

			return context.WaitFor(process);
		}
	}
}