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
		///   The width of the screen resolution used by the application.
		/// </summary>
		[Cvar(640), Range(640u, 1920u), Persistent]
		uint ResolutionWidth { get; set; }

		/// <summary>
		///   The height of the screen resolution used by the application.
		/// </summary>
		[Cvar(360), Range(360u, 1200u), Persistent]
		uint ResolutionHeight { get; set; }

		/// <summary>
		///   If true, the application is run in fullscreen mode.
		/// </summary>
		[Cvar(!PlatformInfo.IsDebug), Persistent]
		bool Fullscreen { get; set; }
	}
}