using System;

namespace Pegasus.Framework.Network
{
	using System.Net;
	using System.Net.Sockets;
	using System.Threading.Tasks;
	using Platform.Logging;
	using Processes;

	/// <summary>
	///   Provides extension methods for awaitable socket operations.
	/// </summary>
	internal static class SocketExtensions
	{
#if Linux
		[System.Runtime.InteropServices.DllImport("libc", EntryPoint = "setsockopt")]
		private static extern int SetSocketOption(IntPtr socket, int level, int optname, ref int optval, int optlen);
#endif

		/// <summary>
		///   Enables the socket to use both IPv6 and IPv6-mapped IPv4 addresses.
		/// </summary>
		/// <param name="socket">The socket for which dual mode should be enabled.</param>
		public static void EnableDualMode(this Socket socket)
		{
			Assert.ArgumentNotNull(socket);
			Assert.ArgumentSatisfies(socket.AddressFamily == AddressFamily.InterNetworkV6, "Not an IPv6 socket.");

#if Windows
			socket.DualMode = true;
			var success = socket.DualMode;
#elif Linux
			int value = 0;
			const int IPPROTO_IPV6 = 41;
			const int IPV6_V6ONLY = 27;
			var success = SetSocketOption(socket.Handle, IPPROTO_IPV6, IPV6_V6ONLY, ref value, sizeof(int)) == 0;
#endif

			if (!success)
				Log.Warn("UDP socket is IPv6-only; dual-stack mode could not be activated.");
		}

		/// <summary>
		///   Delays the execution of the process until the connect operation completes.
		/// </summary>
		/// <param name="socket">The socket that should establish the connection.</param>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="endPoint">The remote endpoint of the connection.</param>
		public static Awaiter<bool> ConnectAsync(this Socket socket, ProcessContext context, IPEndPoint endPoint)
		{
			Assert.ArgumentNotNull(socket);
			Assert.ArgumentNotNull(endPoint);

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
			Assert.ArgumentNotNull(socket);
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
			Assert.ArgumentNotNull(socket);

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
			Assert.ArgumentNotNull(socket);

			var eventArgs = new SocketAsyncEventArgs { SocketFlags = SocketFlags.None };
			eventArgs.SetBuffer(buffer.Array, buffer.Offset, buffer.Count);
			eventArgs.RemoteEndPoint = remoteEndPoint;
			await ExecuteAsync<object>(context, eventArgs, socket.ReceiveFromAsync, e => null);

			var endPoint = (IPEndPoint)eventArgs.RemoteEndPoint;
			remoteEndPoint.Address = endPoint.Address;
			remoteEndPoint.Port = endPoint.Port;

			Assert.That(eventArgs.Offset == buffer.Offset, "Unexpected offset.");
			return eventArgs.BytesTransferred;
		}

		/// <summary>
		///   Delays the execution of the process until data has been sent.
		/// </summary>
		/// <param name="socket">The socket that should be used to send the data.</param>
		/// <param name="context">The context of the process that waits for the asynchronous method to complete.</param>
		/// <param name="buffer">The buffer from which the sent data should be copied.</param>
		public static Awaiter<object> SendAsync(this Socket socket, ProcessContext context, ArraySegment<byte> buffer)
		{
			Assert.ArgumentNotNull(socket);

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
			Assert.ArgumentNotNull(socket);
			Assert.ArgumentNotNull(remoteEndPoint);

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
			Assert.ArgumentNotNull(eventArgs);
			Assert.ArgumentNotNull(asyncOperation);
			Assert.ArgumentNotNull(getResult);

			eventArgs.UserToken = getResult;
			var process = SocketOperation<TResult>.Create(eventArgs);

			var willRaiseEvent = asyncOperation(eventArgs);
			if (!willRaiseEvent)
				process.OnSynchronouslyCompleted(eventArgs);

			return context.WaitFor(process);
		}
	}
}