namespace Pegasus.Platform.Graphics.Direct3D11
{
	using System;
	using Bindings;
	using Interface;

	/// <summary>
	///     Represents an Direct3D11-based texture stored in GPU memory.
	/// </summary>
	internal class TextureD3D11 : GraphicsObjectD3D11, ITexture
	{
		/// <summary>
		///     The underlying Direct3D11 shader resource view.
		/// </summary>
		private D3D11ShaderResourceView _view;

		/// <summary>
		///     Gets the underlying Direct3D11 texture.
		/// </summary>
		internal D3D11Resource Texture;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surface data of the texture.</param>
		public TextureD3D11(GraphicsDeviceD3D11 graphicsDevice, ref TextureDescription description, Surface[] surfaces)
			: base(graphicsDevice)
		{
			switch (description.Type)
			{
				case TextureType.Texture2D:
					CreateTexture2D(ref description, surfaces);
					break;
				case TextureType.CubeMap:
					CreateCubeMap(ref description, surfaces);
					break;
				default:
					throw new InvalidOperationException("Unsupported texture type.");
			}
		}

		/// <summary>
		///     Binds the texture to the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be bound to.</param>
		public unsafe void Bind(int slot)
		{
			var view = _view;
			Context.VSSetShaderResources(slot, 1, &view);
			Context.PSSetShaderResources(slot, 1, &view);
		}

		/// <summary>
		///     Unbinds the texture from the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be unbound from.</param>
		public unsafe void Unbind(int slot)
		{
			Context.VSSetShaderResources(slot, 0, null);
			Context.PSSetShaderResources(slot, 0, null);
		}

		/// <summary>
		///     Generates the mipmaps for this texture.
		/// </summary>
		public void GenerateMipmaps()
		{
			Context.GenerateMips(_view);
		}

		/// <summary>
		///     Sets the debug name of the texture.
		/// </summary>
		/// <param name="name">The debug name of the texture.</param>
		public void SetName(string name)
		{
			Texture.SetDebugName(name);

			if (_view.IsInitialized)
				_view.SetDebugName(name);
		}

		/// <summary>
		///     Initializes a 2D texture.
		/// </summary>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surface data of the texture.</param>
		private unsafe void CreateTexture2D(ref TextureDescription description, Surface[] surfaces)
		{
			D3D11Texture2DDescription desc;
			InitializeDesc2D(ref description, out desc);

			var data = stackalloc D3D11SubResourceData[GraphicsDevice.MaxSurfaceCount];
			InitializeData(data, ref description, surfaces);
			Device.CreateTexture2D(ref desc, surfaces == null ? null : data, out Texture).CheckSuccess("Failed to create 2D texture.");

			if ((description.Flags & TextureFlags.DepthStencil) == TextureFlags.DepthStencil)
				return;

			var viewDesc = new D3D11ShaderResourceViewDescription
			{
				Format = desc.Format,
				Dimension = D3D11ShaderResourceViewDimension.Texture2D,
				Texture2D =
				{
					MipLevels = -1,
					MostDetailedMip = 0
				}
			};

			Device.CreateShaderResourceView(Texture, ref viewDesc, out _view).CheckSuccess("Failed to create shader resource view.");
		}

		/// <summary>
		///     Initializes a cube map.
		/// </summary>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surface data of the texture.</param>
		private unsafe void CreateCubeMap(ref TextureDescription description, Surface[] surfaces)
		{
			D3D11Texture2DDescription desc;
			InitializeDesc2D(ref description, out desc);
			desc.ArraySize *= 6;
			desc.OptionFlags |= D3D11ResourceOptionFlags.TextureCube;

			var data = stackalloc D3D11SubResourceData[GraphicsDevice.MaxSurfaceCount];
			InitializeData(data, ref description, surfaces);
			Device.CreateTexture2D(ref desc, surfaces == null ? null : data, out Texture).CheckSuccess("Failed to create 2D texture.");

			if ((description.Flags & TextureFlags.DepthStencil) == TextureFlags.DepthStencil)
				return;

			var viewDesc = new D3D11ShaderResourceViewDescription
			{
				Format = desc.Format,
				Dimension = D3D11ShaderResourceViewDimension.TextureCube,
				Texture2D =
				{
					MipLevels = -1,
					MostDetailedMip = 0
				}
			};

			Device.CreateShaderResourceView(Texture, ref viewDesc, out _view).CheckSuccess("Failed to create shader resource view.");
		}

		/// <summary>
		///     Initializes a 2D texture description.
		/// </summary>
		/// <param name="description">The Pegasus texture description.</param>
		/// <param name="desc">The initialized Direct3D11 description.</param>
		private static void InitializeDesc2D(ref TextureDescription description, out D3D11Texture2DDescription desc)
		{
			desc.Width = (int)description.Width;
			desc.Height = (int)description.Height;
			desc.MipLevels = (int)description.Mipmaps;
			desc.ArraySize = 1;
			desc.SampleDescription.Count = 1;
			desc.SampleDescription.Quality = 0;
			desc.Usage = D3D11ResourceUsage.Default;
			desc.BindFlags = D3D11BindFlags.ShaderResource;
			desc.CpuAccessFlags = D3D11CpuAccessFlags.None;
			desc.OptionFlags = D3D11ResourceOptionFlags.None;
			desc.Format = description.Format.Map();

			if ((description.Flags & TextureFlags.RenderTarget) == TextureFlags.RenderTarget)
				desc.BindFlags |= D3D11BindFlags.RenderTarget;

			if ((description.Flags & TextureFlags.DepthStencil) == TextureFlags.DepthStencil)
				desc.BindFlags = D3D11BindFlags.DepthStencil;

			if ((description.Flags & TextureFlags.GenerateMipmaps) == TextureFlags.GenerateMipmaps)
			{
				desc.OptionFlags |= D3D11ResourceOptionFlags.GenerateMipMaps;
				desc.BindFlags |= D3D11BindFlags.RenderTarget;
				desc.MipLevels = 0;
			}
		}

		/// <summary>
		///     Initializes the texture data.
		/// </summary>
		/// <param name="data">The texture data that should be initialized.</param>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surfaces of the texture.</param>
		private static unsafe void InitializeData(D3D11SubResourceData* data, ref TextureDescription description, Surface[] surfaces)
		{
			if (surfaces == null)
				return;

			var mipmaps = description.Mipmaps;
			if ((description.Flags & TextureFlags.GenerateMipmaps) == TextureFlags.GenerateMipmaps)
				mipmaps = 0;

			if (description.Type == TextureType.CubeMap)
			{
				int* faces = stackalloc int[6];
				faces[0] = 5;
				faces[1] = 1;
				faces[2] = 4;
				faces[3] = 0;
				faces[4] = 3;
				faces[5] = 2;

				for (var i = 0; i < 6; ++i)
				{
					for (var j = 0; j < mipmaps; ++j)
					{
						var dataIndex = faces[i] * mipmaps + j;
						var surfaceIndex = i * mipmaps + j;
						data[dataIndex].Data = new IntPtr(surfaces[surfaceIndex].Data);
						data[dataIndex].RowPitch = (int)surfaces[surfaceIndex].Stride;
						data[dataIndex].SlicePitch = (int)surfaces[surfaceIndex].SizeInBytes;
					}
				}
			}
			else
			{
				for (var i = 0; i < mipmaps; ++i)
				{
					data[i].Data = new IntPtr(surfaces[i].Data);
					data[i].RowPitch = (int)surfaces[i].Stride;
					data[i].SlicePitch = (int)surfaces[i].SizeInBytes;
				}
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Texture.Release();
			_view.Release();
		}
	}
}