using System;

namespace Lwar.Rendering.Renderers
{
	using Gameplay.Entities;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders phasers into a 3D scene.
	/// </summary>
	public class PhaserRenderer : Renderer<Phaser>
	{
		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		protected override void Initialize()
		{
		}

		/// <summary>
		///   Draws all phasers.
		/// </summary>
		/// <param name="output">The output that the phasers should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
		}

		/// <summary>
		///   Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
		}
	}
}