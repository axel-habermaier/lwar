using System;

namespace Lwar.Client.Rendering.Renderers
{
	using Assets.Effects;
	using Gameplay.Entities;
	using Pegasus.Framework.Platform.Graphics;
	using Pegasus.Framework.Platform.Memory;
	using Pegasus.Framework.Rendering;

	/// <summary>
	///   Renders rockets into a 3D scene.
	/// </summary>
	public class RocketRenderer : Renderer<Rocket>
	{
		/// <summary>
		///   The effect that is used to draw the rockets.
		/// </summary>
		private TexturedQuadEffect _effect;

		/// <summary>
		///   The model that is used to draw the rockets.
		/// </summary>
		private Model _model;

		/// <summary>
		///   Initializes the renderer.
		/// </summary>
		protected override void Initialize()
		{
			var texture = Assets.LoadTexture2D("Textures/Rocket");

			_model = Model.CreateQuad(GraphicsDevice, texture.Size);
			_effect = new TexturedQuadEffect(GraphicsDevice, Assets) { Texture = new Texture2DView(texture, SamplerState.TrilinearClamp) };
		}

		/// <summary>
		///   Draws all rockets.
		/// </summary>
		/// <param name="output">The output that the rockets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			BlendState.Premultiplied.Bind();
			DepthStencilState.DepthDisabled.Bind();

			foreach (var rocket in Elements)
			{
				_effect.World = rocket.Transform.Matrix;
				_model.Draw(output, _effect.TexturedQuad);
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