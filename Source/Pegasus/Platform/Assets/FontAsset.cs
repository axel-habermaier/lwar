namespace Pegasus.Platform.Assets
{
	using System;
	using Math;
	using Memory;
	using Rendering.UserInterface;

	/// <summary>
	///   Represents a font asset.
	/// </summary>
	internal sealed class FontAsset : Asset
	{
		/// <summary>
		///   The font texture.
		/// </summary>
		private Texture2DAsset _texture;

		/// <summary>
		///   The font that is managed by this asset instance.
		/// </summary>
		internal Font Font { get; private set; }

		/// <summary>
		///   Gets the friendly name of the asset.
		/// </summary>
		internal override string FriendlyName
		{
			get { return "Font"; }
		}

		/// <summary>
		///   Loads or reloads the asset using the given asset buffer.
		/// </summary>
		/// <param name="buffer">The buffer that should be used to load the asset.</param>
		/// <param name="name">The name of the asset.</param>
		internal override void Load(BufferReader buffer, string name)
		{
			if (Font == null)
				Font = new Font();

			if (_texture == null)
				_texture = new Texture2DAsset { GraphicsDevice = GraphicsDevice, Assets = Assets };

			// Load the font map
			_texture.Load(buffer, name);

			// Load the font metadata
			var lineHeight = buffer.ReadUInt16();

			// Load the glyph metadata
			var numGlyphs = buffer.ReadUInt16();
			var glyphs = new Glyph[256];

			for (var i = 0; i < numGlyphs; ++i)
			{
				var index = buffer.ReadByte();

				// Read the texture coordinates
				var x = buffer.ReadUInt16();
				var y = buffer.ReadUInt16();
				glyphs[index].Area.Width = buffer.ReadUInt16();
				glyphs[index].Area.Height = buffer.ReadUInt16();

				// Compute the texture coordinates
				var textureLeft = x / (float)_texture.Texture.Width;
				var textureRight = (x + glyphs[index].Area.Width) / (float)_texture.Texture.Width;
				var textureTop = (y + glyphs[index].Area.Height) / (float)_texture.Texture.Height;
				var textureBottom = y / (float)_texture.Texture.Height;

				glyphs[index].TextureArea = new RectangleF(textureLeft, textureTop,
														   textureRight - textureLeft,
														   textureBottom - textureTop);

				// Read the glyph offsets
				glyphs[index].Area.Left = buffer.ReadInt16();
				glyphs[index].Area.Top = buffer.ReadInt16();
				glyphs[index].AdvanceX = buffer.ReadInt16();
			}

			// Load the kerning data
			var kerningCount = buffer.ReadUInt16();
			KerningPair[] kernings = null;
			if (kerningCount != 0)
			{
				kernings = new KerningPair[kerningCount];

				for (var i = 0; i < kerningCount; ++i)
				{
					var first = buffer.ReadUInt16();
					var second = buffer.ReadUInt16();
					var offset = buffer.ReadInt16();

					kernings[i] = new KerningPair((char)first, (char)second, offset);

					if (glyphs[first].KerningStart == 0)
						glyphs[first].KerningStart = i;

					++glyphs[first].KerningCount;
				}
			}

			Font.Reinitialize(glyphs, kernings, _texture.Texture, lineHeight);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_texture.SafeDispose();
		}
	}
}