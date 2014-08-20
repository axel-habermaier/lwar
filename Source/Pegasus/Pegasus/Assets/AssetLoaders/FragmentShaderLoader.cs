namespace Pegasus.Assets.AssetLoaders
{
	using System;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Loads fragment shader assets.
	/// </summary>
	public class FragmentShaderLoader : ShaderLoader
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public FragmentShaderLoader()
		{
			AssetType = (byte)Assets.AssetType.FragmentShader;
			AssetTypeName = "Fragment Shader";
		}

		/// <summary>
		///     Loads the asset data into the given asset.
		/// </summary>
		/// <param name="buffer">The buffer the asset data should be read from.</param>
		/// <param name="asset">The asset instance that should be reinitialized with the loaded data.</param>
		/// <param name="assetName">The name of the asset.</param>
		public override void Load(BufferReader buffer, object asset, string assetName)
		{
			Load(buffer, asset as FragmentShader, assetName);
		}

		/// <summary>
		///     Loads the asset data into the given asset.
		/// </summary>
		/// <param name="buffer">The buffer the asset data should be read from.</param>
		/// <param name="shader">The asset instance that should be reinitialized with the loaded data.</param>
		/// <param name="assetName">The name of the asset.</param>
		public static unsafe void Load(BufferReader buffer, FragmentShader shader, string assetName)
		{
			Assert.ArgumentNotNull(buffer);
			Assert.ArgumentNotNull(shader);
			Assert.ArgumentNotNullOrWhitespace(assetName);

			byte* data;
			int length;
			ExtractShaderCode(buffer, out data, out length);

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
			var shader = new FragmentShader(graphicsDevice);
			shader.SetName(assetName);
			return shader;
		}
	}
}