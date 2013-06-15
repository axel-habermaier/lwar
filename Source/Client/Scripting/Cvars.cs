﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Saturday, 15 June 2013, 22:23:04
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
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Input;
	using Pegasus.Framework.Platform.Logging;
	using Pegasus.Framework.Scripting;
	using Pegasus.Framework.Scripting.Validators;

	internal static class Cvars
	{
		/// <summary>
		///   The name of the player.
		/// </summary>
		public static Cvar<string> PlayerNameCvar { get; private set; }

		/// <summary>
		///   The display time (in seconds) of event messages such as 'X killed Y', 'X joined the game', etc.
		/// </summary>
		public static Cvar<double> EventMessageDisplayTimeCvar { get; private set; }

		/// <summary>
		///   The display time (in seconds) of chat messages.
		/// </summary>
		public static Cvar<double> ChatMessageDisplayTimeCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, shows the scoreboard.
		/// </summary>
		public static Cvar<InputTrigger> InputShowScoreboardCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, moves the player forwards.
		/// </summary>
		public static Cvar<InputTrigger> InputForwardCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, moves the player backwards.
		/// </summary>
		public static Cvar<InputTrigger> InputBackwardCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, turns the player to the left.
		/// </summary>
		public static Cvar<InputTrigger> InputTurnLeftCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, turns the player to the right.
		/// </summary>
		public static Cvar<InputTrigger> InputTurnRightCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, moves the player to the left.
		/// </summary>
		public static Cvar<InputTrigger> InputStrafeLeftCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, moves the player to the right.
		/// </summary>
		public static Cvar<InputTrigger> InputStrafeRightCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, fires the player's primary weapon.
		/// </summary>
		public static Cvar<InputTrigger> InputPrimaryWeaponCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, fires the player's secondary weapon.
		/// </summary>
		public static Cvar<InputTrigger> InputSecondaryWeaponCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, fires the player's tertiary weapon.
		/// </summary>
		public static Cvar<InputTrigger> InputTertiaryWeaponCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, fires the player's quaternary weapon.
		/// </summary>
		public static Cvar<InputTrigger> InputQuaternaryWeaponCvar { get; private set; }

		/// <summary>
		///   When triggered in an active game session, opens the chat input.
		/// </summary>
		public static Cvar<InputTrigger> InputChatCvar { get; private set; }

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
		///   The display time (in seconds) of event messages such as 'X killed Y', 'X joined the game', etc.
		/// </summary>
		public static double EventMessageDisplayTime
		{
			get { return EventMessageDisplayTimeCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				EventMessageDisplayTimeCvar.Value = value;
			}
		}

		/// <summary>
		///   The display time (in seconds) of chat messages.
		/// </summary>
		public static double ChatMessageDisplayTime
		{
			get { return ChatMessageDisplayTimeCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				ChatMessageDisplayTimeCvar.Value = value;
			}
		}

		/// <summary>
		///   When triggered in an active game session, shows the scoreboard.
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
		///   When triggered in an active game session, moves the player forwards.
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
		///   When triggered in an active game session, moves the player backwards.
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
		///   When triggered in an active game session, turns the player to the left.
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
		///   When triggered in an active game session, turns the player to the right.
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
		///   When triggered in an active game session, moves the player to the left.
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
		///   When triggered in an active game session, moves the player to the right.
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
		///   When triggered in an active game session, fires the player's primary weapon.
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
		///   When triggered in an active game session, fires the player's secondary weapon.
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
		///   When triggered in an active game session, fires the player's tertiary weapon.
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
		///   When triggered in an active game session, fires the player's quaternary weapon.
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
		///   When triggered in an active game session, opens the chat input.
		/// </summary>
		public static InputTrigger InputChat
		{
			get { return InputChatCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputChatCvar.Value = value;
			}
		}

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
		///   Raised when the 'EventMessageDisplayTime' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<double> EventMessageDisplayTimeChanging
		{
			add { EventMessageDisplayTimeCvar.Changing += value; }
			remove { EventMessageDisplayTimeCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'EventMessageDisplayTime' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<double> EventMessageDisplayTimeChanged
		{
			add { EventMessageDisplayTimeCvar.Changed += value; }
			remove { EventMessageDisplayTimeCvar.Changed -= value; }
		}

		/// <summary>
		///   Raised when the 'ChatMessageDisplayTime' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<double> ChatMessageDisplayTimeChanging
		{
			add { ChatMessageDisplayTimeCvar.Changing += value; }
			remove { ChatMessageDisplayTimeCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'ChatMessageDisplayTime' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<double> ChatMessageDisplayTimeChanged
		{
			add { ChatMessageDisplayTimeCvar.Changed += value; }
			remove { ChatMessageDisplayTimeCvar.Changed -= value; }
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
		///   Raised when the 'InputChat' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputChatChanging
		{
			add { InputChatCvar.Changing += value; }
			remove { InputChatCvar.Changing -= value; }
		}

		/// <summary>
		///   Raised when the 'InputChat' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<InputTrigger> InputChatChanged
		{
			add { InputChatCvar.Changed += value; }
			remove { InputChatCvar.Changed -= value; }
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
			PlayerNameCvar = new Cvar<string>("player_name", "UnnamedPlayer", "The name of the player.", UpdateMode.Immediate, true, false, new NotEmptyAttribute(), new MaximumLengthAttribute(Specification.PlayerNameLength, true));
			EventMessageDisplayTimeCvar = new Cvar<double>("event_message_display_time", 3, "The display time (in seconds) of event messages such as 'X killed Y', 'X joined the game', etc.", UpdateMode.Immediate, true, false, new RangeAttribute(0.5, 60.0));
			ChatMessageDisplayTimeCvar = new Cvar<double>("chat_message_display_time", 6, "The display time (in seconds) of chat messages.", UpdateMode.Immediate, true, false, new RangeAttribute(0.5, 60.0));
			InputShowScoreboardCvar = new Cvar<InputTrigger>("input_show_scoreboard", Key.Tab.IsPressed(), "When triggered in an active game session, shows the scoreboard.", UpdateMode.Immediate, true, false);
			InputForwardCvar = new Cvar<InputTrigger>("input_forward", Key.W.IsPressed() | Key.Up.IsPressed(), "When triggered in an active game session, moves the player forwards.", UpdateMode.Immediate, true, false);
			InputBackwardCvar = new Cvar<InputTrigger>("input_backward", Key.S.IsPressed() | Key.Down.IsPressed(), "When triggered in an active game session, moves the player backwards.", UpdateMode.Immediate, true, false);
			InputTurnLeftCvar = new Cvar<InputTrigger>("input_turn_left", Key.Q.IsPressed(), "When triggered in an active game session, turns the player to the left.", UpdateMode.Immediate, true, false);
			InputTurnRightCvar = new Cvar<InputTrigger>("input_turn_right", Key.E.IsPressed(), "When triggered in an active game session, turns the player to the right.", UpdateMode.Immediate, true, false);
			InputStrafeLeftCvar = new Cvar<InputTrigger>("input_strafe_left", Key.A.IsPressed(), "When triggered in an active game session, moves the player to the left.", UpdateMode.Immediate, true, false);
			InputStrafeRightCvar = new Cvar<InputTrigger>("input_strafe_right", Key.D.IsPressed(), "When triggered in an active game session, moves the player to the right.", UpdateMode.Immediate, true, false);
			InputPrimaryWeaponCvar = new Cvar<InputTrigger>("input_primary_weapon", MouseButton.Left.IsPressed(), "When triggered in an active game session, fires the player's primary weapon.", UpdateMode.Immediate, true, false);
			InputSecondaryWeaponCvar = new Cvar<InputTrigger>("input_secondary_weapon", MouseButton.Right.IsPressed(), "When triggered in an active game session, fires the player's secondary weapon.", UpdateMode.Immediate, true, false);
			InputTertiaryWeaponCvar = new Cvar<InputTrigger>("input_tertiary_weapon", Key.Num1.IsPressed(), "When triggered in an active game session, fires the player's tertiary weapon.", UpdateMode.Immediate, true, false);
			InputQuaternaryWeaponCvar = new Cvar<InputTrigger>("input_quaternary_weapon", Key.Num2.IsPressed(), "When triggered in an active game session, fires the player's quaternary weapon.", UpdateMode.Immediate, true, false);
			InputChatCvar = new Cvar<InputTrigger>("input_chat", Key.Return.WentDown() | Key.NumpadEnter.WentDown(), "When triggered in an active game session, opens the chat input.", UpdateMode.Immediate, true, false);

			CvarRegistry.Register(PlayerNameCvar);
			CvarRegistry.Register(EventMessageDisplayTimeCvar);
			CvarRegistry.Register(ChatMessageDisplayTimeCvar);
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
			CvarRegistry.Register(InputChatCvar);
		}

		/// <summary>
		///   Initializes the instances imported by the registry.
		/// </summary>
		public static void Resolve()
		{
			TimeScaleCvar = CvarRegistry.Resolve<double>("time_scale");
			ResolutionCvar = CvarRegistry.Resolve<Size>("resolution");
			FullscreenCvar = CvarRegistry.Resolve<bool>("fullscreen");
			WindowSizeCvar = CvarRegistry.Resolve<Size>("window_size");
			WindowPositionCvar = CvarRegistry.Resolve<Vector2i>("window_position");
			WindowModeCvar = CvarRegistry.Resolve<WindowMode>("window_mode");
			ShowStatsCvar = CvarRegistry.Resolve<bool>("show_stats");
		}
	}
}

