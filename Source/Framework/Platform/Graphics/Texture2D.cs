using System;

namespace Pegasus.Framework.Platform.Graphics
{
	using Math;

	/// <summary>
	///   A 2D texture manages two-dimensional texel data.
	/// </summary>
	public sealed class Texture2D : Texture
	{
		/// <summary>
		///   Initializes a new instance, copying the given byte array to GPU memory.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with this instance.</param>
		/// <param name="data">The data that should be copied into the texture's memory.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		public Texture2D(GraphicsDevice graphicsDevice, byte[] data, int width, int height, SurfaceFormat format)
			: base(graphicsDevice, TextureType.Texture2D, data, width, height, 0, format)
		{
		}

		/// <summary>
		///   Gets a 1x1 pixels fully white two-dimensional texture object.
		/// </summary>
		public static Texture2D White { get; private set; }

		/// <summary>
		///   Gets the size of the texture.
		/// </summary>
		public Size Size
		{
			get { return new Size(Width, Height); }
		}

		/// <summary>
		///   Initializes the default instances.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device associated with the default instances.</param>
		internal static void InitializeDefaultInstances(GraphicsDevice graphicsDevice)
		{
			White = new Texture2D(graphicsDevice, new byte[] { 255, 255, 255, 255 }, 1, 1, SurfaceFormat.Rgba8);
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
		///   Reinitializes the texture.
		/// </summary>
		/// <param name="data">The data that should be copied into the texture's memory.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		/// <param name="format">The format of the texture.</param>
		internal void Reinitialize(byte[] data, int width, int height, SurfaceFormat format)
		{
			Assert.ArgumentNotNull(data, () => data);
			Assert.ArgumentSatisfies(width > 0, () => width, "Width must be greater than 0.");
			Assert.ArgumentSatisfies(height > 0, () => height, "Height must be greater than 0.");
			Assert.ArgumentInRange(format, () => format);

			Reinitialize(data, width, height, 0, format);
		}
	}
}