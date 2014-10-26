namespace Pegasus.Platform.Graphics
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Security;
	using Utilities;

	/// <summary>
	///     Represents a GPU texture.
	/// </summary>
	public abstract class Texture : GraphicsObject
	{
		/// <summary>
		///     The native texture instance.
		/// </summary>
		private IntPtr _texture;

		/// <summary>
		///     Initializes a new instance, copying the given byte array to GPU memory.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		protected Texture(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
		}

		/// <summary>
		///     Gets the native texture instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get { return _texture; }
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
		///     Reinitializes the texture.
		/// </summary>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surfaces that should be uploaded to the GPU.</param>
		internal void Reinitialize(TextureDescription description, Surface[] surfaces)
		{
			NativeMethods.DestroyTexture(_texture);
			_texture = IntPtr.Zero;

			Description = description;
			_texture = NativeMethods.CreateTexture(GraphicsDevice.NativePtr, ref description, surfaces);
		}

		/// <summary>
		///     Binds the texture to the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be bound to.</param>
		internal void Bind(int slot)
		{
			ValidateAccess();
			NativeMethods.BindTexture(_texture, slot);
		}

		/// <summary>
		///     Unbinds the texture from the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be unbound from.</param>
		internal void Unbind(int slot)
		{
			ValidateAccess();
			NativeMethods.UnbindTexture(_texture, slot);
		}

		/// <summary>
		///     Generates the mipmaps for this texture.
		/// </summary>
		public void GenerateMipmaps()
		{
			ValidateAccess();
			NativeMethods.GenerateMipmaps(_texture);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyTexture(_texture);
		}

		/// <summary>
		///     In debug builds, checks whether the instance can be safely accessed.
		/// </summary>
		[Conditional("DEBUG"), DebuggerHidden]
		protected void ValidateAccess()
		{
			Assert.NotDisposed(this);
			Assert.NotNull(_texture, "The texture has not yet been initialized.");
		}

#if DEBUG
		/// <summary>
		///     Invoked after the name of the graphics object has changed. This method is only available in debug builds.
		/// </summary>
		protected override void OnRenamed()
		{
			if (_texture != IntPtr.Zero)
				NativeMethods.SetName(_texture, Name);
		}
#endif

		/// <summary>
		///     Provides access to the native Texture2D functions.
		/// </summary>
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateTexture")]
			public static extern IntPtr CreateTexture(IntPtr device, ref TextureDescription description, Surface[] surfaces);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyTexture")]
			public static extern void DestroyTexture(IntPtr texture2D);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindTexture")]
			public static extern void BindTexture(IntPtr texture, int slot);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgUnbindTexture")]
			public static extern void UnbindTexture(IntPtr texture, int slot);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGenerateMipmaps")]
			public static extern void GenerateMipmaps(IntPtr texture);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgSetTextureName")]
			[Conditional("DEBUG")]
			public static extern void SetName(IntPtr texture, string name);
		}
	}
}