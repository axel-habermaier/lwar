using System;

namespace Lwar.Client.Gameplay
{
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Platform.Memory;
	using Scripting;

	/// <summary>
	///   Stores event messages such as 'X killed Y' or received chat messages. Messages are removed from the list
	///   automatically.
	/// </summary>
	public class EventMessageList : DisposableObject
	{
		/// <summary>
		///   The list capacity determines the maximum number of stored event messages.
		/// </summary>
		public const int Capacity = 16;

		/// <summary>
		///   The clock that is used to determine when messages should be removed from the list.
		/// </summary>
		private readonly Clock _clock = Clock.Create();

		/// <summary>
		///   The game session that generates the event messages.
		/// </summary>
		private readonly GameSession _gameSession;

		/// <summary>
		///   The event messages. The array is always compacted such that is has no holes. New messages are added at the first free
		///   index. The order of the messages in the list is always preserved.
		/// </summary>
		private readonly EventMessage[] _messages = new EventMessage[Capacity];

		/// <summary>
		///   The index of the first free message slot.
		/// </summary>
		private int _index;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="gameSession">The game session that generates the event messages.</param>
		public EventMessageList(GameSession gameSession)
		{
			Assert.ArgumentNotNull(gameSession);
			_gameSession = gameSession;
		}

		/// <summary>
		///   Enables or disables the event messages. When disabled, added event messages are ignored.
		/// </summary>
		public bool Enabled { get; set; }

		/// <summary>
		///   Gets the event message stored at the given index.
		/// </summary>
		/// <param name="index">The index of the event message that should be returned.</param>
		public EventMessage this[int index]
		{
			get
			{
				Assert.ArgumentInRange(index, 0, _index - 1);
				return _messages[index];
			}
		}

		/// <summary>
		///   Gets the number of active event messages.
		/// </summary>
		public int Count
		{
			get { return _index; }
		}

		/// <summary>
		///   Adds a chat message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that sent the chat message.</param>
		/// <param name="chatMessage">The chat message that has been sent.</param>
		public void AddChatMessage(Identifier player, string chatMessage)
		{
			Assert.ArgumentNotNullOrWhitespace(chatMessage);

			var message = new EventMessage(EventType.Chat) { Message = chatMessage };
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///   Adds a player joined message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that has joined the session.</param>
		public void AddJoinMessage(Identifier player)
		{
			var message = new EventMessage(EventType.Join);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///   Adds a player left message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that has left the session.</param>
		public void AddLeaveMessage(Identifier player)
		{
			var message = new EventMessage(EventType.Leave);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///   Adds a player kicked message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that has been kicked from the session.</param>
		/// <param name="reason">The reason for the kick.</param>
		public void AddKickedMessage(Identifier player, string reason)
		{
			var message = new EventMessage(EventType.Kicked) { Message = reason };
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///   Adds a player timed out message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that has timed out.</param>
		public void AddTimeoutMessage(Identifier player)
		{
			var message = new EventMessage(EventType.Timeout);
			if (TryGetPlayer(player, out message.Player))
				Add(message);
		}

		/// <summary>
		///   Adds a kill message to the event list.
		/// </summary>
		/// <param name="killer">The identifier of the player that has scored the kill.</param>
		/// <param name="victim">The identifier of the player that has been killed.</param>
		public void AddKillMessage(Identifier killer, Identifier victim)
		{
			EventType type;
			if (killer.Identity == victim.Identity)
				type = EventType.Suicide;
			else if (killer.Identity == Specification.ServerPlayerId)
				type = EventType.EnvironmentKill;
			else
				type = EventType.Kill;

			var message = new EventMessage(type);
			if (TryGetPlayer(killer, out message.Player) && TryGetPlayer(victim, out message.Vicitim))
				Add(message);
		}

		/// <summary>
		///   Adds a player name change message to the event list.
		/// </summary>
		/// <param name="player">The identifier of the player that has changed the name.</param>
		/// <param name="name">The new player name.</param>
		public void AddNameChangeMessage(Identifier player, string name)
		{
			Assert.ArgumentNotNullOrWhitespace(name);

			var message = new EventMessage(EventType.Name) { Message = name };
			if (TryGetPlayer(player, out message.Player))
			{
				if (!String.IsNullOrWhiteSpace(message.Player.Name) && message.Player.Name != name)
					Add(message);
			}

			Assert.That(!String.IsNullOrWhiteSpace(message.Player.Name), "The player's name is unknown.");
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_clock.SafeDispose();
		}

		/// <summary>
		///   Updates the list, removing all messages that have timed out.
		/// </summary>
		public void Update()
		{
			for (var i = 0; i < _index; ++i)
			{
				var timeout = _messages[i].Type == EventType.Chat ? Cvars.ChatMessageDisplayTime : Cvars.EventMessageDisplayTime;
				var timedOut = _clock.Seconds - _messages[i].CreationTime > timeout;

				if (timedOut)
				{
					Remove(i);
					--i;
				}
			}
		}

		/// <summary>
		///   Tries to get the player instance for the player with the given identifier. Returns false if the player could not be
		///   found.
		/// </summary>
		/// <param name="playerId">The identifier of the player instance that should be returned.</param>
		/// <param name="player">Returns the player instance, if found.</param>
		private bool TryGetPlayer(Identifier playerId, out Player player)
		{
			player = _gameSession.Players[playerId];
			return player != null;
		}

		/// <summary>
		///   Adds the given event message to the list.
		/// </summary>
		/// <param name="message">The message that should be added.</param>
		private void Add(EventMessage message)
		{
			if (!Enabled)
				return;

			message.CreationTime = _clock.Seconds;

			// If the list is full, remove the oldest message, compact the list, and add the new one at the end
			if (_index == Capacity)
			{
				var oldest = FindOldest();
				Remove(oldest);
			}

			message.GenerateDisplayString();
			_messages[_index++] = message;

			Log.Info(message.DisplayString);
		}

		/// <summary>
		///   Removes the event message at the given index and compacts the list.
		/// </summary>
		/// <param name="index">The index of the message that should be removed.</param>
		private void Remove(int index)
		{
			Assert.ArgumentInRange(index, 0, Capacity - 1);

			Array.Copy(_messages, index + 1, _messages, index, _index - index - 1);
			--_index;
		}

		/// <summary>
		///   Finds the index of the oldest message.
		/// </summary>
		private int FindOldest()
		{
			var oldest = Double.MaxValue;
			var slot = -1;

			for (var i = 0; i < _index; ++i)
			{
				if (_messages[i].CreationTime < oldest)
				{
					slot = i;
					oldest = _messages[i].CreationTime;
				}
			}

			return slot;
		}
	}
}