using System;

namespace Pegasus.Framework.Network
{
	/// <summary>
	///   Provides logging helper functions for network-related information.
	/// </summary>
	public static class NetworkLog
	{
		/// <summary>
		///   Raises the Log.OnFatalError event for the given server message and terminates the application.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnFatalError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void ServerDie(string message, params object[] arguments)
		{
			Die("Server", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnFatalError event for the given client message and terminates the application.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnFatalError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void ClientDie(string message, params object[] arguments)
		{
			Die("Client", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnFatalError event for the given peer message and terminates the application.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnFatalError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void PeerDie(string message, params object[] arguments)
		{
			Die("Peer", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnError event for the given server message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void ServerError(string message, params object[] arguments)
		{
			Error("Server", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnError event for the given client message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void ClientError(string message, params object[] arguments)
		{
			Error("Client", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnError event for the given peer message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnError event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void PeerError(string message, params object[] arguments)
		{
			Error("Peer", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnWarning event for the given server message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnWarning event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void ServerWarn(string message, params object[] arguments)
		{
			Warn("Server", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnWarning event for the given client message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnWarning event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void ClientWarn(string message, params object[] arguments)
		{
			Warn("Client", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnWarning event for the given peer message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnWarning event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void PeerWarn(string message, params object[] arguments)
		{
			Warn("Peer", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnInfo event for the given server message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void ServerInfo(string message, params object[] arguments)
		{
			Info("Server", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnInfo event for the given client message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void ClientInfo(string message, params object[] arguments)
		{
			Info("Client", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnInfo event for the given peer message.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void PeerInfo(string message, params object[] arguments)
		{
			Info("Peer", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnDebugInfo event for the given server message if network debugging is enabled.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void ServerDebug(string message, params object[] arguments)
		{
			DebugInfo("Server", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnDebugInfo event for the given client message if network debugging is enabled.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void ClientDebug(string message, params object[] arguments)
		{
			DebugInfo("Client", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnDebugInfo event for the given peer message if network debugging is enabled.
		/// </summary>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		public static void PeerDebug(string message, params object[] arguments)
		{
			DebugInfo("Peer", message, arguments);
		}

		/// <summary>
		///   Raises the Log.OnDebugInfo event for the given message if network debugging is enabled.
		/// </summary>
		/// <param name="source">The source of the network message.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		private static void DebugInfo(string source, string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(source);
			Assert.ArgumentNotNullOrWhitespace(message);

			Log.DebugInfo(String.Format("({1}) {0}", message, source), arguments);
		}

		/// <summary>
		///   Raises the Log.OnInfo event for the given message.
		/// </summary>
		/// <param name="source">The source of the network message.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		private static void Info(string source, string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(source);
			Assert.ArgumentNotNullOrWhitespace(message);

			Log.Info(String.Format("({1}) {0}", message, source), arguments);
		}

		/// <summary>
		///   Raises the Log.OnWarning event for the given message.
		/// </summary>
		/// <param name="source">The source of the network message.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		private static void Warn(string source, string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(source);
			Assert.ArgumentNotNullOrWhitespace(message);

			Log.Warn(String.Format("({1}) {0}", message, source), arguments);
		}

		/// <summary>
		///   Raises the Log.OnError event for the given message.
		/// </summary>
		/// <param name="source">The source of the network message.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		private static void Error(string source, string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(source);
			Assert.ArgumentNotNullOrWhitespace(message);

			Log.Error(String.Format("({1}) {0}", message, source), arguments);
		}

		/// <summary>
		///   Raises the Log.OnFatalError event for the given message and terminates the application.
		/// </summary>
		/// <param name="source">The source of the network message.</param>
		/// <param name="message">
		///   The message that should be formatted and passed as an argument of the OnDebugInfo event.
		/// </param>
		/// <param name="arguments">The arguments that should be copied into the message.</param>
		private static void Die(string source, string message, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(source);
			Assert.ArgumentNotNullOrWhitespace(message);

			Log.Die(String.Format("({1}) {0}", message, source), arguments);
		}
	}
}