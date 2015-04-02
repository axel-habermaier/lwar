using Pegasus.Math;
using Pegasus.Platform;
using Pegasus.Platform.Graphics;
using Pegasus.Scripting;
using Pegasus.Scripting.Validators;
using Pegasus.UserInterface.Controls;

// ReSharper disable CheckNamespace

/// <summary>
///     Declares the cvars required by the framework.
/// </summary>
internal interface ICvars
{
	/// <summary>
	///     The scaling factor that is applied to all time-scaling sensitive timing values.
	/// </summary>
	[Cvar(1.0), Range(0.1, 10.0)]
	double TimeScale { get; set; }

	/// <summary>
	///     The screen resolution used by the application in fullscreen mode.
	/// </summary>
	[Cvar("new Size(1024, 768)"), Persistent, WindowSize]
	Size Resolution { get; set; }

	/// <summary>
	///     The size in pixels of the application window in non-fullscreen mode.
	/// </summary>
	[Cvar("new Size(1024, 768)"), Persistent, WindowSize, SystemOnly]
	Size WindowSize { get; set; }

	/// <summary>
	///     The screen position of the application window's top left corner in non-fullscreen mode.
	/// </summary>
	[Cvar("new Vector2(100, 100)"), Persistent, WindowPosition, SystemOnly]
	Vector2 WindowPosition { get; set; }

	/// <summary>
	///     The width of the application's window in non-fullscreen mode.
	/// </summary>
	[Cvar(WindowMode.Fullscreen), Persistent, SystemOnly]
	WindowMode WindowMode { get; set; }

	/// <summary>
	///     Shows or hides the debug overlay.
	/// </summary>
	[Cvar(PlatformInfo.IsDebug), Persistent]
	bool ShowDebugOverlay { get; set; }

	/// <summary>
	///     Determines the graphics API that should be used for rendering. If the chosen graphics API is not available, the
	///     application automatically tries to fall back to a supported one.
	/// </summary>
	[Cvar("PlatformInfo.Platform == PlatformType.Windows ? GraphicsApi.Direct3D11 : GraphicsApi.OpenGL3", UpdateMode.OnAppRestart), Persistent]
	GraphicsApi GraphicsApi { get; set; }

	/// <summary>
	///     Indicates whether the hardware cursor is enabled. Hardware cursors are frame rate independent and are generally
	///     preferable where supported.
	/// </summary>
	[Cvar(true), Persistent]
	bool HardwareCursor { get; set; }

	/// <summary>
	///     Enables or disable vertical synchronization (vsync). Enabling vsync avoids screen tearing but increases latency.
	/// </summary>
	[Cvar(true), Persistent]
	bool Vsync { get; set; }
}