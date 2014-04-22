﻿namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Security;

	/// <summary>
	///     Represents a shader that controls a programmable stage of the graphics pipeline.
	/// </summary>
	internal abstract class Shader : GraphicsObject
	{
		/// <summary>
		///     The native shader instance.
		/// </summary>
		protected IntPtr _shader;

		/// <summary>
		///     Gets the native shader instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get { return _shader; }
		}

		/// <summary>
		///     Raised when the underlying native shader instance has been reinitialized.
		/// </summary>
		public event Action Reinitialized;

		/// <summary>
		///     Destroys the native shader instance.
		/// </summary>
		protected void DestroyShader()
		{
			NativeMethods.DestroyShader(_shader);
			_shader = IntPtr.Zero;
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyShader(_shader);
		}

		/// <summary>
		///     Raises the reinitialization event.
		/// </summary>
		protected void OnReinitialized()
		{
			if (Reinitialized != null)
				Reinitialized();
		}

#if DEBUG
		/// <summary>
		///     Invoked after the name of the graphics object has changed. This method is only available in debug builds.
		/// </summary>
		protected override void OnRenamed()
		{
			if (_shader != IntPtr.Zero)
				NativeMethods.SetName(_shader, Name);
		}
#endif

		/// <summary>
		///     Provides access to the native shader functions.
		/// </summary>
#if !DEBUG
		[SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyShader")]
			public static extern void DestroyShader(IntPtr shader);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetShaderName")]
			[Conditional("DEBUG")]
			public static extern void SetName(IntPtr texture, string name);
		}
	}
}