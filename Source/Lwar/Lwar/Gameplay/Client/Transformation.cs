﻿namespace Lwar.Gameplay.Client
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Pegasus.Math;
	using Pegasus.Utilities;

	/// <summary>
	///     Represents a transformation in a transformation hierarchy.
	/// </summary>
	internal class Transformation
	{
		/// <summary>
		///     The children of the transformation.
		/// </summary>
		private readonly List<Transformation> _children = new List<Transformation>();

		/// <summary>
		///     The parent of the transformation.
		/// </summary>
		private Transformation _parent;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Transformation()
		{
			Reset();
		}

		/// <summary>
		///     Gets the absolute transformation matrix.
		/// </summary>
		public Matrix Matrix { get; private set; }

		/// <summary>
		///     Gets or sets the position relative to the parent.
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		///     Gets or sets the 2D position relative to the parent.
		/// </summary>
		public Vector2 Position2D
		{
			get { return new Vector2(Position.X, Position.Z); }
			set { Position = new Vector3(value.X, Position.Y, value.Y); }
		}

		/// <summary>
		///     Gets or sets the rotation relative to the parent.
		/// </summary>
		public Vector3 Rotation { get; set; }

		/// <summary>
		///     Resest the transformation to its default values.
		/// </summary>
		public void Reset()
		{
			Matrix = Matrix.Identity;
			Position = Vector3.Zero;
			Rotation = Vector3.Zero;

			_children.Clear();
			_parent = null;
		}

		/// <summary>
		///     Attaches the transformation to the given parent. If the transformation already has a parent, it is detached from its
		///     old parent before it is attached to the new one.
		/// </summary>
		/// <param name="parent">The parent the transformation should be attached to.</param>
		public void AttachTo(Transformation parent)
		{
			Assert.ArgumentNotNull(parent);

			if (_parent == parent)
				return;

			if (_parent != null)
			{
				Assert.That(_parent._children.Contains(this), "Inconsistent transformation hierarchy.");
				_parent._children.Remove(this);
			}

			_parent = parent;
			_parent._children.Add(this);
		}

		/// <summary>
		///     Detaches the transformation from the hierarchy. The children of the transformation are attached to the
		///     transformation's parent.
		/// </summary>
		public void Detach()
		{
			Assert.That(_parent != null, "The transformation has already been detached.");
			Assert.That(_parent._children.Contains(this), "Inconsistent transformation hierarchy.");
			Assert.That(_children.All(t => !_parent._children.Contains(t)), "Inconsistent transformation hierarchy.");

			_parent._children.Remove(this);
			_parent._children.AddRange(_children);

			foreach (var child in _children)
				child._parent = _parent;

			_parent = null;
			_children.Clear();
		}

		/// <summary>
		///     Updates the transformation hierarchy.
		/// </summary>
		public void Update()
		{
			Matrix = Matrix.CreateRotationZ(Rotation.Z) * Matrix.CreateRotationX(Rotation.X) *
					 Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateTranslation(Position);

			if (_parent != null)
				Matrix *= _parent.Matrix;

			foreach (var child in _children)
				child.Update();
		}
	}
}