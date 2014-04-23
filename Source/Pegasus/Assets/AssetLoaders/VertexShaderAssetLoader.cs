namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Loads vertex shader assets.
	/// </summary>
	public class VertexShaderAssetLoader : ShaderAssetLoader
	{
		/// <summary>
		///     Gets the type of the asset supported by the loader.
		/// </summary>
		public override byte AssetType
		{
			get { return (byte)Assets.AssetType.VertexShader; }
		}

		/// <summary>
		///     Gets the name of the asset type supported by the loader.
		/// </summary>
		public override string AssetTypeName
		{
			get { return "Vertex Shader"; }
		}

		/// <summary>
		///     Loads the asset data into the given asset.
		/// </summary>
		/// <param name="buffer">The buffer the asset data should be read from.</param>
		/// <param name="asset">The asset instance that should be reinitialized with the loaded data.</param>
		/// <param name="assetName">The name of the asset.</param>
		public override void Load(BufferReader buffer, object asset, string assetName)
		{
			Load(buffer, (VertexShader)asset, assetName);
		}

		/// <summary>
		///     Loads the asset data into the given asset.
		/// </summary>
		/// <param name="buffer">The buffer the asset data should be read from.</param>
		/// <param name="shader">The asset instance that should be reinitialized with the loaded data.</param>
		/// <param name="assetName">The name of the asset.</param>
		public static unsafe void Load(BufferReader buffer, VertexShader shader, string assetName)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentNotNull(shader);
			Assert.ArgumentNotNullOrWhitespace(assetName);

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

			shader.Reinitialize(data, length, inputs);
			shader.SetName(assetName);
		}

		/// <summary>
		///     Allocates a new asset.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to allocate the asset.</param>
		public override IDisposable Allocate(GraphicsDevice graphicsDevice)
		{
			return new VertexShader(graphicsDevice);
		}
	}
}