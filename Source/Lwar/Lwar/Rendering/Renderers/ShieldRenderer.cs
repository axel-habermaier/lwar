namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Client.Actors;
	using Pegasus.Math;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders shield effects into a 3D scene.
	/// </summary>
	internal class ShieldRenderer : Renderer<ShieldActor>
	{
		/// <summary>
		///     The effect that is used to draw the shields.
		/// </summary>
		private SphereEffect _effect;

		/// <summary>
		///     The model that is used to draw the shields.
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
			_texture = assets.Shields;
			_effect = assets.SphereEffect;

			_model = Model.CreateSphere(renderContext.GraphicsDevice, 1.0f, 16);
			_effect.SphereTexture = new CubeMapView(_texture, renderContext.SamplerStates.TrilinearClamp);
		}

		/// <summary>
		///     Draws all registered 3D elements.
		/// </summary>
		/// <param name="output">The output that the elements should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			foreach (var shield in Elements)
			{
				_effect.World = Matrix.CreateScale(shield.Ship.Template.Radius) * shield.Transform.Matrix;
				_effect.ImpactPosition = new Vector3(shield.ImpactPosition.X, 0, shield.ImpactPosition.Y);
				_effect.TimeToLive = shield.TimeToLive;
				_model.Draw(output, _effect.Shield);
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