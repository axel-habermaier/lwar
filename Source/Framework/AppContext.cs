using System;

namespace Pegasus.Framework
{
	using System.Diagnostics;
	using Platform;
	using Platform.Graphics;
	using Platform.Input;
	using Rendering;
	using Scripting;

	/// <summary>
	///   Represents the context of an application, providing access to all framework objects that can be used by an
	///   application.
	/// </summary>
	public class AppContext : IAppContext
	{
		/// <summary>
		///   Gets or sets the name of the default font that is used to draw the console and the statistics.
		/// </summary>
		public string DefaultFontName { get; set; }

		/// <summary>
		///   Gets or sets the sprite effect adapter that is used to draw sprite batches.
		/// </summary>
		public ISpriteEffectAdapter SpriteEffect { get; set; }

		/// <summary>
		///   Gets or sets the name of the application.
		/// </summary>
		public string AppName { get; set; }

		/// <summary>
		///   Gets or sets the command registry that is used to handle application commands.
		/// </summary>
		public CommandRegistry Commands { get; set; }

		/// <summary>
		///   Gets or sets the cvar registry that is used to retrieve and store configuration values.
		/// </summary>
		public CvarRegistry Cvars { get; set; }

		/// <summary>
		///   Gets or sets the statistics instance that is used for statistical measurements.
		/// </summary>
		public Statistics Statistics { get; set; }

		/// <summary>
		///   Gets the graphics device that renders the output of the application.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; internal set; }

		/// <summary>
		///   Gets the application window the application renders to.
		/// </summary>
		public Window Window { get; internal set; }

		/// <summary>
		///   Gets the asset manager that manages the assets of the application.
		/// </summary>
		public AssetsManager Assets { get; internal set; }

		/// <summary>
		///   Gets the logical input device that handles all user input to the application..
		/// </summary>
		public LogicalInputDevice LogicalInputDevice { get; internal set; }

		/// <summary>
		///   In debug builds, validates the context's data.
		/// </summary>
		[Conditional("DEBUG")]
		internal void Validate()
		{
			Assert.That(!String.IsNullOrWhiteSpace(AppName), "The application name has not been set.");
			Assert.That(!String.IsNullOrWhiteSpace(DefaultFontName), "The default font name has not been set.");
			Assert.NotNull(Commands, "The command registry has not been set.");
			Assert.NotNull(Cvars, "The cvar registry has not been set.");
			Assert.NotNull(SpriteEffect, "The sprite effect adapter has not been set.");
			Assert.NotNull(Statistics, "The statistics instance adapter has not been set.");
		}
	}
}