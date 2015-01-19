namespace Pegasus.UserInterface.Input
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using Math;
	using Platform.Graphics;
	using Platform.Logging;
	using Platform.Memory;
	using Platform.SDL2;
	using Rendering;
	using Scripting;
	using Utilities;

	/// <summary>
	///     Represents the mouse cursor.
	/// </summary>
	public class Cursor : DisposableObject
	{
		/// <summary>
		///     The cursor that is displayed when the mouse hovers an UI element or any of its children.
		///     If null, the displayed cursor is determined by the hovered child element or the default cursor is displayed.
		/// </summary>
		public static readonly DependencyProperty<Cursor> CursorProperty = new DependencyProperty<Cursor>();

		/// <summary>
		///     The graphics device that is used to draw the cursor.
		/// </summary>
		private readonly GraphicsDevice _graphicsDevice;

		/// <summary>
		///     The underlying hardware cursor instance.
		/// </summary>
		private IntPtr _cursor;

		/// <summary>
		///     The hot spot of the cursor, i.e., the relative offset to the texture's origin where the cursor's
		///     click location lies.
		/// </summary>
		private Vector2 _hotSpot;

		/// <summary>
		///     The texture that defines the cursors visual appearance.
		/// </summary>
		private Texture2D _texture;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		private Cursor(GraphicsDevice graphicsDevice)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			_graphicsDevice = graphicsDevice;
		}

		/// <summary>
		///     Creates a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to create the cursor.</param>
		/// <param name="buffer">The buffer the cursor data should be read from.</param>
		public static Cursor Create(GraphicsDevice graphicsDevice, ref BufferReader buffer)
		{
			Assert.ArgumentNotNull(graphicsDevice);

			var cursor = new Cursor(graphicsDevice);
			cursor.Load(ref buffer);
			return cursor;
		}

		/// <summary>
		///     Loads the cursor from the given buffer.
		/// </summary>
		/// <param name="buffer">The buffer the cursor should be loaded from.</param>
		public unsafe void Load(ref BufferReader buffer)
		{
			_texture.SafeDispose();
			SDL_FreeCursor(_cursor);

			TextureDescription description;
			Surface[] surfaces;
			Texture.ExtractMetadata(ref buffer, out description, out surfaces);

			_texture = new Texture2D(_graphicsDevice, ref description, surfaces);
			_hotSpot = new Vector2(buffer.ReadInt16(), buffer.ReadInt16());

			Assert.That(surfaces.Length == 1, "Unsupported number of surfaces.");
			Assert.That(description.Type == TextureType.Texture2D, "Unsupported texture type.");
			Assert.That(description.Format == SurfaceFormat.Rgba8, "Unsupported texture format.");

			var surface = SDL_CreateRGBSurfaceFrom(surfaces[0].Data, description.Width, description.Height, 32, surfaces[0].Stride,
				0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000);

			if (surface == IntPtr.Zero)
				Log.Die("Failed to create surface for hardware cursor: {0}.", NativeLibrary.GetError());

			_cursor = SDL_CreateColorCursor(surface, _hotSpot.IntegralX, _hotSpot.IntegralY);
			if (_cursor == IntPtr.Zero)
				Log.Die("Failed to create hardware cursor: {0}.", NativeLibrary.GetError());

			SDL_FreeSurface(surface);
		}

		/// <summary>
		///     Sets the debug name of the cursor.
		/// </summary>
		/// <param name="name">The name of the cursor.</param>
		[Conditional("DEBUG")]
		public void SetName(string name)
		{
			_texture.SetName(name);
		}

		/// <summary>
		///     Gets the cursor that is displayed when the mouse hovers the UI element or any of its children.
		/// </summary>
		/// <param name="element">The UI element the cursor should be returned for.</param>
		public static Cursor GetCursor(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			return element.GetValue(CursorProperty);
		}

		/// <summary>
		///     Sets the cursor that should be displayed when the mouse hovers the UI element or any of its children.
		/// </summary>
		/// <param name="element">The UI element the cursor should be set for.</param>
		/// <param name="cursor">The cursor that should be displayed when the mouse hovers the UI element or any of its children.</param>
		public static void SetCursor(UIElement element, Cursor cursor)
		{
			Assert.ArgumentNotNull(element);
			Assert.ArgumentNotNull(cursor);

			element.SetValue(CursorProperty, cursor);
		}

		/// <summary>
		///     Draws the cursor at the given position.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the cursor.</param>
		/// <param name="position">The position the cursor should be drawn at.</param>
		internal void Draw(SpriteBatch spriteBatch, Vector2 position)
		{
			Assert.ArgumentNotNull(spriteBatch);
			Assert.ArgumentInRange(_hotSpot.X, 0, _texture.Width);
			Assert.ArgumentInRange(_hotSpot.Y, 0, _texture.Height);

			if (Cvars.HardwareCursor)
				SDL_SetCursor(_cursor);
			else
			{
				position = position - _hotSpot;
				spriteBatch.Layer = Int32.MaxValue;
				spriteBatch.Draw(_texture, position, Colors.White);
			}
		}

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr SDL_CreateColorCursor(IntPtr surface, int x, int y);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_FreeCursor(IntPtr cursor);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_FreeSurface(IntPtr surface);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern void SDL_SetCursor(IntPtr cursor);

		[DllImport(NativeLibrary.Name, CallingConvention = CallingConvention.Cdecl)]
		private static extern unsafe IntPtr SDL_CreateRGBSurfaceFrom(void* pixels, uint width, uint height, uint depth, uint pitch,
																	 uint red, uint green, uint blue, uint alpha);

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_texture.SafeDispose();
			SDL_FreeCursor(_cursor);
		}
	}
}