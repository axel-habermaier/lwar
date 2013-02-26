using System;

namespace Lwar.Client.Gameplay
{
	using Pegasus.Framework;
	using Pegasus.Framework.Math;

	/// <summary>
	///   Represents a transformation in a transformation hierarchy.
	/// </summary>
	public class Transformation : PooledObject<Transformation>
	{
		/// <summary>
		///   Gets the matrix representing the global transformation.
		/// </summary>
		public Matrix Matrix { get; private set; }

		/// <summary>
		///   Gets or sets the position.
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		///   Gets or sets the rotation.
		/// </summary>
		public Vector3 Rotation { get; set; }

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		/// <param name="parent">The parent of the transformation.</param>
		public static Transformation Create(Transformation parent)
		{
			Assert.ArgumentNotNull(parent, () => parent);
			return GetInstance();
		}

		/// <summary>
		///   Creates a new instance.
		/// </summary>
		public static Transformation Create()
		{
			return GetInstance();
		}
	}
}