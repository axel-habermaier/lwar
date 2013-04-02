using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;

	/// <summary>
	///   Represents a GPU texture.
	/// </summary>
	public abstract class Texture : GraphicsObject
	{
		/// <summary>
		///   The native texture instance.
		/// </summary>
		private IntPtr _texture;

		/// <summary>
		///   Initializes a new instance, copying the given byte array to GPU memory.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="type">The type of the texture.</param>
		protected Texture(GraphicsDevice graphicsDevice)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
		}

		/// <summary>
		///   Gets the native texture instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get { return _texture; }
		}

		/// <summary>
		///   Gets the description of the texture.
		/// </summary>
		protected TextureDescription Description { get; private set; }

		/// <summary>
		///   Gets a value indicating whether the texture has mipmaps, either loaded explicitely or generated automatically.
		/// </summary>
		public bool HasMipmaps
		{
			get { return AutogenerateMipmaps || Description.Mipmaps > 1; }
		}

		/// <summary>
		///   Gets a value indicating whether automatic mipmap generation is supported for this texture.
		/// </summary>
		public bool AutogenerateMipmaps
		{
			get { return (Description.Flags & TextureFlags.GenerateMipmaps) != 0; }
		}

		/// <summary>
		///   Reinitializes the texture.
		/// </summary>
		/// <param name="description">The description of the texture.</param>
		/// <param name="surfaces">The surfaces that should be uploaded to the GPU.</param>
		public void Reinitialize(TextureDescription description, Surface[] surfaces)
		{
			NativeMethods.DestroyTexture(_texture);
			_texture = IntPtr.Zero;

			Description = description;
			_texture = NativeMethods.CreateTexture(GraphicsDevice.NativePtr, ref description, surfaces);
		}

		/// <summary>
		///   Binds the texture to the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be bound to.</param>
		public void Bind(int slot)
		{
			Assert.NotDisposed(this);
			NativeMethods.BindTexture(_texture, slot);
		}

		/// <summary>
		///   Generates the mipmaps for this texture.
		/// </summary>
		public void GenerateMipmaps()
		{
			Assert.NotDisposed(this);
			NativeMethods.GenerateMipmaps(_texture);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyTexture(_texture);
		}

		/// <summary>
		///   Provides access to the native Texture2D functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateTexture")]
			public static extern IntPtr CreateTexture(IntPtr device, ref TextureDescription description, Surface[] surfaces);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyTexture")]
			public static extern void DestroyTexture(IntPtr texture2D);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindTexture")]
			public static extern void BindTexture(IntPtr texture, int slot);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgGenerateMipmaps")]
			public static extern void GenerateMipmaps(IntPtr texture);
		}
	}
}