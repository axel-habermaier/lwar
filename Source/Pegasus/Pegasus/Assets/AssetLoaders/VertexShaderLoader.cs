namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Loads vertex shader assets.
	/// </summary>
	public class VertexShaderLoader : ShaderLoader
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public VertexShaderLoader()
		{
			AssetType = (byte)Assets.AssetType.VertexShader;
			AssetTypeName = "Vertex Shader";
		}

		/// <summary>
		///     Loads the asset data into the given asset.
		/// </summary>
		/// <param name="buffer">The buffer the asset data should be read from.</param>
		/// <param name="asset">The asset instance that should be reinitialized with the loaded data.</param>
		/// <param name="assetName">The name of the asset.</param>
		public override unsafe void Load(BufferReader buffer, object asset, string assetName)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentNotNull(asset);
			Assert.ArgumentOfType<VertexShader>(asset);
			Assert.ArgumentNotNullOrWhitespace(assetName);

			byte* data;
			int length;
			ExtractShaderCode(buffer, out data, out length);

			var shader = (VertexShader)asset;
			shader.Reinitialize(data, length);
			shader.SetName(assetName);
		}

		/// <summary>
		///     Allocates a new asset.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to allocate the asset.</param>
		/// <param name="assetName">The name of the asset.</param>
		public override IDisposable Allocate(GraphicsDevice graphicsDevice, string assetName)
		{
			var shader = new VertexShader(graphicsDevice);
			shader.SetName(assetName);
			return shader;
		}
	}
}