﻿namespace Lwar.Rendering.Renderers
{
	using System;
	using Gameplay.Entities;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders phasers into a 3D scene.
	/// </summary>
	public class PhaserRenderer : Renderer<Phaser>
	{
		/// <summary>
		///     Draws all phasers.
		/// </summary>
		/// <param name="output">The output that the phasers should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
		}
	}
}