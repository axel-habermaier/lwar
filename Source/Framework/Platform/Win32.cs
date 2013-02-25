using System;

namespace Pegasus.Framework.Platform
{
	using System.Diagnostics;
	using System.Runtime.InteropServices;

	/// <summary>
	///   Provides access to the native Win32 API functions.
	/// </summary>
	public static class Win32
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "MessageBox")]
		private static extern int ShowMessage(IntPtr hWnd, String text, String caption, MessageBoxStyle options);

		[DllImport("kernel32.dll")]
		private static extern bool AttachConsole(int dwProcessId);

		[DllImport("kernel32.dll")]
		private static extern bool FreeConsole();

		/// <summary>
		///   Shows a Win32 native message box.
		/// </summary>
		/// <param name="caption">The caption of the message box.</param>
		/// <param name="text">The text that should be displayed in the message box.</param>
		/// <param name="arguments">The arguments for the text format string.</param>
		[Conditional("Windows")]
		public static void ShowMessage(string caption, string text, params object[] arguments)
		{
			Assert.ArgumentNotNullOrWhitespace(caption, () => text);
			Assert.ArgumentNotNullOrWhitespace(text, () => text);

			ShowMessage(IntPtr.Zero, String.Format(text, arguments), caption, MessageBoxStyle.IconError | MessageBoxStyle.Ok);
		}

		/// <summary>
		///   Attaches a console to the process so that the console output is visible.
		/// </summary>
		[Conditional("Windows")]
		public static void AttachConsole()
		{
			AttachConsole(-1);
		}

		/// <summary>
		///   Detaches the previously attached console from the process.
		/// </summary>
		[Conditional("Windows")]
		public static void DetachConsole()
		{
			FreeConsole();
		}

		/// <summary>
		///   Specifies the contents and behavior of the dialog box
		/// </summary>
		[Flags]
		private enum MessageBoxStyle : uint
		{
			Ok = 0,
			IconError = 16,
		}
	}
}