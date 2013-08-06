﻿using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Actors;
	using Pegasus.Framework.Math;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders shield effects into a 3D scene.
	/// </summary>
	public class ShieldRenderer : Renderer<Shield>
	{
		/// <summary>
		///   The effect that is used to draw the shields.
		/// </summary>
		private SphereEffect _effect;

		/// <summary>
		///   The model that is used to draw the shields.
		/// </summary>
		private Model _model;

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		protected override void Initialize()
		{
			_model = Model.CreateSphere(GraphicsDevice, 1.0f, 16);
			_effect = new SphereEffect(GraphicsDevice, Assets)
			{
				SphereTexture = new CubeMapView(Assets.LoadCubeMap("Textures/Shields"), SamplerState.TrilinearClamp)
			};
		}

		/// <summary>
		///   Draws all registered 3D elements.
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
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}