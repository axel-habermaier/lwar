﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Thursday, 26 September 2013, 11:38:44
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pegasus.Scripting
{
	using System;
	using System.Diagnostics;
	using Framework.UserInterface.Controls;
	using Math;
	using Platform;
	using Validators;

	internal static class Cvars
	{
		/// <summary>
		///   The scaling factor that is applied to all time-scaling sensitive timing values.
		/// </summary>
		public static Cvar<double> TimeScaleCvar { get; private set; }

		/// <summary>
		///   The screen resolution used by the application in fullscreen mode.
		/// </summary>
		public static Cvar<Size> ResolutionCvar { get; private set; }

		/// <summary>
		///   If true, the application is run in fullscreen mode.
		/// </summary>
		public static Cvar<bool> FullscreenCvar { get; private set; }

		/// <summary>
		///   The size in pixels of the application window in non-fullscreen mode.
		/// </summary>
		public static Cvar<Size> WindowSizeCvar { get; private set; }

		/// <summary>
		///   The screen position of the application window's top left corner in non-fullscreen mode.
		/// </summary>
		public static Cvar<Vector2i> WindowPositionCvar { get; private set; }

		/// <summary>
		///   The width of the application's window in non-fullscreen mode.
		/// </summary>
		public static Cvar<WindowMode> WindowModeCvar { get; private set; }

		/// <summary>
		///   Shows or hides the platform information.
		/// </summary>
		public static Cvar<bool> ShowPlatformInfoCvar { get; private set; }

		/// <summary>
		///   Shows or hides the frame statistics.
		/// </summary>
		public static Cvar<bool> ShowFrameStatsCvar { get; private set; }

		/// <summary>
		///   The scaling factor that is applied to all time-scaling sensitive timing values.
		/// </summary>
		public static double TimeScale
		{
			get { return TimeScaleCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				TimeScaleCvar.Value = value;
			}
		}

		/// <summary>
		///   The screen resolution used by the application in fullscreen mode.
		/// </summary>
		public static Size Resolution
		{
			get { return ResolutionCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				ResolutionCvar.Value = value;
			}
		}

		/// <summary>
		///   If true, the application is run in fullscreen mode.
		/// </summary>
		public static bool Fullscreen
		{
			get { return FullscreenCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				FullscreenCvar.Value = value;
			}
		}

		/// <summary>
		///   The size in pixels of the application window in non-fullscreen mode.
		/// </summary>
		public static Size WindowSize
		{
			get { return WindowSizeCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				WindowSizeCvar.Value = value;
			}
		}

		/// <summary>
		///   The screen position of the application window's top left corner in non-fullscreen mode.
		/// </summary>
		public static Vector2i WindowPosition
		{
			get { return WindowPositionCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				WindowPositionCvar.Value = value;
			}
		}

		/// <summary>
		///   The width of the application's window in non-fullscreen mode.
		/// </summary>
		public static WindowMode WindowMode
		{
			get { return WindowModeCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				WindowModeCvar.Value = value;
			}
		}

		/// <summary>
		///   Shows or hides the platform information.
		/// </summary>
		public static bool ShowPlatformInfo
		{
			get { return ShowPlatformInfoCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				ShowPlatformInfoCvar.Value = value;
			}
		}

		/// <summary>
		///   Shows or hides the frame statistics.
		/// </summary>
		public static bool ShowFrameStats
		{
			get { return ShowFrameStatsCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				ShowFrameStatsCvar.Value = value;
			}
		}

		/// <summary>
		///   Raised when the 'TimeScale' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<double> TimeScaleChanging
		{
			add { TimeScaleCvar.Changing += value; }
			remove { TimeScaleCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'TimeScale' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<double> TimeScaleChanged
		{
			add { TimeScaleCvar.Changed += value; }
			remove { TimeScaleCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'Resolution' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<Size> ResolutionChanging
		{
			add { ResolutionCvar.Changing += value; }
			remove { ResolutionCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'Resolution' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<Size> ResolutionChanged
		{
			add { ResolutionCvar.Changed += value; }
			remove { ResolutionCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'Fullscreen' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<bool> FullscreenChanging
		{
			add { FullscreenCvar.Changing += value; }
			remove { FullscreenCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'Fullscreen' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<bool> FullscreenChanged
		{
			add { FullscreenCvar.Changed += value; }
			remove { FullscreenCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowSize' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<Size> WindowSizeChanging
		{
			add { WindowSizeCvar.Changing += value; }
			remove { WindowSizeCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowSize' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<Size> WindowSizeChanged
		{
			add { WindowSizeCvar.Changed += value; }
			remove { WindowSizeCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowPosition' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<Vector2i> WindowPositionChanging
		{
			add { WindowPositionCvar.Changing += value; }
			remove { WindowPositionCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowPosition' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<Vector2i> WindowPositionChanged
		{
			add { WindowPositionCvar.Changed += value; }
			remove { WindowPositionCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowMode' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<WindowMode> WindowModeChanging
		{
			add { WindowModeCvar.Changing += value; }
			remove { WindowModeCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowMode' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<WindowMode> WindowModeChanged
		{
			add { WindowModeCvar.Changed += value; }
			remove { WindowModeCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'ShowPlatformInfo' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<bool> ShowPlatformInfoChanging
		{
			add { ShowPlatformInfoCvar.Changing += value; }
			remove { ShowPlatformInfoCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'ShowPlatformInfo' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<bool> ShowPlatformInfoChanged
		{
			add { ShowPlatformInfoCvar.Changed += value; }
			remove { ShowPlatformInfoCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'ShowFrameStats' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<bool> ShowFrameStatsChanging
		{
			add { ShowFrameStatsCvar.Changing += value; }
			remove { ShowFrameStatsCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'ShowFrameStats' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<bool> ShowFrameStatsChanged
		{
			add { ShowFrameStatsCvar.Changed += value; }
			remove { ShowFrameStatsCvar.Changed -= value; }
		}

		/// <summary>
		///   Initializes the instances declared by the registry.
		/// </summary>
		public static void Initialize()
		{
			TimeScaleCvar = new Cvar<double>("time_scale", 1.0, "The scaling factor that is applied to all time-scaling sensitive timing values.",
											 UpdateMode.Immediate, false, false, new RangeAttribute(0.1, 10.0));
			ResolutionCvar = new Cvar<Size>("resolution", new Size(1024, 768), "The screen resolution used by the application in fullscreen mode.",
											UpdateMode.OnGraphicsRestart, true, false, new WindowSizeAttribute());
			FullscreenCvar = new Cvar<bool>("fullscreen", !PlatformInfo.IsDebug, "If true, the application is run in fullscreen mode.",
											UpdateMode.OnGraphicsRestart, true, false);
			WindowSizeCvar = new Cvar<Size>("window_size", new Size(1024, 768),
											"The size in pixels of the application window in non-fullscreen mode.", UpdateMode.Immediate, true, true,
											new WindowSizeAttribute());
			WindowPositionCvar = new Cvar<Vector2i>("window_position", Vector2i.Zero,
													"The screen position of the application window's top left corner in non-fullscreen mode.", UpdateMode.Immediate, true, true,
													new WindowPositionAttribute());
			WindowModeCvar = new Cvar<WindowMode>("window_mode", WindowMode.Normal, "The width of the application's window in non-fullscreen mode.",
												  UpdateMode.Immediate, true, true);
			ShowPlatformInfoCvar = new Cvar<bool>("show_platform_info", PlatformInfo.IsDebug, "Shows or hides the platform information.",
												  UpdateMode.Immediate, true, false);
			ShowFrameStatsCvar = new Cvar<bool>("show_frame_stats", PlatformInfo.IsDebug, "Shows or hides the frame statistics.",
												UpdateMode.Immediate, true, false);

			CvarRegistry.Register(TimeScaleCvar);
			CvarRegistry.Register(ResolutionCvar);
			CvarRegistry.Register(FullscreenCvar);
			CvarRegistry.Register(WindowSizeCvar);
			CvarRegistry.Register(WindowPositionCvar);
			CvarRegistry.Register(WindowModeCvar);
			CvarRegistry.Register(ShowPlatformInfoCvar);
			CvarRegistry.Register(ShowFrameStatsCvar);
		}

		/// <summary>
		///   Initializes the instances imported by the registry.
		/// </summary>
		public static void Resolve()
		{
		}
	}
}