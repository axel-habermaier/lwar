namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Client.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders shockwaves into a 3D scene.
	/// </summary>
	internal class ShockwaveRenderer : Renderer<ShockwaveEntity>
	{
		/// <summary>
		///     The effect that is used to draw the shockwaves.
		/// </summary>
		private SphereEffect _effect;

		/// <summary>
		///     The model that is used to draw the shockwaves.
		/// </summary>
		private Model _model;

		private CubeMap _texture;

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		public override void Initialize(RenderContext renderContext, GameBundle assets)
		{
			_texture = assets.SunHeat;
			_effect = assets.SphereEffect;

			_model = Model.CreateSphere(renderContext.GraphicsDevice, 1, 10);
			_effect.SphereTexture = new CubeMapView(_texture, renderContext.SamplerStates.BilinearClampNoMipmaps);
		}

		/// <summary>
		///     Draws all shockwaves.
		/// </summary>
		/// <param name="output">The output that the shockwaves should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			foreach (var shockwave in Elements)
			{
				_effect.World = Matrix.CreateScale(shockwave.Radius) * shockwave.Transform.Matrix;
				_model.Draw(output, _effect.Default);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			_model.SafeDispose();
		}
	}
}