namespace Pegasus.Platform.Assets
{
	using System;
	using Graphics;
	using Memory;

	/// <summary>
	///     Represents a fragment shader asset.
	/// </summary>
	internal sealed class FragmentShaderAsset : ShaderAsset<FragmentShader>
	{
		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		internal override AssetType Type
		{
			get { return AssetType.FragmentShader; }
		}

		/// <summary>
		///     Loads or reloads the asset using the given asset reader.
		/// </summary>
		/// <param name="buffer">The buffer that should be used to load the asset.</param>
		/// <param name="name">The name of the asset.</param>
		internal override unsafe void Load(BufferReader buffer, string name)
		{
			AssetHeader.Validate(buffer, AssetType.FragmentShader, name);

			if (Shader == null)
				Shader = new FragmentShader(GraphicsDevice);

			byte* data;
			int length;
			ExtractShaderCode(buffer, out data, out length);

			Shader.Reinitialize(data, length);
			Shader.SetName(name);
		}
	}
}