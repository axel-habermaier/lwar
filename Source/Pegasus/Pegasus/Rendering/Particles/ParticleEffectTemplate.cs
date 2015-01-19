namespace Pegasus.Rendering.Particles
{
	using System;
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
		/// <param name="renderContext">The render context the particle effect belongs to.</param>
		/// <param name="displayName">The display name of the particle effect template.</param>
		protected ParticleEffectTemplate(RenderContext renderContext, string displayName)
		{
			Assert.ArgumentNotNull(renderContext);
			Assert.ArgumentNotNullOrWhitespace(displayName);

			RenderContext = renderContext;
			DisplayName = displayName;
		}

		/// <summary>
		///     Gets the render context that is used for drawing the particle effect.
		/// </summary>
		protected RenderContext RenderContext { get; private set; }

		/// <summary>
		///     Gets the display name of the particle effect template.
		/// </summary>
		public string DisplayName { get; private set; }

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