namespace Pegasus.Scene
{
	using System;
	using System.Collections.Generic;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Arranges the logical and spacial representation of a graphical 3D scene.
	/// </summary>
	public class SceneGraph : DisposableObject
	{
		/// <summary>
		///     The updates that should be applied to the scene graph.
		/// </summary>
		private readonly Queue<Update> _deferredUpdates = new Queue<Update>();

		/// <summary>
		///     The number of enumerators that are currently enumerating the scene graph.
		/// </summary>
		internal int EnumeratorCount;

		/// <summary>
		///     The scene graph version, used to detect modifications during enumerations.
		/// </summary>
		internal int Version;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="allocator">The allocator that should be used to allocated pooled objects.</param>
		public SceneGraph(PoolAllocator allocator)
		{
			Assert.ArgumentNotNull(allocator);

			Root = RootNode.Create(allocator);
			Root.Attach(this, null);
		}

		/// <summary>
		///     Gets a value indicating whether the scene graph is currently being enumerated.
		/// </summary>
		private bool IsEnumerated
		{
			get { return EnumeratorCount > 0; }
		}

		/// <summary>
		///     Gets the root node of the scene graph.
		/// </summary>
		public SceneNode Root { get; private set; }

		/// <summary>
		///     Raised when a scene node has been added to the scene graph.
		/// </summary>
		public event Action<SceneNode> NodeAdded;

		/// <summary>
		///     Raised when a scene node has been removed from the scene graph.
		/// </summary>
		public event Action<SceneNode> NodeRemoved;

		/// <summary>
		///     Adds the given scene node to the scene graph's root.
		/// </summary>
		/// <param name="sceneNode">The scene node that should be added.</param>
		public void Add(SceneNode sceneNode)
		{
			Add(sceneNode, Root);
		}

		/// <summary>
		///     Adds the given scene node to the given parent node.
		/// </summary>
		/// <param name="sceneNode">The scene node that should be added.</param>
		/// <param name="parentNode">The parent the scene node should be added to.</param>
		public void Add(SceneNode sceneNode, SceneNode parentNode)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(sceneNode);
			Assert.ArgumentNotNull(parentNode);
			Assert.NotPooled(sceneNode);
			Assert.NotPooled(parentNode);
			Assert.ArgumentSatisfies(sceneNode.SceneGraph == null, "The scene node already belongs to a scene graph.");
			Assert.ArgumentSatisfies(!sceneNode.IsRemoved, "The scene node has already been removed and cannot be re-added.");
			Assert.ArgumentSatisfies(sceneNode != parentNode, "The scene node cannot be its own parent.");
			Assert.ArgumentSatisfies(parentNode.SceneGraph == this, "The parent does not belong to this scene graph.");

			sceneNode.SceneGraph = this;

			if (IsEnumerated)
				_deferredUpdates.Enqueue(new Update { UpdateType = UpdateType.AddNode, SceneNode = sceneNode, ParentNode = parentNode });
			else
				AddImmediately(sceneNode, parentNode);
		}

		/// <summary>
		///     Removes the given scene node from the scene graph.
		/// </summary>
		/// <param name="sceneNode">The scene node that should be removed.</param>
		public void Remove(SceneNode sceneNode)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(sceneNode);
			Assert.NotPooled(sceneNode);
			Assert.ArgumentSatisfies(sceneNode.Parent != null, "Cannot remove the root node.");
			Assert.ArgumentSatisfies(sceneNode.SceneGraph == this, "The scene node does not belong to this scene graph.");

			if (sceneNode.IsRemoved)
				return;

