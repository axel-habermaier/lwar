namespace Pegasus.Framework.UserInterface
{
	using System;
	using Controls;
	using Math;
	using Platform;
	using Platform.Assets;
	using Platform.Graphics;
	using Platform.Input;
	using Platform.Memory;
	using Rendering;

	/// <summary>
	///   Represents the UI context that layouts and draws several logical trees as well as handling their input. Multiple UI
	///   contexts can exists within a single application, but there can be only one UI context per window.
	/// </summary>
	public class UIContext : DisposableObject
	{
		/// <summary>
		///   The root of all logical trees managed by the context.
		/// </summary>
		private readonly Canvas _canvas = new Canvas();

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used to render the UI elements of the UI context.</param>
		/// <param name="window">The application window the UI elements of the UI context should be rendered to.</param>
		public UIContext(GraphicsDevice graphicsDevice, Window window)
		{
			Assert.ArgumentNotNull(window);
			Assert.ArgumentNotNull(graphicsDevice);

			Window = window;
			GraphicsDevice = graphicsDevice;

			SharedAssets = new AssetsManager(graphicsDevice);
		}

		/// <summary>
		///   Gets or sets the font loader that is used by all UI elements of the context.
		/// </summary>
		public IFontLoader FontLoader
		{
			get { return _canvas.Resources[typeof(IFontLoader)] as IFontLoader; }
			set
			{
				Assert.ArgumentNotNull(value);
				_canvas.Resources[typeof(IFontLoader)] = value;
			}
		}

		/// <summary>
		///   Gets the asset manager that is used to load the shared assets required by the UI context such as fonts and UI
		///   shaders. Textures, on the other hand, should typically be loaded by more short-lived assets managers.
		/// </summary>
		public AssetsManager SharedAssets { get; private set; }

		/// <summary>
		///   Gets the graphics device that is used to render the UI elements of the UI context.
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///   Gets the application window the UI elements of the UI context are rendered to.
		/// </summary>
		public Window Window { get; private set; }

		/// <summary>
		///   Gets the logical input device that handles all user input to the application.
		/// </summary>
		public LogicalInputDevice InputDevice { get; private set; }

		/// <summary>
		///   Gets or sets the resources shared by all UI elements.
		/// </summary>
		public ResourceDictionary Resources
		{
			get { return _canvas.Resources; }
			set { _canvas.Resources = value; }
		}

		/// <summary>
		///   Adds the UI element to the context.
		/// </summary>
		/// <param name="element">The element that should be added.</param>
		public void Add(UIElement element)
		{
			Assert.ArgumentNotNull(element);
			Assert.ArgumentSatisfies(element.Parent == null, "The element is already part of a logical tree.");

			_canvas.Children.Add(element);

			if (element.ViewModel != null)
				element.ViewModel.UIContext = this;
		}

		/// <summary>
		///   Removes the UI element from the context.
		/// </summary>
		/// <param name="element">The UI element that should be removed.</param>
		public bool Remove(UIElement element)
		{
			Assert.ArgumentNotNull(element);

			if (!_canvas.Children.Remove(element))
				return false;

			if (element.ViewModel != null)
				element.ViewModel.UIContext = null;

			return true;
		}

		/// <summary>
		///   Updates the UI context.
		/// </summary>
		public void Update()
		{
			var size = new SizeD(Window.Width, Window.Height);

			_canvas.Measure(size);
			_canvas.Arrange(new RectangleD(0, 0, size));
			_canvas.ApplyVisualOffset(Vector2d.Zero);
		}

		/// <summary>
		///   Draws all UI elements in the order they have been added to the context.
		/// </summary>
		public void Draw(SpriteBatch spriteBatch)
		{
			_canvas.Draw(spriteBatch);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			SharedAssets.SafeDispose();
		}
	}
}