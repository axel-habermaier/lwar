namespace Lwar.Rendering.Renderers
{
	using System;
	using Assets;
	using Assets.Effects;
	using Gameplay.Client.Entities;
	using Pegasus.Math;
	using Pegasus.Platform;
	using Pegasus.Platform.Graphics;
	using Pegasus.Platform.Memory;
	using Pegasus.Rendering;

	/// <summary>
	///     Renders suns into a 3D scene.
	/// </summary>
	internal class SunRenderer : Renderer<SunEntity>
	{
		private GaussianBlur _blur;
		private Clock _clock = new Clock();

		/// <summary>
		///     The render target that is used to draw the sun effect.
		/// </summary>
		private RenderTarget _effectTarget;

		/// <summary>
		///     The full-screen quad that is used to draw the sun special effects.
		/// </summary>
		private FullscreenQuad _fullscreenQuad;

		private Texture2D _heat;
		private RenderOutput _heatOutput;

		/// <summary>
		///     The sun model.
		/// </summary>
		private Model _model;

		private TexturedQuadEffect _quadEffect;
		private Vector2 _rotation = new Vector2(0, 4);
		private SphereEffect _sphereEffect;
		private CubeMap _sun;
		private SunEffect _sunEffect;
		private CubeMap _turbulence;

		/// <summary>
		///     Initializes the renderer.
		/// </summary>
		/// <param name="renderContext">The render context that should be used for drawing.</param>
		/// <param name="assets">The asset bundle that provides access to Lwar assets.</param>
		public override void Initialize(RenderContext renderContext, GameBundle assets)
		{
			_sun = assets.Sun;
			_turbulence = assets.SunHeat;
			_heat = assets.Heat;

			_sphereEffect = assets.SphereEffect;
			_sunEffect = assets.SunEffect;
			_quadEffect = assets.TexturedQuadEffect;

			_model = Model.CreateSphere(renderContext.GraphicsDevice, EntityTemplates.Sun.Radius, 20);
			_fullscreenQuad = new FullscreenQuad(renderContext.GraphicsDevice);

			var w = 640;
			var h = 360;
			var flags = TextureFlags.GenerateMipmaps | TextureFlags.RenderTarget;
			var effectTexture = new Texture2D(renderContext.GraphicsDevice, w, h, SurfaceFormat.Rgba8, flags);
			effectTexture.SetName("SunRenderer.EffectTexture");

			_effectTarget = new RenderTarget(renderContext.GraphicsDevice, null, effectTexture);
			_heatOutput = new RenderOutput(renderContext) { RenderTarget = _effectTarget, Viewport = new Rectangle(0, 0, w, h) };
			_blur = new GaussianBlur(renderContext, effectTexture);
		}

		/// <summary>
		///     Draws all suns.
		/// </summary>
		/// <param name="output">The output that the bullets should be rendered to.</param>
		public override void Draw(RenderOutput output)
		{
			_sphereEffect.SphereTexture = new CubeMapView(_sun, output.RenderContext.SamplerStates.TrilinearClamp);
			_sunEffect.CubeMap = new CubeMapView(_turbulence, output.RenderContext.SamplerStates.TrilinearClamp);
			_sunEffect.HeatMap = new Texture2DView(_heat, output.RenderContext.SamplerStates.BilinearClampNoMipmaps);

			foreach (var sun in Elements)
			{
				output.RenderContext.BlendStates.Premultiplied.Bind();
				output.RenderContext.DepthStencilStates.DepthEnabled.Bind();

				var elapsed = (float)_clock.Seconds;
				_clock.Reset();

				_rotation.X += 0.1f * elapsed;
				_rotation.Y -= 0.05f * elapsed;

				_sphereEffect.World = Matrix.CreateRotationY(-_rotation.X * 2) * sun.Transform.Matrix;
				_model.Draw(output, _sphereEffect.Default);

				_sunEffect.World = Matrix.CreateScale(1.03f) * Matrix.CreateRotationY(-_rotation.X * 2) * sun.Transform.Matrix;
				_sunEffect.Rotation1 = Matrix.CreateRotationY(-_rotation.X) * Matrix.CreateRotationX(_rotation.Y * 2);
				_sunEffect.Rotation2 = Matrix.CreateRotationY(-_rotation.Y) * Matrix.CreateRotationZ(_rotation.X * 2);

				output.RenderContext.DepthStencilStates.DepthRead.Bind();
				_heatOutput.ClearColor(new Color(0, 0, 0, 0));
				_heatOutput.Camera = output.Camera;
				_model.Draw(_heatOutput, _sunEffect.Default);

				var blurredTexture = _blur.Blur(output);

				output.RenderContext.DepthStencilStates.DepthDisabled.Bind();
				_quadEffect.Texture = new Texture2DView(blurredTexture, output.RenderContext.SamplerStates.BilinearClampNoMipmaps);
				_quadEffect.World = Matrix.Identity;
				_fullscreenQuad.Draw(output, _quadEffect.FullScreen);
			}
		}

		/// <summary>
		///     Disposes the object, releasing all managed and unmanaged resources.
		/// </summary>
		protected override void OnDisposingCore()
		{
			_model.SafeDispose();
			_effectTarget.SafeDispose();
			_fullscreenQuad.SafeDispose();
			_blur.SafeDispose();
		}
	}
}