			foreach (var node in EnumeratePostOrder(sceneNode))
			{
				if (node.IsRemoved)
					continue;

				node.IsRemoved = true;

				if (IsEnumerated)
					_deferredUpdates.Enqueue(new Update { UpdateType = UpdateType.RemoveNode, SceneNode = node });
				else
					RemoveImmediately(node);
			}
		}

		/// <summary>
		///     Changes the parent node of the given scene node.
		/// </summary>
		/// <param name="sceneNode">The scene node that should be attached to the new parent.</param>
		/// <param name="parentNode">The new parent of the scene node.</param>
		public void Reparent(SceneNode sceneNode, SceneNode parentNode)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(sceneNode);
			Assert.ArgumentNotNull(parentNode);
			Assert.ArgumentSatisfies(!sceneNode.IsRemoved, "The scene node has already been removed.");
			Assert.ArgumentSatisfies(!parentNode.IsRemoved, "The parent has already been removed.");
			Assert.ArgumentSatisfies(sceneNode.SceneGraph == this, "The scene node belongs to a different scene graph.");
			Assert.ArgumentSatisfies(parentNode.SceneGraph == this, "The parent belongs to a different scene graph.");
			Assert.ArgumentSatisfies(sceneNode.Parent != null, "Cannot change the parent of the root node.");
			Assert.ArgumentSatisfies(sceneNode != parentNode, "The scene node cannot be its own parent.");

			if (IsEnumerated)
				_deferredUpdates.Enqueue(new Update { UpdateType = UpdateType.ReparentNode, SceneNode = sceneNode, ParentNode = parentNode });
			else
				ReparentImmediately(sceneNode, parentNode);
		}

		/// <summary>
		///     Executes the behaviors of all scene nodes in the scene graph.
		/// </summary>
		/// <param name="elapsedSeconds">The elapsed time in seconds since the last execution of the behaviors.</param>
		public void ExecuteBehaviors(float elapsedSeconds)
		{
			Assert.NotDisposed(this);

			foreach (var sceneNode in EnumeratePreOrder())
			{
				foreach (var behavior in sceneNode.Behaviors)
					behavior.Execute(elapsedSeconds);
			}
		}

		/// <summary>
		///     Enumerates all scene nodes of the scene graph in pre-order, that is, the parent node first,
		///     followed by the first child subtree, followed by the second child subtree, etc.
		/// </summary>
		/// <param name="startNode">The scene node where the enumeration should start. Defaults to the root node.</param>
		public PreOrderEnumerator EnumeratePreOrder(SceneNode startNode = null)
		{
			return new PreOrderEnumerator(this, startNode);
		}

		/// <summary>
		///     Enumerates all scene nodes of the scene graph in pre-order, that is, the first child subtree first, followed by the
		///     second child subtree, ..., and the parent node last.
		/// </summary>
		/// <param name="startNode">The scene node where the enumeration should start. Defaults to the root node.</param>
		public PostOrderEnumerator EnumeratePostOrder(SceneNode startNode = null)
		{
			return new PostOrderEnumerator(this, startNode);
		}

		/// <summary>
		///     Enumerates all scene nodes of the scene graph in pre-order, that is, the parent node first,
		///     followed by the first child subtree, followed by the second child subtree, etc. Only scene nodes of the given type are
		///     enumerated.
		/// </summary>
		/// <typeparam name="T">The type of the scene nodes that should be enumerated.</typeparam>
		/// <param name="startNode">The scene node where the enumeration should start. Defaults to the root node.</param>
		public TypedPreOrderEnumerator<T> EnumeratePreOrder<T>(SceneNode startNode = null)
			where T : SceneNode
		{
			return new TypedPreOrderEnumerator<T>(this, startNode);
		}

		/// <summary>
		///     Enumerates all scene nodes of the scene graph in pre-order, that is, the first child subtree first, followed by the
		///     second child subtree, ..., and the parent node last. Only scene nodes of the given type are enumerated.
		/// </summary>
		/// <typeparam name="T">The type of the scene nodes that should be enumerated.</typeparam>
		/// <param name="startNode">The scene node where the enumeration should start. Defaults to the root node.</param>
		public TypedPostOrderEnumerator<T> EnumeratePostOrder<T>(SceneNode startNode = null)
			where T : SceneNode
		{
			return new TypedPostOrderEnumerator<T>(this, startNode);
		}

		/// <summary>
		///     Adds the given behavior to the scene node.
		/// </summary>
		/// <param name="sceneNode">The scene node the behavior should be added to.</param>
		/// <param name="behavior">The behavior that should be attached.</param>
		internal void AddBehavior(SceneNode sceneNode, IBehavior behavior)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(sceneNode);
			Assert.ArgumentNotNull(behavior);
			Assert.ArgumentSatisfies(behavior.SceneNode == null, "The behavior is already attached to a scene node.");

			if (IsEnumerated)
				_deferredUpdates.Enqueue(new Update { UpdateType = UpdateType.AddBehavior, SceneNode = sceneNode, Behavior = behavior });
			else
				AddBehaviorImmediately(sceneNode, behavior);
		}

		/// <summary>
		///     Detaches the given behavior from the scene node.
		/// </summary>
		/// <param name="behavior">The behavior that should be detached.</param>
		internal void RemoveBehavior(IBehavior behavior)
		{
			Assert.NotDisposed(this);
			Assert.ArgumentNotNull(behavior);

			if (IsEnumerated)
				_deferredUpdates.Enqueue(new Update { UpdateType = UpdateType.RemoveBehavior, Behavior = behavior });
			else
				RemoveBehaviorImmediately(behavior);
		}

		/// <summary>
		///     Immediately adds the given scene node to the given parent node.
		/// </summary>
		/// <param name="sceneNode">The scene node that should be added.</param>
		/// <param name="parentNode">The parent the scene node should be added to.</param>
		private void AddImmediately(SceneNode sceneNode, SceneNode parentNode)
		{
			sceneNode.Attach(this, parentNode);

			if (NodeAdded != null)
				NodeAdded(sceneNode);

			++Version;
		}

		/// <summary>
		///     Immediately removes the given scene node from the scene graph.
		/// </summary>
		/// <param name="sceneNode">The scene node that should be removed.</param>
		private void RemoveImmediately(SceneNode sceneNode)
		{
			sceneNode.Detach();
			sceneNode.SafeDispose();

			if (NodeRemoved != null)
				NodeRemoved(sceneNode);

			++Version;
		}

		/// <summary>
		///     Immediately changes the parent node of the given scene node.
		/// </summary>
		/// <param name="sceneNode">The scene node that should be attached to the new parent.</param>
		/// <param name="parentNode">The new parent of the scene node.</param>
		private void ReparentImmediately(SceneNode sceneNode, SceneNode parentNode)
		{
			sceneNode.Detach();
			sceneNode.Attach(this, parentNode);
			++Version;
		}

		/// <summary>
		///     Immediately adds the given behavior to the scene node.
		/// </summary>
		/// <param name="sceneNode">The scene node the behavior should be added to.</param>
		/// <param name="behavior">The behavior that should be attached.</param>
		private void AddBehaviorImmediately(SceneNode sceneNode, IBehavior behavior)
		{
			behavior.Attach(sceneNode);
			++Version;
		}

		/// <summary>
		///     Immediately detaches the given behavior from the scene node.
		/// </summary>
		/// <param name="behavior">The behavior that should be detached.</param>
		private void RemoveBehaviorImmediately(IBehavior behavior)
		{
			behavior.Detach();
			++Version;
		}

		/// <summary>
		///     Applies all deferred scene graph updates.
		/// </summary>
		internal void ApplyDeferredUpdates()
		{
			Assert.NotDisposed(this);

			if (IsEnumerated)
				return;

			while (_deferredUpdates.Count > 0)
			{
				var update = _deferredUpdates.Dequeue();
				switch (update.UpdateType)
				{
					case UpdateType.AddNode:
						AddImmediately(update.SceneNode, update.ParentNode);
						break;
					case UpdateType.RemoveNode:
						RemoveImmediately(update.SceneNode);
						break;
					case UpdateType.ReparentNode:
						ReparentImmediately(update.SceneNode, update.ParentNode);
						break;
					case UpdateType.AddBehavior:
						AddBehaviorImmediately(update.SceneNode, update.Behavior);
						break;
					case UpdateType.RemoveBehavior:
						RemoveBehaviorImmediately(update.Behavior);
						break;
					default:
						throw new InvalidOperationException("Unknown update type.");
				}
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Root.SafeDispose();
		}

		/// <summary>
		///     Represents an update of a scene graph.
		/// </summary>
		private struct Update
		{
			/// <summary>
			///     The behavior that should be updated.
			/// </summary>
			public IBehavior Behavior;

			/// <summary>
			///     The parent scene node.
			/// </summary>
			public SceneNode ParentNode;

			/// <summary>
			///     The scene node that should be updated.
			/// </summary>
			public SceneNode SceneNode;

			/// <summary>
			///     Indicates the type of the update that should be applied to a scene graph.
			/// </summary>
			public UpdateType UpdateType;
		}

		/// <summary>
		///     Indicates the type of update that should be applied to a scene graph.
		/// </summary>
		private enum UpdateType
		{
			/// <summary>
			///     Indicates that a scene node should be added.
			/// </summary>
			AddNode,

			/// <summary>
			///     Indicates that a scene node should be removed.
			/// </summary>
			RemoveNode,

			/// <summary>
			///     Indicates that the parent of a scene node should be changed.
			/// </summary>
			ReparentNode,

			/// <summary>
			///     Indicates that a behavior should be added to a scene node.
			/// </summary>
			AddBehavior,

			/// <summary>
			///     Indicates that a behavior should be removed from a scene node.
			/// </summary>
			RemoveBehavior
		}
	}
}