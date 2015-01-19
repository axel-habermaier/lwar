namespace Pegasus.Platform.SDL2
{
	using System;

	/// <summary>
	///     Describes the type of an SDL event.
	/// </summary>
	internal enum EventType
	{
		Quit = 0x100,
		Window = 0x200,
		System,
		KeyDown = 0x300,
		KeyUp,
		TextEditing,
		TextInput,
		MouseMotion = 0x400,
		MouseButtonDown,
		MouseButtonUp,
		MouseWheel,
		JoystickAxisMotion = 0x600,
		JoystickBallMotion,
		JoystickHatMotion,
		JoystickButtonDown,
		JoystickButtonUp,
		JoystickDeviceAdded,
		JoystickDeviceRemoved,
		ControllerAxisMotion = 0x650,
		ControllerButtonDown,
		ControllerButtonUp,
		ControllerDeviceAdded,
		ControllerDeviceRemoved,
		ControllerDeviceRemapped,
		FingerDown = 0x700,
		FingerUp,
		FingerMotion,
		DollarGesture = 0x800,
		DollarRecord,
		MultiGesture,
		ClipboardUpdate = 0x900,
		DropFile = 0x1000,
		RenderTargetsReset = 0x2000,
		UserEvent = 0x8000
	}
}