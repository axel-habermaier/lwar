using System;

namespace Lwar.Client.GameStates
{
	using Pegasus.Framework;
	using Pegasus.Framework.Platform.Logging;
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
		/// <param name="entry">The log entry that should be displayed.</param>
		/// <param name="removeState">Indicates whether the current state should be removed from the state manager.</param>
		protected void ShowMessageBox(LogEntry entry, bool removeState = false)
		{
			entry.RaiseLogEvent();
			StateManager.Add(new MessageBox(entry.Message));

			if (removeState)
				StateManager.Remove(this);
		}
	}
}