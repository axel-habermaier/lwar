using System;

namespace Pegasus.Framework.Platform.Assets
{
	using Graphics;

	/// <summary>
	///   Represents a fragment shader asset.
	/// </summary>
	internal sealed class FragmentShaderAsset : Asset
	{
		/// <summary>
		///   The fragment shader that is managed by this asset instance.
		/// </summary>
		internal FragmentShader Shader { get; private set; }

		/// <summary>
		///   Gets the friendly name of the asset.
		/// </summary>
		internal override string FriendlyName
		{
			get { return "Fragment Shader"; }
		}

		/// <summary>
		///   Loads or reloads the asset using the given asset reader.
		/// </summary>
		/// <param name="buffer">The buffer that should be used to load the asset.</param>
		internal override unsafe void Load(BufferReader buffer)
		{
			if (Shader == null)
				Shader = new FragmentShader(GraphicsDevice);

			Shader.Reinitialize(buffer.Pointer, buffer.BufferSize);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Shader.SafeDispose();
		}
	}
}