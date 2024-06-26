﻿namespace Lwar.Gameplay.Client
{
	using System;
	using System.Collections.Generic;
	using Network;
	using Pegasus.Platform;
	using Pegasus.Platform.Logging;
	using Pegasus.UserInterface;
	using Pegasus.Utilities;
	using Scripting;

	/// <summary>
	///     Stores event messages such as 'X killed Y' or received chat messages. Messages are removed from the list
	///     automatically.
	/// </summary>
	internal class EventMessageList
	{
		/// <summary>
		///     The maximum number of event messages that can be displayed simultaneously.
		/// </summary>
		private const int MaxMessageCount = 16;

		/// <summary>
		///     The game session that generates the event messages.
		/// </summary>
		private readonly ClientGameSession _gameSession;

		/// <summary>
		///     The event messages.
		/// </summary>
		private readonly ObservableCollection<EventMessage> _messages = new ObservableCollection<EventMessage>();

		/// <summary>
		///     The clock that is used to determine when messages should be removed from the list.
		/// </summary>
		private Clock _clock = new Clock();

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that generates the event messages.</param>
		public EventMessageList(ClientGameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);
			_gameSession = gameSession;
		}

		/// <summary>
		///     Gets the event messages.
		/// </summary>
		public IEnumerable<EventMessage> Messages
		{
			get { return _messages; }
		}

		/// <summary>
		///     Adds a chat message to the event list.
		/// </summary>
		/// <param name="player">The identity of the player that sent the chat message.</param>
		/// <param name="chatMessage">The chat message that has been sent.</param>
		public void AddChatMessage(NetworkIdentity player, string chatMessage)
		{
			Assert.ArgumentNotNullOrWhitespace(chatMessage);

			var message = new EventMessage(EventType.Chat, chatMessage);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///     Adds a player joined message to the event list.
		/// </summary>
		/// <param name="player">The identity of the player that has joined the session.</param>
		public void AddJoinMessage(NetworkIdentity player)
		{
			if (player == NetworkProtocol.ServerPlayerIdentity)
				return;

			var message = new EventMessage(EventType.Join);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///     Adds a player left message to the event list.
		/// </summary>
		/// <param name="player">The identity of the player that has left the session.</param>
		public void AddLeaveMessage(NetworkIdentity player)
		{
			var message = new EventMessage(EventType.Leave);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///     Adds a player kicked message to the event list.
		/// </summary>
		/// <param name="player">The identity of the player that has been kicked from the session.</param>
		/// <param name="reason">The reason for the kick.</param>
		public void AddKickedMessage(NetworkIdentity player, string reason)
		{
			var message = new EventMessage(EventType.Kicked, reason);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///     Adds a player timed out message to the event list.
		/// </summary>
		/// <param name="player">The identity of the player that has timed out.</param>
		public void AddTimeoutMessage(NetworkIdentity player)
		{
			var message = new EventMessage(EventType.Timeout);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///     Adds a kill message to the event list.
		/// </summary>
		/// <param name="killer">The identity of the player that has scored the kill.</param>
		/// <param name="victim">The identity of the player that has been killed.</param>
		public void AddKillMessage(NetworkIdentity killer, NetworkIdentity victim)
		{
			EventType type;
			if (killer.Identifier == victim.Identifier)
				type = EventType.Suicide;
			else if (killer == NetworkProtocol.ServerPlayerIdentity)
				type = EventType.EnvironmentKill;
			else
				type = EventType.Kill;

			var message = new EventMessage(type);
			if (TryGetPlayer(killer, out message.Player) && TryGetPlayer(victim, out message.Vicitim))
				Add(message);
		}

		/// <summary>
		///     Adds a player name change message to the event list.
		/// </summary>
		/// <param name="player">The identity of the player that has changed the name.</param>
		/// <param name="previousName">The previous player name.</param>
		public void AddNameChangeMessage(NetworkIdentity player, string previousName)
		{
			Assert.ArgumentNotNullOrWhitespace(previousName);

			var message = new EventMessage(EventType.Name, previousName);
			if (TryGetPlayer(player, out message.Player))
			{
				if (!String.IsNullOrWhiteSpace(message.Player.Name) && message.Player.DisplayName != previousName)
					Add(message);
			}

			Assert.That(!String.IsNullOrWhiteSpace(message.Player.Name), "The player's name is unknown.");
		}

		/// <summary>
		///     Updates the list, removing all messages that have timed out.
		/// </summary>
		public void Update()
		{
			for (var i = 0; i < _messages.Count; ++i)
			{
				var timeout = _messages[i].Type == EventType.Chat ? Cvars.ChatMessageDisplayTime : Cvars.EventMessageDisplayTime;
				var timedOut = _clock.Seconds - _messages[i].CreationTime > timeout;

				if (!timedOut)
					continue;

				_messages.RemoveAt(i);
				--i;
			}
		}

		/// <summary>
		///     Clears the list, removing all messages.
		/// </summary>
		public void Clear()
		{
			_messages.Clear();
		}

		/// <summary>
		///     Tries to get the player instance for the player with the given identity. Returns false if the player could not be
		///     found.
		/// </summary>
		/// <param name="playerId">The identity of the player instance that should be returned.</param>
		/// <param name="player">Returns the player instance, if found.</param>
		private bool TryGetPlayer(NetworkIdentity playerId, out Player player)
		{
			player = _gameSession.Players[playerId];
			return player != null;
		}

		/// <summary>
		///     Adds the given event message to the list.
		/// </summary>
		/// <param name="message">The message that should be added.</param>
		private void Add(EventMessage message)
		{
			message.CreationTime = _clock.Seconds;

			if (_messages.Count == MaxMessageCount)
				_messages.RemoveAt(0);

			message.GenerateDisplayString();
			_messages.Add(message);

			Log.Info("{0}", message.DisplayString);
		}
	}
}