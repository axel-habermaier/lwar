using System;

namespace Pegasus.Framework.Scripting
{
	using Platform;

	/// <summary>
	///   Declares the cvars required by the framework.
	/// </summary>
	public interface ICvarRegistry
	{
		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		[Cvar(1.0), Range(0.1, 10.0)]
		double TimeScale { get; set; }

		/// <summary>
		///   The width of the screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
		///   restart of the graphics subsystem.
		/// </summary>
		[Cvar(1024, UpdateMode.OnGraphicsRestart), Range(320, 4096), Persistent]
		int ResolutionWidth { get; set; }

		/// <summary>
		///   The height of the screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
		///   restart of the graphics subsystem.
		/// </summary>
		[Cvar(768, UpdateMode.OnGraphicsRestart), Range(240, 4096), Persistent]
		int ResolutionHeight { get; set; }

		/// <summary>
		///   If true, the application is run in fullscreen mode. Any changes to this cvar require a restart of the graphics
		///   subsystem.
		/// </summary>
		[Cvar(!PlatformInfo.IsDebug, UpdateMode.OnGraphicsRestart), Persistent]
		bool Fullscreen { get; set; }

		/// <summary>
		///   The width of the application's window in non-fullscreen mode.
		/// </summary>
		[Cvar(640), Range(320, 4096), Persistent]
		int WindowWidth { get; set; }

		/// <summary>
		///   The height of the application's window in non-fullscreen mode.
		/// </summary>
		[Cvar(360), Range(240, 4096), Persistent]
		int WindowHeight { get; set; }
	}
}