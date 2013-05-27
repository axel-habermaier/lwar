using System;

namespace Pegasus.Framework.Platform.Graphics
{
	// ReSharper disable InconsistentNaming

	/// <summary>
	///   Represents a shader-based rendering effect.
	/// </summary>
	/// <remarks>
	///   The effect cannot be derived from DisposableObject, as that might introduce name clashes with variable names used by
	///   the effect.
	/// </remarks>
	public abstract class Effect : IDisposable
	{
		/// <summary>
		///   Indicates whether the effect has already been disposed.
		/// </summary>
		private bool _isDisposed;

		/// <summary>
		///   The context of the effect that can be used to load and bind effect resources.
		/// </summary>
		protected EffectContext __context;

		/// <summary>
		///   Initializes a new instance.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device this instance belongs to.</param>
		/// <param name="assets">The assets manager that should be used to load required assets.</param>
		protected Effect(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			__context = new EffectContext(graphicsDevice, assets);
		}

#if DEBUG
	/// <summary>
	///   Ensures that the instance has been disposed.
	/// </summary>
		~Effect()
		{
			Log.Die("Finalizer runs for effect '{0}'", GetType().FullName);
		}
#endif

		/// <summary>
		///   Disposes the effect, releasing all managed and unmanaged resources.
		/// </summary>
		void IDisposable.Dispose()
		{
			Assert.That(!_isDisposed, "The effect has already been disposed.");

			__OnDisposing();
			_isDisposed = true;
#if DEBUG
			GC.SuppressFinalize(this);
#endif
		}

		/// <summary>
		///   Disposes the effect, releasing all managed and unmanaged resources.
		/// </summary>
		protected virtual void __OnDisposing()
		{
		}
	}

	// ReSharper restore InconsistentNaming
}