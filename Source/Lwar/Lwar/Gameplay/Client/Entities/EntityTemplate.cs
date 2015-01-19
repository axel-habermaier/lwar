namespace Lwar.Gameplay.Client.Entities
{
	using System;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Manages common properties of entities.
	/// </summary>
	internal class EntityTemplate : DisposableObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		public EntityTemplate(float maxEnergy, float maxHealth, float radius, Texture2D texture, CubeMap cubeMap, Model model)
		{
			MaxEnergy = maxEnergy;
			MaxHealth = maxHealth;
			Radius = radius;
			Texture = texture;
			CubeMap = cubeMap;
			Model = model;
		}

		/// <summary>
		///     The cube map that should be used to draw the entity.
		/// </summary>
		public CubeMap CubeMap { get; private set; }

		/// <summary>
		///     The entity's maximum energy level.
		/// </summary>
		public float MaxEnergy { get; private set; }

		/// <summary>
		///     The entity's maximum health.
		/// </summary>
		public float MaxHealth { get; private set; }

		/// <summary>
		///     The model that is used to draw the entity.
		/// </summary>
		public Model Model { get; private set; }

		/// <summary>
		///     The entity's radius, defining its size.
		/// </summary>
		public float Radius { get; private set; }

		/// <summary>
		///     The texture that should be used to draw the entity.
		/// </summary>
		public Texture2D Texture { get; private set; }

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			Model.SafeDispose();
		}
	}
}