using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using System.Runtime.InteropServices;
	using Math;

	/// <summary>
	///   A 2D texture manages two-dimensional texel data.
	/// </summary>
	public sealed class Texture2D : GraphicsObject
	{
		/// <summary>
		///   The native texture instance.
		/// </summary>
		private readonly IntPtr _texture;

		/// <summary>
		///   Initializes a new instance, copying the given byte array to GPU memory.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="data">The data that should be copied into the texture's memory.</param>
		/// <param name="size">The size of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		public Texture2D(GraphicsDevice graphicsDevice, byte[] data, Size size, SurfaceFormat format)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(data, () => data);
			Assert.That(size.Width > 0, "Width must be greater than 0.");
			Assert.That(size.Height > 0, "Height must be greater than 0.");
			Assert.InRange(format);

			Size = size;
			_texture = NativeMethods.CreateTexture2D(graphicsDevice.NativePtr, data, size.Width, size.Height, format);
		}

		/// <summary>
		///   Gets the native texture instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get { return _texture; }
		}

		/// <summary>
		///   Gets a 1x1 pixels fully white two-dimensional texture object.
		/// </summary>
		public static Texture2D White { get; private set; }

		/// <summary>
		///   Gets the size of the texture.
		/// </summary>
		public Size Size { get; private set; }

		/// <summary>
		///   Initializes the default instances.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with the default instances.</param>
		internal static void InitializeDefaultInstances(GraphicsDevice graphicsDevice)
		{
			White = new Texture2D(graphicsDevice, new byte[] { 255, 255, 255, 255 }, new Size(1, 1), SurfaceFormat.Color);
		}

		/// <summary>
		///   Disposes the default instances.
		/// </summary>
		internal static void DisposeDefaultInstances()
		{
			White.SafeDispose();
			White = null;
		}

		/// <summary>
		///   Binds the texture to the given slot.
		/// </summary>
		/// <param name="slot">The slot the texture should be bound to.</param>
		public void Bind(int slot)
		{
			Assert.NotDisposed(this);
			Assert.InRange(slot, 0, GraphicsDevice.TextureSlotCount);

			if (DeviceState.Textures[slot] == this)
				return;

			DeviceState.Textures[slot] = this;
			NativeMethods.BindTexture(_texture, slot);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			NativeMethods.DestroyTexture2D(_texture);
		}

		/// <summary>
		///   Provides access to the native Texture2D functions.
		/// </summary>
#if !DEBUG
		[System.Security.SuppressUnmanagedCodeSecurity]
#endif
		private static class NativeMethods
		{
			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgCreateTexture2D")]
			public static extern IntPtr CreateTexture2D(IntPtr device, byte[] data, int width, int height, SurfaceFormat format);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyTexture2D")]
			public static extern void DestroyTexture2D(IntPtr texture2D);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindTexture")]
			public static extern void BindTexture(IntPtr texture2D, int slot);
		}
	}
}