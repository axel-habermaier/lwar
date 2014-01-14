using System;

namespace Lwar.AssetsCompiler.EntityTemplates.Compilation
{
	using System;
	using Pegasus.Math;

	/// <summary>
	///     Represents an entity template that is shared between the client and the server.
	/// </summary>
	public class EntityTemplate
	{
		public Vector2 Acceleration;
		public string Act;
		public string Collide;
		public string CubeMap;
		public Vector2 Decelaration;
		public float Energy;
		public string Format;
		public float Health;
		public int Interval;
		public float Length;
		public float Mass;
		public Func<EntityTemplate, string> Model;

		/// <summary>
		///     The name of the template. This field is set by the template compiler.
		/// </summary>
		public string Name;

		public float Radius;
		public float Rotation;
		public float Shield;
		public string Texture;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public EntityTemplate()
		{
			Model = _ => null;
		}
	}
}