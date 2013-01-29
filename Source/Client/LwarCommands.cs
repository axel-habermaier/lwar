﻿using System;

namespace Lwar.Client
{
	using System.Net;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Provides access to lwar-specific commands.
	/// </summary>
	public class LwarCommands
	{
		/// <summary>
		///   Starts up a new server instance.
		/// </summary>
		public static readonly Command StartServer = new Command("start", "Starts up a new server instance.");

		/// <summary>
		///   Shuts down the currently running server.
		/// </summary>
		public static readonly Command StopServer = new Command("stop", "Shuts down the currently running server.");

		/// <summary>
		///   Connects to a game session on a server.
		/// </summary>
		public static readonly Command<IPEndPoint> Connect = new Command<IPEndPoint>("connect",
																					 "Connects to a game session on a server.");

		/// <summary>
		///   Disconnects from the current game session.
		/// </summary>
		public static readonly Command Disconnect = new Command("disconnect", "Disconnects from the current game session.");
	}
}