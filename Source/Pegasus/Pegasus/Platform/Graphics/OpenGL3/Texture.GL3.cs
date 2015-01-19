namespace Pegasus.Platform.Graphics.OpenGL3
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Represents an OpenGL3-based texture stored in GPU memory.
	/// </summary>
	internal unsafe class TextureGL3 : GraphicsObjectGL3, ITexture
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surface data of the texture.</param>
		public TextureGL3(GraphicsDeviceGL3 graphicsDevice, ref TextureDescription description, Surface[] surfaces)
			: base(graphicsDevice)
		{
			Handle = GLContext.Allocate(_gl.GenTextures, "Texture");

			switch (description.Type)
			{
				case TextureType.Texture2D:
				{
					TargetType = GL.Texture_2D;
					_gl.BindTexture(TargetType, Handle);

					if (surfaces != null)
					{
						for (var i = 0; i < description.Mipmaps; ++i)
							UploadTexture(surfaces[i], description.Format, GL.Texture_2D, i);
					}
					else
					{
						var surface = new Surface { Width = description.Width, Height = description.Height };
						UploadTexture(surface, description.Format, GL.Texture_2D, 0);
					}
					break;
				}
				case TextureType.CubeMap:
				{
					TargetType = GL.TextureCubeMap;
					_gl.BindTexture(TargetType, Handle);

					var faces = stackalloc uint[6];
					faces[0] = GL.TextureCubeMapNegativeZ;
					faces[1] = GL.TextureCubeMapNegativeX;
					faces[2] = GL.TextureCubeMapPositiveZ;
					faces[3] = GL.TextureCubeMapPositiveX;
					faces[4] = GL.TextureCubeMapNegativeY;
					faces[5] = GL.TextureCubeMapPositiveY;

					if (surfaces != null)
					{
						for (var i = 0; i < 6; ++i)
						{
							for (var j = 0; j < description.Mipmaps; ++j)
							{
								var index = i * description.Mipmaps + j;
								UploadTexture(surfaces[index], description.Format, faces[i], j);
							}
						}
					}
					else
					{
						var surface = new Surface { Width = description.Width, Height = description.Height };
						for (var i = 0; i < 6; ++i)
							UploadTexture(surface, description.Format, faces[i], 0);
					}
					break;
				}
				default:
					throw new InvalidOperationException("Unsupported texture type.");
			}

			if ((description.Flags & TextureFlags.GenerateMipmaps) == TextureFlags.GenerateMipmaps)
				_gl.GenerateMipmap(GL.Texture_2D);

			RebindTexture();
		}

		/// <summary>
		///     Gets the texture target type.
		/// </summary>
		internal uint TargetType { get; private set; }

		/// <summary>
		///     Gets the texture handle.
		/// </summary>
		internal uint Handle { get; private set; }

		/// <summary>
		///     Binds the texture to the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be bound to.</param>
		public void Bind(int slot)
		{
			GraphicsDevice.ChangeActiveTexture(slot);
			_gl.BindTexture(TargetType, Handle);

			GraphicsDevice.State.Texture = Handle;
			GraphicsDevice.State.TextureType = TargetType;
		}

		/// <summary>
		///     Unbinds the texture from the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be unbound from.</param>
		public void Unbind(int slot)
		{
			GraphicsDevice.ChangeActiveTexture(slot);
			_gl.BindTexture(TargetType, 0);

			GraphicsDevice.State.Texture = 0;
			GraphicsDevice.State.TextureType = TargetType;
		}

		/// <summary>
		///     Generates the mipmaps for this texture.
		/// </summary>
		public void GenerateMipmaps()
		{
			_gl.BindTexture(TargetType, Handle);
			_gl.GenerateMipmap(TargetType);

			RebindTexture();
		}

		/// <summary>
		///     Sets the debug name of the texture.
		/// </summary>
		/// <param name="name">The debug name of the texture.</param>
		public void SetName(string name)
		{
			// Not supported by OpenGL
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			GLContext.Deallocate(_gl.DeleteTextures, Handle);
		}

		/// <summary>
		///     Uploads a surface to the graphics device.
		/// </summary>
		/// <param name="surface">The surface that should be uploaded.</param>
		/// <param name="format">The format of the surface.</param>
		/// <param name="target">The target texture type.</param>
		/// <param name="level">The resource level the surface should be uploaded to.</param>
		private void UploadTexture(Surface surface, SurfaceFormat format, uint target, int level)
		{
			var internalFormat = format.GetInternalFormat();
			var glFormat = format.Map();
			var type = Texture.IsFloatingPointFormat(format) ? GL.Float : GL.UnsignedByte;

			if (Texture.IsDepthStencilFormat(format))
				type = GL.UnsignedInt248;

			if (Texture.IsCompressedFormat(format))
				_gl.CompressedTexImage2D(target, level, internalFormat, (int)surface.Width, (int)surface.Height, 0, (int)surface.SizeInBytes,
					surface.Data);
			else
				_gl.TexImage2D(target, level, (int)internalFormat, (int)surface.Width, (int)surface.Height, 0, glFormat, type, surface.Data);
		}

		/// <summary>
		///     Rebinds the previously bound texture.
		/// </summary>
		private void RebindTexture()
		{
			if (GraphicsDevice.State.Texture != 0)
				_gl.BindTexture(GraphicsDevice.State.TextureType, GraphicsDevice.State.Texture);
		}
	}
}