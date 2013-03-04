using System;

namespace Lwar.Client.Rendering
{
	using System.Collections.Generic;
	using Pegasus.Framework;

	/// <summary>
	///   Renders elements into a 3D scene.
	/// </summary>
	/// <typeparam name="TElement">The type of the elements that the renderer draws.</typeparam>
	/// <typeparam name="TDrawState">The type of the draw states of the elements that the renderer draws.</typeparam>
	public abstract class Renderer<TElement, TDrawState> : DisposableObject
		where TElement : class
		//where TDrawState : struct
	{
		/// <summary>
		///   The draw state of the elements that the renderer draws into the scene.
		/// </summary>
		private readonly List<TDrawState> _drawStates = new List<TDrawState>();

		/// <summary>
		///   The elements that the renderer draws into the scene.
		/// </summary>
		private readonly List<TElement> _elements = new List<TElement>();

		/// <summary>
		///   Gets an enumerator that enumerates all registered draw states.
		/// </summary>
		protected DrawStateEnumerator RegisteredElements
		{
			get { return new DrawStateEnumerator(_drawStates.GetEnumerator()); }
		}

		/// <summary>
		///   Adds the element to the renderer.
		/// </summary>
		/// <param name="element">The element that should be drawn by the renderer.</param>
		public void Add(TElement element)
		{
			Assert.ArgumentNotNull(element, () => element);

			_elements.Add(element);
			_drawStates.Add(OnAdded(element));
		}

		/// <summary>
		///   Invoked when an element has been added to the renderer.
		/// </summary>
		/// <param name="element">The element that should be drawn by the renderer.</param>
		protected abstract TDrawState OnAdded(TElement element);

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

			_drawStates[index] = _drawStates[last];
			_drawStates.RemoveAt(last);
		}

		/// <summary>
		///   Provides an GetEnumerator() method that allows the given enumerator to be used in C#'s foreach statement.
		/// </summary>
		protected struct DrawStateEnumerator
		{
			/// <summary>
			///   The enumerator that can be used in a foreach statement.
			/// </summary>
			private readonly List<TDrawState>.Enumerator _enumerator;

			/// <summary>
			///   Initializes a new instance.
			/// </summary>
			/// <param name="enumerator">The enumerator that should be used in a foreach statement.</param>
			public DrawStateEnumerator(List<TDrawState>.Enumerator enumerator)
			{
				_enumerator = enumerator;
			}

			/// <summary>
			///   Gets the enumerator.
			/// </summary>
			public List<TDrawState>.Enumerator GetEnumerator()
			{
				return _enumerator;
			}
		}
	}
}