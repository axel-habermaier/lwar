using System;

namespace Pegasus.Framework.Platform.Assets
{
    using Graphics;
    using Math;

	/// <summary>
    ///   Loads compiled textures.
    /// </summary>
    internal sealed class TextureReader
    {
        /// <summary>
        ///   The graphics device that should be used to create the textures.
        /// </summary>
        private readonly GraphicsDevice _device;

        /// <summary>
        ///   Initializes a new instance.
        /// </summary>
        /// <param name="device">The graphics device that should be used to create the textures.</param>
        public TextureReader(GraphicsDevice device)
        {
            _device = device;
        }

        /// <summary>
        ///   Loads a texture.
        /// </summary>
		/// <param name="assetReader">The asset reader that should be used to load the texture.</param>
		public Texture2D Load(AssetReader assetReader)
        {
	        var reader = assetReader.Reader;
            var width = reader.ReadUInt16();
			var height = reader.ReadUInt16();
			var componentCount = reader.ReadByte();

            var length = width * height * componentCount;
            var data = new byte[length];
	        for (var i = 0; i < length; ++i)
		        data[i] = reader.ReadByte();

            var format = SurfaceFormat.Color;
            if (componentCount == 4)
                format = SurfaceFormat.Color;
            else
                Log.Die("All compiled textures should have 4 channels.");

            return new Texture2D(_device, data, new Size(width, height), format);
        }
    }
}