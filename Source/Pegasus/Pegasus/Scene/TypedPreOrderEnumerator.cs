namespace Pegasus.Scene
{
	using System;
	using Utilities;

	/// <summary>
	///     Represents a scene graph enumerator that enumerates all of the scene graph's scene nodes of the
	///     given type in pre-order.
	/// </summary>
	/// <typeparam name="T">The type of the scene nodes that should be enumerated.</typeparam>
	public struct TypedPreOrderEnumerator<T> : IDisposable
		where T : SceneNode
	{
		/// <summary>
		///     The pre-order enumerator that is used to enumerate the scene graph.
		/// </summary>
		private PreOrderEnumerator _enumerator;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="sceneGraph">The scene graph that should be enumerated.</param>
		/// <param name="startNode">The node where the enumeration should start. Defaults to the root node.</param>
		public TypedPreOrderEnumerator(SceneGraph sceneGraph, SceneNode startNode = null)
			: this()
		{
			Assert.ArgumentNotNull(sceneGraph);
			_enumerator = new PreOrderEnumerator(sceneGraph, startNode);
		}

		/// <summary>
		///     Gets the scene node at the current position of the enumerator.
		/// </summary>
		public T Current { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_enumerator.Dispose();
		}

		/// <summary>
		///     Advances the enumerator to the next item.
		/// </summary>
		public bool MoveNext()
		{
			while (_enumerator.MoveNext())
			{
				var typedNode = _enumerator.Current as T;
				if (typedNode == null)
					continue;

				Current = typedNode;
				return true;
			}

			return false;
		}

		/// <summary>
		///     Enables C#'s foreach support for the enumerator.
		/// </summary>
		public TypedPreOrderEnumerator<T> GetEnumerator()
		{
			return this;
		}
	}
}