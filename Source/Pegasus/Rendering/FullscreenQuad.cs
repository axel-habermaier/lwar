namespace Pegasus.Rendering
{
	using System;
	using Platform.Assets;
	using Platform.Graphics;
	using Platform.Memory;

	/// <summary>
	///     Represents a textured full-screen quad.
	/// </summary>
	public class FullscreenQuad : DisposableObject
	{
		/// <summary>
		///     The full screen quad model.
		/// </summary>
		private readonly Model _model;

		/// <summary>
		///     Initializes a new instance.
		/// </summary>
		/// <param name="assets">The assets manager that should be used to load the quad resources.</param>
		public FullscreenQuad(AssetsManager assets)
		{
			Assert.ArgumentNotNull(assets);
			_model = Model.CreateFullScreenQuad();
		}

		/// <summary>
		///     Draws the full-screen quad.
		/// </summary>
		/// <param name="output">The output the fullscreen quad should be rendered to.</param>
		/// <param name="effect">The effect technique that should be used for rendering.</param>
		public void Draw(RenderOutput output, EffectTechnique effect)
		{
			_model.Draw(output, effect);
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_model.SafeDispose();
		}
	}
}