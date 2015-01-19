namespace Pegasus.UserInterface
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Platform;
	using Platform.Logging;
	using Platform.SDL2;
	using Utilities;
	using ViewModels;

	/// <summary>
	///     Provides convenience methods for displaying message boxes.
	/// </summary>
	public static class MessageBox
	{
		/// <summary>
		///     The message boxes that are currently open.
		/// </summary>
		private static readonly List<MessageBoxViewModel> MessageBoxes = new List<MessageBoxViewModel>();

		/// <summary>
		///     Shows a message box with the given header and message.
		/// </summary>
		/// <param name="header">The header of the message box.</param>
		/// <param name="message">The message that the message box should display.</param>
		public static void Show(string header, string message)
		{
			Assert.ArgumentNotNullOrWhitespace(header);
			Assert.ArgumentNotNullOrWhitespace(message);

			var messageBox = new MessageBoxViewModel(header, message);
			messageBox.Closed += OnClosed;
			messageBox.Show();

			MessageBoxes.Add(messageBox);
		}

		/// <summary>
		///     Closes all open message boxes.
		/// </summary>
		public static void CloseAll()
		{
			while (MessageBoxes.Count > 0)
				MessageBoxes[0].Close();
		}

		/// <summary>
		///     Removes the given message box from the internal list.
		/// </summary>
		/// <param name="messageBox">The message box that has been closed.</param>
		private static void OnClosed(MessageBoxViewModel messageBox)
		{
			Assert.ArgumentNotNull(messageBox);
			Assert.ArgumentSatisfies(MessageBoxes.Contains(messageBox), "Unknown message box.");

			MessageBoxes.Remove(messageBox);
		}

		/// <summary>
		///     Shows an OS-specific message box with the given header and message.
		/// </summary>
		/// <param name="header">The header of the message box.</param>
		/// <param name="message">The message that the message box should display.</param>
		internal static unsafe void ShowNativeError(string header, string message)
		{
			Assert.ArgumentNotNullOrWhitespace(header);
			Assert.ArgumentNotNullOrWhitespace(message);

			if (!NativeLibrary.IsInitialized)
				return;

			// The native Windows message box looks better, so on Windows, use the native message box instead of the SDL2 one
			if (PlatformInfo.Platform == PlatformType.Windows)
				MessageBoxW(null, message, header, 0x10 /* MB_ICONERROR | MB_OK */);
			else if (SDL_ShowSimpleMessageBox(0x10 /* SDL_MESSAGEBOX_ERROR */, header, message, IntPtr.Zero) != 0)
				Log.Error("Failed to show message box: {0}.", NativeLibrary.GetError());
		}

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_ShowSimpleMessageBox(
			uint type,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string title,
			[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler))] string message, 
			IntPtr window);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern unsafe int MessageBoxW(void* hWnd, string text, string caption, uint type);
	}
}