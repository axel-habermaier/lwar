﻿using System;

namespace Pegasus.Framework
{
	using Math;
	using Platform;
	using Platform.Graphics;
	using Platform.Input;
	using Rendering;
	using Rendering.UserInterface;
	using Scripting;

	/// <summary>
	///   Represents the application.
	/// </summary>
	public abstract class App
	{
		/// <summary>
		///   Indicates whether the application should continue to run.
		/// </summary>
		private bool _running = true;

		/// <summary>
		///   Gets the context of the application, providing access to all framework objects that can be used by the application.
		/// </summary>
		protected IAppContext Context { get; private set; }

		/// <summary>
		///   Invoked when the application should update the game state.
		/// </summary>
		protected abstract void Update();

		/// <summary>
		///   Invoked when the application should draw a frame.
		/// </summary>
		/// <param name="output">The output that the scene should be rendered to.</param>
		protected abstract void Draw(RenderOutput output);

		/// <summary>
		///   Invoked when the application should draw the user interface.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		protected abstract void DrawUserInterface(SpriteBatch spriteBatch);

		/// <summary>
		///   Exists the application.
		/// </summary>
		protected void Exit()
		{
			Log.Info("Exiting...");
			_running = false;
		}

		/// <summary>
		///   Invoked when the application is initializing.
		/// </summary>
		protected virtual void Initialize()
		{
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected virtual void Dispose()
		{
		}

		/// <summary>
		///   Runs the application. This method does not return until the application is shut down.
		/// </summary>
		/// <param name="context">
		///   The context of the application, providing access to all framework objects that can be used by the application.
		/// </param>
		/// <param name="logFile">The log file that writes all generated log entries to the file system.</param>
		internal void Run(AppContext context, LogFile logFile)
		{
			Assert.ArgumentNotNull(context, () => context);
			Assert.ArgumentNotNull(logFile, () => logFile);

			using (new NativeLibrary())
			using (context.Window = new Window())
			using (context.GraphicsDevice = new GraphicsDevice())
			using (context.Statistics)
			using (context.SpriteEffect)
			using (var swapChain = new SwapChain(context.GraphicsDevice, context.Window))
			using (context.Assets = new AssetsManager(context.GraphicsDevice))
			using (var keyboard = new Keyboard(context.Window))
			using (var mouse = new Mouse(context.Window))
			using (context.LogicalInputDevice = new LogicalInputDevice(keyboard, mouse))
			using (var bindings = new Bindings(context.LogicalInputDevice, context.Commands, context.Cvars))
			using (var camera2D = new Camera2D(context.GraphicsDevice))
			using (var sceneOutput = new RenderOutput(context.GraphicsDevice) { RenderTarget = swapChain.BackBuffer })
			using (var uiOutput = new RenderOutput(context.GraphicsDevice) { Camera = camera2D, RenderTarget = swapChain.BackBuffer })
			{
				swapChain.BackBuffer.SetName("Back Buffer");
				context.SpriteEffect.Initialize(context.GraphicsDevice, context.Assets);

				context.Commands.OnExit += Exit;
				Context = context;
				Initialize();

				var defaultFont = context.Assets.LoadFont(context.DefaultFontName);
				using (var spriteBatch = new SpriteBatch(context.GraphicsDevice, uiOutput, context.SpriteEffect))
				using (var console = new Console(context.GraphicsDevice, context.LogicalInputDevice, spriteBatch, defaultFont,
												 context.Commands, context.Cvars))
				{
					context.Statistics.Initialize(context.GraphicsDevice, spriteBatch, defaultFont);

					// Ensure that the size of the console and the statistics always matches that of the window
					console.Resize(context.Window.Size);
					context.Statistics.Resize(context.Window.Size);

					context.Window.Resized += console.Resize;
					context.Window.Resized += context.Statistics.Resize;
					context.Commands.OnReloadAssets += context.Assets.ReloadAssets;

					// Copy the recorded log history to the console and explain the usage of the console
					logFile.WriteToConsole(console);
					context.Commands.Help();

					while (_running)
					{
						// Update the input system and let the console respond to any input
						using (new Measurement(context.Statistics.UpdateInput))
						{
							// Update the keyboard and mouse state first (this ensures that WentDown returns 
							// false for all keys and buttons, etc.)
							context.LogicalInputDevice.Keyboard.Update();
							context.LogicalInputDevice.Mouse.Update();

							// Process all new input; this might set WentDown, etc. to true for some keys and buttons
							context.Window.ProcessEvents();

							// Update the logical inputs based on the new state of the input system
							context.LogicalInputDevice.Update();
							console.HandleInput();
						}

						// Check if any command bindings have been triggered
						bindings.Update();

						// Update the application logic 
						Update();

						var viewport = new Rectangle(Vector2i.Zero, context.Window.Size);
						sceneOutput.Viewport = viewport;
						uiOutput.Viewport = viewport;
						camera2D.Viewport = viewport;

						context.Statistics.Update();

						using (new Measurement(context.Statistics.GpuFrameTime))
						using (new Measurement(context.Statistics.CpuFrameTime))
						{
							// Let the application draw the current frame
							Draw(sceneOutput);
							DrawUserInterface(spriteBatch);

							// Draw the console and the statistics on top of the current frame
							console.Draw();
							context.Statistics.Draw();
						}

						// Present the current frame to the screen and write the log file, if necessary
						swapChain.Present();
						logFile.WriteToFile();
					}

					Dispose();
				}
			}
		}
	}
}