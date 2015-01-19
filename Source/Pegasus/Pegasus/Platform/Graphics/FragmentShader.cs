namespace Pegasus.Platform.Graphics
{
	using System;
	using Memory;
	using Utilities;

	/// <summary>
	///     A fragment shader is a program that controls the fragment-shader stage.
	/// </summary>
	public sealed class FragmentShader : Shader
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		private FragmentShader(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///     Loads a fragment shader from the given buffer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device the fragment shader should be created for.</param>
		/// <param name="buffer">The buffer the fragment shader should be read from.</param>
		public static FragmentShader Create(GraphicsDevice graphicsDevice, ref BufferReader buffer)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			var shader = new FragmentShader(graphicsDevice);
			shader.Load(ref buffer);
			return shader;
		}

		/// <summary>
		///     Loads the fragment shader from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the fragment shader should be read from.</param>
		public void Load(ref BufferReader buffer)
		{
			Load(ShaderType.FragmentShader, ref buffer);
		}
	}
}