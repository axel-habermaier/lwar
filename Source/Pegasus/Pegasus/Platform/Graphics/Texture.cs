namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a GPU texture.
	/// </summary>
	public abstract unsafe class Texture : GraphicsObject
	{
		/// <summary>
		///     Initializes a new instance, copying the given byte array to GPU memory.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		protected Texture(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///     Gets a value indicating whether the texture has been fully initialized and can be used for drawing.
		/// </summary>
		private bool IsInitialized
		{
			get { return NativeObject != null; }
		}

		/// <summary>
		///     Gets the description of the texture.
		/// </summary>
		protected TextureDescription Description { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the texture has mipmaps, either loaded explicitly or generated automatically.
		/// </summary>
		public bool HasMipmaps
		{
			get
			{
				ValidateAccess();
				return AutogenerateMipmaps || Description.Mipmaps > 1;
			}
		}

		/// <summary>
		///     Gets a value indicating whether automatic mipmap generation is supported for this texture.
		/// </summary>
		public bool AutogenerateMipmaps
		{
			get
			{
				ValidateAccess();
				return (Description.Flags & TextureFlags.GenerateMipmaps) != 0;
			}
		}

		/// <summary>
		///     Gets a value indicating whether the texture is used as a color buffer of a render target.
		/// </summary>
		public bool IsColorBuffer
		{
			get
			{
				ValidateAccess();
				return (Description.Flags & TextureFlags.RenderTarget) != 0;
			}
		}

		/// <summary>
		///     Gets a value indicating whether the texture is used as a depth stencil buffer of a render target.
		/// </summary>
		public bool IsDepthStencilBuffer
		{
			get
			{
				ValidateAccess();
				return (Description.Flags & TextureFlags.DepthStencil) != 0;
			}
		}

		/// <summary>
		///     Gets the function that should be used to set the debug name of the native object.
		/// </summary>
		protected override SetNameDelegate SetNameFunction
		{
			get { return DeviceInterface->SetTextureName; }
		}

		/// <summary>
		///     Binds the texture to the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be bound to.</param>
		internal void Bind(int slot)
		{
			Assert.That(slot < GraphicsDevice.TextureSlotCount, "Invalid slot.");
			ValidateAccess();

			if (DeviceState.Change(DeviceState.Textures, slot, this))
				DeviceInterface->BindTexture(NativeObject, slot);
		}

		/// <summary>
		///     Unbinds the texture from the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be unbound from.</param>
		internal void Unbind(int slot)
		{
			Assert.That(slot < GraphicsDevice.TextureSlotCount, "Invalid slot.");
			ValidateAccess();

			if (DeviceState.Change(DeviceState.Textures, slot, null))
				DeviceInterface->UnbindTexture(NativeObject, slot);
		}

		/// <summary>
		///     Generates the mipmaps for this texture.
		/// </summary>
		public void GenerateMipmaps()
		{
			ValidateAccess();
			Assert.That(Description.Flags.HasFlag(TextureFlags.GenerateMipmaps),
				"Cannot generate mipmaps for a texture that does not have the corresponding flag set.");

			DeviceInterface->GenerateMipmaps(NativeObject);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			DeviceInterface->FreeTexture(NativeObject);
		}

		/// <summary>
		///     Loads the texture from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the texture should be read from.</param>
		public void Load(ref BufferReader buffer)
		{
			TextureDescription description;
			Surface[] surfaces;

			ExtractMetadata(ref buffer, out description, out surfaces);
			Initialize(description, surfaces);
			SetName();
		}

		/// <summary>
		///     Extracts the texture meta data from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the meta data should be read from.</param>
		/// <param name="description">Returns the texture description.</param>
		/// <param name="surfaces">Returns the surfaces of the texture.</param>
		internal static void ExtractMetadata(ref BufferReader buffer, out TextureDescription description, out Surface[] surfaces)
		{
			description = new TextureDescription
			{
				Width = buffer.ReadInt32(),
				Height = buffer.ReadInt32(),
				Depth = buffer.ReadInt32(),
				ArraySize = buffer.ReadInt32(),
				Type = (TextureType)buffer.ReadInt32(),
				Format = (SurfaceFormat)buffer.ReadInt32(),
				Mipmaps = buffer.ReadInt32(),
				SurfaceCount = buffer.ReadInt32()
			};

			surfaces = new Surface[description.SurfaceCount];

			for (var i = 0; i < description.SurfaceCount; ++i)
			{
				surfaces[i] = new Surface
				{
					Width = buffer.ReadInt32(),
					Height = buffer.ReadInt32(),
					Depth = buffer.ReadInt32(),
					SizeInBytes = buffer.ReadInt32(),
					Stride = buffer.ReadInt32(),
					Data = buffer.Pointer
				};

				var surfaceSize = surfaces[i].SizeInBytes * surfaces[i].Depth;
				buffer.Skip(surfaceSize);
			}
		}

		/// <summary>
		///     Initializes the texture.
		/// </summary>
		/// <param name="description">The description of the texture's properties.</param>
		/// <param name="surfaces">The optional surface data for the texture.</param>
		protected void Initialize(TextureDescription description, Surface[] surfaces)
		{
			Assert.InRange(description.Mipmaps, 1, GraphicsDevice.MaxMipmaps);
			Assert.That(description.Width > 0 && description.Height > 0, "Invalid size.");
			Assert.That(description.SurfaceCount <= GraphicsDevice.MaxSurfaceCount, "Too many surfaces stored in texture asset file.");
			Assert.That(surfaces != null || description.Mipmaps == 1, "Surfaces must be specified.");
			Assert.That(!description.Flags.HasFlag(TextureFlags.GenerateMipmaps) || description.Mipmaps == 1,
				"Cannot set mipmaps for a texture that has the generate mipmaps flag set.");
			Assert.That(description.Type != TextureType.CubeMap || !description.Flags.HasFlag(TextureFlags.DepthStencil),
				"A cube map cannot be used as the depth stencil buffer of a render target.");
			Assert.That(description.Type != TextureType.CubeMap || !description.Flags.HasFlag(TextureFlags.RenderTarget),
				"A cube map cannot be used as the color buffer of a render target.");

			DeviceInterface->FreeTexture(NativeObject);

			fixed (void* surfacesArray = surfaces)
				NativeObject = DeviceInterface->InitializeTexture(&description, surfacesArray);

			SetName();
			Description = description;
		}

		/// <summary>
		///     In debug builds, checks whether the instance can be safely accessed.
		/// </summary>
		[Conditional("DEBUG"), DebuggerHidden]
		protected void ValidateAccess()
		{
			Assert.NotDisposed(this);
			Assert.That(IsInitialized, "The texture has not yet been initialized.");
		}
	}
}