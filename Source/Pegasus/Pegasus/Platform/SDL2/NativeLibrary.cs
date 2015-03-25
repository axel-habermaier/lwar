namespace Pegasus.Platform.SDL2
{
	using System;
	using System.Runtime.InteropServices;
	using Graphics.OpenGL3.Bindings;
	using Logging;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents an instance of the SDL2 native library.
	/// </summary>
	internal class NativeLibrary : DisposableObject
	{
		/// <summary>
		///     The name of the SDL2 dynamic link library.
		/// </summary>
		public const string Name = "SDL2.dll";

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public NativeLibrary()
		{
			Assert.That(!IsInitialized, "SDL2 has already been initialized.");

			try
			{
				const int initVideo = 0x20;
				if (SDL_Init(initVideo) != 0)
					Log.Die("SDL initialization failed: {0}.", GetError());

				Clock.Initialize();
			}
			catch (DllNotFoundException)
			{
				Log.Die("Unable to load SDL2. Make sure 'SDL2.dll' (or the platform-specific equivalent) is installed on your system.");
			}

			if (VersionInfo.Actual < VersionInfo.Required)
				Log.Die("SDL2 is outdated: Version {0} is installed but at version {1} is required.", VersionInfo.Actual, VersionInfo.Required);

			SDL_StopTextInput();
			GLContext.InitializeAttributes();
			IsInitialized = true;
		}

		/// <summary>
		///     Gets a value indicating whether the SDL2 library is initialized.
		/// </summary>
		internal static bool IsInitialized { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			SDL_Quit();
			IsInitialized = false;
		}

		/// <summary>
		///     Gets a description of the last platform error.
		/// </summary>
		public static string GetError()
		{
			var error = SDL_GetError().Trim();
			if (error.EndsWith("."))
				return error.Substring(0, error.Length - 1);

			return error;
		}

		[DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_Init(uint flags);

		[DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_Quit();

		[DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(StringMarshaler), MarshalCookie = StringMarshaler.NoFree)]
		private static extern string SDL_GetError();

		[DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_StopTextInput();
	}
}