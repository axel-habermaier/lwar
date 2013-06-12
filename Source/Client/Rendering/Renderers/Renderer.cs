using System;

namespace Lwar.Client.Rendering.Renderers
{
	using System.Collections.Generic;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders elements into a 3D scene.
	/// </summary>
	/// <typeparam name="TElement">The type of the elements that the renderer draws.</typeparam>
	public abstract class Renderer<TElement> : DisposableObject, IRenderer
		where TElement : class
	{
		/// <summary>
		///   The elements that the renderer draws into the scene.
		/// </summary>
		private readonly List<TElement> _elements = new List<TElement>();

		/// <summary>
		///   Gets the elements that the renderer should draw into the scene.
		/// </summary>
		protected Enumerator Elements
		{
			get { return new Enumerator(_elements.GetEnumerator()); }
		}

		/// <summary>
		///   Gets the graphics device that is used for drawing.
		/// </summary>
		protected GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///   Gets the assets manager that is used to load all required assets.
		/// </summary>
		protected AssetsManager Assets { get; private set; }

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			GraphicsDevice = graphicsDevice;
			Assets = assets;
			Initialize();
		}

		/// <summary>
		///   Draws all registered 3D elements.
		/// </summary>
		/// <param name="output">The output that the elements should be rendered to.</param>
		public virtual void Draw(RenderOutput output)
		{
		}

		/// <summary>
		///   Draws all registered 2D elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the 2D elements.</param>
		public virtual void Draw(SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		///   Draws the user interface elements.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch that should be used to draw the user interface.</param>
		/// <param name="camera">The camera that is used to draw the scene.</param>
		public virtual void DrawUserInterface(SpriteBatch spriteBatch, GameCamera camera)
		{
		}

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		protected abstract void Initialize();

		/// <summary>
		///   Adds the element to the renderer.
		/// </summary>
		/// <param name="element">The element that should be drawn by the renderer.</param>
		public void Add(TElement element)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(element);

			_elements.Add(element);
			OnAdded(element);
		}

		/// <summary>
		///   Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="element">The element that has been added.</param>
		protected virtual void OnAdded(TElement element)
		{
		}

		/// <summary>
		///   Invoked when an element has been removed from the renderer.
		/// </summary>
		/// <param name="element">The element that has been removed.</param>
		/// <param name="index">The index of the element that has been removed.</param>
		protected virtual void OnRemoved(TElement element, int index)
		{
		}

		/// <summary>
		///   Removes the element from the renderer.
		/// </summary>
		/// <param name="element">The element that should be removed from the renderer.</param>
		public void Remove(TElement element)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(element);
			Assert.ArgumentSatisfies(_elements.Contains(element), "The element is not drawn by this renderer.");

			var index = _elements.IndexOf(element);
			var last = _elements.Count - 1;

			_elements[index] = _elements[last];
			_elements.RemoveAt(last);

			OnRemoved(element, index);
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected sealed override void OnDisposing()
		{
			while (_elements.Count != 0)
				Remove(_elements[0]);

			OnDisposingCore();
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected abstract void OnDisposingCore();

		/// <summary>
		///   Provides an GetEnumerator() method that allows the given enumerator to be used in C#'s foreach statement.
		/// </summary>
		protected struct Enumerator
		{
			/// <summary>
			///   The enumerator that can be used in a foreach statement.
			/// </summary>
			private readonly List<TElement>.Enumerator _enumerator;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="enumerator">The enumerator that should be used in a foreach statement.</param>
			public Enumerator(List<TElement>.Enumerator enumerator)
			{
				_enumerator = enumerator;
			}

			/// <summary>
			///   Gets the enumerator.
			/// </summary>
			public List<TElement>.Enumerator GetEnumerator()
			{
				return _enumerator;
			}
		}
	}
}