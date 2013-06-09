using System;

namespace Lwar.Client.Gameplay
{
	/// <summary>
	///   Represents an event message. Valid fields depend on the type of the events.
	/// </summary>
	public struct EventMessage
	{
		/// <summary>
		///   The creation time of the message.
		/// </summary>
		public double CreationTime;

		/// <summary>
		///   The chat message, new player name, or kick reason.
		/// </summary>
		public string Message;

		/// <summary>
		///   The player that scored the killed, joined, left, was kicked, timed out, was renamed, or sent the chat message.
		/// </summary>
		public Player Player;

		/// <summary>
		///   The player that has been killed or committed suicide.
		/// </summary>
		public Player Vicitim;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="type">The type of the event message.</param>
		public EventMessage(EventType type)
			: this()
		{
			Type = type;
		}

		/// <summary>
		///   The type of the event message.
		/// </summary>
		public EventType Type { get; private set; }

		/// <summary>
		///   The display string of the event message.
		/// </summary>
		public string DisplayString { get; private set; }

		/// <summary>
		///   Generates the display string for the event message.
		/// </summary>
		public void GenerateDisplayString()
		{
			switch (Type)
			{
				case EventType.Chat:
					DisplayString = String.Format("{0}\\white: \\yellow{1}", Player.Name, Message);
					break;
				case EventType.Join:
					DisplayString = String.Format("{0} has joined the game.", Player.Name);
					break;
				case EventType.Kicked:
					if (String.IsNullOrWhiteSpace(Message))
						DisplayString = String.Format("{0} has been kicked.", Player.Name);
					else
						DisplayString = String.Format("{0} has been kicked: {1}.", Player.Name, Message);
					break;
				case EventType.Leave:
					DisplayString = String.Format("{0} has left the game.", Player.Name);
					break;
				case EventType.Suicide:
					DisplayString = String.Format("{0} got himself killed.", Vicitim.Name);
					break;
				case EventType.EnvironmentKill:
					DisplayString = String.Format("{0} was not afraid of environmental hazards. Then he died.", Vicitim.Name);
					break;
				case EventType.Timeout:
					DisplayString = String.Format("The connection to {0} has been lost.", Player.Name);
					break;
				case EventType.Name:
					DisplayString = String.Format("{0} was renamed to {1}.", Player.Name, Message);
					break;
				case EventType.Kill:
					DisplayString = String.Format("{0} killed {1}.", Player.Name, Vicitim.Name);
					break;
				default:
					throw new InvalidOperationException("Unsupported event message type.");
			}
		}
	}
}