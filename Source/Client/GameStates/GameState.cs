using System;

namespace Lwar.Client.GameStates
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Scripting;

	/// <summary>
	///   Represents a game state that shows and manages, for instance, a menu or a game session.
	/// </summary>
	public abstract class GameState : AppState
	{
		/// <summary>
		///   Gets the command registry that handles the application commands.
		/// </summary>
		protected CommandRegistry Commands
		{
			get { return (CommandRegistry)Context.Commands; }
		}

		/// <summary>
		///   Gets the cvar registry that handles the application cvars.
		/// </summary>
		protected CvarRegistry Cvars
		{
			get { return (CvarRegistry)Context.Cvars; }
		}

		/// <summary>
		///   Shows a message box with the given message, optionally removing the current state from the state manager.
		/// </summary>
		/// <param name="message">The message that should be displayed.</param>
		/// <param name="logType">The type of the message that should be logged.</param>
		/// <param name="removeState">Indicates whether the current state should be removed from the state manager.</param>
		protected void ShowMessageBox(string message, LogType logType, bool removeState = false)
		{
			new LogEntry(logType, message).RaiseLogEvent();
			StateManager.Add(new MessageBox(message));

			if (removeState)
				StateManager.Remove(this);
		}
	}
}