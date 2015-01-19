namespace Lwar.Network.Server
{
	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Pegasus.Platform.Network;
	using Pegasus.UserInterface;
	using Pegasus.Utilities;
	using Scripting;

	/// <summary>
	///     A base class for server implementations.
	/// </summary>
	internal abstract class LwarServer : DisposableObject
	{
		/// <summary>
		///     The currently running server instance.
		/// </summary>
		private static LwarServer _server;

		/// <summary>
		///     Allows the cancellation of the server task.
		/// </summary>
		private readonly CancellationTokenSource _cancellation;

		/// <summary>
		///     Periodically sends server discovery messages.
		/// </summary>
		private readonly ServerDiscovery _serverDiscovery;

		/// <summary>
		///     The step timer that is used to update the server at a fixed rate.
		/// </summary>
		private readonly StepTimer _timer = new StepTimer { IsFixedTimeStep = true };

		/// <summary>
		///     The task that executes the server.
		/// </summary>
		private Task _task;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="serverName">The name of the server that is displayed in the Join screen.</param>
		/// <param name="port">The port that the server should be used to listen for connecting clients.</param>
		/// <param name="updateRate">The server update rate in frames per second.</param>
		protected LwarServer(string serverName, ushort port, float updateRate = 1 / 60.0f)
		{
			_timer.TargetElapsedSeconds = updateRate;
			_timer.UpdateRequired += () => Update(_timer.ElapsedSeconds);
			_serverDiscovery = new ServerDiscovery(serverName, port);
			_cancellation = new CancellationTokenSource();
		}

		/// <summary>
		///     Tries to start a native or a debug server.
		/// </summary>
		/// <param name="serverName">The name of the server that is displayed in the Join screen.</param>
		/// <param name="port">The port that the server should be used to listen for connecting clients.</param>
		public static bool TryStart(string serverName, ushort port)
		{
			Stop();

			try
			{
				if (Cvars.UseNativeServer)
					_server = new CSharpServer(serverName, port);
				else
					_server = new NativeServer(serverName, port);

				var token = _server._cancellation.Token;
				_server._task = Task.Run(() =>
				{
					while (!token.IsCancellationRequested)
					{
						_server._timer.Update();
						Thread.Sleep(1);
					}
				}, token);

				return true;
			}
			catch (NetworkException e)
			{
				Log.Error("Unable to start the server: {0}", e.Message);
				MessageBox.Show("Server Failure", String.Format("Unable to start the server: {0}", e.Message));

				return false;
			}
		}

		/// <summary>
		///     Stops the currently running server, if any.
		/// </summary>
		public static void Stop()
		{
			if (_server == null)
				return;

			try
			{
				_server._cancellation.Cancel();
				_server._task.Wait();
			}
			catch (AggregateException e)
			{
				try
				{
					e.Handle(inner => inner is OperationCanceledException);
				}
				catch (AggregateException aggregateException)
				{
					throw new InvalidOperationException(String.Join("\n", aggregateException.InnerExceptions.Select(ex => ex.Message)));
				}
			}
			finally
			{
				_server._serverDiscovery.SafeDispose();
				_server.SafeDispose();
				_server = null;
			}
		}

		/// <summary>
		///     Checks whether any server errors occurred. If so, stops the server and raises an exception.
		/// </summary>
		public static void CheckForErrors()
		{
			if (_server == null)
				return;

			if (_server._task != null && _server._task.IsFaulted)
				Stop();
		}

		/// <summary>
		///     Updates the server.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		protected virtual void Update(double elapsedSeconds)
		{
			_serverDiscovery.SendDiscoveryMessage(elapsedSeconds);
		}
	}
}