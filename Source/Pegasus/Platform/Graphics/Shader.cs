﻿namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;

	/// <summary>
	///     Represents a shader that controls a programmable stage of the graphics pipeline.
	/// </summary>
	public abstract class Shader : GraphicsObject
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
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		protected Shader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

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