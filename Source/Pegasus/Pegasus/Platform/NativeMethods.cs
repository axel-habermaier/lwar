namespace Pegasus.Platform
{
	using System;
	using System.Runtime.InteropServices;
	using System.Security;
	using Graphics;
	using Logging;
	using Network;
	using UserInterface;
	using Utilities;

	/// <summary>
	///     Provides access to native functions.
	/// </summary>
	[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
	[SuppressUnmanagedCodeSecurity]
	internal static unsafe class NativeMethods
	{
		#region Callbacks

		[StructLayout(LayoutKind.Sequential)]
		public struct WindowCallbacks
		{
			public KeyPressedCallback KeyPressed;
			public KeyReleasedCallback KeyReleased;
			public MouseMovedCallback MouseMoved;
			public MousePressedCallback MousePressed;
			public MouseReleasedCallback MouseReleased;
			public MouseWheelCallback MouseWheel;
			public TextEnteredCallback TextEntered;
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void LogCallback(LogType logType, string message);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void KeyPressedCallback(int key, int scanCode);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void KeyReleasedCallback(int key, int scanCode);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void MouseMovedCallback(int x, int y);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void MousePressedCallback(int button, [MarshalAs(UnmanagedType.I1)] bool doubleClick, int x, int y);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void MouseReleasedCallback(int button, int x, int y);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void MouseWheelCallback(int delta);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		public delegate void TextEnteredCallback(byte* text);

		#endregion

		#region Library

		[DllImport(PlatformLibrary.Name)]
		internal static extern void Initialize(LogCallback logCallback);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void Shutdown();

		#endregion

		#region Graphics

		[DllImport(PlatformLibrary.Name)]
		internal static extern DeviceInterface* CreateDeviceInterface(GraphicsApi graphicsApi);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void FreeDeviceInterface(DeviceInterface* deviceInterface);

		[DllImport(PlatformLibrary.Name)]
		internal static extern int GetConstantBufferSlotCount();

		[DllImport(PlatformLibrary.Name)]
		internal static extern int GetTextureSlotCount();

		[DllImport(PlatformLibrary.Name)]
		internal static extern int GetMaxColorBuffers();

		[DllImport(PlatformLibrary.Name)]
		internal static extern int GetMaxMipmaps();

		[DllImport(PlatformLibrary.Name)]
		internal static extern int GetMaxTextureSize();

		[DllImport(PlatformLibrary.Name)]
		internal static extern int GetMaxSurfaceCount();

		[DllImport(PlatformLibrary.Name)]
		internal static extern int GetMaxVertexBindings();

		[DllImport(PlatformLibrary.Name)]
		internal static extern void EnableVsync([MarshalAs(UnmanagedType.I1)] bool enable);

		#endregion

		#region Network

		[DllImport(PlatformLibrary.Name)]
		internal static extern UdpInterface* CreateUdpInterface();

		[DllImport(PlatformLibrary.Name)]
		internal static extern void FreeUdpInterface(UdpInterface* udpInterface);

		#endregion

		#region Keyboard

		[DllImport(PlatformLibrary.Name)]
		internal static extern void EnableTextInput([MarshalAs(UnmanagedType.I1)] bool enable);

		[DllImport(PlatformLibrary.Name)]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool IsTextInputEnabled();

		[DllImport(PlatformLibrary.Name)]
		internal static extern void SetTextInputArea(int left, int top, int width, int height);

		[DllImport(PlatformLibrary.Name)]
		internal static extern uint GetScanCodeCount();

		[DllImport(PlatformLibrary.Name)]
		internal static extern int KeyToScanCode(int key);

		#endregion

		#region Mouse

		[DllImport(PlatformLibrary.Name)]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool IsRelativeMouseModeEnabled();

		[DllImport(PlatformLibrary.Name)]
		internal static extern void ShowHardwareCursor([MarshalAs(UnmanagedType.I1)] bool show);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void EnableRelativeMouseMode([MarshalAs(UnmanagedType.I1)] bool enable);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void GetMousePosition(void* window, int* x, int* y);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void* CreateHardwareCursor(Surface* surface, int hotspotX, int hotspotY);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void SetHardwareCursor(void* cursor);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void FreeHardwareCursor(void* cursor);

		#endregion

		#region Window

		[DllImport(PlatformLibrary.Name)]
		internal static extern WindowInterface* CreateWindowInterface(ref WindowCallbacks callbacks);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void FreeWindowInterface(WindowInterface* windowInterface);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void GetSupportedWindowDimensions(int* minWidth, int* minHeight, int* maxWidth, int* maxHeight);

		#endregion

		#region Other

		[DllImport(PlatformLibrary.Name)]
		internal static extern void ShowMessageBox(string caption, string message);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void HandleEventMessages();

		[DllImport(PlatformLibrary.Name)]
		internal static extern double GetTime();

		[DllImport(PlatformLibrary.Name)]
		internal static extern void ValidateInterfaceSizes(int deviceInterface, int udpInterface, int windowInterface);

		[DllImport(PlatformLibrary.Name)]
		internal static extern void ValidateStructSizes(int blendDescription, int bufferDescription, int color,
														int depthStencilDescription, int rasterizerDescription, int samplerDescription,
														int stencilOperationDescription, int surface, int textureDescription,
														int timestampDisjointQueryResult, int vertexBinding,
														int vertexLayoutDescription, int windowCallbacks);

		#endregion
	}
}