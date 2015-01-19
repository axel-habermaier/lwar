namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using System.Runtime.InteropServices;
	using Bindings;
	using Interface;
	using Logging;

	/// <summary>
	///     Represents an OpenGL3-based shader that controls a programmable stage of the graphics pipeline.
	/// </summary>
	internal unsafe class ShaderGL3 : GraphicsObjectGL3, IShader
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="shaderType">The type of the shader.</param>
		/// <param name="byteCode">The shader byte code.</param>
		/// <param name="byteCodeLength">The length of the byte code in bytes.</param>
		public ShaderGL3(GraphicsDeviceGL3 graphicsDevice, ShaderType shaderType, IntPtr byteCode, int byteCodeLength)
			: base(graphicsDevice)
		{
			const int bufferSize = 4096;
			var buffer = stackalloc byte[bufferSize];

			uint type;
			switch (shaderType)
			{
				case ShaderType.VertexShader:
					type = GL.VertexShader;
					break;
				case ShaderType.FragmentShader:
					type = GL.FragmentShader;
					break;
				default:
					throw new InvalidProgramException("Unsupported shader type.");
			}

			Handle = _gl.CreateShader(type);
			if (Handle == 0)
				Log.Die("Failed to create OpenGL shader object.");

			var shaderLength = byteCodeLength;
			var shaderCode = (byte*)byteCode;
			_gl.ShaderSource(Handle, 1, &shaderCode, &shaderLength);
			_gl.CompileShader(Handle);

			int success, logLength;
			_gl.GetShaderiv(Handle, GL.CompileStatus, &success);
			_gl.GetShaderInfoLog(Handle, bufferSize, &logLength, buffer);

			if (success == GL.False)
				Log.Die("Shader compilation failed: {0}", Marshal.PtrToStringAnsi(new IntPtr(buffer)).Trim());

			if (logLength != 0)
				Log.Warn("{0}", Marshal.PtrToStringAnsi(new IntPtr(buffer)).Trim());
		}

		/// <summary>
		///     Gets the native shader object.
		/// </summary>
		internal uint Handle { get; private set; }

		/// <summary>
		///     Sets the debug name of the shader.
		/// </summary>
		/// <param name="name">The debug name of the shader.</param>
		public void SetName(string name)
		{
			// Not supported by OpenGL
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_gl.DeleteShader(Handle);
		}
	}
}