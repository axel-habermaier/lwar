namespace Lwar.Gameplay
{
	using System;
	using System.Collections.Generic;
	using Network;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Platform;
	using Pegasus.Platform.Logging;
	using Pegasus.Platform.Memory;
	using Scripting;

	/// <summary>
	///     Stores event messages such as 'X killed Y' or received chat messages. Messages are removed from the list
	///     automatically.
	/// </summary>
	public class EventMessageList : DisposableObject
	{
		/// <summary>
		///     The maximum number of event messages that can be displayed simultaneously.
		/// </summary>
		private const int MaxMessageCount = 16;

		/// <summary>
		///     The game session that generates the event messages.
		/// </summary>
		private readonly GameSession _gameSession;

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
		public EventMessageList(GameSession gameSession)
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
		///     Enables or disables the event messages. When disabled, added event messages are ignored.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		///     Adds a chat message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that sent the chat message.</param>
		/// <param name="chatMessage">The chat message that has been sent.</param>
		public void AddChatMessage(Identifier player, string chatMessage)
		{
			Assert.ArgumentNotNullOrWhitespace(chatMessage);

			var message = EventMessage.Create(EventType.Chat, chatMessage);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///     Adds a player joined message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that has joined the session.</param>
		public void AddJoinMessage(Identifier player)
		{
			var message = EventMessage.Create(EventType.Join);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///     Adds a player left message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that has left the session.</param>
		public void AddLeaveMessage(Identifier player)
		{
			var message = EventMessage.Create(EventType.Leave);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///     Adds a player kicked message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that has been kicked from the session.</param>
		/// <param name="reason">The reason for the kick.</param>
		public void AddKickedMessage(Identifier player, string reason)
		{
			var message = EventMessage.Create(EventType.Kicked, reason);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///     Adds a player timed out message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that has timed out.</param>
		public void AddTimeoutMessage(Identifier player)
		{
			var message = EventMessage.Create(EventType.Timeout);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///     Adds a kill message to the event list.
		/// </summary>
		/// <param name="killer">The identifier of the player that has scored the kill.</param>
		/// <param name="victim">The identifier of the player that has been killed.</param>
		public void AddKillMessage(Identifier killer, Identifier victim)
		{
			EventType type;
			if (killer.Identity == victim.Identity)
				type = EventType.Suicide;
			else if (killer == Specification.ServerPlayerIdentifier)
				type = EventType.EnvironmentKill;
			else
				type = EventType.Kill;

			var message = EventMessage.Create(type);
			if (TryGetPlayer(killer, out message.Player) && TryGetPlayer(victim, out message.Vicitim))
				Add(message);
		}

		/// <summary>
		///     Adds a player name change message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that has changed the name.</param>
		/// <param name="name">The new player name.</param>
		public void AddNameChangeMessage(Identifier player, string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);

			var message = EventMessage.Create(EventType.Name, name);
			if (TryGetPlayer(player, out message.Player))
			{
				if (!String.IsNullOrWhiteSpace(message.Player.Name) && message.Player.Name != name)
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

				RemoveAt(i);
				--i;
			}
		}

		/// <summary>
		///     Tries to get the player instance for the player with the given identifier. Returns false if the player could not be
		///     found.
		/// </summary>
		/// <param name="playerId">The identifier of the player instance that should be returned.</param>
		/// <param name="player">Returns the player instance, if found.</param>
		private bool TryGetPlayer(Identifier playerId, out Player player)
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
			if (!Enabled)
				return;

			message.CreationTime = _clock.Seconds;

			if (_messages.Count == MaxMessageCount)
				RemoveAt(0);

			message.GenerateDisplayString();
			_messages.Add(message);

			Log.Info("{0}", message.DisplayString);
		}

		/// <summary>
		///     Removes the event message at the given index.
		/// </summary>
		/// <param name="index">The zero-based index of the event message that should be removed.</param>
		private void RemoveAt(int index)
		{
			_messages[index].SafeDispose();
			_messages.RemoveAt(index);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_messages.SafeDisposeAll();
		}
	}
}