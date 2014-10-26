namespace Lwar
{
	using System;
	using Pegasus.Platform.Memory;
	using Pegasus.Utilities;

	/// <summary>
	///     A base class for server implementations.
	/// </summary>
	public abstract class Server : DisposableObject
	{
		/// <summary>
		///     Periodically sends server discovery messages.
		/// </summary>
		private readonly ServerDiscovery _serverDiscovery;

		/// <summary>
		///     The step timer that is used to update the server at a fixed rate.
		/// </summary>
		private readonly StepTimer _timer = new StepTimer { IsFixedTimeStep = true };

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="port">The port that the server should be used to listen for connecting clients.</param>
		/// <param name="updateRate">The server update rate in frames per second.</param>
		protected Server(ushort port, float updateRate = 1 / 60.0f)
		{
			_timer.TargetElapsedSeconds = updateRate;
			_timer.UpdateRequired += () => Update(_timer.ElapsedSeconds);
			_serverDiscovery = new ServerDiscovery(port);
		}

		/// <summary>
		///     Updates the server.
		/// </summary>
		public void Update()
		{
			_timer.Update();
		}

		/// <summary>
		///     Updates the server.
		/// </summary>
		/// <param name="elapsedSeconds">The number of seconds that have elapsed since the last update.</param>
		protected virtual void Update(double elapsedSeconds)
		{
			_serverDiscovery.SendDiscoveryMessage(elapsedSeconds);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_serverDiscovery.SafeDispose();
		}
	}
}