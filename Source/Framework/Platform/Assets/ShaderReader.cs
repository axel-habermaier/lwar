using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;

	/// <summary>
	///   Loads compiled shaders.
	/// </summary>
	internal sealed class ShaderReader
	{
		/// <summary>
		///   The graphics device that should be used to create the shaders.
		/// </summary>
		private readonly GraphicsDevice _device;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="device">The graphics device that should be used to create the shaders.</param>
		public ShaderReader(GraphicsDevice device)
		{
			_device = device;
		}

		/// <summary>
		///   Loads a vertex shader.
		/// </summary>
		/// <param name="reader">The asset reader that should be used to load the shader.</param>
		public VertexShader LoadVertexShader(AssetReader reader)
		{
			return new VertexShader(_device, reader.ReadAll());
		}

		/// <summary>
		///   Loads a fragment shader.
		/// </summary>
		/// <param name="reader">The asset reader that should be used to load the shader.</param>
		public FragmentShader LoadFragmentShader(AssetReader reader)
		{
			return new FragmentShader(_device, reader.ReadAll());
		}
	}
}