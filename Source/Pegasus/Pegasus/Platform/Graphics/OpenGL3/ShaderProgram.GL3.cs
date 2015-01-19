namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using System.Runtime.InteropServices;
	using Bindings;
	using Interface;
	using Logging;

	/// <summary>
	///     Represents an OpenGL3-based combination of different shader programs that control the various pipeline
	///     stages of the GPU.
	/// </summary>
	internal unsafe class ShaderProgramGL3 : GraphicsObjectGL3, IShaderProgram
	{
		/// <summary>
		///     The native shader program instance.
		/// </summary>
		private readonly uint _handle;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="vertexShader">The vertex shader that should be used by the shader program.</param>
		/// <param name="fragmentShader">The fragment shader that should be used by the shader program.</param>
		public ShaderProgramGL3(GraphicsDeviceGL3 graphicsDevice, VertexShader vertexShader, FragmentShader fragmentShader)
			: base(graphicsDevice)
		{
			_handle = _gl.CreateProgram();
			if (_handle == 0)
				Log.Die("Failed to create OpenGL program object.");

			const int bufferSize = 4096;
			var buffer = stackalloc byte[bufferSize];

			_gl.AttachShader(_handle, ((ShaderGL3)vertexShader.ShaderObject).Handle);
			_gl.AttachShader(_handle, ((ShaderGL3)fragmentShader.ShaderObject).Handle);
			_gl.LinkProgram(_handle);

			int success, logLength;
			_gl.GetProgramiv(_handle, GL.LinkStatus, &success);
			_gl.GetProgramInfoLog(_handle, bufferSize, &logLength, buffer);

			if (success == GL.False)
				Log.Die("Program linking failed: {0}", Marshal.PtrToStringAnsi(new IntPtr(buffer)).Trim());

			if (logLength != 0)
				Log.Warn("{0}", Marshal.PtrToStringAnsi(new IntPtr(buffer)).Trim());

			_gl.DetachShader(_handle, ((ShaderGL3)vertexShader.ShaderObject).Handle);
			_gl.DetachShader(_handle, ((ShaderGL3)fragmentShader.ShaderObject).Handle);
		}

		/// <summary>
		///     Binds the shaders of the shader program to the various pipeline stages.
		/// </summary>
		public void Bind()
		{
			_gl.UseProgram(_handle);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_gl.DeleteProgram(_handle);
		}
	}
}