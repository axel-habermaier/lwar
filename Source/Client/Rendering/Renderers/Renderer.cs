using System;

namespace Lwar.Client.Rendering.Renderers
{
	using System.Collections.Generic;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
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
		///   Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public abstract void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets);

		/// <summary>
		///   Draws all registered elements.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public abstract void Draw(RenderOutput output);

		/// <summary>
		///   Adds the element to the renderer.
		/// </summary>
		/// <param name="element">The element that should be drawn by the renderer.</param>
		public void Add(TElement element)
		{
			Assert.ArgumentNotNull(element, () => element);
			_elements.Add(element);
		}

		/// <summary>
		///   Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="element">The element that should be drawn by the renderer.</param>
		protected virtual void OnAdded(TElement element)
		{
		}

		/// <summary>
		///   Removes the element from the renderer.
		/// </summary>
		/// <param name="element">The element that should be removed from the renderer.</param>
		public void Remove(TElement element)
		{
			Assert.ArgumentNotNull(element, () => element);
			Assert.ArgumentSatisfies(_elements.Contains(element), () => element, "The element is not drawn by this renderer.");

			var index = _elements.IndexOf(element);
			var last = _elements.Count - 1;

			_elements[index] = _elements[last];
			_elements.RemoveAt(last);
		}

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