namespace Pegasus.Scene
{
	using System;
	using Math;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents the base class for all scene nodes of a scene graph.
	/// </summary>
	public abstract class SceneNode : PooledObject
	{
		/// <summary>
		///     The local transformation matrix of the scene node, relative to the parent scene node.
		/// </summary>
		private Matrix _localMatrix = Matrix.Identity;

		/// <summary>
		///     The scene node's position in 3D space, relative to the parent scene node.
		/// </summary>
		private Vector3 _position;

		/// <summary>
		///     The scene node's rotation, relative to the parent scene node.
		/// </summary>
		private Vector3 _rotation;

		/// <summary>
		///     The world transformation matrix of the scene node.
		/// </summary>
		private Matrix _worldMatrix = Matrix.Identity;

		/// <summary>
		///     Gets the first child scene node.
		/// </summary>
		internal SceneNode Child { get; private set; }

		/// <summary>
		///     Gets the next sibling scene node.
		/// </summary>
		internal SceneNode NextSibling { get; private set; }

		/// <summary>
		///     Get the previous sibling scene node.
		/// </summary>
		internal SceneNode PreviousSibling { get; private set; }

		/// <summary>
		///     Gets the local transformation matrix of the scene node, relative to the parent scene node.
		/// </summary>
		public Matrix LocalMatrix
		{
			get
			{
				Assert.NotPooled(this);
				return _localMatrix;
			}
		}

		/// <summary>
		///     Gets the world transformation matrix of the scene node.
		/// </summary>
		public Matrix WorldMatrix
		{
			get
			{
				Assert.NotPooled(this);
				return _worldMatrix;
			}
		}

		/// <summary>
		///     Gets or sets the scene node's position in 3D space, relative to the parent scene node.
		/// </summary>
		public Vector3 Position
		{
			get
			{
				Assert.NotPooled(this);
				return _position;
			}
			set
			{
				Assert.NotPooled(this);
				_position = value;
				UpdateMatrices();
			}
		}

		/// <summary>
		///     Gets or sets the scene node's rotation, relative to the parent scene node.
		/// </summary>
		public Vector3 Rotation
		{
			get
			{
				Assert.NotPooled(this);
				return _rotation;
			}
			set
			{
				Assert.NotPooled(this);
				_rotation = value;
				UpdateMatrices();
			}
		}

		/// <summary>
		///     Gets the scene node's world position in 3D space.
		/// </summary>
		public Vector3 WorldPosition
		{
			get { return _worldMatrix.Translation; }
		}

		/// <summary>
		///     Gets the parent scene node or null if the scene node is the root of the scene graph.
		/// </summary>
		public SceneNode Parent { get; private set; }

		/// <summary>
		///     Gets or sets the head of the behavior list.
		/// </summary>
		internal IBehavior Behavior { get; set; }

		/// <summary>
		///     Gets an enumerator that can be used to enumerate all behaviors of the scene node.
		/// </summary>
		internal BehaviorEnumerator Behaviors
		{
			get
			{
				Assert.NotPooled(this);
				return new BehaviorEnumerator(Behavior);
			}
		}

		/// <summary>
		///     Gets an enumerator that can be used to enumerate all children of the scene node.
		/// </summary>
		internal SceneNodeEnumerator Children
		{
			get
			{
				Assert.NotPooled(this);
				return new SceneNodeEnumerator(Child);
			}
		}

		/// <summary>
		///     Gets the scene graph the scene node belongs to or null if the scene node does not belong to any scene graph.
		/// </summary>
		public SceneGraph SceneGraph { get; internal set; }

		/// <summary>
		///     Gets a value indicating whether the scene node has been removed from the scene graph it belonged to.
		/// </summary>
		public bool IsRemoved { get; internal set; }

