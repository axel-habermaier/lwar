using System;
using Pegasus.Framework.Math;
using Pegasus.Framework.Platform;
using Pegasus.Framework.Scripting;
using Pegasus.Framework.Scripting.Validators;

// ReSharper disable CheckNamespace

/// <summary>
///   Declares the cvars required by the framework.
/// </summary>
internal interface ICvars
{
	/// <summary>
	///   The scaling factor that is applied to all timing values.
	/// </summary>
	[Cvar(1.0), Range(0.1, 10.0)]
	double TimeScale { get; set; }

	/// <summary>
	///   The screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
	///   restart of the graphics subsystem.
	/// </summary>
	[Cvar("new Size(1024, 768)", UpdateMode.OnGraphicsRestart), Persistent, WindowSize]
	Size Resolution { get; set; }

	/// <summary>
	///   If true, the application is run in fullscreen mode. Any changes to this cvar require a restart of the graphics
	///   subsystem.
	/// </summary>
	[Cvar(!PlatformInfo.IsDebug, UpdateMode.OnGraphicsRestart), Persistent]
	bool Fullscreen { get; set; }

	/// <summary>
	///   The size in pixels of the application window in non-fullscreen mode.
	/// </summary>
	[Cvar("new Size(1024, 768)"), Persistent, WindowSize]
	Size WindowSize { get; set; }

	/// <summary>
	///   The screen position of the application window's top left corner in non-fullscreen mode.
	/// </summary>
	[Cvar("Vector2i.Zero"), Persistent, WindowPosition]
	Vector2i WindowPosition { get; set; }

	/// <summary>
	///   The width of the application's window in non-fullscreen mode.
	/// </summary>
	[Cvar(WindowState.Normal), Persistent]
	WindowState WindowStateFlag { get; set; }

	/// <summary>
	///   Shows or hides the statistics.
	/// </summary>
	[Cvar(PlatformInfo.IsDebug), Persistent]
	bool ShowStats { get; set; }
}