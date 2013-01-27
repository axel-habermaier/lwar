using System;

namespace Client.Gameplay
{
	using System.Collections.Generic;
	using Pegasus;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Processes;
	using Pegasus.Framework.Rendering;
	using Pegasus.Gameplay;

	/// <summary>
	///   Represents a game session.
	/// </summary>
	public class GameSession : DisposableObject
	{
		/// <summary>
		///   Gets or sets the window that displays the game session.
		/// </summary>
		public Window Window { get; set; }

		/// <summary>
		///   Gets or sets the logical input device that provides all the user input to the game session.
		/// </summary>
		public LogicalInputDevice InputDevice { get; set; }

		/// <summary>
		///   Gets or sets the graphics device that is used to draw the game session.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; set; }

		/// <summary>
		///   Gets the sprite batch that is used to draw all sprites.
		/// </summary>
		public SpriteBatch SpriteBatch { get; private set; }

		/// <summary>
		///   Gets the process scheduler that schedules all processes of the game session.
		/// </summary>
		public ProcessScheduler Scheduler { get; private set; }

		/// <summary>
		///   Gets the input manager that is used to handle all user input of the game session.
		/// </summary>
		public InputManager InputManager { get; private set; }

		/// <summary>
		///   Gets the entities of the game session.
		/// </summary>
		public EntityList Entities { get; private set; }

		/// <summary>
		///   Gets or sets the list of players that participate in the game session.
		/// </summary>
		public List<Player> Players { get; set; }

		/// <summary>
		///   Initializes the game session.
		/// </summary>
		public void Initialize()
		{
			Assert.NotNull(Window, "No window has been set.");
			Assert.NotNull(InputDevice, "No input device has been set.");
			Assert.NotNull(GraphicsDevice, "No graphics device has been set.");
			Assert.NotNull(Players, "No player list has been set.");

			SpriteBatch = new SpriteBatch(GraphicsDevice);
			Scheduler = new ProcessScheduler();
			Entities = new EntityList(this);
			InputManager = new InputManager(this);

			Window.Resized += UpdateProjectionMatrix;
			UpdateProjectionMatrix(Window.Size);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			InputManager.SafeDispose();
			Entities.SafeDispose();
			SpriteBatch.SafeDispose();
			Scheduler.SafeDispose();

			if (Window != null)
				Window.Resized -= UpdateProjectionMatrix;
		}

		/// <summary>
		///   Updates the state of the session.
		/// </summary>
		public void Update()
		{
			InputManager.Update();

			Entities.Update();
			Scheduler.RunProcesses();
		}

		/// <summary>
		///   Draws the session state to the screen.
		/// </summary>
		public void Draw()
		{
			Entities.Draw();
			InputManager.Draw();

			SpriteBatch.DrawBatch();
		}

		/// <summary>
		///   Updates the projection matrix such that one pixel corresponds to one floating point unit.
		/// </summary>
		/// <param name="windowSize">The new size of the window.</param>
		private void UpdateProjectionMatrix(Size windowSize)
		{
			SpriteBatch.ProjectionMatrix = Matrix.OrthographicProjection(0, windowSize.Width, windowSize.Height, 0, 0, 1);
		}
	}
}