		/// <summary>
		///     Changes the local position and rotation of the scene node, relative to the parent scene node.
		/// </summary>
		/// <param name="position">The local position relative to the parent scene node's position.</param>
		/// <param name="rotation">The local rotation relative to the parent scene node's rotation.</param>
		public void ChangeLocalTransformation(ref Vector3 position, ref Vector3 rotation)
		{
			Assert.NotPooled(this);

			_position = position;
			_rotation = rotation;

			UpdateMatrices();
		}

		/// <summary>
		///     Updates the transformation matrices.
		/// </summary>
		private void UpdateMatrices()
		{
			// TODO: Could we do that in a more efficient way?
			_localMatrix = Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationX(Rotation.X) *
						   Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);

			var parentMatrix = Parent == null ? Matrix.Identity : Parent.WorldMatrix;
			_worldMatrix = parentMatrix * LocalMatrix;

			foreach (var child in Children)
				child.UpdateMatrices();
		}

		/// <summary>
		///     Attaches the scene node to the given scene graph and parent node.
		/// </summary>
		/// <param name="sceneGraph">The scene graph the scene node should be attached to.</param>
		/// <param name="parent">The parent node the scene node should be attached to.</param>
		internal void Attach(SceneGraph sceneGraph, SceneNode parent)
		{
			Assert.ArgumentNotNull(sceneGraph);
			Assert.NotPooled(this);

			SceneGraph = sceneGraph;

			if (parent != null)
			{
				NextSibling = parent.Child;

				if (parent.Child != null)
					parent.Child.PreviousSibling = this;

				NextSibling = parent.Child;
				Parent = parent;
				parent.Child = this;
			}

			UpdateMatrices();
			OnAttached();
		}

		/// <summary>
		///     Detaches the scene node from the scene graph and parent node it is currently attached to.
		/// </summary>
		internal void Detach()
		{
			Assert.NotPooled(this);

			OnDetached();

			if (Parent.Child == this)
				Parent.Child = NextSibling;

			if (NextSibling != null)
				NextSibling.PreviousSibling = PreviousSibling;

			if (PreviousSibling != null)
				PreviousSibling.NextSibling = NextSibling;

			NextSibling = null;
			PreviousSibling = null;
			Parent = null;
			SceneGraph = null;
		}

		/// <summary>
		///     Invoked when the scene node is attached to a parent scene node.
		/// </summary>
		protected virtual void OnAttached()
		{
		}

		/// <summary>
		///     Invoked when the scene node is detached from its scene graph.
		/// </summary>
		/// <remarks>This method is not called when the scene graph is disposed.</remarks>
		protected virtual void OnDetached()
		{
		}

		/// <summary>
		///     Adds the given behavior to the scene node.
		/// </summary>
		/// <param name="behavior">The behavior that should be attached.</param>
		public void AddBehavior(IBehavior behavior)
		{
			Assert.NotPooled(this);
			Assert.ArgumentNotNull(behavior);
			Assert.ArgumentSatisfies(behavior.SceneNode == null, "The behavior is already attached to a scene node.");

			// If we're not attached to a scene graph yet, we can just add the behavior; otherwise, the scene
			// graph might have to defer the operation
			if (SceneGraph == null)
				behavior.Attach(this);
			else
				SceneGraph.AddBehavior(this, behavior);
		}

		/// <summary>
		///     Detaches the given behavior from the scene node.
		/// </summary>
		/// <param name="behavior">The behavior that should be detached.</param>
		public void RemoveBehavior(IBehavior behavior)
		{
			Assert.NotPooled(this);
			Assert.ArgumentNotNull(behavior);
			Assert.ArgumentSatisfies(behavior.SceneNode == this, "The behavior is not attached to this scene node.");

			// If we're not attached to a scene graph yet, we can just add the behavior; otherwise, the scene
			// graph might have to defer the operation
			if (SceneGraph == null)
				behavior.Detach();
			else
				SceneGraph.RemoveBehavior(behavior);
		}

		/// <summary>
		///     Attaches the scene node to the given parent node.
		/// </summary>
		/// <param name="parent">The parent the scene node should be added to.</param>
		public void AttachTo(SceneNode parent)
		{
			Assert.NotPooled(this);
			Assert.ArgumentNotNull(parent);
			Assert.ArgumentSatisfies(parent.SceneGraph != null, "The parent does not belong to a scene graph.");

			parent.SceneGraph.Add(this, parent);
		}

