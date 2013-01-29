﻿using System;

namespace Lwar.Client.Gameplay
{
	using System.Collections.Generic;
	using System.Net;
	using System.Threading.Tasks;
	using Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Processes;
	using Pegasus.Framework.Rendering;
	using Pegasus.Framework.Rendering.UserInterface;
	using Pegasus.Framework.Scripting;

	/// <summary>
	///   Represents a game session.
	/// </summary>
	public class GameSession : DisposableObject
	{
		/// <summary>
		///   The font that is used to draw the text of the loading screen.
		/// </summary>
		[Asset("Fonts/Liberation Mono 12")]
		public static Font LoadingFont;

		/// <summary>
		///   The process scheduler that schedules all draw processes of the game session.
		/// </summary>
		private readonly ProcessScheduler _drawScheduler;

		/// <summary>
		///   Manages the draw state of the game session.
		/// </summary>
		private readonly StateMachine _drawState;

		/// <summary>
		///   Manages the update state of the game session.
		/// </summary>
		private readonly StateMachine _updateState;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		public GameSession(Window window, GraphicsDevice graphicsDevice, LogicalInputDevice inputDevice)
		{
			Assert.ArgumentNotNull(window, () => window);
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentNotNull(inputDevice, () => inputDevice);

			Window = window;
			GraphicsDevice = graphicsDevice;
			InputDevice = inputDevice;
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			Scheduler = new ProcessScheduler();
			_updateState = StateMachine.Create(Scheduler);
			_drawScheduler = new ProcessScheduler();
			_drawState = StateMachine.Create(_drawScheduler);
			Window.Resized += UpdateProjectionMatrix;
			UpdateProjectionMatrix(Window.Size);

			LwarCommands.Connect.Invoked += serverEndPoint => _updateState.ChangeState(ctx => Loading(ctx, serverEndPoint));
			LwarCommands.Disconnect.Invoked += () => _updateState.ChangeState(Inactive);
			_updateState.ChangeState(Inactive);
		}

		/// <summary>
		///   Gets or sets the window that displays the game session.
		/// </summary>
		public Window Window { get; private set; }

		/// <summary>
		///   Gets the logical input device that provides all the user input to the game session.
		/// </summary>
		public LogicalInputDevice InputDevice { get; private set; }

		/// <summary>
		///   Gets the graphics device that is used to draw the game session.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///   Gets the sprite batch that is used to draw all sprites.
		/// </summary>
		public SpriteBatch SpriteBatch { get; private set; }

		/// <summary>
		///   Gets the process scheduler that schedules all update processes of the game session.
		/// </summary>
		public ProcessScheduler Scheduler { get; private set; }

		/// <summary>
		///   Gets the entities of the game session.
		/// </summary>
		public EntityList Entities { get; private set; }

		/// <summary>
		///   Gets the list of players that participate in the game session.
		/// </summary>
		public List<Player> Players { get; private set; }

		/// <summary>
		///   Gets the proxy to the server that hosts the game session.
		/// </summary>
		public ServerProxy ServerProxy { get; private set; }

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Entities.SafeDispose();
			SpriteBatch.SafeDispose();
			_drawState.SafeDispose();
			_updateState.SafeDispose();
			ServerProxy.SafeDispose();
			Scheduler.SafeDispose();
			_drawScheduler.SafeDispose();

			if (Window != null)
				Window.Resized -= UpdateProjectionMatrix;
		}

		/// <summary>
		///   Updates the state of the session.
		/// </summary>
		public void Update()
		{
			Scheduler.RunProcesses();
		}

		/// <summary>
		///   Processes the given message.
		/// </summary>
		/// <param name="message">The message that should be processed.</param>
		private void ProcessMessage(IMessage message)
		{
			Assert.ArgumentNotNull(message, () => message);
			message.Process(this);
		}

		/// <summary>
		///   Draws the session state to the screen.
		/// </summary>
		public void Draw()
		{
			_drawScheduler.RunProcesses();
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

		/// <summary>
		///   Cleans up all session-specific state.
		/// </summary>
		private void Cleanup()
		{
			Entities.SafeDispose();
			ServerProxy.SafeDispose();

			Entities = null;
			ServerProxy = null;
		}

		/// <summary>
		///   Aborts the game session and shows a 'server is full' message.
		/// </summary>
		public void ServerIsFull()
		{
			Log.Error("The server is full.");
			Commands.ShowConsole.Invoke(true);
			_updateState.ChangeStateDelayed(Inactive);
		}

		/// <summary>
		///   Active when the game session is inactive.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		private async Task Inactive(ProcessContext context)
		{
			Cleanup();
			_drawState.ChangeStateDelayed(async ctx => await ctx.NextFrame());
			await context.NextFrame();
		}

		/// <summary>
		///   Active when a connection to a server is established and the game is loaded.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		/// <param name="serverEndPoint">The end point of the server that hosts the game session.</param>
		private async Task Loading(ProcessContext context, IPEndPoint serverEndPoint)
		{
			Commands.ShowConsole.Invoke(false);
			Cleanup();

			Entities = new EntityList(this);
			ServerProxy = new ServerProxy(serverEndPoint, Scheduler);
			ServerProxy.MessageReceived += ProcessMessage;

			var label = new Label(LoadingFont)
			{
				Alignment = TextAlignment.Centered | TextAlignment.Middle,
				Text = String.Format("Connecting to {0}...", serverEndPoint)
			};

			_drawState.ChangeState(ctx => LoadingDraw(ctx, label));
			await ServerProxy.Connect(context);

			if (!ServerProxy.IsConnected)
			{
				Commands.ShowConsole.Invoke(true);
				_updateState.ChangeStateDelayed(Inactive);
				return;
			}

			label.Text = "Awaiting game state...";
			await context.Delay(1000);

			_updateState.ChangeStateDelayed(Playing);
		}

		/// <summary>
		///   Draws the loading screen.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		/// <param name="label">The label describing the current loading state.</param>
		private async Task LoadingDraw(ProcessContext context, Label label)
		{
			while (!context.IsCanceled)
			{
				label.Area = new Rectangle(Vector2i.Zero, Window.Size);
				label.Draw(SpriteBatch);
				await context.NextFrame();
			}
		}

		/// <summary>
		///   Active when the game session is running.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		private async Task Playing(ProcessContext context)
		{
			_drawState.ChangeState(PlayingDraw);

			while (!context.IsCanceled)
			{
				Entities.Update();
				await context.NextFrame();
			}
		}

		/// <summary>
		///   Draws the game session.
		/// </summary>
		/// <param name="context">The context in which the state function should be executed.</param>
		private async Task PlayingDraw(ProcessContext context)
		{
			while (!context.IsCanceled)
			{
				Entities.Draw();
				await context.NextFrame();
			}
		}
	}
}