namespace Pegasus.Platform.Assets
{
	using System;
	using Graphics;
	using Memory;

	/// <summary>
	///     Represents a vertex shader asset.
	/// </summary>
	internal sealed class VertexShaderAsset : ShaderAsset<VertexShader>
	{
		/// <summary>
		///     Gets the type of the asset.
		/// </summary>
		internal override AssetType Type
		{
			get { return AssetType.VertexShader; }
		}

		/// <summary>
		///     Loads or reloads the asset using the given asset reader.
		/// </summary>
		/// <param name="buffer">The buffer that should be used to load the asset.</param>
		/// <param name="name">The name of the asset.</param>
		internal override unsafe void Load(BufferReader buffer, string name)
		{
			AssetHeader.Validate(buffer, AssetType.VertexShader, name);

			if (Shader == null)
				Shader = new VertexShader();

			var inputCount = buffer.ReadByte();
			var inputs = new ShaderInput[inputCount];

			for (var i = 0; i < inputCount; ++i)
			{
				inputs[i] = new ShaderInput
				{
					Format = (VertexDataFormat)buffer.ReadByte(),
					Semantics = (DataSemantics)buffer.ReadByte()
				};
			}

			byte* data;
			int length;
			ExtractShaderCode(buffer, out data, out length);

			Shader.Reinitialize(data, length, inputs);
			Shader.SetName(name);
		}
	}
}