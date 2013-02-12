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
		///   The type of the texture.
		/// </summary>
		private readonly TextureType _type;

		/// <summary>
		///   The native texture instance.
		/// </summary>
		private IntPtr _texture;

		/// <summary>
		///   Initializes a new instance, copying the given byte array to GPU memory.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="type">The type of the texture.</param>
		/// <param name="data">The data that should be copied into the texture's memory.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="depth">The depth of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		protected Texture(GraphicsDevice graphicsDevice, TextureType type, byte[] data, int width, int height, int depth,
						  SurfaceFormat format)
			: base(graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice, () => graphicsDevice);
			Assert.ArgumentInRange(type, () => type);

			_type = type;
			Reinitialize(data, width, height, depth, format);
		}

		/// <summary>
		///   Gets the native texture instance.
		/// </summary>
		internal IntPtr NativePtr
		{
			get { return _texture; }
		}

		/// <summary>
		///   Gets the width of the texture.
		/// </summary>
		protected int Width { get; private set; }

		/// <summary>
		///   Gets the height of the texture.
		/// </summary>
		protected int Height { get; private set; }

		/// <summary>
		///   Gets the depth of the texture.
		/// </summary>
		protected int Depth { get; private set; }

		/// <summary>
		///   Reinitializes the texture.
		/// </summary>
		/// <param name="data">The data that should be copied into the texture's memory.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="depth">The depth of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		protected void Reinitialize(byte[] data, int width, int height, int depth, SurfaceFormat format)
		{
			Assert.ArgumentNotNull(data, () => data);
			Assert.ArgumentSatisfies(width > 0, () => width, "Width must be greater than 0.");
			Assert.ArgumentSatisfies(height >= 0, () => height, "Height must be greater than or equal to 0.");
			Assert.ArgumentSatisfies(depth >= 0, () => depth, "Depth must be greater than or equal to 0.");
			Assert.ArgumentInRange(format, () => format);

			if (_texture != IntPtr.Zero)
				NativeMethods.DestroyTexture(_texture);

			Width = width;
			Height = height;
			Depth = depth;
			_texture = NativeMethods.CreateTexture(GraphicsDevice.NativePtr, _type, data, width, height, depth, format);
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
			public static extern IntPtr CreateTexture(IntPtr device, TextureType type, byte[] data, int width, int height, int depth,
													  SurfaceFormat format);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgDestroyTexture")]
			public static extern void DestroyTexture(IntPtr texture2D);

			[DllImport(NativeLibrary.LibraryName, EntryPoint = "pgBindTexture")]
			public static extern void BindTexture(IntPtr texture, int slot);
		}
	}
}