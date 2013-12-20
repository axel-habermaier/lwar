namespace Lwar.Gameplay
{
	using System;

	/// <summary>
	///     Indicates the type of an event message.
	/// </summary>
	public enum EventType
	{
		/// <summary>
		///     Indicates that the event message describes a player killing another player.
		/// </summary>
		Kill,

		/// <summary>
		///     Indicates that the event message describes a player getting himself killed.
		/// </summary>
		Suicide,

		/// <summary>
		///     Indicates that the event message describes the death of a player due to an environmental hazard.
		/// </summary>
		EnvironmentKill,

		/// <summary>
		///     Indicates that the event message describes a player that has just joined the game session.
		/// </summary>
		Join,

		/// <summary>
		///     Indicates that the event message describes a player that has just left the game session.
		/// </summary>
		Leave,

		/// <summary>
		///     Indicates that the event message describes a player that has just been kicked from the game session.
		/// </summary>
		Kicked,

		/// <summary>
		///     Indicates that the event message describes a player that has just left the game session after a connection timeout.
		/// </summary>
		Timeout,

		/// <summary>
		///     Indicates that the event message describes a player chat message.
		/// </summary>
		Chat,

		/// <summary>
		///     Indicates that the event message describes a player name change.
		/// </summary>
		Name
	}
}