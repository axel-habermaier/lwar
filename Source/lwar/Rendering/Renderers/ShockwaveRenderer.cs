namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders shockwaves into a 3D scene.
	/// </summary>
	public class ShockwaveRenderer : Renderer<Shockwave>
	{
		/// <summary>
		///     The effect that is used to draw the shockwaves.
		/// </summary>
		private SphereEffect _effect;

		/// <summary>
		///     The model that is used to draw the shockwaves.
		/// </summary>
		private Model _model;

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		protected override void Initialize()
		{
			var texture = Assets.LoadCubeMap(Textures.SunHeatCubemap);

			_model = Model.CreateSphere(1, 10);
			_effect = new SphereEffect(Assets) { SphereTexture = new CubeMapView(texture, SamplerState.BilinearClampNoMipmaps) };
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
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}