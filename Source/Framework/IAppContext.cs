using System;

namespace Pegasus.Framework
{
	using Platform;
	using Platform.Graphics;
	using Platform.Input;

	/// <summary>
	///   Represents the context of an application, providing access to all framework objects that can be used by an
	///   application.
	/// </summary>
	public interface IAppContext
	{
		/// <summary>
		///   Gets the name of the application.
		/// </summary>
		string AppName { get; }

		/// <summary>
		///   Gets the statistics instance that should be used for statistical measurements.
		/// </summary>
		Statistics Statistics { get; }

		/// <summary>
		///   Gets the graphics device that renders the output of the application.
		/// </summary>
		GraphicsDevice GraphicsDevice { get; }

		/// <summary>
		///   Gets the application window the application renders to.
		/// </summary>
		Window Window { get; }

		/// <summary>
		///   Gets the asset manager that manages the assets of the application.
		/// </summary>
		AssetsManager Assets { get; }

		/// <summary>
		///   Gets the logical input device that handles all user input to the application..
		/// </summary>
		LogicalInputDevice LogicalInputDevice { get; }
	}
}