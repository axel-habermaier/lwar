using System;

namespace Pegasus.Framework.Platform.Graphics
{
	/// <summary>
	///   Represents a combination of shaders that are currently set on the GPU to create a rendering effect.
	/// </summary>
	public struct EffectTechnique : IDisposable
	{
		/// <summary>
		///   The effect the running technique belongs to.
		/// </summary>
		private readonly Effect _effect;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="effect">The effect the technique should belong to.</param>
		internal EffectTechnique(Effect effect)
		{
			Assert.ArgumentNotNull(effect, () => effect);

			_effect = effect;
			_effect.IsActive = true;
		}

		/// <summary>
		///   Marks the effect the technique belongs to as no longer used.
		/// </summary>
		void IDisposable.Dispose()
		{
			Assert.NotNull(_effect, "No effect has been set.");
			_effect.IsActive = false;
		}
	}
}