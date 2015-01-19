namespace Lwar.Assets.EntityTemplates.Compilation
{
	using System;

	/// <summary>
	///     Represents a two-dimensional vector.
	/// </summary>
	public struct Vector2
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public Vector2(float x, float y)
			: this()
		{
			X = x;
			Y = y;
		}

		/// <summary>
		///     The X-component of the vector.
		/// </summary>
		public float X { get; private set; }

		/// <summary>
		///     The Y-component of the vector.
		/// </summary>
		public float Y { get; private set; }
	}
}