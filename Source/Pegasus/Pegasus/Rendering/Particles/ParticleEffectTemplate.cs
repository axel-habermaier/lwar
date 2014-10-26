namespace Pegasus.Rendering.Particles
{
	using System;
	using Assets;
	using Platform.Graphics;
	using Platform.Memory;
	using Utilities;

	/// <summary>
	///     Represents a template from which a certain particle effect can be created.
	/// </summary>
	public abstract class ParticleEffectTemplate : DisposableObject
	{
		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="displayName">The display name of the particle effect template.</param>
		protected ParticleEffectTemplate(string displayName)
		{
			Assert.ArgumentNotNullOrWhitespace(displayName);
			DisplayName = displayName;
		}

		/// <summary>
		///     Gets the assets manager that is used to load all required assets.
		/// </summary>
		protected AssetsManager Assets { get; private set; }

		/// <summary>
		///     Gets the graphics device that is used for drawing the particle effect.
		/// </summary>
		protected GraphicsDevice GraphicsDevice { get; private set; }

		/// <summary>
		///     Gets the display name of the particle effect template.
		/// </summary>
		public string DisplayName { get; private set; }

		/// <summary>
		///     Preloads the required assets of the particle effect template.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing the particle effect.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public void Load(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			GraphicsDevice = graphicsDevice;
			Assets = assets;

			Load();
		}

		/// <summary>
		///     Loads the required assets of the particle effect template.
		/// </summary>
		protected abstract void Load();

		/// <summary>
		///     Initializes the given particle effect with the template's parameters.
		/// </summary>
		/// <param name="particleEffect">The particle effect that should be initialized.</param>
		public abstract void Initialize(ParticleEffect particleEffect);

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
		}
	}
}