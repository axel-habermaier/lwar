namespace Pegasus.Platform.Graphics.OpenGL3.Bindings
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using Logging;
	using Math;
	using Memory;
	using SDL2;
	using UserInterface;
	using Utilities;

	/// <summary>
	///     Represents the OpenGL context.
	/// </summary>
	internal sealed unsafe class GLContext : DisposableObject
	{
		/// <summary>
		///     Represents a pointer to an OpenGL allocation function.
		/// </summary>
		/// <param name="count">The number of objects that should be allocated.</param>
		/// <param name="objects">A pointer to the allocated objects.</param>
		public delegate void Allocator(int count, uint* objects);

		/// <summary>
		///     Represents a pointer to an OpenGL deallocation function.
		/// </summary>
		/// <param name="count">The number of objects that should be deallocated.</param>
		/// <param name="objects">A pointer to the objects that should be deallocated.</param>
		public delegate void Deallocator(int count, uint* objects);

		/// <summary>
		///     The native OpenGL context that the graphics device represents.
		/// </summary>
		private readonly IntPtr _context;

		/// <summary>
		///     The native window instance that was used to initialize the OpenGL context.
		/// </summary>
		private readonly NativeWindow _contextWindow;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public GLContext()
		{
			_contextWindow = new NativeWindow("CtxWnd", Vector2.Zero, new Size(), WindowFlags.Hidden | WindowFlags.OpenGL);
			if (_contextWindow == null)
				Log.Die("Failed to create the OpenGL context window: {0}.", NativeLibrary.GetError());

			_context = SDL_GL_CreateContext(_contextWindow.NativePtr);
			if (_context == IntPtr.Zero)
				Log.Die("Failed to initialize the OpenGL context. OpenGL 3.3 is not supported by your graphics card.");

			MakeCurrent();
			EntryPoints = new EntryPoints(CheckErrors, LoadEntryPoint);

			int major, minor;
			EntryPoints.GetIntegerv(GL.MajorVersion, &major);
			EntryPoints.GetIntegerv(GL.MinorVersion, &minor);

			if (major < 3 || (major == 3 && minor < 3))
				Log.Die("Only OpenGL {0}.{1} seems to be supported. OpenGL 3.3 is required.", major, minor);

			var glExtsSupported = true;
			glExtsSupported &= IsExtensionSupported("GL_ARB_shading_language_420pack");
			glExtsSupported &= IsExtensionSupported("GL_ARB_base_instance");
			glExtsSupported &= IsExtensionSupported("GL_EXT_texture_filter_anisotropic");
			glExtsSupported &= IsExtensionSupported("GL_EXT_texture_compression_s3tc");

			if (!glExtsSupported)
				Log.Die("Incompatible graphics card. Not all required OpenGL extensions are supported.");

			EntryPoints.Load();
		}

		/// <summary>
		///     Gets the context's entry points.
		/// </summary>
		public EntryPoints EntryPoints { get; private set; }

		/// <summary>
		///     Initializes the OpenGL context attributes.
		/// </summary>
		internal static void InitializeAttributes()
		{
			SDL_GL_SetAttribute(ContextAttribute.RedSize, 8);
			SDL_GL_SetAttribute(ContextAttribute.GreenSize, 8);
			SDL_GL_SetAttribute(ContextAttribute.BlueSize, 8);
			SDL_GL_SetAttribute(ContextAttribute.DepthSize, 0);
			SDL_GL_SetAttribute(ContextAttribute.StencilSize, 0);
			SDL_GL_SetAttribute(ContextAttribute.MajorVersion, 3);
			SDL_GL_SetAttribute(ContextAttribute.MinorVersion, 3);
			SDL_GL_SetAttribute(ContextAttribute.ProfileMask, ContextProfile.Core);

			var contextFlags = ContextFlags.ForwardCompatible;
#if DEBUG
			contextFlags |= ContextFlags.Debug;
#endif
			SDL_GL_SetAttribute(ContextAttribute.ContextFlags, contextFlags);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			SDL_GL_DeleteContext(_context);
			_contextWindow.SafeDispose();
		}

		/// <summary>
		///     Loads an OpenGL entry point of the given name.
		/// </summary>
		/// <param name="entryPoint">The name of the entry point that should be loaded.</param>
		private static IntPtr LoadEntryPoint(string entryPoint)
		{
			var function = SDL_GL_GetProcAddress(entryPoint);

			// Stupid, but might be necessary; see also https://www.opengl.org/wiki/Load_OpenGL_Functions
			if ((long)function >= -1 && (long)function <= 3)
				Log.Die("Failed to load OpenGL entry point '{0}'.", entryPoint);

			return function;
		}

		/// <summary>
		///     Allocates an OpenGL object using the given allocator.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocate the object.</param>
		/// <param name="type">A user-friendly name of the type of the allocated object.</param>
		public static uint Allocate(Allocator allocator, string type)
		{
			Assert.ArgumentNotNull(allocator);
			Assert.ArgumentNotNullOrWhitespace(type);

			uint handle = 0;
			allocator(1, &handle);

			if (handle == 0)
				Log.Die("Failed to allocate an OpenGL object of type '{0}'.", type);

			return handle;
		}

		/// <summary>
		///     Deallocates an OpenGL object using the given deallocator.
		/// </summary>
		/// <param name="deallocator">The deallocator that should be used to allocate the object.</param>
		/// <param name="obj">The object that should be deallocated.</param>
		public static void Deallocate(Deallocator deallocator, uint obj)
		{
			Assert.ArgumentNotNull(deallocator);
			deallocator(1, &obj);
		}

		/// <summary>
		///     Makes the given OpenGL context the current one for the given window.
		/// </summary>
		/// <param name="window">
		///     The window that should be made the current one. If null, the internal context window is made the
		///     current one.
		/// </param>
		public void MakeCurrent(NativeWindow window = null)
		{
			if (SDL_GL_MakeCurrent(window == null ? _contextWindow.NativePtr : window.NativePtr, _context) != 0)
				Log.Die("Failed to make OpenGL context current: {0}.", NativeLibrary.GetError());
		}

		/// <summary>
		///     Checks whether the OpenGL extension with the given name is supported by the current OpenGL context.
		/// </summary>
		/// <param name="extensionName">The name of the extension that should be checked.</param>
		private static bool IsExtensionSupported(string extensionName)
		{
			if (SDL_GL_ExtensionSupported(extensionName))
				return true;

			Log.Error("Extension '{0}' is not supported.", extensionName);
			return false;
		}

		/// <summary>
		///     In debug builds, checks for OpenGL errors.
		/// </summary>
		[DebuggerHidden]
		private void CheckErrors()
		{
			var glErrorOccurred = false;
			for (var glError = EntryPoints.GetError(); glError != GL.NoError; glError = EntryPoints.GetError())
			{
				string msg;
				switch (glError)
				{
					case GL.InvalidEnum:
						msg = "GL_INVALID_ENUM";
						break;
					case GL.InvalidValue:
						msg = "GL_INVALID_VALUE";
						break;
					case GL.InvalidOperation:
						msg = "GL_INVALID_OPERATION";
						break;
					case GL.OutOfMemory:
						msg = "GL_OUT_OF_MEMORY";
						break;
					default:
						msg = String.Format("Unknown (id {0})", glError);
						break;
				}

				Log.Error("OpenGL error: {0}", msg);
				glErrorOccurred = true;
			}

			if (glErrorOccurred)
				Log.Die("Stopped after OpenGL error.");
		}

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_GL_GetProcAddress(string functionName);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_GL_CreateContext(IntPtr window);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern bool SDL_GL_ExtensionSupported(string extensionName);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_GL_MakeCurrent(IntPtr window, IntPtr context);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_GL_DeleteContext(IntPtr context);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_GL_SetAttribute(ContextAttribute attr, ContextFlags value);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_GL_SetAttribute(ContextAttribute attr, ContextProfile value);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SDL_GL_SetAttribute(ContextAttribute attr, int value);
	}
}