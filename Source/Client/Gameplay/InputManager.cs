using System;

namespace Client.Gameplay
{
	using Pegasus.Framework;

	/// <summary>
	///   Manages all user input to the game, for instance unit selection, context-sensitive unit commands, etc. All user input
	///   is validated and recorded as user commands that are synchronized accross the network and executed.
	/// </summary>
	public class InputManager : DisposableObject
	{
		/// <summary>
		///   The game session the input manager belongs to.
		/// </summary>
		private readonly GameSession _session;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="session">The game session the input manager belongs to.</param>
		public InputManager(GameSession session)
		{
			Assert.ArgumentNotNull(session, () => session);

			_session = session;
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
		}

		/// <summary>
		///   Updates the state of the input manager.
		/// </summary>
		public void Update()
		{
		}

		/// <summary>
		///   Draws the visual effects of the input system, for instance an indicator that a unit is selected.
		/// </summary>
		public void Draw()
		{
		}
	}
}