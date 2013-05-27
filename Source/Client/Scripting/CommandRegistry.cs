﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Tuesday, April 9, 2013, 15:43:09
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Lwar.Client.Scripting
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Scripting;
	using System.Net;

	public class CommandRegistry : Pegasus.Framework.Scripting.CommandRegistry
	{
		/// <summary>
		///   Provides access to the actual instances of the cvars or commands managed by the registry.
		/// </summary>
		public new InstanceList Instances { get; private set; }

		/// <summary>
		///   Starts up a new server instance.
		/// </summary>
		[DebuggerHidden]
		public void StartServer()
		{
			Instances.StartServer.Invoke();
		}

		/// <summary>
		///   Shuts down the currently running server.
		/// </summary>
		[DebuggerHidden]
		public void StopServer()
		{
			Instances.StopServer.Invoke();
		}

		/// <summary>
		///   Connects to a game session on a remote or local server.
		/// </summary>
		/// <param name="ipAddress">
		///   The IP address of the server in either IPv4 or IPv6 format. For instance, either 127.0.0.1 or ::1 can be used to
		///   connect to a local server.
		/// </param>
		/// <param name="port">The port of the server.</param>
		[DebuggerHidden]
		public void Connect(IPAddress ipAddress, ushort port = Specification.DefaultServerPort)
		{
			Assert.ArgumentNotNull((object)ipAddress);
			Assert.ArgumentNotNull((object)port);
			Instances.Connect.Invoke(ipAddress, port);
		}

		/// <summary>
		///   Disconnects from the current game session.
		/// </summary>
		[DebuggerHidden]
		public void Disconnect()
		{
			Instances.Disconnect.Invoke();
		}

		/// <summary>
		///   Sends a chat message to all peers.
		/// </summary>
		/// <param name="message">The message that should be sent.</param>
		[DebuggerHidden]
		public void Chat(string message)
		{
			Assert.ArgumentNotNull((object)message);
			Instances.Chat.Invoke(message);
		}

		/// <summary>
		///   Toggles between the game and the debugging camera.
		/// </summary>
		[DebuggerHidden]
		public void ToggleDebugCamera()
		{
			Instances.ToggleDebugCamera.Invoke();
		}

		/// <summary>
		///   Raised when the StartServer command is invoked.
		/// </summary>
		public event Action OnStartServer
		{
			add { Instances.StartServer.Invoked += value; }
			remove { Instances.StartServer.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the StopServer command is invoked.
		/// </summary>
		public event Action OnStopServer
		{
			add { Instances.StopServer.Invoked += value; }
			remove { Instances.StopServer.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the Connect command is invoked.
		/// </summary>
		public event Action<IPAddress, ushort> OnConnect
		{
			add { Instances.Connect.Invoked += value; }
			remove { Instances.Connect.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the Disconnect command is invoked.
		/// </summary>
		public event Action OnDisconnect
		{
			add { Instances.Disconnect.Invoked += value; }
			remove { Instances.Disconnect.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the Chat command is invoked.
		/// </summary>
		public event Action<string> OnChat
		{
			add { Instances.Chat.Invoked += value; }
			remove { Instances.Chat.Invoked -= value; }
		}

		/// <summary>
		///   Raised when the ToggleDebugCamera command is invoked.
		/// </summary>
		public event Action OnToggleDebugCamera
		{
			add { Instances.ToggleDebugCamera.Invoked += value; }
			remove { Instances.ToggleDebugCamera.Invoked -= value; }
		}

		/// <summary>
		///   Initializes the registry.
		/// </summary>
		protected override void Initialize(object instances)
		{
			if (instances == null)
				instances = new InstanceList();

			Instances = (InstanceList)instances;
			base.Initialize(instances);

			Register(Instances.StartServer, "start_server");
			Register(Instances.StopServer, "stop_server");
			Register(Instances.Connect, "connect");
			Register(Instances.Disconnect, "disconnect");
			Register(Instances.Chat, "chat");
			Register(Instances.ToggleDebugCamera, "toggle_debug_camera");
		}

		/// <summary>
		///   Stores the actual instances of the cvars or commands managed by the registry.
		/// </summary>
		public new class InstanceList : Pegasus.Framework.Scripting.CommandRegistry.InstanceList
		{
			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			public InstanceList()
			{
				StartServer = new Command("start_server", "Starts up a new server instance.");
				StopServer = new Command("stop_server", "Shuts down the currently running server.");
				Connect = new Command<IPAddress, ushort>("connect", "Connects to a game session on a remote or local server.", 
					new CommandParameter("ipAddress", typeof(IPAddress), false, default(IPAddress), "The IP address of the server in either IPv4 or IPv6 format. For instance, either 127.0.0.1 or ::1 can be used to connect to a local server."),
					new CommandParameter("port", typeof(ushort), true, Specification.DefaultServerPort, "The port of the server."));
				Disconnect = new Command("disconnect", "Disconnects from the current game session.");
				Chat = new Command<string>("chat", "Sends a chat message to all peers.", 
					new CommandParameter("message", typeof(string), false, default(string), "The message that should be sent."));
				ToggleDebugCamera = new Command("toggle_debug_camera", "Toggles between the game and the debugging camera.");
			}

			/// <summary>
			///   Starts up a new server instance.
			/// </summary>
			public Command StartServer { get; private set; }

			/// <summary>
			///   Shuts down the currently running server.
			/// </summary>
			public Command StopServer { get; private set; }

			/// <summary>
			///   Connects to a game session on a remote or local server.
			/// </summary>
			public Command<IPAddress, ushort> Connect { get; private set; }

			/// <summary>
			///   Disconnects from the current game session.
			/// </summary>
			public Command Disconnect { get; private set; }

			/// <summary>
			///   Sends a chat message to all peers.
			/// </summary>
			public Command<string> Chat { get; private set; }

			/// <summary>
			///   Toggles between the game and the debugging camera.
			/// </summary>
			public Command ToggleDebugCamera { get; private set; }
		}
	}
}

