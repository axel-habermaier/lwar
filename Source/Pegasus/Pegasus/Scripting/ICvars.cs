﻿using Pegasus.Framework.UserInterface.Controls;
using Pegasus.Math;
using Pegasus.Platform;
using Pegasus.Scripting;
using Pegasus.Scripting.Validators;

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
	[Cvar("Vector2i.Zero"), Persistent, WindowPosition, SystemOnly]
	Vector2i WindowPosition { get; set; }

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
}