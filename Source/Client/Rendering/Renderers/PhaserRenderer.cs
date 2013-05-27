﻿using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Framework;
	using Pegasus.Framework.Platform;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders phasers into a 3D scene.
	/// </summary>
	public class PhaserRenderer : Renderer<Phaser>
	{
		/// <summary>
		///   The effect that is used to draw the phasers.
		/// </summary>
		private TexturedQuadEffect _effect;

		/// <summary>
		///   The model that is used to draw the phasers.
		/// </summary>
		private Model _model;

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device that should be used for drawing.</param>
		/// <param name="assets">The assets manager that should be used to load all required assets.</param>
		public override void Initialize(GraphicsDevice graphicsDevice, AssetsManager assets)
		{
			Assert.ArgumentNotNull(graphicsDevice);
			Assert.ArgumentNotNull(assets);

			var texture = assets.LoadTexture2D("Textures/Ship");

			_model = Model.CreateQuad(graphicsDevice, texture.Size);
			_effect = new TexturedQuadEffect(graphicsDevice, assets) { Texture = new Texture2DView(texture, SamplerState.TrilinearClamp) };
		}

		/// <summary>
		///   Draws all phasers.
		/// </summary>
		/// <param name="output">The output that the phasers should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			//foreach (var phaser in Elements)
			//{
			//	_effect.World = phaser.Transform.Matrix;
			//	_model.Draw(output, _effect.Default);
			//}
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposing()
		{
			_effect.SafeDispose();
			_model.SafeDispose();
		}
	}
}