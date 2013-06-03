﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Tuesday, June 4, 2013, 1:48:28
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
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Scripting;

	internal static class Cvars
	{
		/// <summary>
		///   The name of the player.
		/// </summary>
		public static Cvar<string> PlayerNameCvar { get; private set; }

		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		public static Cvar<bool> DrawWireframeCvar { get; private set; }

		/// <summary>
		///   When triggered, shows the scoreboard in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputShowScoreboardCvar { get; private set; }

		/// <summary>
		///   When triggered, moves the player forwards in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputForwardCvar { get; private set; }

		/// <summary>
		///   When triggered, moves the player backwards in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputBackwardCvar { get; private set; }

		/// <summary>
		///   When triggered, turns the player to the left in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputTurnLeftCvar { get; private set; }

		/// <summary>
		///   When triggered, turns the player to the right in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputTurnRightCvar { get; private set; }

		/// <summary>
		///   When triggered, moves the player to the left in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputStrafeLeftCvar { get; private set; }

		/// <summary>
		///   When triggered, moves the player to the right in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputStrafeRightCvar { get; private set; }

		/// <summary>
		///   When triggered, fires the player's primary weapon in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputPrimaryWeaponCvar { get; private set; }

		/// <summary>
		///   When triggered, fires the player's secondary weapon in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputSecondaryWeaponCvar { get; private set; }

		/// <summary>
		///   When triggered, fires the player's tertiary weapon in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputTertiaryWeaponCvar { get; private set; }

		/// <summary>
		///   When triggered, fires the player's quaternary weapon in an active game session.
		/// </summary>
		public static Cvar<InputTrigger> InputQuaternaryWeaponCvar { get; private set; }

		/// <summary>
		///   The scaling factor that is applied to all timing values.
		/// </summary>
		public static Cvar<double> TimeScaleCvar { get; private set; }

		/// <summary>
		///   The width of the screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
		///   restart of the graphics subsystem.
		/// </summary>
		public static Cvar<int> ResolutionWidthCvar { get; private set; }

		/// <summary>
		///   The height of the screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
		///   restart of the graphics subsystem.
		/// </summary>
		public static Cvar<int> ResolutionHeightCvar { get; private set; }

		/// <summary>
		///   If true, the application is run in fullscreen mode. Any changes to this cvar require a restart of the graphics
		///   subsystem.
		/// </summary>
		public static Cvar<bool> FullscreenCvar { get; private set; }

		/// <summary>
		///   The width of the application's window in non-fullscreen mode.
		/// </summary>
		public static Cvar<int> WindowWidthCvar { get; private set; }

		/// <summary>
		///   The height of the application's window in non-fullscreen mode.
		/// </summary>
		public static Cvar<int> WindowHeightCvar { get; private set; }

		/// <summary>
		///   Shows or hides the statistics.
		/// </summary>
		public static Cvar<bool> ShowStatsCvar { get; private set; }

		/// <summary>
		///   The name of the player.
		/// </summary>
		public static string PlayerName
		{
			get { return PlayerNameCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				PlayerNameCvar.Value = value;
			}
		}

		/// <summary>
		///   If true, all 3D geometry is drawn in wireframe mode.
		/// </summary>
		public static bool DrawWireframe
		{
			get { return DrawWireframeCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				DrawWireframeCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, shows the scoreboard in an active game session.
		/// </summary>
		public static InputTrigger InputShowScoreboard
		{
			get { return InputShowScoreboardCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputShowScoreboardCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, moves the player forwards in an active game session.
		/// </summary>
		public static InputTrigger InputForward
		{
			get { return InputForwardCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputForwardCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, moves the player backwards in an active game session.
		/// </summary>
		public static InputTrigger InputBackward
		{
			get { return InputBackwardCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputBackwardCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, turns the player to the left in an active game session.
		/// </summary>
		public static InputTrigger InputTurnLeft
		{
			get { return InputTurnLeftCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputTurnLeftCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, turns the player to the right in an active game session.
		/// </summary>
		public static InputTrigger InputTurnRight
		{
			get { return InputTurnRightCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputTurnRightCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, moves the player to the left in an active game session.
		/// </summary>
		public static InputTrigger InputStrafeLeft
		{
			get { return InputStrafeLeftCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputStrafeLeftCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, moves the player to the right in an active game session.
		/// </summary>
		public static InputTrigger InputStrafeRight
		{
			get { return InputStrafeRightCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputStrafeRightCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, fires the player's primary weapon in an active game session.
		/// </summary>
		public static InputTrigger InputPrimaryWeapon
		{
			get { return InputPrimaryWeaponCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputPrimaryWeaponCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, fires the player's secondary weapon in an active game session.
		/// </summary>
		public static InputTrigger InputSecondaryWeapon
		{
			get { return InputSecondaryWeaponCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputSecondaryWeaponCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, fires the player's tertiary weapon in an active game session.
		/// </summary>
		public static InputTrigger InputTertiaryWeapon
		{
			get { return InputTertiaryWeaponCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputTertiaryWeaponCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered, fires the player's quaternary weapon in an active game session.
		/// </summary>
		public static InputTrigger InputQuaternaryWeapon
		{
			get { return InputQuaternaryWeaponCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputQuaternaryWeaponCvar.Value = value;
			}
		}

		/// <summary>
		///   The scaling factor that is applied to all timing values.
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
		///   The width of the screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
		///   restart of the graphics subsystem.
		/// </summary>
		public static int ResolutionWidth
		{
			get { return ResolutionWidthCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				ResolutionWidthCvar.Value = value;
			}
		}

		/// <summary>
		///   The height of the screen resolution used by the application in fullscreen mode. Any changes to this cvar require a
		///   restart of the graphics subsystem.
		/// </summary>
		public static int ResolutionHeight
		{
			get { return ResolutionHeightCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				ResolutionHeightCvar.Value = value;
			}
		}

		/// <summary>
		///   If true, the application is run in fullscreen mode. Any changes to this cvar require a restart of the graphics
		///   subsystem.
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
		///   The width of the application's window in non-fullscreen mode.
		/// </summary>
		public static int WindowWidth
		{
			get { return WindowWidthCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				WindowWidthCvar.Value = value;
			}
		}

		/// <summary>
		///   The height of the application's window in non-fullscreen mode.
		/// </summary>
		public static int WindowHeight
		{
			get { return WindowHeightCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				WindowHeightCvar.Value = value;
			}
		}

		/// <summary>
		///   Shows or hides the statistics.
		/// </summary>
		public static bool ShowStats
		{
			get { return ShowStatsCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				ShowStatsCvar.Value = value;
			}
		}

		/// <summary>
		///   Raised when the 'PlayerName' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<string> PlayerNameChanging
		{
			add { PlayerNameCvar.Changing += value; }
			remove { PlayerNameCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'PlayerName' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<string> PlayerNameChanged
		{
			add { PlayerNameCvar.Changed += value; }
			remove { PlayerNameCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'DrawWireframe' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<bool> DrawWireframeChanging
		{
			add { DrawWireframeCvar.Changing += value; }
			remove { DrawWireframeCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'DrawWireframe' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<bool> DrawWireframeChanged
		{
			add { DrawWireframeCvar.Changed += value; }
			remove { DrawWireframeCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputShowScoreboard' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputShowScoreboardChanging
		{
			add { InputShowScoreboardCvar.Changing += value; }
			remove { InputShowScoreboardCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputShowScoreboard' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputShowScoreboardChanged
		{
			add { InputShowScoreboardCvar.Changed += value; }
			remove { InputShowScoreboardCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputForward' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputForwardChanging
		{
			add { InputForwardCvar.Changing += value; }
			remove { InputForwardCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputForward' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputForwardChanged
		{
			add { InputForwardCvar.Changed += value; }
			remove { InputForwardCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputBackward' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputBackwardChanging
		{
			add { InputBackwardCvar.Changing += value; }
			remove { InputBackwardCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputBackward' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputBackwardChanged
		{
			add { InputBackwardCvar.Changed += value; }
			remove { InputBackwardCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputTurnLeft' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputTurnLeftChanging
		{
			add { InputTurnLeftCvar.Changing += value; }
			remove { InputTurnLeftCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputTurnLeft' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputTurnLeftChanged
		{
			add { InputTurnLeftCvar.Changed += value; }
			remove { InputTurnLeftCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputTurnRight' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputTurnRightChanging
		{
			add { InputTurnRightCvar.Changing += value; }
			remove { InputTurnRightCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputTurnRight' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputTurnRightChanged
		{
			add { InputTurnRightCvar.Changed += value; }
			remove { InputTurnRightCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputStrafeLeft' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputStrafeLeftChanging
		{
			add { InputStrafeLeftCvar.Changing += value; }
			remove { InputStrafeLeftCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputStrafeLeft' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputStrafeLeftChanged
		{
			add { InputStrafeLeftCvar.Changed += value; }
			remove { InputStrafeLeftCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputStrafeRight' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputStrafeRightChanging
		{
			add { InputStrafeRightCvar.Changing += value; }
			remove { InputStrafeRightCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputStrafeRight' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputStrafeRightChanged
		{
			add { InputStrafeRightCvar.Changed += value; }
			remove { InputStrafeRightCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputPrimaryWeapon' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputPrimaryWeaponChanging
		{
			add { InputPrimaryWeaponCvar.Changing += value; }
			remove { InputPrimaryWeaponCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputPrimaryWeapon' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputPrimaryWeaponChanged
		{
			add { InputPrimaryWeaponCvar.Changed += value; }
			remove { InputPrimaryWeaponCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputSecondaryWeapon' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputSecondaryWeaponChanging
		{
			add { InputSecondaryWeaponCvar.Changing += value; }
			remove { InputSecondaryWeaponCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputSecondaryWeapon' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputSecondaryWeaponChanged
		{
			add { InputSecondaryWeaponCvar.Changed += value; }
			remove { InputSecondaryWeaponCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputTertiaryWeapon' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputTertiaryWeaponChanging
		{
			add { InputTertiaryWeaponCvar.Changing += value; }
			remove { InputTertiaryWeaponCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputTertiaryWeapon' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputTertiaryWeaponChanged
		{
			add { InputTertiaryWeaponCvar.Changed += value; }
			remove { InputTertiaryWeaponCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'InputQuaternaryWeapon' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputQuaternaryWeaponChanging
		{
			add { InputQuaternaryWeaponCvar.Changing += value; }
			remove { InputQuaternaryWeaponCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputQuaternaryWeapon' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputQuaternaryWeaponChanged
		{
			add { InputQuaternaryWeaponCvar.Changed += value; }
			remove { InputQuaternaryWeaponCvar.Changed -= value; }
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
		///   Raised when the 'ResolutionWidth' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<int> ResolutionWidthChanging
		{
			add { ResolutionWidthCvar.Changing += value; }
			remove { ResolutionWidthCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'ResolutionWidth' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<int> ResolutionWidthChanged
		{
			add { ResolutionWidthCvar.Changed += value; }
			remove { ResolutionWidthCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'ResolutionHeight' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<int> ResolutionHeightChanging
		{
			add { ResolutionHeightCvar.Changing += value; }
			remove { ResolutionHeightCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'ResolutionHeight' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<int> ResolutionHeightChanged
		{
			add { ResolutionHeightCvar.Changed += value; }
			remove { ResolutionHeightCvar.Changed -= value; }
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
		///   Raised when the 'WindowWidth' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<int> WindowWidthChanging
		{
			add { WindowWidthCvar.Changing += value; }
			remove { WindowWidthCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowWidth' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<int> WindowWidthChanged
		{
			add { WindowWidthCvar.Changed += value; }
			remove { WindowWidthCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowHeight' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<int> WindowHeightChanging
		{
			add { WindowHeightCvar.Changing += value; }
			remove { WindowHeightCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'WindowHeight' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<int> WindowHeightChanged
		{
			add { WindowHeightCvar.Changed += value; }
			remove { WindowHeightCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'ShowStats' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<bool> ShowStatsChanging
		{
			add { ShowStatsCvar.Changing += value; }
			remove { ShowStatsCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'ShowStats' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<bool> ShowStatsChanged
		{
			add { ShowStatsCvar.Changed += value; }
			remove { ShowStatsCvar.Changed -= value; }
		}

		/// <summary>
		///   Initializes the instances declared by the registry.
		/// </summary>
		public static void Initialize()
		{
			PlayerNameCvar = new Cvar<string>("player_name", "UnnamedPlayer", "The name of the player.", UpdateMode.Immediate, true, new NotEmptyAttribute(), new MaximumLengthAttribute(Specification.MaximumPlayerNameLength, true));
			DrawWireframeCvar = new Cvar<bool>("draw_wireframe", false, "If true, all 3D geometry is drawn in wireframe mode.", UpdateMode.Immediate, false);
			InputShowScoreboardCvar = new Cvar<InputTrigger>("input_show_scoreboard", Key.Tab.IsPressed(), "When triggered, shows the scoreboard in an active game session.", UpdateMode.Immediate, true);
			InputForwardCvar = new Cvar<InputTrigger>("input_forward", Key.W.IsPressed() | Key.Up.IsPressed(), "When triggered, moves the player forwards in an active game session.", UpdateMode.Immediate, true);
			InputBackwardCvar = new Cvar<InputTrigger>("input_backward", Key.S.IsPressed() | Key.Down.IsPressed(), "When triggered, moves the player backwards in an active game session.", UpdateMode.Immediate, true);
			InputTurnLeftCvar = new Cvar<InputTrigger>("input_turn_left", Key.A.IsPressed() | Key.Left.IsPressed(), "When triggered, turns the player to the left in an active game session.", UpdateMode.Immediate, true);
			InputTurnRightCvar = new Cvar<InputTrigger>("input_turn_right", Key.D.IsPressed() | Key.Right.IsPressed(), "When triggered, turns the player to the right in an active game session.", UpdateMode.Immediate, true);
			InputStrafeLeftCvar = new Cvar<InputTrigger>("input_strafe_left", Key.Q.IsPressed(), "When triggered, moves the player to the left in an active game session.", UpdateMode.Immediate, true);
			InputStrafeRightCvar = new Cvar<InputTrigger>("input_strafe_right", Key.E.IsPressed(), "When triggered, moves the player to the right in an active game session.", UpdateMode.Immediate, true);
			InputPrimaryWeaponCvar = new Cvar<InputTrigger>("input_primary_weapon", MouseButton.Left.IsPressed(), "When triggered, fires the player's primary weapon in an active game session.", UpdateMode.Immediate, true);
			InputSecondaryWeaponCvar = new Cvar<InputTrigger>("input_secondary_weapon", MouseButton.Right.IsPressed(), "When triggered, fires the player's secondary weapon in an active game session.", UpdateMode.Immediate, true);
			InputTertiaryWeaponCvar = new Cvar<InputTrigger>("input_tertiary_weapon", Key.Num1.IsPressed(), "When triggered, fires the player's tertiary weapon in an active game session.", UpdateMode.Immediate, true);
			InputQuaternaryWeaponCvar = new Cvar<InputTrigger>("input_quaternary_weapon", Key.Num2.IsPressed(), "When triggered, fires the player's quaternary weapon in an active game session.", UpdateMode.Immediate, true);

			CvarRegistry.Register(PlayerNameCvar);
			CvarRegistry.Register(DrawWireframeCvar);
			CvarRegistry.Register(InputShowScoreboardCvar);
			CvarRegistry.Register(InputForwardCvar);
			CvarRegistry.Register(InputBackwardCvar);
			CvarRegistry.Register(InputTurnLeftCvar);
			CvarRegistry.Register(InputTurnRightCvar);
			CvarRegistry.Register(InputStrafeLeftCvar);
			CvarRegistry.Register(InputStrafeRightCvar);
			CvarRegistry.Register(InputPrimaryWeaponCvar);
			CvarRegistry.Register(InputSecondaryWeaponCvar);
			CvarRegistry.Register(InputTertiaryWeaponCvar);
			CvarRegistry.Register(InputQuaternaryWeaponCvar);
		}

		/// <summary>
		///   Initializes the instances imported by the registry.
		/// </summary>
		public static void Resolve()
		{
			TimeScaleCvar = CvarRegistry.Resolve<double>("time_scale");
			ResolutionWidthCvar = CvarRegistry.Resolve<int>("resolution_width");
			ResolutionHeightCvar = CvarRegistry.Resolve<int>("resolution_height");
			FullscreenCvar = CvarRegistry.Resolve<bool>("fullscreen");
			WindowWidthCvar = CvarRegistry.Resolve<int>("window_width");
			WindowHeightCvar = CvarRegistry.Resolve<int>("window_height");
			ShowStatsCvar = CvarRegistry.Resolve<bool>("show_stats");
		}
	}
}