		/// <summary>
		///     Attaches the given scene node as a child of the current node.
		/// </summary>
		/// <param name="childNode">The child node that should be attached.</param>
		public void AttachChild(SceneNode childNode)
		{
			Assert.NotPooled(this);
			Assert.ArgumentNotNull(childNode);
			Assert.ArgumentSatisfies(SceneGraph != null, "The scene node does not belong to a scene graph.");

			SceneGraph.Add(childNode, this);
		}

		/// <summary>
		///     Removes the scene node from the scene graph.
		/// </summary>
		public void Remove()
		{
			Assert.NotPooled(this);

			if (SceneGraph != null)
				SceneGraph.Remove(this);
		}

		/// <summary>
		///     Changes the parent node of the scene node.
		/// </summary>
		/// <param name="parent">The new parent of the scene node.</param>
		public void Reparent(SceneNode parent)
		{
			Assert.NotPooled(this);
			Assert.NotNull(SceneGraph, "The scene node does not belong to a scene graph.");

			SceneGraph.Reparent(this, parent);
		}

		/// <summary>
		///     Invoked when the pooled instance is returned to the pool.
		/// </summary>
		protected override void OnReturning()
		{
			foreach (var behavior in Behaviors)
			{
				behavior.Detach();
				behavior.SafeDispose();
			}

			foreach (var child in Children)
				child.SafeDispose();

			Behavior = null;
			Parent = null;
			SceneGraph = null;
			IsRemoved = false;

			_localMatrix = Matrix.Identity;
			_worldMatrix = Matrix.Identity;
			Child = null;
			NextSibling = null;
			PreviousSibling = null;
			_position = Vector3.Zero;
			_rotation = Vector3.Zero;
		}

		/// <summary>
		///     Enumerates a list of behaviors.
		/// </summary>
		public struct BehaviorEnumerator
		{
			/// <summary>
			///     The remaining behaviors that have yet to be enumerated.
			/// </summary>
			private IBehavior _behavior;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="behavior">The behaviors that should be enumerated.</param>
			public BehaviorEnumerator(IBehavior behavior)
				: this()
			{
				_behavior = behavior;
			}

			/// <summary>
			///     Gets the behavior at the current position of the enumerator.
			/// </summary>
			public IBehavior Current { get; private set; }

			/// <summary>
			///     Advances the enumerator to the next item.
			/// </summary>
			public bool MoveNext()
			{
				Current = _behavior;
				if (Current == null)
					return false;

				_behavior = _behavior.Next;
				return true;
			}

			/// <summary>
			///     Enables C#'s foreach support for the enumerator.
			/// </summary>
			public BehaviorEnumerator GetEnumerator()
			{
				return this;
			}
		}

		/// <summary>
		///     Enumerates a list of scene nodes.
		/// </summary>
		public struct SceneNodeEnumerator
		{
			/// <summary>
			///     The remaining scene nodes that have yet to be enumerated.
			/// </summary>
			private SceneNode _node;

			/// <summary>
			///     Initializes a new instance.
			/// </summary>
			/// <param name="node">The scene nodes that should be enumerated.</param>
			public SceneNodeEnumerator(SceneNode node)
				: this()
			{
				_node = node;
			}

			/// <summary>
			///     Gets the scene node at the current position of the enumerator.
			/// </summary>
			public SceneNode Current { get; private set; }

			/// <summary>
			///     Advances the enumerator to the next item.
			/// </summary>
			public bool MoveNext()
			{
				Current = _node;
				if (Current == null)
					return false;

				_node = _node.NextSibling;
				return true;
			}

			/// <summary>
			///     Enables C#'s foreach support for the enumerator.
			/// </summary>
			public SceneNodeEnumerator GetEnumerator()
			{
				return this;
			}
		}
	}
}