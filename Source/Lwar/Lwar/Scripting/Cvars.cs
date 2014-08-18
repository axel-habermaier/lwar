﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the Pegasus Asset Compiler.
//     Monday, 18 August 2014, 14:35:13
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Lwar.Scripting
{
	using Lwar.Network;
	using Pegasus;
	using Pegasus.Framework.UserInterface.Controls;
	using Pegasus.Framework.UserInterface.Input;
	using Pegasus.Math;
	using Pegasus.Platform;
	using Pegasus.Platform.Logging;
	using Pegasus.Scripting;
	using Pegasus.Scripting.Validators;

	internal static class Cvars
	{
		public static Cvar<string> PlayerNameCvar { get; private set; }

		public static Cvar<double> EventMessageDisplayTimeCvar { get; private set; }

		public static Cvar<double> ChatMessageDisplayTimeCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputShowScoreboardCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputRespawnCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputForwardCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputBackwardCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputStrafeLeftCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputStrafeRightCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputPrimaryWeaponCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputSecondaryWeaponCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputTertiaryWeaponCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputQuaternaryWeaponCvar { get; private set; }

		public static Cvar<ConfigurableInput> InputChatCvar { get; private set; }

		public static Cvar<double> TimeScaleCvar { get; private set; }

		public static Cvar<Size> ResolutionCvar { get; private set; }

		public static Cvar<Size> WindowSizeCvar { get; private set; }

		public static Cvar<Vector2i> WindowPositionCvar { get; private set; }

		public static Cvar<WindowMode> WindowModeCvar { get; private set; }

		public static Cvar<bool> ShowDebugOverlayCvar { get; private set; }

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

		public static ConfigurableInput InputShowScoreboard
		{
			get { return InputShowScoreboardCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputShowScoreboardCvar.Value = value;
			}
		}

		public static ConfigurableInput InputRespawn
		{
			get { return InputRespawnCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputRespawnCvar.Value = value;
			}
		}

		public static ConfigurableInput InputForward
		{
			get { return InputForwardCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputForwardCvar.Value = value;
			}
		}

		public static ConfigurableInput InputBackward
		{
			get { return InputBackwardCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputBackwardCvar.Value = value;
			}
		}

		public static ConfigurableInput InputStrafeLeft
		{
			get { return InputStrafeLeftCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputStrafeLeftCvar.Value = value;
			}
		}

		public static ConfigurableInput InputStrafeRight
		{
			get { return InputStrafeRightCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputStrafeRightCvar.Value = value;
			}
		}

		public static ConfigurableInput InputPrimaryWeapon
		{
			get { return InputPrimaryWeaponCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputPrimaryWeaponCvar.Value = value;
			}
		}

		public static ConfigurableInput InputSecondaryWeapon
		{
			get { return InputSecondaryWeaponCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputSecondaryWeaponCvar.Value = value;
			}
		}

		public static ConfigurableInput InputTertiaryWeapon
		{
			get { return InputTertiaryWeaponCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputTertiaryWeaponCvar.Value = value;
			}
		}

		public static ConfigurableInput InputQuaternaryWeapon
		{
			get { return InputQuaternaryWeaponCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputQuaternaryWeaponCvar.Value = value;
			}
		}

		public static ConfigurableInput InputChat
		{
			get { return InputChatCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				InputChatCvar.Value = value;
			}
		}

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

		public static bool ShowDebugOverlay
		{
			get { return ShowDebugOverlayCvar.Value; }
			[DebuggerHidden]
			set
			{
				Assert.ArgumentNotNull((object)value);
				ShowDebugOverlayCvar.Value = value;
			}
		}

		/// <summary>
		///     Raised when the 'PlayerName' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<string> PlayerNameChanging
		{
			add { PlayerNameCvar.Changing += value; }
			remove { PlayerNameCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'PlayerName' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<string> PlayerNameChanged
		{
			add { PlayerNameCvar.Changed += value; }
			remove { PlayerNameCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'EventMessageDisplayTime' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<double> EventMessageDisplayTimeChanging
		{
			add { EventMessageDisplayTimeCvar.Changing += value; }
			remove { EventMessageDisplayTimeCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'EventMessageDisplayTime' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<double> EventMessageDisplayTimeChanged
		{
			add { EventMessageDisplayTimeCvar.Changed += value; }
			remove { EventMessageDisplayTimeCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'ChatMessageDisplayTime' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<double> ChatMessageDisplayTimeChanging
		{
			add { ChatMessageDisplayTimeCvar.Changing += value; }
			remove { ChatMessageDisplayTimeCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'ChatMessageDisplayTime' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<double> ChatMessageDisplayTimeChanged
		{
			add { ChatMessageDisplayTimeCvar.Changed += value; }
			remove { ChatMessageDisplayTimeCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputShowScoreboard' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputShowScoreboardChanging
		{
			add { InputShowScoreboardCvar.Changing += value; }
			remove { InputShowScoreboardCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputShowScoreboard' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputShowScoreboardChanged
		{
			add { InputShowScoreboardCvar.Changed += value; }
			remove { InputShowScoreboardCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputRespawn' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputRespawnChanging
		{
			add { InputRespawnCvar.Changing += value; }
			remove { InputRespawnCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputRespawn' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputRespawnChanged
		{
			add { InputRespawnCvar.Changed += value; }
			remove { InputRespawnCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputForward' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputForwardChanging
		{
			add { InputForwardCvar.Changing += value; }
			remove { InputForwardCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputForward' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputForwardChanged
		{
			add { InputForwardCvar.Changed += value; }
			remove { InputForwardCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputBackward' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputBackwardChanging
		{
			add { InputBackwardCvar.Changing += value; }
			remove { InputBackwardCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputBackward' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputBackwardChanged
		{
			add { InputBackwardCvar.Changed += value; }
			remove { InputBackwardCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputStrafeLeft' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputStrafeLeftChanging
		{
			add { InputStrafeLeftCvar.Changing += value; }
			remove { InputStrafeLeftCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputStrafeLeft' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputStrafeLeftChanged
		{
			add { InputStrafeLeftCvar.Changed += value; }
			remove { InputStrafeLeftCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputStrafeRight' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputStrafeRightChanging
		{
			add { InputStrafeRightCvar.Changing += value; }
			remove { InputStrafeRightCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputStrafeRight' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputStrafeRightChanged
		{
			add { InputStrafeRightCvar.Changed += value; }
			remove { InputStrafeRightCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputPrimaryWeapon' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputPrimaryWeaponChanging
		{
			add { InputPrimaryWeaponCvar.Changing += value; }
			remove { InputPrimaryWeaponCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputPrimaryWeapon' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputPrimaryWeaponChanged
		{
			add { InputPrimaryWeaponCvar.Changed += value; }
			remove { InputPrimaryWeaponCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputSecondaryWeapon' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputSecondaryWeaponChanging
		{
			add { InputSecondaryWeaponCvar.Changing += value; }
			remove { InputSecondaryWeaponCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputSecondaryWeapon' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputSecondaryWeaponChanged
		{
			add { InputSecondaryWeaponCvar.Changed += value; }
			remove { InputSecondaryWeaponCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputTertiaryWeapon' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputTertiaryWeaponChanging
		{
			add { InputTertiaryWeaponCvar.Changing += value; }
			remove { InputTertiaryWeaponCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputTertiaryWeapon' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputTertiaryWeaponChanged
		{
			add { InputTertiaryWeaponCvar.Changed += value; }
			remove { InputTertiaryWeaponCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputQuaternaryWeapon' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputQuaternaryWeaponChanging
		{
			add { InputQuaternaryWeaponCvar.Changing += value; }
			remove { InputQuaternaryWeaponCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputQuaternaryWeapon' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputQuaternaryWeaponChanged
		{
			add { InputQuaternaryWeaponCvar.Changed += value; }
			remove { InputQuaternaryWeaponCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'InputChat' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputChatChanging
		{
			add { InputChatCvar.Changing += value; }
			remove { InputChatCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'InputChat' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<ConfigurableInput> InputChatChanged
		{
			add { InputChatCvar.Changed += value; }
			remove { InputChatCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'TimeScale' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<double> TimeScaleChanging
		{
			add { TimeScaleCvar.Changing += value; }
			remove { TimeScaleCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'TimeScale' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<double> TimeScaleChanged
		{
			add { TimeScaleCvar.Changed += value; }
			remove { TimeScaleCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'Resolution' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<Size> ResolutionChanging
		{
			add { ResolutionCvar.Changing += value; }
			remove { ResolutionCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'Resolution' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<Size> ResolutionChanged
		{
			add { ResolutionCvar.Changed += value; }
			remove { ResolutionCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'WindowSize' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<Size> WindowSizeChanging
		{
			add { WindowSizeCvar.Changing += value; }
			remove { WindowSizeCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'WindowSize' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<Size> WindowSizeChanged
		{
			add { WindowSizeCvar.Changed += value; }
			remove { WindowSizeCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'WindowPosition' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<Vector2i> WindowPositionChanging
		{
			add { WindowPositionCvar.Changing += value; }
			remove { WindowPositionCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'WindowPosition' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<Vector2i> WindowPositionChanged
		{
			add { WindowPositionCvar.Changed += value; }
			remove { WindowPositionCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'WindowMode' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<WindowMode> WindowModeChanging
		{
			add { WindowModeCvar.Changing += value; }
			remove { WindowModeCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'WindowMode' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<WindowMode> WindowModeChanged
		{
			add { WindowModeCvar.Changed += value; }
			remove { WindowModeCvar.Changed -= value; }
		}

		/// <summary>
		///     Raised when the 'ShowDebugOverlay' cvar is changing. The new value is passed to the event handler.
		/// </summary>
		public static event Action<bool> ShowDebugOverlayChanging
		{
			add { ShowDebugOverlayCvar.Changing += value; }
			remove { ShowDebugOverlayCvar.Changing -= value; }
		}

		/// <summary>
		///     Raised when the 'ShowDebugOverlay' cvar is changed. The previous value is passed to the event handler.
		/// </summary>
		public static event Action<bool> ShowDebugOverlayChanged
		{
			add { ShowDebugOverlayCvar.Changed += value; }
			remove { ShowDebugOverlayCvar.Changed -= value; }
		}

		/// <summary>
		///     Initializes the instances declared by the registry.
		/// </summary>
		public static void Initialize()
		{
			PlayerNameCvar = new Cvar<string>("player_name", "UnnamedPlayer", "", UpdateMode.Immediate, true, false, new NotEmptyAttribute(), new MaximumLengthAttribute(Specification.PlayerNameLength, true));
			EventMessageDisplayTimeCvar = new Cvar<double>("event_message_display_time", 3, "", UpdateMode.Immediate, true, false, new RangeAttribute(0.5, 60.0));
			ChatMessageDisplayTimeCvar = new Cvar<double>("chat_message_display_time", 6, "", UpdateMode.Immediate, true, false, new RangeAttribute(0.5, 60.0));
			InputShowScoreboardCvar = new Cvar<ConfigurableInput>("input_show_scoreboard", Key.Tab, "", UpdateMode.Immediate, true, false);
			InputRespawnCvar = new Cvar<ConfigurableInput>("input_respawn", MouseButton.Left, "", UpdateMode.Immediate, true, false);
			InputForwardCvar = new Cvar<ConfigurableInput>("input_forward", Key.W, "", UpdateMode.Immediate, true, false);
			InputBackwardCvar = new Cvar<ConfigurableInput>("input_backward", Key.S, "", UpdateMode.Immediate, true, false);
			InputStrafeLeftCvar = new Cvar<ConfigurableInput>("input_strafe_left", Key.A, "", UpdateMode.Immediate, true, false);
			InputStrafeRightCvar = new Cvar<ConfigurableInput>("input_strafe_right", Key.D, "", UpdateMode.Immediate, true, false);
			InputPrimaryWeaponCvar = new Cvar<ConfigurableInput>("input_primary_weapon", MouseButton.Left, "", UpdateMode.Immediate, true, false);
			InputSecondaryWeaponCvar = new Cvar<ConfigurableInput>("input_secondary_weapon", MouseButton.Right, "", UpdateMode.Immediate, true, false);
			InputTertiaryWeaponCvar = new Cvar<ConfigurableInput>("input_tertiary_weapon", Key.Num1, "", UpdateMode.Immediate, true, false);
			InputQuaternaryWeaponCvar = new Cvar<ConfigurableInput>("input_quaternary_weapon", Key.Num2, "", UpdateMode.Immediate, true, false);
			InputChatCvar = new Cvar<ConfigurableInput>("input_chat", Key.Return, "", UpdateMode.Immediate, true, false);

			CvarRegistry.Register(PlayerNameCvar);
			CvarRegistry.Register(EventMessageDisplayTimeCvar);
			CvarRegistry.Register(ChatMessageDisplayTimeCvar);
			CvarRegistry.Register(InputShowScoreboardCvar);
			CvarRegistry.Register(InputRespawnCvar);
			CvarRegistry.Register(InputForwardCvar);
			CvarRegistry.Register(InputBackwardCvar);
			CvarRegistry.Register(InputStrafeLeftCvar);
			CvarRegistry.Register(InputStrafeRightCvar);
			CvarRegistry.Register(InputPrimaryWeaponCvar);
			CvarRegistry.Register(InputSecondaryWeaponCvar);
			CvarRegistry.Register(InputTertiaryWeaponCvar);
			CvarRegistry.Register(InputQuaternaryWeaponCvar);
			CvarRegistry.Register(InputChatCvar);
		}

		/// <summary>
		///     Initializes the instances imported by the registry.
		/// </summary>
		public static void Resolve()
		{
			TimeScaleCvar = CvarRegistry.Resolve<double>("time_scale");
			ResolutionCvar = CvarRegistry.Resolve<Size>("resolution");
			WindowSizeCvar = CvarRegistry.Resolve<Size>("window_size");
			WindowPositionCvar = CvarRegistry.Resolve<Vector2i>("window_position");
			WindowModeCvar = CvarRegistry.Resolve<WindowMode>("window_mode");
			ShowDebugOverlayCvar = CvarRegistry.Resolve<bool>("show_debug_overlay");
		}
	}
}

