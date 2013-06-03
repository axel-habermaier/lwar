﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Monday, June 3, 2013, 21:21:46
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Lwar.Client.Scripting
{
	using Lwar.Client.Network;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Scripting;

	internal static class Cvars
	{
		/// <summary>
		///   The name of the player.
		/// </summary>
		private static Cvar<string> _playerName;

		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		private static Cvar<bool> _drawWireframe;

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		private static Cvar<double> _timeScale;

		/// <summary>
		///   The width of the screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
		///   restart of the graphics subsystem.
		/// </summary>
		private static Cvar<int> _resolutionWidth;

		/// <summary>
		///   The height of the screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
		///   restart of the graphics subsystem.
		/// </summary>
		private static Cvar<int> _resolutionHeight;

		/// <summary>
		///   If true, the application is run in fullscreen mode. Any changes to this cvar require a restart of the graphics
		///   subsystem.
		/// </summary>
		private static Cvar<bool> _fullscreen;

		/// <summary>
		///   The width of the application's window in non-fullscreen mode.
		/// </summary>
		private static Cvar<int> _windowWidth;

		/// <summary>
		///   The height of the application's window in non-fullscreen mode.
		/// </summary>
		private static Cvar<int> _windowHeight;

		/// <summary>
		///   Shows or hides the statistics.
		/// </summary>
		private static Cvar<bool> _showStats;

		/// <summary>
		///   The name of the player.
		/// </summary>
		public static string PlayerName
		{
			get { return _playerName.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				_playerName.Value = value;
			}
		}

		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		public static bool DrawWireframe
		{
			get { return _drawWireframe.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				_drawWireframe.Value = value;
			}
		}

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		public static double TimeScale
		{
			get { return _timeScale.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				_timeScale.Value = value;
			}
		}

		/// <summary>
		///   The width of the screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
		///   restart of the graphics subsystem.
		/// </summary>
		public static int ResolutionWidth
		{
			get { return _resolutionWidth.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				_resolutionWidth.Value = value;
			}
		}

		/// <summary>
		///   The height of the screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
		///   restart of the graphics subsystem.
		/// </summary>
		public static int ResolutionHeight
		{
			get { return _resolutionHeight.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				_resolutionHeight.Value = value;
			}
		}

		/// <summary>
		///   If true, the application is run in fullscreen mode. Any changes to this cvar require a restart of the graphics
		///   subsystem.
		/// </summary>
		public static bool Fullscreen
		{
			get { return _fullscreen.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				_fullscreen.Value = value;
			}
		}

		/// <summary>
		///   The width of the application's window in non-fullscreen mode.
		/// </summary>
		public static int WindowWidth
		{
			get { return _windowWidth.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				_windowWidth.Value = value;
			}
		}

		/// <summary>
		///   The height of the application's window in non-fullscreen mode.
		/// </summary>
		public static int WindowHeight
		{
			get { return _windowHeight.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				_windowHeight.Value = value;
			}
		}

		/// <summary>
		///   Shows or hides the statistics.
		/// </summary>
		public static bool ShowStats
		{
			get { return _showStats.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				_showStats.Value = value;
			}
		}

		/// <summary>
		///   Raised when the 'PlayerName' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<string> PlayerNameChanging
		{
			add { _playerName.Changing += value; }
			remove { _playerName.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'PlayerName' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<string> PlayerNameChanged
		{
			add { _playerName.Changed += value; }
			remove { _playerName.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'DrawWireframe' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<bool> DrawWireframeChanging
		{
			add { _drawWireframe.Changing += value; }
			remove { _drawWireframe.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'DrawWireframe' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<bool> DrawWireframeChanged
		{
			add { _drawWireframe.Changed += value; }
			remove { _drawWireframe.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'TimeScale' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<double> TimeScaleChanging
		{
			add { _timeScale.Changing += value; }
			remove { _timeScale.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'TimeScale' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<double> TimeScaleChanged
		{
			add { _timeScale.Changed += value; }
			remove { _timeScale.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'ResolutionWidth' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<int> ResolutionWidthChanging
		{
			add { _resolutionWidth.Changing += value; }
			remove { _resolutionWidth.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'ResolutionWidth' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<int> ResolutionWidthChanged
		{
			add { _resolutionWidth.Changed += value; }
			remove { _resolutionWidth.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'ResolutionHeight' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<int> ResolutionHeightChanging
		{
			add { _resolutionHeight.Changing += value; }
			remove { _resolutionHeight.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'ResolutionHeight' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<int> ResolutionHeightChanged
		{
			add { _resolutionHeight.Changed += value; }
			remove { _resolutionHeight.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'Fullscreen' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<bool> FullscreenChanging
		{
			add { _fullscreen.Changing += value; }
			remove { _fullscreen.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'Fullscreen' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<bool> FullscreenChanged
		{
			add { _fullscreen.Changed += value; }
			remove { _fullscreen.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowWidth' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<int> WindowWidthChanging
		{
			add { _windowWidth.Changing += value; }
			remove { _windowWidth.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowWidth' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<int> WindowWidthChanged
		{
			add { _windowWidth.Changed += value; }
			remove { _windowWidth.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowHeight' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<int> WindowHeightChanging
		{
			add { _windowHeight.Changing += value; }
			remove { _windowHeight.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowHeight' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<int> WindowHeightChanged
		{
			add { _windowHeight.Changed += value; }
			remove { _windowHeight.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'ShowStats' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<bool> ShowStatsChanging
		{
			add { _showStats.Changing += value; }
			remove { _showStats.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'ShowStats' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<bool> ShowStatsChanged
		{
			add { _showStats.Changed += value; }
			remove { _showStats.Changed -= value; }
		}

		/// <summary>
		///   Initializes the instances declared by the registry.
		/// </summary>
		public static void Initialize()
		{
			_playerName = new Cvar<string>("player_name", "UnnamedPlayer", "The name of the player.", UpdateMode.Immediate, true, new NotEmptyAttribute(), new MaximumLengthAttribute(Specification.MaximumPlayerNameLength, true));
			_drawWireframe = new Cvar<bool>("draw_wireframe", false, "If true, all 3D geometry is drawn in wireframe mode.", UpdateMode.Immediate, false);

			CvarRegistry.Register(_playerName);
			CvarRegistry.Register(_drawWireframe);

		}

		/// <summary>
		///   Initializes the instances imported by the registry.
		/// </summary>
		public static void Resolve()
		{
			_timeScale = CvarRegistry.Resolve<double>("time_scale");
			_resolutionWidth = CvarRegistry.Resolve<int>("resolution_width");
			_resolutionHeight = CvarRegistry.Resolve<int>("resolution_height");
			_fullscreen = CvarRegistry.Resolve<bool>("fullscreen");
			_windowWidth = CvarRegistry.Resolve<int>("window_width");
			_windowHeight = CvarRegistry.Resolve<int>("window_height");
			_showStats = CvarRegistry.Resolve<bool>("show_stats");
		}
	}
}
