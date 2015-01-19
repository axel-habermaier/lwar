namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;
	using Interface;
	using Logging;
	using Memory;
	using Utilities;

	/// <summary>
	///     Represents a GPU texture.
	/// </summary>
	public abstract class Texture : GraphicsObject
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
			get { return TextureObject != null; }
		}

		/// <summary>
		///     Gets the underlying texture object.
		/// </summary>
		internal ITexture TextureObject { get; private set; }

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
		///     Gets a value indicating whether the given format is a compressed format.
		/// </summary>
		/// <param name="format">The surface format that should be checked.</param>
		internal static bool IsCompressedFormat(SurfaceFormat format)
		{
			return format == SurfaceFormat.Bc1 || format == SurfaceFormat.Bc2 || format == SurfaceFormat.Bc3 ||
				   format == SurfaceFormat.Bc4 || format == SurfaceFormat.Bc5;
		}

		/// <summary>
		///     Gets a value indicating whether the given format is a floating point format.
		/// </summary>
		/// <param name="format">The surface format that should be checked.</param>
		internal static bool IsFloatingPointFormat(SurfaceFormat format)
		{
			return format == SurfaceFormat.Rgba16F;
		}

		/// <summary>
		///     Gets a value indicating whether the given format is a depth stencil format.
		/// </summary>
		/// <param name="format">The surface format that should be checked.</param>
		internal static bool IsDepthStencilFormat(SurfaceFormat format)
		{
			return format == SurfaceFormat.Depth24Stencil8;
		}

		/// <summary>
		///     Binds the texture to the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be bound to.</param>
		internal void Bind(int slot)
		{
			Assert.That(slot < GraphicsDevice.TextureSlotCount, "Invalid slot.");
			ValidateAccess();

			if (DeviceState.Change(GraphicsDevice.State.Textures, slot, this))
				TextureObject.Bind(slot);
		}

		/// <summary>
		///     Unbinds the texture from the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be unbound from.</param>
		internal void Unbind(int slot)
		{
			Assert.That(slot < GraphicsDevice.TextureSlotCount, "Invalid slot.");
			ValidateAccess();

			if (DeviceState.Change(GraphicsDevice.State.Textures, slot, null))
				TextureObject.Unbind(slot);
		}

		/// <summary>
		///     Generates the mipmaps for this texture.
		/// </summary>
		public void GenerateMipmaps()
		{
			ValidateAccess();
			Assert.That(Description.Flags.HasFlag(TextureFlags.GenerateMipmaps),
				"Cannot generate mipmaps for a texture that does not have the corresponding flag set.");

			TextureObject.GenerateMipmaps();
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			TextureObject.SafeDispose();
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
			Initialize(ref description, surfaces);
			SetName();
		}

		/// <summary>
		///     Extracts the texture meta data from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the meta data should be read from.</param>
		/// <param name="description">Returns the texture description.</param>
		/// <param name="surfaces">Returns the surfaces of the texture.</param>
		internal static unsafe void ExtractMetadata(ref BufferReader buffer, out TextureDescription description, out Surface[] surfaces)
		{
			description = new TextureDescription
			{
				Width = buffer.ReadUInt32(),
				Height = buffer.ReadUInt32(),
				Depth = buffer.ReadUInt32(),
				ArraySize = buffer.ReadUInt32(),
				Type = (TextureType)buffer.ReadInt32(),
				Format = (SurfaceFormat)buffer.ReadInt32(),
				Mipmaps = buffer.ReadUInt32(),
				SurfaceCount = buffer.ReadUInt32()
			};

			if (description.SurfaceCount > GraphicsDevice.MaxSurfaceCount)
				Log.Die("Too many surfaces stored in texture asset file.");

			surfaces = new Surface[description.SurfaceCount];

			for (var i = 0; i < description.SurfaceCount; ++i)
			{
				surfaces[i] = new Surface
				{
					Width = buffer.ReadUInt32(),
					Height = buffer.ReadUInt32(),
					Depth = buffer.ReadUInt32(),
					SizeInBytes = buffer.ReadUInt32(),
					Stride = buffer.ReadUInt32(),
					Data = buffer.Pointer
				};

				var surfaceSize = surfaces[i].SizeInBytes * surfaces[i].Depth;
				buffer.Skip((int)surfaceSize);
			}
		}

		/// <summary>
		///     Initializes the texture.
		/// </summary>
		/// <param name="description">The description of the texture's properties.</param>
		/// <param name="surfaces">The optional surface data for the texture.</param>
		protected void Initialize(ref TextureDescription description, Surface[] surfaces)
		{
			Assert.InRange((int)description.Mipmaps, 1, GraphicsDevice.MaxMipmaps);
			Assert.That(description.Width > 0 && description.Height > 0, "Invalid size.");
			Assert.That(surfaces != null || description.Mipmaps == 1, "Surfaces must be specified.");
			Assert.That(!description.Flags.HasFlag(TextureFlags.GenerateMipmaps) || description.Mipmaps == 1,
				"Cannot set mipmaps for a texture that has the generate mipmaps flag set.");
			Assert.That(description.Type != TextureType.CubeMap || !description.Flags.HasFlag(TextureFlags.DepthStencil),
				"A cube map cannot be used as the depth stencil buffer of a render target.");
			Assert.That(description.Type != TextureType.CubeMap || !description.Flags.HasFlag(TextureFlags.RenderTarget),
				"A cube map cannot be used as the color buffer of a render target.");

			TextureObject.SafeDispose();
			TextureObject = GraphicsDevice.CreateTexture(ref description, surfaces);

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

		/// <summary>
		///     Invoked after the name of the graphics object has changed. This method is only invoked in debug builds.
		/// </summary>
		/// <param name="name">The new name of the graphics object.</param>
		protected override void OnRenamed(string name)
		{
			TextureObject.SetName(name);
		}
	}
}